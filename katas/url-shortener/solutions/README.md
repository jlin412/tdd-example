# URL Shortener — reference solutions

**Spoilers.** Keep this folder closed until the retro.

- **Backend track:** two complete solutions, [isolated](#backend--two-solutions)
  and integrated — opposing answers to "what should a test touch?".
- **Frontend track:** [one solution](#frontend--one-solution), the page and its
  table, driven against a hand-written fake.

## Backend — two solutions

They satisfy the same story, the same extended story and the same R-rules in
`facilitator/RULES.md`, and they disagree about almost everything else — because
they were driven by different kinds of test.

| | [**Solution 1 — Isolated**](Solution1-Isolated.md) | [**Solution 2 — Integrated**](Solution2-Integrated.md) |
|---|---|---|
| Code | [`csharp-isolated/`](csharp-isolated/) | [`csharp-integrated/`](csharp-integrated/) |
| Direction | inside-out — domain first | outside-in — endpoint first |
| Storage under test | a hand-written fake, plus SQLite in a contract suite | real SQLite, always |
| Doubles | a fake repository, a stub generator, a fake clock | a stub generator and a fake clock, each for one named test |
| Repository interface | yes — `IUrlRepository` | **none** — nothing ever forced one |
| Source files | 6 | 3 |
| Test classes | 3 | 1 |
| Test executions | 39 | 19 |
| Whole suite | ~200–215 ms | ~250–270 ms |
| **Fast subset** | **29 tests in ~10 ms** | **none — every test starts a host** |
| Can prove "survives a restart" | no | **yes** |
| Can simulate a database failure | yes | no |
| Can find a concurrency defect | no | **yes — it did** |

Neither is the right answer. Having both is the point: the mob compares its own
design against two defensible end states and has to say which trade it made and
why.

**How the numbers were measured** — .NET SDK 8.0.129 on macOS (arm64).
Suite times are `dotnet test` over five runs. Per-test costs are medians of 50
warmed-up iterations of one test's worth of setup: a `TestServer` host built,
started, sent one request and disposed costs **~9 ms**; a `SqliteTestDatabase`
created and written to once costs **~0.1 ms** — about **85×** apart. Re-measure
rather than reword these if the code changes.

### Read these first

- **[Solution1-Isolated.md](Solution1-Isolated.md)** — test order, rationale,
  pros and cons.
- **[Solution2-Integrated.md](Solution2-Integrated.md)** — the same, plus the
  honest synthesis at the end (including a third design neither file shows).

### The two things worth demonstrating live

Both take fifteen seconds and settle arguments that otherwise run on taste.

**1. A fake can lie — but only solution 1 can have this bug.**
In `csharp-isolated/UrlRepositories.cs`, replace the fake's `TryAdd` guard with
the first draft nearly everyone writes:

```csharp
_links[link.Code] = new Entry(link, ++_saved);   // instead of the TryAdd guard
```

```
Failed  UrlShortenerTests.GivesUpRatherThanLoopingForeverWhenEveryCodeIsTaken
Failed  InMemoryUrlRepositoryContractTests.RefusesToSaveACodeThatIsAlreadyTaken
Failed  InMemoryUrlRepositoryContractTests.DoesNotLoseTheOriginalLinkWhenACodeIsRefused
Failed  UrlShortenerTests.MintsAnotherCodeWhenTheGeneratorRepeatsItself
Failed!  - Failed: 4, Passed: 35
```

Four failures, **every one on the in-memory path; not a single SQLite test
fails** — the database was refusing that write all along. The identical test,
written once in the shared contract suite, passes against the real store and
fails against the double. Solution 2 cannot reproduce this at all, because it
has nothing to lie to it.

**2. The transport catches what nothing else can.**
In either solution's `UrlEndpoints.cs`, change the `"/links"` route group to
`"/lnks"` — one character. In solution 1, **nine tests fail and every one is in
`UrlEndpointsTests`** (all but the 404 for an unknown code, which is a 404
either way); the service and contract suites do not notice, because nothing
about the domain changed. The service is perfect and completely unreachable.
Tests that stopped short of the transport would have shipped that with a green
board. (In solution 2 every test goes through HTTP, so 18 of 19 fail — the same
fact, with nothing left over to tell you the domain is fine.)

A third, from the extended story: **delete either store's tie-break** (the
`ThenByDescending` in the fake, `rowid DESC` in SQLite) and exactly one test
fails — the same-instant ordering test, in that store's run of the contract
suite. Sorting by time alone passes every test with distinct times.

### Design choices common to both

These are contract decisions the story deliberately left open. A different mob
may defensibly have gone the other way — and if it did, neither solution drops
in verbatim, which is the interesting part of the retro.

| | Both solutions | The other defensible answer |
|---|---|---|
| Same URL twice | a **fresh code** each time | hand back the existing code — then HTTP returns 200, not 201, and the table shows one row |
| Unknown code | **throws**, mapped to 404 at the edge | a Try-pattern; then menu item (d) is taken, not declined |
| Blank / whitespace URL | **throws**, mapped to 400 | — |
| Scheme validation | **not this object's job** | validate here; note `Uri.TryCreate` accepts `javascript:` |
| Case | **case-sensitive** | case-insensitive — but in solution 1 that means changing *both* stores, which is the lesson |
| Collision | the store **refuses**; the service mints again | let it propagate; but then callers must understand minting |
| Two links, one instant | the **later-saved** lists first | earlier first — any rule, as long as both stores keep it |
| Creation time | **UTC ticks**, stamped by the service from an injected `TimeProvider` | let the database default it — then no test can say what time it is |
| What a create answers | **the whole link** — code, url, createdAt, the shape of a table row (R24) | the code alone — which is what both solutions did until the [smoke test](../smoke/) put the page's contract beside them |

### Running them

Each solution replaces the skeleton's source. **Copy one or the other, never
both** — they each define `UrlShortener` and would collide.

```bash
# from katas/url-shortener/ — over a scratch copy to keep the skeleton pristine
cp solutions/csharp-isolated/*.cs csharp/ && (cd csharp && dotnet test)   # 39 passing

# or, in a different fresh copy — the skeleton's own UrlShortenerTests.cs would
# not compile against solution 2's constructor, so it goes first:
rm csharp/UrlShortenerTests.cs && cp solutions/csharp-integrated/*.cs csharp/ \
  && (cd csharp && dotnet test)                                            # 19 passing
```

`csharp/SqliteTestDatabase.cs` is **not** in either solution folder on purpose —
it ships in `csharp/` as given infrastructure, so the copies above leave it
untouched. Both solutions use it.

Verified on .NET 8 with `Microsoft.Data.Sqlite` 8.0.8 and
`Microsoft.AspNetCore.TestHost` 8.0.8: solution 1 **39/39**, solution 2
**19/19**, no warnings in either.

## Frontend — one solution

Code: [`angular/`](angular/). The page is driven through the DOM; the service is
the given contract (`link-service.ts`), and the stand-in is a hand-written fake.

| File | What it is |
|---|---|
| [`shortener-page.ts`](angular/shortener-page.ts) + [`.spec.ts`](angular/shortener-page.spec.ts) | the story: the bar (F1–F6), 13 tests |
| [`in-memory-link-service.ts`](angular/in-memory-link-service.ts) | the fake — remembers what it was asked, fails on demand, holds its answers back |
| [`link-table.ts`](angular/link-table.ts) | the extended story's presentational table |
| [`shortener-page-table.ts`](angular/shortener-page-table.ts) + [`.spec.ts`](angular/shortener-page-table.spec.ts) | the extended story (F7–F10) on the same selector, 7 tests |

**Pattern-menu picks:** **(c) computed** — what the short box shows is derived,
which makes F6 a one-liner; **(e) a reusable stand-in** — the fake moved into its
own file once a second spec needed it. **(a) smart/dumb split** was declined for
the bar and earned by the table. **(b) signal store** and **(d) observables**
declined, with reasons in the file headers.

**Fake, not mock — and what that costs.** The fake made "what was the service
asked?" an array assertion and "the page while it waits" a `hold()` /
`release()` pair. What no stand-in can do is disagree with the real shortener:
nothing in this track checks that the backend keeps the contract's promises.
The [smoke test](../smoke/) is the one place that does — and the first time it
ran, the backend did *not* keep them: `POST /links` answered with a bare code,
where `create()` promises the whole link. Every suite in both tracks was green.
That became R24.

**One trap it had to solve.** `whenStable()` waits only for work Angular knows
about, and a promise the fake resolves is not that. The specs' `settle()` helper
lets pending promises run before asking Angular to render; with a plain
`whenStable()` after `release()`, the "enabled again after the answer" test
failed — the page had not seen the answer yet, which is the wrong reason.

**Worth demonstrating** — each mutation fails exactly the tests you'd hope:

| Change in the reference | Fails |
|---|---|
| drop the `waiting()` guard in `create()` | only *Enter pressed again while waiting* — the disabled button still stops clicks |
| drop `[disabled]` on Create | only *disables Create while the service is working* |
| store the short URL instead of deriving it | only *never leaves a short URL beside a long URL it was not made for* |
| append new links instead of prepending | *puts a link you create on top*, and the same-URL-twice table test |

```bash
# from katas/url-shortener/, over a scratch copy
cp solutions/angular/*.ts angular/src/app/ && (cd angular && npx ng test --watch=false)   # 20 passing
```

`link-service.ts` is not in the solution folder — it ships in the skeleton as
the given contract, so the copy leaves it in place. The story-1 page and the
table page share the selector `app-shortener-page`; `main.ts` bootstraps the
story-1 one.

Verified with Angular v21.2 + Vitest 4: **13/13** and **7/7**, and `ng build`
compiles both pages under strict templates.

Keep them closed until the retro. The solutions deliberately *show their
judgment* — which patterns they took, which they declined, and why — so the retro
can argue with them.
