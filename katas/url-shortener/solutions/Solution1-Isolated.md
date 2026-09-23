# Solution 1 — Isolated (separated) tests

**Spoilers.** Keep this closed until the retro.

Code: [`csharp-isolated/`](csharp-isolated/) · Compare with
[Solution2-Integrated.md](Solution2-Integrated.md).

This is the **inside-out** solution. It starts at the domain, tests each piece
against doubles, and works outward to the database and then the web. It is the
design the kata's checkpoints and stories are written to produce.

## The test order, and why each step is where it is

Each line is one red bar. The rationale matters more than the list.

| # | Test | What it forces | Where it runs |
|---|---|---|---|
| 1 | shortening a URL gives back a code that resolves to it | the two methods exist. Minimum green is a hard-coded pair | service, no collaborators |
| 2 | two different URLs get two different codes | the hard-coded pair dies; a `Dictionary` and some way to mint codes appear. A counter is a legitimate first answer | service |
| 3 | resolving a code nobody minted | contract decision: throw. First deliberate choice, not an accident | service |
| 4 | the same URL twice mints a fresh code | contract decision: idempotency. Comes back in story 3 as a status code | service |
| 5 | a blank URL is refused | the negative column | service |
| — | **the forcing prompt**: *write the test that proves a link survives a restart* | **nothing** — they can't write it. That failure is what earns the seam | — |
| 6 | *(all of 1–5, unchanged and still green)* | **menu (a)**: storage moves behind `IUrlRepository`; the `Dictionary` becomes `InMemoryUrlRepository`. The green bar through a structural change is the receipt | service + fake |
| 7 | a specific code is minted | **menu (b)**: the generator becomes a constructor argument, so a test can name the exact code | service + fake + stub |
| 8 | a repeated code mints another rather than overwriting | the collision. The naive `dict[code] = url` silently loses a link — found by a test, in code the mob wrote | service + fake |
| — | **story 2 begins** | | |
| 9 | *(the repository tests, moved — not rewritten)* | **menu (c)**: they become an abstract `UrlRepositoryContractTests`. xUnit inherits `[Fact]`s, so the in-memory subclass goes green immediately | contract suite |
| 10 | the same suite, against SQLite | `SqliteUrlRepository`. Three lines of subclass, and now every storage rule is asked of both stores | contract suite ×2 |
| 11 | a duplicate code is refused, and the original survives | **menu (e)**: `SqliteException` must not escape, so it is translated at the boundary | contract suite ×2 |
| 12 | codes are case-sensitive | R14 — the quieter fidelity gap. Both stores must give the same answer | contract suite ×2 |
| — | **story 3 begins** | | |
| 13–20 | status-code mapping, a misspelled route, a malformed body | the wiring, and only the wiring. No domain rule is re-tested here | real HTTP |

**Steps 6 and 9 are the two that teach the most, and neither is a new test.**
One is a structural refactor that keeps every existing test green; the other is
a *move*. If a mob rewrites tests at either point, the tests were coupled to
structure and that is the lesson.

## The design that fell out

- `IUrlRepository` — a port in the **domain's** vocabulary (`Save`, `Find`), so
  the service never learns what SQL is.
- `InMemoryUrlRepository` — a real, working implementation that happens to keep
  links in memory. Twelve lines, hand-written, no library.
- `SqliteUrlRepository` — the same interface, translating `SqliteException` into
  `CodeAlreadyTakenException` at the boundary.
- `UrlRepositoryContractTests` — one abstract suite, two subclasses.
- Menu items **(d)** Try-pattern and **(f)** value object declined, with reasons
  in the file headers.

## Honest note for the facilitator

A mob that works checkpoint #2 properly **fixes its own fake in story 1** — it
finds the silent overwrite, decides the store should refuse, and makes the fake
refuse. So when story 2 arrives, both stores agree and nothing goes red.

That is not a failed lesson; it is a different one. They decided how storage
behaves and then made their own double behave that way, which is exactly the
position every team is in. Story 2 is what *verifies* the guess against
something that does not care what they think. Say that out loud.

Two ways a genuine red bar still appears:

- **If they chose case-INSENSITIVE codes** (R8), `StringComparer.OrdinalIgnoreCase`
  and SQLite's default `BINARY` collation disagree, and R14 fires for real.
- **The revert experiment**, which works for any mob: change the fake's `TryAdd`
  guard back to `_links[code] = url` and run. Four tests fail, all on the
  in-memory path, none on SQLite. That is what would have happened if they had
  missed it — and it takes fifteen seconds to show.

## Pros

- **A genuinely fast inner loop.** Twenty-two of the thirty tests run in about
  10 ms *combined*. During red/green/refactor you run those constantly and the
  eight slow ones rarely. Solution 2 has no equivalent cheap tier.
- **Failures name a class.** A broken repository reddens the contract suite; a
  broken route reddens the HTTP suite. You know where to look before you look.
- **Unreachable states are reachable.** The collision test exists because a stub
  generator exists. So would "the database times out" or "the disk is full".
- **Design feedback.** The seam appeared because a test demanded it. That
  pressure is most of why TDD improves designs at all.
- **The service is genuinely portable.** Swapping SQLite for Postgres touches
  one class and zero tests in the service suite.
- **Cheap combinatorics.** Eight validation branches cost eight method calls.

## Cons

- **More moving parts.** Six source files, three test classes, an interface, a
  fake, an abstract suite and two subclasses — for a service with two methods.
- **The fake must be kept honest**, and that is a standing obligation. Contract
  tests are the answer, but they are machinery you now maintain forever.
- **It is possible to be green and broken.** Both failure modes are
  demonstrable here: the fake that overwrites, and the route typo. Solution 1
  needs its HTTP suite specifically *because* the rest of it is isolated.
- **Tests are coupled to structure.** Merge the service and the repository one
  day and the contract suite has to move with them.
- **An interface that exists partly for testing.** `IUrlRepository` has one
  production implementation. That is a real cost, and "test-induced design
  damage" is a fair charge to answer rather than dismiss.

## Choose this when

The suite will grow past a few hundred tests · integrated setup in your stack is
expensive (a real Postgres, containers, migrations) · you need to simulate
failures you cannot cause on demand · several implementations of a port are
genuinely coming · or the team is using TDD to *drive design*, not just to
verify behaviour.
