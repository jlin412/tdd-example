# URL Shortener — the HTTP Story

You've shipped [the shortener](UrlShortenerStory.md) and made it
[actually persist](UrlShortenerDatabaseStory.md). Now it needs a front door:
other people's software has to be able to call it.

## What changes

Two endpoints, speaking JSON, and nothing else:

| request | on success | body |
|---|---|---|
| `POST /links` with `{ "url": "https://…" }` | **201 Created**, plus a `Location` header pointing at the new link | `{ "code": "k3f9q2" }` |
| `GET /links/{code}` | **200 OK** | `{ "url": "https://…" }` |

And the unhappy paths, which are the actual content of this story:

| request | what should come back |
|---|---|
| `GET /links/nope99` — a code nobody minted | **404 Not Found** |
| `POST /links` with something that isn't a URL | **400 Bad Request** |
| `POST /links` with a URL you've already shortened | *whatever you decided in story 1* |

That last row is not a cop-out. In story 1 you chose whether shortening the same
URL twice hands back the same code or a fresh one. If you chose *the same code*,
this is a **200 OK** with the existing code and no `Location` — nothing was
created. If you chose *a fresh one*, it's another **201**. A mob that never made
that decision can't answer this, and now they have to.

## What you're building

Real routes, on a real server. Your tests will start an in-process host and call
it with a real `HttpClient` — routing, model binding, JSON serialization, status
codes and headers all genuinely exercised. Nothing is simulated, no port is
opened, and there is nothing to install.

One structural note, because it's the thing that makes this pleasant: put the
route definitions in an **extension method**, not in a `Program.cs`.

```csharp
public static void MapUrlEndpoints(this IEndpointRouteBuilder app)
{
    app.MapPost("/links", (ShortenRequest request, UrlShortener shortener) => { ... });
    app.MapGet("/links/{code}", (string code, UrlShortener shortener) => { ... });
}
```

Production owns the routes; a real host and a test host can both mount them. In
a real service the entire `Program.cs` is then:

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IUrlRepository, SqliteUrlRepository>();
builder.Services.AddSingleton<IShortCodeGenerator, RandomShortCodeGenerator>();
builder.Services.AddSingleton<UrlShortener>();
var app = builder.Build();
app.MapUrlEndpoints();
app.Run();
```

and your test spins up the same thing with `UseTestServer()`, swapping in the
stub generator you already wrote back in story 1. That swap is menu item (b)
paying for itself a third time.

## Why this earns a new pattern

Look at what a 404 and a 400 actually require of you. The route has to tell
"nobody minted that code" apart from "that isn't a URL" apart from "the database
is on fire" — and map each to a different number.

If your service still lets a database exception escape, it cannot. The route
would have to catch an infrastructure type and pattern-match on an error code
from a storage engine it was never supposed to know existed. Swap SQLite for
Postgres one day and every one of those routes is wrong.

That's pattern-menu item **(e)**, and it's why story 2 asked you to translate
errors at the boundary. If you declined it then, this is the bill arriving. Drive
it test-first and watch a pattern you passed on become the obvious call.

**Requirements, not fashion, justify a pattern.**

## Your job, in order

1. **Decide what belongs here.** You have a suite that already proves two URLs
   get different codes, that resolve round-trips, that a blank URL is refused,
   that a repeated code never loses a link. How many of those should be repeated
   through HTTP?

   The answer is **none of them**, and being able to say why is the point of this
   story. Re-proving a domain rule through a web server is a slower, more
   indirect copy of a test you already have, sitting further from the code that
   decides it. What's genuinely new at this level is the **wiring**: that a
   request reaches the right handler and comes back as the right status code.
2. **Write the list**, then drive it one red at a time. It should be short.
3. **At the end, count** — your HTTP tests against your service tests against
   your contract tests. That shape has a name, and you just derived it instead of
   being shown it.

## Prove what the transport is worth

Two tests exist in the reference that could not have been written at all if you
had tested the handlers as plain functions. Write your own versions:

- **A misspelled route.** Call a path that doesn't exist and assert a 404. No
  domain code is involved — it passes only because a real router looked at a real
  request and found nothing.
- **A malformed JSON body.** Send `{ not json` and assert a 400. ASP.NET Core
  rejects it before a single line of your code runs. Sit with that: an entire
  class of failure is handled by the framework, and you'd have no idea whether it
  was handled *well* without a test that speaks HTTP.

Then run the experiment that settles the argument. Open your routes file and
**change `/links` to `/lnks`** — one character. Run everything.

In the reference, that typo turns **seven tests red, and every single one of
them is in the HTTP suite.** The service suite and the contract suite don't
notice at all, because nothing about the domain changed. The service is perfect
and completely unreachable.

That is exactly the class of bug you'd have shipped with a green board if these
tests stopped short of the transport — and it's the same shape of problem story 2
detonated, one layer up: a double that agrees with you right until something real
disagrees.

## And what it costs

Run the suite and watch the clock. These tests are roughly **an order of
magnitude slower** than the service tests — each one builds a host, starts a
server and round-trips real HTTP. In the reference that's the difference between
a suite that finishes in about 200ms and one that takes about 600ms.

That is the whole argument for the pyramid, in numbers rather than assertion:
tests at this level catch things nothing else can, and you cannot afford many of
them. Put each test at the *lowest* level that can still catch what you're
worried about — and be able to say which level that is, out loud, before you
write it.

## Where things live

- Reference solution:
  [solutions/csharp/UrlEndpoints.cs](solutions/csharp/UrlEndpoints.cs) +
  [solutions/csharp/UrlEndpointsTests.cs](solutions/csharp/UrlEndpointsTests.cs)
