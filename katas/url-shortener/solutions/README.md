# URL Shortener — reference solutions

**Spoilers.** Keep this folder closed until the retro.

One finished implementation covering all three stories, with complete test
suites. This is **one possible end state**, not the only correct answer — the
point of the retro is to compare your mob's design against it and argue with its
choices.

| File | Story | What's in it |
|---|---|---|
| [`UrlShortener.cs`](csharp/UrlShortener.cs) | 1 | the service, the port, the generator interface, the domain errors |
| [`UrlRepositories.cs`](csharp/UrlRepositories.cs) | 1 + 2 | the hand-written fake, the SQLite store, the production generator |
| [`UrlRepositoryContractTests.cs`](csharp/UrlRepositoryContractTests.cs) | 2 | **the shared contract suite** — the point of the kata |
| [`UrlShortenerTests.cs`](csharp/UrlShortenerTests.cs) | 1 | the service's own rules, run against the fake |
| [`UrlEndpoints.cs`](csharp/UrlEndpoints.cs) | 3 | handlers as plain functions |
| [`UrlEndpointsTests.cs`](csharp/UrlEndpointsTests.cs) | 3 | status-code mapping, and nothing else |

## Design shown here

From the checkpoint-#1 pattern menu, this "team" implemented — one at a time,
per the coach's rule:

- **(a) Port + hand-written fake** — `IUrlRepository` in the domain's own
  vocabulary, with `InMemoryUrlRepository` behind it. Twelve lines, no library.
- **(b) Injected code generator** — the one unpredictable thing in the system,
  behind a seam, so tests name the exact code they expect.

And deliberately **declined** (reasons in each file's header comment):

- **(d) Try-pattern** — this team throws from `Resolve`, and throwing is
  unambiguous. `TryResolve` earns its place the day callers are writing
  try/catch around something that isn't exceptional. *(The Stack solution
  declined its own menu item (d) for exactly this reason.)*
- **(f) Short code as a value object** — at one call site, a `string` carries its
  weight. The moment a second rule about codes appears, this flips.

Two more were declined in story 1 and **earned later** — which is the most
useful thing in this folder:

- **(c) Shared contract tests** — correctly ceremony when one store exists.
  Earned the instant a second one does. See below.
- **(e) Domain vs infrastructure errors** — nothing forced it while the only
  store was one this team wrote. `SqliteUrlRepository` earns it, and story 3's
  handlers are where it gets paid.

## The contract choices, and why

Every one of these is a gap the story left open on purpose:

| | This solution | The other defensible answer |
|---|---|---|
| Same URL twice | a **fresh code** each time | hand back the existing code — then story 3 returns 200, not 201 |
| Unknown code | **throws** `UnknownCodeException` | a Try-pattern; then menu item (d) is taken, not declined |
| Blank / whitespace URL | **throws** `ArgumentException` | — |
| Scheme validation | **not this object's job** | validate here; but note `Uri.TryCreate` accepts `javascript:` |
| Case | **case-sensitive**, stated in one place per store | case-insensitive — but then *both* stores must be changed, which is the lesson |
| Collision | the store **refuses**; the service mints again | let it propagate; but then a caller must understand minting |

There is no parameterless `UrlShortener()` constructor, and that is deliberate:
a convenient in-memory default would let someone build a shortener that quietly
violates the story's central promise. Making storage a required argument means
that cannot happen by accident.

## What the database story adds — and what it breaks

`SqliteUrlRepository` implements the same interface. The service does not change
by a single line. What changes is that a second implementation now exists, and
the shared contract suite has something to compare.

**This is verifiable, and worth doing live in the retro.** Open
`UrlRepositories.cs` and replace the fake's `TryAdd` guard with the first draft
almost everyone writes:

```csharp
_links[code] = url;      // instead of the TryAdd guard
```

Then run `dotnet test`:

```
Failed  InMemoryUrlRepositoryContractTests.RefusesToSaveACodeThatIsAlreadyTaken
Failed  InMemoryUrlRepositoryContractTests.DoesNotLoseTheOriginalLinkWhenACodeIsRefused
Failed  UrlShortenerTests.MintsAnotherCodeWhenTheGeneratorRepeatsItself
Failed  UrlShortenerTests.GivesUpRatherThanLoopingForeverWhenEveryCodeIsTaken
Failed!  - Failed: 4, Passed: 24
```

Four failures, **every one of them on the in-memory path. Not one SQLite test
fails** — the database was refusing that write all along. The identical test,
written once, passes against the real store and fails against the fake.

That is the entire thesis of the kata, on demand, in fifteen seconds. A team
that had only ever tested against its own fake would have shipped the green
suite and lost a URL.

The second, quieter version of the same gap is case sensitivity: a `Dictionary`
built with `StringComparer.OrdinalIgnoreCase` looks perfectly reasonable and
disagrees with SQLite's default `BINARY` collation. `TreatsCodesAsCaseSensitive`
is the only thing in the codebase that would ever say so.

## What the HTTP story adds

Handlers as plain functions — no server, no routing, no JSON. `UrlEndpoints`
catches `UnknownCodeException` and `ArgumentException`: **the service's own
vocabulary**. That is menu item (e) being paid for. Without it, these handlers
would have to catch `SqliteException` and switch on database error codes to
choose between 404 and 500 — and would all break the day the store changed.

Count the tests: six here, eight in the service suite, six in the contract suite
running twice. The mob derives the test pyramid by counting rather than being
shown a diagram. The story is also honest that this layer is itself a *fake of
HTTP* with its own fidelity gap — the same shape of problem, one level up.

## Running the finished state

These use `Shorten()`/`Resolve()`, throw on an unknown code, and mint a fresh
code per shortening. If your mob picked the same shape they're drop-in:

```bash
# from katas/url-shortener/
cp solutions/csharp/*.cs csharp/
(cd csharp && dotnet test)
```

`SqliteTestDatabase.cs` is **not** in this folder on purpose — it ships in
`csharp/` as given infrastructure, so the copy above leaves it untouched rather
than creating a second copy that drifts.

Verified on .NET 8 with `Microsoft.Data.Sqlite` 8.0.8: **28/28 passing**.

(Use a scratch copy of the repo if you want to keep the skeleton pristine — the
copy above overwrites `csharp/UrlShortener.cs` and `csharp/UrlShortenerTests.cs`.)
