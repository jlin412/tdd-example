using Microsoft.AspNetCore.Builder;   // where MapGet/MapPost actually live
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Kata;

// URL Shortener kata — reference solution 2 of 2: INTEGRATED (story 3).
// See ../Solution2-Integrated.md.
//
// These routes are byte-for-byte the same decisions as solution 1's, which is
// the point worth noticing: the HTTP layer is where the two solutions AGREE.
// Both map an unknown code to 404 and a bad URL to 400; both hand back 201 with
// a Location header. The designs diverge underneath, not at the edge.
//
// One difference, and it is the coupling surfacing again. Solution 1's routes
// catch UnknownCodeException and ArgumentException — the service's own
// vocabulary — because a repository translated SQLite's errors at the boundary.
// These routes catch the same two types, but only because UrlShortener happens
// to have swallowed the SqliteException itself. The translation still occurs;
// it just occurs inside the domain object instead of at a seam. If a THIRD
// kind of database error ever needed distinct handling up here, solution 1 has
// a place to put it and this design does not.
public record ShortenRequest(string Url);

public record ShortenResponse(string Code);

public record ResolveResponse(string Url);

public static class UrlEndpoints
{
    public static void MapUrlEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/links", (ShortenRequest request, UrlShortener shortener) =>
        {
            try
            {
                var code = shortener.Shorten(request.Url);
                return Results.Created($"/links/{code}", new ShortenResponse(code));
            }
            catch (ArgumentException)
            {
                return Results.BadRequest("not a valid url");
            }
        });

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
