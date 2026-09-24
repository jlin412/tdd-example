using Microsoft.AspNetCore.Builder;   // where MapGet/MapPost actually live
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Kata;

// URL Shortener kata — reference solution (the HTTP layer: R15–R19, plus R23
// and R24 from the extended story).
//
// These are REAL minimal-API routes. The tests next door start an in-process
// server and drive them with a real HttpClient, so routing, model binding,
// JSON serialization, status codes and headers are all genuinely exercised —
// a typo in a route template below fails a test.
//
// Why HTTP at all: the product story's customers use a web page. Nothing in
// the story says "HTTP" — a page in a browser simply has no other way to reach
// a service, so checkpoint #4 is where the mob discovers it.
//
// Note what this file does NOT contain: an entry point. The routes live in an
// extension method on IEndpointRouteBuilder rather than in a Program.cs, which
// means production owns the route definitions while both a real host and a
// test host can mount them. It also keeps this kata on a plain
// Microsoft.NET.Sdk project — the web SDK would insist on an entry point and
// collide with the one Microsoft.NET.Test.Sdk generates.
//
// In a real service, the whole of Program.cs is then:
//
//     var builder = WebApplication.CreateBuilder(args);
//     builder.Services.AddSingleton<IUrlRepository, SqliteUrlRepository>();
//     builder.Services.AddSingleton<IShortCodeGenerator, RandomShortCodeGenerator>();
//     builder.Services.AddSingleton(TimeProvider.System);
//     builder.Services.AddSingleton<UrlShortener>();
//     var app = builder.Build();
//     app.MapUrlEndpoints();
//     app.Run();
//
// WHAT MENU ITEM (e) BOUGHT — look at the catch clauses. They name
// UnknownCodeException and ArgumentException: the service's OWN vocabulary.
// Had SqliteUrlRepository not translated its errors at the boundary, these
// routes would have to catch SqliteException and switch on integer error codes
// from a database driver in order to choose a status code — and the day the
// store changed, every one of them would be silently wrong. That is the bill
// checkpoint #3 warned about, and it arrives here.

// The wire contract. Records rather than strings, so the tests exercise real
// JSON binding in both directions.
public record ShortenRequest(string Url);

public record ResolveResponse(string Url);

public record LinkResponse(string Code, string Url, DateTimeOffset CreatedAt);

public static class UrlEndpoints
{
    public static void MapUrlEndpoints(this IEndpointRouteBuilder app)
    {
        // One prefix, written once. Before the extended story "/links" appeared
        // in two templates; the list route would have made it three, and a
        // group means the one-character typo experiment still has exactly one
        // place to make the typo.
        var links = app.MapGroup("/links");

        // R15 / R18 / R24 — 201 Created, with a Location header pointing at the
        // new link, and the link itself in the same shape as a table row. (R24
        // came late: the page's contract wanted url and createdAt back, and
        // only the end-to-end smoke test ever put the two side by side.)
        //
        // R18 needs no special case HERE, and that is worth noticing:
        // because R6 chose "a fresh code each time", shortening an
        // already-known URL genuinely does create a new link. A mob that chose
        // "hand back the same code" has a different job on this line — 200 OK
        // and no Location — and that is a checkpoint-one decision arriving
        // much later, in something a customer can see.
        links.MapPost("/", (ShortenRequest request, UrlShortener shortener) =>
        {
            try
            {
                var link = shortener.Shorten(request.Url);
                return Results.Created(
                    $"/links/{link.Code}", new LinkResponse(link.Code, link.Url, link.CreatedAt));
            }
            catch (ArgumentException)
            {
                // R17 — the caller sent something that isn't a URL. Their
                // mistake, so 400 rather than 500.
                return Results.BadRequest("not a valid url");
            }
        });

        // R23 — the table. Newest first because the service says so; this
        // route only translates Link into the wire shape.
        links.MapGet("/", (UrlShortener shortener) =>
            Results.Ok(shortener.List().Select(link =>
                new LinkResponse(link.Code, link.Url, link.CreatedAt))));

        // R16 / R17 — 200 with the URL, or 404 if nobody ever minted that code.
        links.MapGet("/{code}", (string code, UrlShortener shortener) =>
        {
            try
            {
                return Results.Ok(new ResolveResponse(shortener.Resolve(code)));
            }
            catch (UnknownCodeException)
            {
                return Results.NotFound();
            }
        });
    }
}
