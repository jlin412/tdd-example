# Solution 2 — Integrated tests

**Spoilers.** Keep this closed until the retro.

Code: [`csharp-integrated/`](csharp-integrated/) · Compare with
[Solution1-Isolated.md](Solution1-Isolated.md).

This is the **outside-in** solution. Every test drives a real in-process server,
over real HTTP, against a real SQLite database. There are no fakes — with one
exception, introduced for one reason, and the reason is written down.

It satisfies exactly the same three stories and the same numbered rules. It is
not a shortcut, and it is not "the lazy version". It is what a team that holds
to *test through the thing you ship* produces when it TDDs this kata honestly.

## The test order, and why each step is where it is

Outside-in means the first test is a customer-visible outcome, and design is
whatever is needed to satisfy it.

| # | Test | What it forces |
|---|---|---|
| 1 | a shortened URL can be resolved again | everything at once: a route, model binding, a service, an INSERT, a SELECT, JSON out. The first red bar stays red for a while — that is normal outside-in, and it is why an *inner* loop matters (see below) |
| 2 | two different URLs get two different codes | real code generation. Note the assertion: codes **differ**, never what they are — reading the code out of the response is what lets the real generator run |
| 3 | each code resolves to its own URL | that links are actually kept apart, in real SQL |
| 4 | a code nobody minted is 404 | contract decision, expressed where a caller sees it |
| 5 | a blank URL is 400 | the negative column |
| 6 | the same URL twice mints a fresh code | the idempotency decision — and in this solution it is *immediately* a status-code decision too, because there is nowhere else for it to live |
| 7 | codes are case-sensitive | decided once, because there is only one store to decide it in |
| 8 | a repeated code mints another rather than overwriting | **the one seam.** A real generator will not collide on demand, so `IShortCodeGenerator` is introduced *here* — the only double in the solution, justified by a state that is otherwise unreachable |
| 9 | giving up beats looping forever | and this is where the suite earns its keep: see below |
| 10 | a misspelled route is 404 | real routing |
| 11 | a malformed JSON body is 400 | real model binding, handled by the framework before any of this code runs |
| 12 | links survive the process that created them | tear the whole app down, build a new one over the same database, resolve the old code. **R4 proven rather than promised** — solution 1 cannot write this test at all |

**Tests 9 and 12 are the ones to read.** Test 12 is the requirement that
*motivates* solution 1's entire architecture, and solution 1 can never actually
verify it. Test 9 found a real gap while this solution was being written:
nothing maps the give-up exception to a status code, so a caller gets an
unhandled exception. Solution 1's isolated suite asserts the same give-up
behaviour, passes, and never asks what a caller sees.

## The design that fell out — and what is missing from it

**There is no `IUrlRepository`, and no fake.** That is the headline.

Not because a repository would be wrong, but because **nothing ever forced
one**. No test was blocked by the lack of a seam, because every test had a real
database. Adding an interface with exactly one implementation on the strength of
a hunch is speculative design, and YAGNI won on the evidence available.

What that buys and costs:

- **Bought:** no double can lie to you, because there is no double. The bug
  solution 1 spends a whole story on — a fake that silently overwrites a taken
  code — is not *mitigated* here, it is **unreachable**. There is also no
  contract suite to maintain, because there is nothing to hold to a contract.
- **Cost:** `UrlShortener` now knows what SQLite is. It catches `SqliteException`
  and pattern-matches on error code 19. A service that is supposedly about
  minting links imports a database driver. Swap the store and you edit the
  domain; solution 1's service would not change by a line.

One seam survives — `IShortCodeGenerator` — and only because test 8 demanded it.
That is the whole rule this solution follows: **a double needs a test that
cannot be written without it.**

## Pros

- **It tests what you ship.** Routing, binding, serialization, DI wiring, SQL,
  constraints and collation are exercised on every line. Both of the "green but
  broken" failure modes solution 1 has to guard against are impossible here.
- **It can prove persistence.** Test 12 restarts the application. That is the
  requirement the whole kata turns on, and only this solution verifies it.
- **Far less code.** 3 files against 6; 1 test class against 3; 13 test
  executions against 30 — for identical requirements.
- **Enormous refactoring freedom.** The tests touch nothing but HTTP, so every
  decision underneath is yours to change. Merge the service into the endpoint,
  split it into five classes, swap the SQL — the suite neither knows nor cares.
  This is the single strongest argument for this style.
- **Tests read as requirements.** Every test names a customer-visible outcome.
  A non-programmer could review this file.
- **No fake to keep honest**, and therefore no contract suite to maintain.

## Cons

- **There is no fast tier.** Every test costs ~37 ms because each one builds and
  starts a host. Solution 1 runs 22 of its 30 tests in about 10 ms *combined*.
  Today the totals are comparable; at ten times the requirements this suite is
  ~5 s and solution 1's inner loop is still ~100 ms. **That gap is the whole
  argument**, and it compounds.
- **Weak at driving design.** The tests never pushed back on anything, which is
  why no port exists. Sometimes that is YAGNI working correctly; sometimes it is
  a missing abstraction nobody was told about. The tests cannot tell you which.
- **Diffuse failures.** A broken SELECT and a broken route both look like "the
  API test went red".
- **Unreachable states stay unreachable.** "The database times out", "the
  connection drops mid-write" — you cannot cause those without a seam, and this
  solution deliberately has only one.
- **Domain coupled to infrastructure.** `Microsoft.Data.Sqlite` is imported into
  the service; the retry loop pattern-matches a driver error code.
- **Combinatorics get expensive.** Every extra validation branch is another full
  HTTP round-trip. Ten more input cases cost ~370 ms here and ~10 ms in solution 1.
- **The test host is not production.** Test 9 proves it — `TestServer` rethrows
  where a deployed app would return 500. A fake of the transport, with its own
  fidelity gap.

## Choose this when

Integrated setup is genuinely cheap in your stack (in-memory SQLite is ~0.2 ms —
it *is* cheap here) · the suite will stay small · the wiring is where your bugs
actually come from · the team wants maximum freedom to restructure · or the code
is stable and being verified rather than designed.

## The honest synthesis

Neither column wins outright, and the ratio should follow **measured** cost
rather than a diagram. In this kata:

- integrating the **database** costs ~0.2 ms → there is almost no argument for a
  fake repository, and solution 1's is the more questionable half of its design;
- integrating the **web host** costs ~37 ms → 170× more, and that is what you
  should be selective about.

Which points at a third design neither solution shows: **real database, real
service, no repository interface — but call the service directly instead of over
HTTP**, keeping a handful of integrated HTTP tests for the wiring. That is
"double-loop TDD" (*Growing Object-Oriented Software Guided by Tests*): a slow
outer test that expresses the requirement, fast cycles inside it.

Ask the mob to design that third solution in the retro. They now have the
numbers to argue it honestly rather than by taste.
