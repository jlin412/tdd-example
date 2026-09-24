# Solution 2 — Integrated tests

**Spoilers.** Keep this closed until the retro.

Code: [`csharp-integrated/`](csharp-integrated/) · Compare with
[Solution1-Isolated.md](Solution1-Isolated.md).

This is the **outside-in** solution. Every test drives a real in-process server,
over real HTTP, against a real SQLite database. There are no fakes — with two
exceptions, each introduced for one reason, and the reasons are written down.

It satisfies exactly the same story, the same extended story and the same
numbered rules. It is
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
| 10 | **concurrent requests never issue the same code twice** | fifty requests in flight at once over a deliberately small code space. See below — this one found a real defect |
| 11 | a misspelled route is 404 | real routing |
| 12 | a malformed JSON body is 400 | real model binding, handled by the framework before any of this code runs |
| 13 | links survive the process that created them | tear the whole app down, build a new one over the same database, resolve the old code. **R4 proven rather than promised** — solution 1 cannot write this test at all |
| — | **the extended story: the table** | |
| 14 | the table is empty before anything is shortened | a list route, and an empty list rather than a 404 |
| 15 | the table lists every link newest first | **the second seam.** Asserting *when* each link was made needs a clock the test controls, so `TimeProvider` arrives here — .NET 8's own abstraction, faked by a five-line subclass |
| 16 | two links created in the same instant list the later first | a tie rule, and `rowid` as its tie-break — decided once, because there is one store |
| 17 | the table follows creation time even when servers' clocks disagree | restart onto a clock that runs *behind*: the link that arrived later was stamped earlier, and the table follows the stamp. No real clock does this on demand, which is what justifies the seam |
| 18 | creating a link answers with the whole link | R24 — `Shorten` hands back the `Link` it made, and `POST` answers with it. No test in either track asked for this; the end-to-end smoke test did, the first time the page's contract met this API |

**Tests 9, 10 and 13 are the ones to read.** Test 13 is the requirement that
*motivates* solution 1's entire architecture, and solution 1 can never actually
verify it. Test 9 found a gap: nothing maps the give-up exception to a status
code, so a caller gets an unhandled exception — solution 1's isolated suite
asserts the same give-up behaviour, passes, and never asks what a caller sees.

### Test 10 found a real bug, and nothing else could have

The first version of this solution's `UrlShortener` took a shared
`SqliteConnection`, which is how nearly everyone writes it. It passed every
test here except the concurrent one, which failed **intermittently** with a
`NullReferenceException` thrown from inside the driver.

The cause: **a `SqliteConnection` is not thread-safe**, and a web application is
the most concurrent thing there is. The fix was to inject a connection *string*
and open a connection per operation (pooled, so it is cheap).

Sit with what it would have taken to find that any other way:

- it is not reachable through a fake repository — a fake behaves however you
  imagined it would under concurrency, which is precisely the assurance you do
  not want;
- it is invisible when the service is called directly from one thread, which is
  every test in solution 1;
- it does not violate any rule in the story, so no amount of
  requirements analysis would have produced a test for it.

It is a property of the **wiring**, and it was found by a test that exercised the
wiring. That is the strongest single argument in this solution's favour, and it
arrived by accident rather than by design — which is rather the point.

A footnote worth keeping: an early probe of 200 concurrent requests passed
cleanly and was briefly taken as evidence that the shared connection was fine.
It was luck. Undefined behaviour is *allowed* to work. A race that passes is not
a race that is absent.

## The design that fell out — and what is missing from it

**There is no `IUrlRepository`, and no fake.** That is the headline.

Not because a repository would be wrong, but because **nothing ever forced
one**. No test was blocked by the lack of a seam, because every test had a real
database. Adding an interface with exactly one implementation on the strength of
a hunch is speculative design, and YAGNI won on the evidence available.

What that buys and costs:

- **Bought:** no double can lie to you, because there is no double. The bug
  solution 1 spends a whole checkpoint on — a fake that silently overwrites a taken
  code — is not *mitigated* here, it is **unreachable**. There is also no
  contract suite to maintain, because there is nothing to hold to a contract.
- **Cost:** `UrlShortener` now knows what SQLite is. It catches `SqliteException`
  and pattern-matches on error code 19. A service that is supposedly about
  minting links imports a database driver. Swap the store and you edit the
  domain; solution 1's service would not change by a line.

Two seams survive — `IShortCodeGenerator` because test 8 demanded it, and
`TimeProvider` because tests 15 and 17 did. That is the whole rule this solution
follows: **a double needs a test that cannot be written without it.** Note what
the clock did *not* need: a contract suite. There is one store, so its ORDER BY
has no twin to drift from — the same trade as everywhere else in this design.

## Pros

- **It tests what you ship.** Routing, binding, serialization, DI wiring, SQL,
  constraints and collation are exercised on every line. Both of the "green but
  broken" failure modes solution 1 has to guard against are impossible here.
- **It can prove persistence.** Test 13 restarts the application. That is the
  requirement the whole kata turns on, and only this solution verifies it.
- **Far less code.** 3 files against 6; 1 test class against 3; 19 test
  executions against 39 — for identical requirements.
- **Enormous refactoring freedom.** The tests touch nothing but HTTP, so every
  decision underneath is yours to change. Merge the service into the endpoint,
  split it into five classes, swap the SQL — the suite neither knows nor cares.
  This is the single strongest argument for this style.
- **Tests read as requirements.** Every test names a customer-visible outcome.
  A non-programmer could review this file.
- **No fake to keep honest**, and therefore no contract suite to maintain.
- **It can find wiring defects nobody thought to look for** — see test 10. No
  isolated suite can reach a thread-safety bug in a shared connection.

## Cons

- **There is no fast tier.** Every test costs ~9 ms because each one builds and
  starts a host. Solution 1 runs 29 of its 39 tests in about 10 ms *combined*.
  Today the totals are comparable (~260 ms against ~210 ms); at ten times the
  requirements this suite is ~2 s and solution 1's inner loop is still ~100 ms.
  **That gap is the whole argument**, and it compounds.
- **Weak at driving design.** The tests never pushed back on anything, which is
  why no port exists. Sometimes that is YAGNI working correctly; sometimes it is
  a missing abstraction nobody was told about. The tests cannot tell you which.
- **Diffuse failures.** A broken SELECT and a broken route both look like "the
  API test went red".
- **Unreachable states stay unreachable.** "The database times out", "the
  connection drops mid-write" — you cannot cause those without a seam, and this
  solution deliberately has only one.
- **Domain coupled to infrastructure, and it got worse.** `Microsoft.Data.Sqlite`
  is imported into the service, the retry loop pattern-matches a driver error
  code, and after the concurrency fix the service also owns connection
  lifetime. Solution 1 keeps every one of those behind a port.
- **Combinatorics get expensive.** Every extra validation branch is another full
  HTTP round-trip. Ten more input cases cost ~90 ms here and ~1 ms in solution 1.
- **The test host is not production.** Test 9 proves it — `TestServer` rethrows
  where a deployed app would return 500. A fake of the transport, with its own
  fidelity gap.

## Choose this when

Integrated setup is genuinely cheap in your stack (in-memory SQLite is ~0.1 ms —
it *is* cheap here) · the suite will stay small · the wiring is where your bugs
actually come from · the team wants maximum freedom to restructure · or the code
is stable and being verified rather than designed.

## The honest synthesis

Neither column wins outright, and the ratio should follow **measured** cost
rather than a diagram. In this kata:

- integrating the **database** costs ~0.1 ms → there is almost no argument for a
  fake repository, and solution 1's is the more questionable half of its design;
- integrating the **web host** costs ~9 ms → about 85× more, and that is what you
  should be selective about.

Which points at a third design neither solution shows: **real database, real
service, no repository interface — but call the service directly instead of over
HTTP**, keeping a handful of integrated HTTP tests for the wiring. That is
"double-loop TDD" (*Growing Object-Oriented Software Guided by Tests*): a slow
outer test that expresses the requirement, fast cycles inside it.

Ask the mob to design that third solution in the retro. They now have the
numbers to argue it honestly rather than by taste.
