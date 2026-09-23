# URL Shortener — the Story (standard)

Build a link shortener — the thing that turns a sprawling URL into something you
can read down a phone line — but grow it **test-first**, one behavior at a time.
Everything you need is below; nothing here is a numbered requirement list,
because turning this into a list of tests is your first job.

## The service

People hand it a long URL and get back a **short code** — half a dozen
characters, nothing more. Later, anyone holding that code can hand it back and
get the **original URL** returned to them.

That's the whole product. Two things you can do with it:

- **shorten** — give it a long URL, get a short code.
- **resolve** — give it a short code, get the long URL back.

Codes are short and URL-safe. Exactly how long, and out of which characters, is
the mob's call — as is what the class is called and what the two operations are
named. `shorten` and `resolve` are the ordinary words for this, so feel free to
keep them.

One thing is not negotiable, because it's the entire reason anybody would use
this: **a short link keeps working.** Paste one into a document, come back to it
a week later — after the service has been restarted, and quite possibly on a
different server from the one that minted it — and it still takes you to the
right place.

## A walk through it

| you do this | you get back | the store now holds |
|---|---|---|
| shorten `https://example.com/a/very/long/path` | a short code, say `k3f9q2` | `k3f9q2` → `…/a/very/long/path` |
| resolve `k3f9q2` | `https://example.com/a/very/long/path` | unchanged |
| shorten `https://example.com/somewhere/else` | a **different** code, say `p8w1zt` | both links |
| resolve `p8w1zt` | `https://example.com/somewhere/else` | unchanged |
| resolve `nope99` | *nothing — that code was never issued* | unchanged |
| shorten the **first** URL again | **?** *(the story doesn't say)* | **?** |

Read that table carefully before writing anything. The third row is the only
reason you can tell a real shortener from one that hands everybody the same
answer — and the last row is a decision nobody has made yet.

## What the story doesn't say

Deliberate gaps. They're yours to decide as a mob — then encode each decision as
a test:

- **The same URL, twice.** The same code back, or a fresh one? Both are real
  products that real companies ship. Pick one, and notice you've just made a
  promise about what gets stored.
- **A code nobody minted.** What should resolving `nope99` do? Throw? Hand back
  nothing? Something else? There's no universal answer — and whatever you pick,
  `resolve` should behave the same way every time it happens.
- **A URL that isn't one.** Is `""` a URL? `"   "`? `"banana"`? And is checking
  that even *this* object's job, or does it belong to whoever calls it?
- **Upper and lower case.** Is `K3F9Q2` the same link as `k3f9q2`? Ask the
  follow-up too: wherever you answer that, is it written down in one place, or
  does it just happen to be however your storage behaves?
- **Two codes colliding.** Whatever mints your codes, nothing in the universe
  guarantees it never produces the same one twice. What should happen if it
  does — and would you find out, or would it be silent?

There is no single correct contract. What matters is that you **choose, and your
tests express the choice.**

## The loop

1. **RED** — write ONE failing test. Run it. Watch it fail for the right reason.
2. **GREEN** — write the *minimum* production code to pass. Nothing more.
3. **REFACTOR** — improve the code (and tests) while staying green. Then repeat.

Before any of that, though: write the **test list** (STEP 0 in the test file).

One thing that's new here: this is the first kata in the repo where the thing
you're building will need **something else** to do its job. Nobody is going to
tell you when that moment arrives. A test you find you *cannot write* is what
tells you — so when you hit one, stop and look at it rather than working around
it.

## Where things live

- Skeleton to fill in: [csharp/UrlShortener.cs](csharp/UrlShortener.cs) +
  [csharp/UrlShortenerTests.cs](csharp/UrlShortenerTests.cs)
- Reference solution (reveal at the end):
  [solutions/csharp-isolated/](solutions/csharp-isolated/) — and a second,
  very different one in [solutions/csharp-integrated/](solutions/csharp-integrated/).
  [solutions/README.md](solutions/README.md) compares them.
- Run the tests: from `csharp/`, `dotnet test`.

(There's a `SqliteTestDatabase.cs` sitting in that folder. Ignore it — it
belongs to the next story, and nothing here needs it.)

## What next

Shipped the shortener? Then it has to become **true**. See
[UrlShortenerDatabaseStory.md](UrlShortenerDatabaseStory.md).
