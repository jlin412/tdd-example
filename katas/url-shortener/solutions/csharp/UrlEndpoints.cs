namespace Kata;

// URL Shortener kata — reference solution (story 3: HTTP, R15–R18).
// See UrlShortenerHttpStory.md for the requirement.
//
// These are plain functions. No web server starts, nothing is routed, no JSON
// is parsed. That is deliberate — and the story is honest about the cost: this
// is a FAKE of HTTP, with a fidelity gap of exactly the kind story 2 was about.
// What it does contain is the part with decisions in it: turning an outcome
// into a status code.
//
// In a real minimal API the wiring around these is about this thick:
//
//     app.MapPost("/links", async (HttpRequest req) =>
//         Send(UrlEndpoints.Shorten(await ReadBody(req), shortener)));
//     app.MapGet("/links/{code}", (string code) =>
//         Send(UrlEndpoints.Resolve(code, shortener)));
//
// WHAT MENU ITEM (e) BOUGHT — look at the catch clauses below. They name
// UnknownCodeException and ArgumentException: the service's OWN vocabulary.
// Had SqliteUrlRepository not translated its errors at the boundary, this file
// would have to catch SqliteException and switch on integer error codes from a
// database driver in order to choose a status code — and the day the store
// changed, every handler here would be silently wrong. That is the bill story 2
// warned about, and it arrives here.
public record HttpOutcome(int Status, string? Body = null, string? Location = null);

public static class UrlEndpoints
{
    // R15 / R18 — 201 Created, every time.
    //
    // R18 needs no special case HERE, and that is worth noticing: because R6
    // chose "a fresh code each time", shortening an already-known URL genuinely
    // does create a new link. A mob that chose "same code back" has a different
    // job on this line — 200 OK, no Location — and that difference is a
    // decision from story 1 arriving three sessions later.
    public static HttpOutcome Shorten(string requestBody, UrlShortener shortener)
    {
        try
        {
            var code = shortener.Shorten(requestBody);
            return new HttpOutcome(201, code, $"/links/{code}");
        }
        catch (ArgumentException)
        {
            // R17 — the caller sent something that isn't a URL. Their mistake,
            // so 400 rather than 500.
            return new HttpOutcome(400, "not a valid url");
        }
    }

    // R16 / R17 — 200 with the URL, or 404 if nobody ever minted that code.
    public static HttpOutcome Resolve(string code, UrlShortener shortener)
    {
        try
        {
            return new HttpOutcome(200, shortener.Resolve(code));
        }
        catch (UnknownCodeException)
        {
            return new HttpOutcome(404, "no such link");
        }
    }
}
