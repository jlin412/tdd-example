# Kata: URL Shortener (C#)

## The brief

Build a link shortener — long URL in, short code out, and back again —
test-first. The full requirement is in
[UrlShortenerStory.md](UrlShortenerStory.md). Once that ships,
[UrlShortenerDatabaseStory.md](UrlShortenerDatabaseStory.md) makes it persist
for real, and [UrlShortenerHttpStory.md](UrlShortenerHttpStory.md) puts a front
door on it.

Three stories rather than the usual two, because the middle one is where the
kata's whole point lands. More on that below.

## Why this kata exists

Every other kata in this repo tests an object that **stands alone**. FizzBuzz is
a pure function. Stack is a stateful object. Todo List is a component. In each
one you can build the thing, poke it, and be done.

This is the first kata where the thing under test **depends on something else** —
and that changes everything about how you test it:

- **The seam is discovered, not given.** The mob will reach for a `Dictionary`,
  and every test will pass. The story's promise that *a short link still works
  tomorrow, after a restart, from another server* is what makes that wrong. The
  interface appears because a test **could not be written** — which is the
  strongest argument in TDD, and one no other kata here can stage.
- **You write the test double yourself.** Not `Moq.Setup(...)` — a real, working
  implementation. The throwaway `Dictionary` *becomes* the fake the moment the
  interface is extracted. Nothing is imported. (This repo has deliberately never
  added a mocking library; this kata explains why.)
- **Then your fake lies to you.** A `Dictionary` does `dict[code] = url` and
  silently overwrites. SQLite has a `PRIMARY KEY` and refuses. Your suite was
  green the whole time and a URL is gone.
- **Contract tests are the fix.** One abstract suite, run against *every*
  implementation, is the only thing that keeps a fake honest.

That third bullet is the kata. Everything before it is setup, and everything
after it is consequence.

## A note on testing with a collaborator

- **Choose which suite a test belongs to.** Rules about *minting and resolving*
  belong in the service suite, run against the fast in-memory store. Rules about
  *how storage behaves* belong in the shared contract suite, run against every
  store. Getting this wrong is how teams end up with slow suites that still miss
  things.
- **A fake agrees with whatever you believed when you wrote it.** That is not a
  flaw to be fixed, it is the nature of the thing — which is why the contract
  suite is not optional ceremony. It is also why some defects are invisible to
  every isolated test you could write: the integrated solution found a real
  thread-safety bug in a shared database connection, and a fake behaves under
  concurrency however you imagined it would.
- **The database is real, and needs nothing installed.** `Microsoft.Data.Sqlite`
  bundles its own native binaries. No server, no Docker, no `sqlite3` on PATH —
  `dotnet test` and you have a real SQL engine with real constraints.

## How the skeleton works

The `csharp/` folder hands the mob exactly three things:

1. **The requirement — in the story, not the code.**
   [`UrlShortener.cs`](csharp/UrlShortener.cs) points at the story files and
   says nothing about what a shortener does.
2. **An empty class** — no members, and deliberately no constructor parameters.
   Collaborators have to arrive by refactor, not be handed over.
3. **A test list to write first** (STEP 0) and **ONE worked example** (STEP 1),
   plus comment prompts. STEP 1 ships with its assertion **commented out** and
   the RED → GREEN → REFACTOR steps labelled inside.

Plus one thing that is **given**, not exercise:
[`SqliteTestDatabase.cs`](csharp/SqliteTestDatabase.cs). It is unused until
story 2, and it exists because SQLite has two sharp edges that teach nothing
about TDD and would eat ten minutes: an in-memory database dies with its
connection, and a `SqliteConnection` is not thread-safe.

On a fresh skeleton everything passes: STEP 1 is commented out and the STEP 0
todos report as skipped. **Uncommenting STEP 1 is what gets you your first red**
— and in C# that red is a build error, `CS1061: 'UrlShortener' does not contain
a definition for 'Shorten'`, before a single test runs.

## Session flow

| Phase | What happens |
|-------|--------------|
| **STEP 0** | Write the test LIST first — as many skipped Facts as the mob can name. A test here is a *sequence*: you can't resolve a code you didn't shorten. |
| **STEP 1** | Uncomment the given assertion, watch the **build** fail, minimum green (a constant code and a constant URL are legal). |
| **Invent tests** | Promote todos — a second URL, codes that differ, an unknown code, the same URL twice. ONE red at a time. |
| **CHECKPOINT #1 · pattern menu** | First the forcing prompt: *write the test that proves a link survives a restart.* They can't. **That** is what earns the seam. Then the mob picks **ONE** pattern from (a)–(f). |
| **CHECKPOINT #2 · negative & edge** | Blank input, case sensitivity, and the collision that silently destroys a link while every test stays green. Ends on the cliffhanger. |
| **STORY 2 · the database** | [UrlShortenerDatabaseStory.md](UrlShortenerDatabaseStory.md): real SQLite behind the same interface. Predict what breaks *before* running. Something green goes red. |
| **STORY 3 · HTTP** | [UrlShortenerHttpStory.md](UrlShortenerHttpStory.md): real routes, driven by a real `HttpClient` against an in-process server. Status-code mapping, what the transport catches that nothing else can, and deriving the test pyramid by counting. |

**Timing.** Story 1 fits the standard 60 minutes and ends on the cliffhanger.
Story 2 is the 0:40–0:50 stretch slot if the mob moved fast, or the start of a
second session — it is worth protecting, because it is the payoff. Story 3 is a
separate sitting.

## The contract (decide this as a mob)

The story leaves gaps on purpose — choose, then encode each as a test:

- **The same URL twice** — same code back, or a fresh one? Both ship in the real
  world. Whichever you pick, story 3 turns it into a status code.
- **A code nobody minted** — throw? hand back nothing? Apply it consistently.
- **A URL that isn't one** — is `""` valid? `"banana"`? And is validating that
  even this object's job?
- **Case** — is `K3F9Q2` the same link as `k3f9q2`? Then the real question:
  *where is that written down?* If the answer is "wherever my storage happens to
  land", you inherited a decision instead of making one.
- **A code collision** — nothing guarantees a generator never repeats. What
  happens if it does, and **would you find out?**

## Test taxonomy

| Category | What it checks | URL shortener examples |
|----------|----------------|------------------------|
| **Positive** | correct behavior for valid use | the round trip; a second link resolving to its own URL |
| **Boundary** | the edges of a rule | two links proving codes are generated, not constant |
| **Edge** | ambiguous / contract-defining states | an unknown code; the same URL twice; case mismatch |
| **Negative** | invalid use rejected, not mishandled | blank input; **a collision that must not lose a link** |

## The pattern menu (checkpoint #1)

The mob **decides which ONE to implement first**; the coach enforces
one-pattern-at-a-time.

| Item | Pattern | The move |
|------|---------|----------|
| (a) | **Port + hand-written fake** | storage moves behind an interface the *domain* owns, with an in-memory class behind it. One item on purpose — you can't extract an interface and stay green with nothing implementing it |
| (b) | **Injected code generator** | the only unpredictable thing becomes a constructor argument. Payoff: assert the *exact* code, no regex |
| (c) | **Shared contract tests** | one abstract suite every store must pass. Honestly ceremony today — you have one store. Write down *when* it pays, then decline it |
| (d) | **Try-pattern instead of throwing** | `TryResolve(code, out var url)`, the .NET idiom (cf. the Stack kata's menu item (d)) |
| (e) | **Domain vs infrastructure errors** | whatever the store throws is translated at the boundary. Nothing forces this today — story 2 will |
| (f) | **Short code as a value object** | one home for the alphabet, the length, and the case decision |

Weigh and likely decline: a **mocking library** (you just wrote a working fake in
a dozen lines — a configured mock can only tell you what you told it, which is
precisely the failure story 2 is about), a generic `IRepository<T>` (it can't
express *"is this code taken?"*), async everywhere, unit-of-work.

Two of these are declined in story 1 and **earned later** — (c) by the database
story, (e) by the HTTP story. Watching a pattern you passed on become the
obvious call is the point of declining it out loud.

## Running the tests

| From | Install | Run |
|------|---------|-----|
| `csharp/` | — | `dotnet test` |

.NET SDK 8+. The first run restores `Microsoft.Data.Sqlite` and
`Microsoft.AspNetCore.TestHost`, which needs network once; after that it is
offline like everything else. Neither needs anything installed — SQLite bundles
its native binaries, and the test server runs in-process without opening a port.
A fresh skeleton is all green — **1 passed, 2 skipped**.

## Revealing the solution

There are **two** complete solutions, and the second one is the retro. Both
satisfy all three stories and every rule in `facilitator/RULES.md`; they
disagree about nearly everything else, because different kinds of test drove
them.

| | [Solution 1 — Isolated](solutions/Solution1-Isolated.md) | [Solution 2 — Integrated](solutions/Solution2-Integrated.md) |
|---|---|---|
| Direction | inside-out, domain first | outside-in, endpoint first |
| Doubles | a fake repository **and** a stub generator | a stub generator, for one test |
| `IUrlRepository` | yes | **none — nothing ever forced one** |
| Files / test classes / tests | 6 / 3 / 30 | 3 / 1 / 14 |
| Fast subset | **22 tests in ~10 ms** | none — every test ~40 ms |
| Proves "survives a restart" | no | **yes** |
| Can simulate a DB failure | yes | no |
| Found a concurrency defect | no | **yes** |

Solution 1 is what this kata's checkpoints and stories are written to produce:
menu items **(a)** and **(b)** taken, **(d)** and **(f)** declined, **(c)** and
**(e)** picked up where the later stories earn them.

Solution 2 exists because the obvious objection to all of that — *in a real
system everything is already wired together, so why not just test it that way?* —
is a good one with a real school of thought behind it. Rather than answer it in
prose, the kata answers it with a second working solution and measured numbers.
[solutions/README.md](solutions/README.md) compares them, and each
`Solution*.md` gives its own test order, rationale, pros and cons.

Their shared contract choices — a fresh code per shortening, throwing on an
unknown code, case-sensitive codes — are *one* set of answers, not a mandate. If
your mob chose differently, neither drops in verbatim, and the interesting part
of the retro is arguing about why.

```bash
# from katas/url-shortener/, over a scratch copy to keep the skeleton pristine
# copy ONE or the OTHER — both define UrlShortener and would collide
cp solutions/csharp-isolated/*.cs   csharp/ && (cd csharp && dotnet test)   # 30 passing
cp solutions/csharp-integrated/*.cs csharp/ && (cd csharp && dotnet test)   # 14 passing
```

`SqliteTestDatabase.cs` deliberately lives only in `csharp/`, not in either
solution folder, so the copies above leave it in place rather than creating
copies that drift apart. Both solutions use it.

Keep them closed until the retro. The solutions deliberately *show their
judgment* — which patterns they took, which they declined, and why — so the retro
can argue with them.
