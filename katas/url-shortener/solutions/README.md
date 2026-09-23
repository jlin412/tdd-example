# URL Shortener — reference solutions

**Spoilers.** Keep this folder closed until the retro.

There are **two complete solutions** to this kata. They satisfy the same three
stories and the same numbered rules in `facilitator/RULES.md`, and they disagree
about almost everything else — because they were driven by different kinds of
test.

| | [**Solution 1 — Isolated**](Solution1-Isolated.md) | [**Solution 2 — Integrated**](Solution2-Integrated.md) |
|---|---|---|
| Code | [`csharp-isolated/`](csharp-isolated/) | [`csharp-integrated/`](csharp-integrated/) |
| Direction | inside-out — domain first | outside-in — endpoint first |
| Storage under test | a hand-written fake, plus SQLite in a contract suite | real SQLite, always |
| Doubles | a fake repository **and** a stub generator | a stub generator, and only for one test |
| Repository interface | yes — `IUrlRepository` | **none** — nothing ever forced one |
| Source files | 6 | 3 |
| Test classes | 3 | 1 |
| Test executions | 30 | 13 |
| Whole suite | ~360–680 ms | ~480 ms |
| **Fast subset** | **22 tests in ~10 ms** | **none — every test ~37 ms** |
| Can prove "survives a restart" | no | **yes** |
| Can simulate a database failure | yes | no |

Neither is the right answer. Having both is the point: the mob compares its own
design against two defensible end states and has to say which trade it made and
why.

## Read these first

- **[Solution1-Isolated.md](Solution1-Isolated.md)** — test order, rationale,
  pros and cons.
- **[Solution2-Integrated.md](Solution2-Integrated.md)** — the same, plus the
  honest synthesis at the end (including a third design neither file shows).

## The two things worth demonstrating live

Both take fifteen seconds and settle arguments that otherwise run on taste.

**1. A fake can lie — but only solution 1 can have this bug.**
In `csharp-isolated/UrlRepositories.cs`, replace the fake's `TryAdd` guard with
the first draft nearly everyone writes:

```csharp
_links[code] = url;      // instead of the TryAdd guard
```

```
Failed  InMemoryUrlRepositoryContractTests.RefusesToSaveACodeThatIsAlreadyTaken
Failed  InMemoryUrlRepositoryContractTests.DoesNotLoseTheOriginalLinkWhenACodeIsRefused
Failed  UrlShortenerTests.MintsAnotherCodeWhenTheGeneratorRepeatsItself
Failed  UrlShortenerTests.GivesUpRatherThanLoopingForeverWhenEveryCodeIsTaken
Failed!  - Failed: 4, Passed: 24
```

Four failures, **every one on the in-memory path; not a single SQLite test
fails** — the database was refusing that write all along. The identical test,
written once in the shared contract suite, passes against the real store and
fails against the double. Solution 2 cannot reproduce this at all, because it
has nothing to lie to it.

**2. The transport catches what nothing else can.**
In either solution's `UrlEndpoints.cs`, change `"/links"` to `"/lnks"` — one
character. In solution 1, **seven tests fail and every one is in
`UrlEndpointsTests`**; the service and contract suites do not notice, because
nothing about the domain changed. The service is perfect and completely
unreachable. Tests that stopped short of the transport would have shipped that
with a green board.

## Design choices common to both

These are contract decisions the stories deliberately left open. A different mob
may defensibly have gone the other way — and if it did, neither solution drops
in verbatim, which is the interesting part of the retro.

| | Both solutions | The other defensible answer |
|---|---|---|
| Same URL twice | a **fresh code** each time | hand back the existing code — then story 3 returns 200, not 201 |
| Unknown code | **throws**, mapped to 404 at the edge | a Try-pattern; then menu item (d) is taken, not declined |
| Blank / whitespace URL | **throws**, mapped to 400 | — |
| Scheme validation | **not this object's job** | validate here; note `Uri.TryCreate` accepts `javascript:` |
| Case | **case-sensitive** | case-insensitive — but in solution 1 that means changing *both* stores, which is the lesson |
| Collision | the store **refuses**; the service mints again | let it propagate; but then callers must understand minting |

## Running them

Each solution replaces the skeleton's source. **Copy one or the other, never
both** — they each define `UrlShortener` and would collide.

```bash
# from katas/url-shortener/ — over a scratch copy to keep the skeleton pristine
cp solutions/csharp-isolated/*.cs   csharp/ && (cd csharp && dotnet test)   # 30 passing
# or
cp solutions/csharp-integrated/*.cs csharp/ && (cd csharp && dotnet test)   # 13 passing
```

`csharp/SqliteTestDatabase.cs` is **not** in either solution folder on purpose —
it ships in `csharp/` as given infrastructure, so the copies above leave it
untouched. Both solutions use it.

Verified on .NET 8 with `Microsoft.Data.Sqlite` 8.0.8 and
`Microsoft.AspNetCore.TestHost` 8.0.8: solution 1 **30/30**, solution 2
**13/13**, no warnings in either.

Keep them closed until the retro. The solutions deliberately *show their
judgment* — which patterns they took, which they declined, and why — so the retro
can argue with them.
