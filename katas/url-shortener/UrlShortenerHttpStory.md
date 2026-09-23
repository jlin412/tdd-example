# URL Shortener — the HTTP Story

You've shipped [the shortener](UrlShortenerStory.md) and made it
[actually persist](UrlShortenerDatabaseStory.md). Now it needs a front door:
other people's software has to be able to call it.

## What changes

Two endpoints, and nothing else:

| request | on success | the body |
|---|---|---|
| `POST /links` with the long URL | **201 Created**, and a `Location` header pointing at the new link | the short code |
| `GET /links/{code}` | **200 OK** | the original URL |

And the unhappy paths, which are the actual content of this story:

| request | what should come back |
|---|---|
| `GET /links/nope99` — a code nobody minted | **404 Not Found** |
| `POST /links` with something that isn't a URL | **400 Bad Request** |
| `POST /links` with a URL you've already shortened | *whatever you decided in story 1* |

That last row is not a cop-out. In story 1 you chose whether shortening the same
URL twice hands back the same code or a fresh one. If you chose *the same code*,
this is a **200 OK** with the existing code — nothing was created. If you chose
*a fresh one*, it's another **201**. A mob that never made that decision can't
answer this, and now they have to.

## What you're building

A pair of plain functions. Each takes what a request carries, asks the shortener
to do the work, and turns the answer into a status code and a body:

```csharp
public record HttpOutcome(int Status, string? Body = null, string? Location = null);

// yours to name and shape
HttpOutcome Shorten(string requestBody, UrlShortener shortener);
HttpOutcome Resolve(string code, UrlShortener shortener);
```

No web server starts. No routing, no JSON, no framework — just the mapping,
which is the part with decisions in it. In a real app these functions are what a
minimal API hands off to, and the wiring around them is about this thick:

```csharp
// not part of the kata — just so you can see where your functions plug in
app.MapPost("/links", async (HttpRequest req) => Shorten(await ReadBody(req), shortener));
app.MapGet("/links/{code}", (string code) => Resolve(code, shortener));
```

## Why this earns a new pattern

Look at what a 404 and a 409 actually require of you. The handler has to tell
"nobody minted that code" apart from "that code is already taken" apart from
"the database is on fire" — and map each to a different number.

If your service still lets a database exception escape, the handler cannot do
that. It would have to catch an infrastructure type and pattern-match on an
error code from a storage engine it was never supposed to know existed. Swap
SQLite for Postgres one day and every one of those handlers is wrong.

That's pattern-menu item **(e)**, and it's why story 2 asked you to translate
errors at the boundary. If you declined it then, this is the bill arriving. Drive
it test-first and watch a pattern you passed on become the obvious call.

**Requirements, not fashion, justify a pattern.**

## Your job, in order

1. **Decide what belongs here.** You have a suite that already proves two URLs
   get different codes, that resolve round-trips, that a bad URL is rejected.
   How many of those tests should be repeated through the handler?
   The answer is **none of them**, and being able to say why is the point of this
   story. A handler test that re-proves domain rules is a slow, indirect copy of
   a test you already have. What's genuinely new here — what no existing test
   covers — is the **mapping**: outcome in, status code out.
2. **Write the list**, then drive it one red at a time. It should be short.
3. **At the end, count.** How many handler tests did you need? Compare that to
   how many domain tests you have. That shape — a lot at the bottom, few at the
   top — has a name, and you just derived it rather than being shown it.

## One more thing, before you feel too good about this

These tests never start a server. They don't exercise routing, model binding,
JSON serialization, headers, or middleware. Every one of them could pass while
the real thing 404s on every request because a route template has a typo.

So: **this is a fake of HTTP, and it has its own fidelity gap.**

That should feel familiar. It's exactly the shape of the gap story 2 detonated,
one layer up — a double that agrees with you, right until something real
disagrees.

The retro question, then:

> What would a test against a real in-process server catch that these cannot?
> Is that the same *kind* of gap your in-memory repository had — and what's the
> equivalent of a shared contract suite at this level?

There's a good answer, and "write some of both" is most of it. The useful part is
noticing that you now recognise the problem on sight.

## Where things live

- Reference solution:
  [solutions/csharp/UrlEndpoints.cs](solutions/csharp/UrlEndpoints.cs) +
  [solutions/csharp/UrlEndpointsTests.cs](solutions/csharp/UrlEndpointsTests.cs)
