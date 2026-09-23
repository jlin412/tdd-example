using Microsoft.AspNetCore.Builder;   // where MapGet/MapPost actually live
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Kata;

// URL Shortener kata — reference solution (story 3: HTTP, R15–R19).
// See UrlShortenerHttpStory.md for the requirement.
//
// These are REAL minimal-API routes. The tests next door start an in-process
// server and drive them with a real HttpClient, so routing, model binding,
// JSON serialization, status codes and headers are all genuinely exercised —
// a typo in a route template below fails a test.
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
// story 2 warned about, and it arrives here.

// The wire contract. Records rather than strings, so the tests exercise real
// JSON binding in both directions.
public record ShortenRequest(string Url);

public record ShortenResponse(string Code);

public record ResolveResponse(string Url);

public static class UrlEndpoints
{
    public static void MapUrlEndpoints(this IEndpointRouteBuilder app)
    {
        // R15 / R18 — 201 Created, with a Location header pointing at the new
        // link. R18 needs no special case HERE, and that is worth noticing:
        // because R6 chose "a fresh code each time", shortening an
        // already-known URL genuinely does create a new link. A mob that chose
        // "hand back the same code" has a different job on this line — 200 OK
        // and no Location — and that is a story-1 decision arriving three
        // sessions later, in something a customer can see.
        app.MapPost("/links", (ShortenRequest request, UrlShortener shortener) =>
        {
            try
            {
                var code = shortener.Shorten(request.Url);
                return Results.Created($"/links/{code}", new ShortenResponse(code));
            }
            catch (ArgumentException)
            {
                // R17 — the caller sent something that isn't a URL. Their
                // mistake, so 400 rather than 500.
                return Results.BadRequest("not a valid url");
            }
        });

        // R16 / R17 — 200 with the URL, or 404 if nobody ever minted that code.
        app.MapGet("/links/{code}", (string code, UrlShortener shortener) =>
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
