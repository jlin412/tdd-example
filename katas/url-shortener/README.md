# Kata: URL Shortener (C# backend · Angular frontend)

## The brief

Build a link shortener — paste a long URL, press Create, get a short one back —
test-first. There is **one product story**,
[UrlShortenerStory.md](UrlShortenerStory.md), and it describes what people see
and what they're promised, never how it's built. Once it ships,
[UrlShortenerExtendedStory.md](UrlShortenerExtendedStory.md) adds a table of
every link, newest first.

The product has two halves, and the kata has a **track** for each. A mob picks
one per session — or does both, in either order, in two sessions:

| | Backend track | Frontend track |
|---|---|---|
| Builds | the shortener: minting, resolving, keeping links | the page: the bar, and later the table |
| Language | C# (xUnit) — [`csharp/`](csharp/) | Angular (Vitest) — [`angular/`](angular/) |
| The collaborator | **discovered** — storage, then a real database, then HTTP, each when a test demands it | **given** as a contract — the mob writes a fake or a mock for it |
| What it teaches | seam discovery, hand-written fakes, contract tests, the test pyramid measured | driving a component through the DOM with a stand-in you control — including *when* it answers |
| Rules in the answer key | R1–R24 | F1–F10 |

The two tracks share the story and nothing else: the page never talks to the C#
service, and neither track needs the other to run.

## Why this kata exists

FizzBuzz and Stack test objects that **stand alone** — a pure function, a
stateful object. In each one you can build the thing, poke it, and be done.

This is the kata where the thing under test **depends on something else** — and
it shows that from both sides of the wire.

**On the backend track:**

- **The seam is discovered, not given.** The mob will reach for a `Dictionary`,
  and every test will pass. The story's promise that *a short link still works
  tomorrow, after a restart, from another server* is what makes that wrong. The
  interface appears because a test **could not be written** — which is the
  strongest argument in TDD, and one no other kata here can stage.
- **You write the test double yourself.** Not `Moq.Setup(...)` — a real, working
  implementation. The throwaway `Dictionary` *becomes* the fake the moment the
  interface is extracted. Nothing is imported. (This repo has deliberately never
  added a mocking library; this track explains why.)
- **Then your fake lies to you.** A `Dictionary` does `dict[code] = url` and
  silently overwrites. SQLite has a `PRIMARY KEY` and refuses. Your suite was
  green the whole time and a URL is gone.
- **Contract tests are the fix.** One abstract suite, run against *every*
  implementation, is the only thing that keeps a fake honest.

Nobody tells the mob to add a database or a web API. The story promises links
that survive a restart, and a page in a browser; the track's checkpoints are
where each of those turns into a test that can't pass yet.

**On the frontend track**, the seam is handed over on purpose — the page's
service is a contract with no implementation — so the session stays on what a
page does:

- **Fake or mock?** The first test that presses Create cannot pass without a
  service. The mob must choose a stand-in — a small class that really works, or
  `vi.fn()` with canned answers — and the retro compares what each made easy.
- **You decide when the answer arrives.** A stand-in that holds its answer back
  is the only way to test the page *while it waits*: the disabled button, the
  second press that must not ask twice.
- **`whenStable()` only knows what Angular knows.** A promise your stand-in
  resolves is invisible to zoneless change detection — the kind of trap you only
  find by driving real async work through a real component.
- **And nothing in the track checks your stand-in.** It agrees with whatever you
  believed — exactly like the backend track's fake, with no contract suite
  behind it. That is the retro's closing question, and [the smoke
  test](#the-smoke-test) is the repo's answer.

## A note on testing with a collaborator

- **Choose which suite a test belongs to** (backend). Rules about *minting and
  resolving* belong in the service suite, run against the fast in-memory store.
  Rules about *how storage behaves* — including, in the extended story, what
  order it returns things in — belong in the shared contract suite, run against
  every store. Getting this wrong is how teams end up with slow suites that
  still miss things.
- **A fake agrees with whatever you believed when you wrote it.** That is not a
  flaw to be fixed, it is the nature of the thing — which is why the contract
  suite is not optional ceremony. It is also why some defects are invisible to
  every isolated test you could write: the integrated backend solution found a
  real thread-safety bug in a shared database connection, and a fake behaves
  under concurrency however you imagined it would.
- **The database is real, and needs nothing installed.** `Microsoft.Data.Sqlite`
  bundles its own native binaries. No server, no Docker, no `sqlite3` on PATH —
  `dotnet test` and you have a real SQL engine with real constraints.
- **Test the page through the DOM** (frontend). Render with
  `TestBed.createComponent`, act on `fixture.nativeElement`, and assert on what a
  user would see. Asserting on what a *user* sees is what lets you refactor the
  component (checkpoint #1) without touching the tests.

## How the skeletons work

Each track hands the mob the same three things:

1. **The requirement — in the story, not the code.** The production file points
   at the story and says nothing about what a shortener does.
2. **An empty class or component.** On the backend, deliberately no constructor
   parameters — collaborators arrive by refactor, not by gift.
3. **A test list to write first** (STEP 0) and **ONE worked example** (STEP 1),
   plus comment prompts and checkpoints. STEP 1 ships with its assertion
   **commented out** and the RED → GREEN → REFACTOR steps labelled inside.

Plus one thing per track that is **given**, not exercise:

- **Backend:** [`SqliteTestDatabase.cs`](csharp/SqliteTestDatabase.cs), unused
  until checkpoint #3. It exists because SQLite has two sharp edges that teach
  nothing about TDD and would eat ten minutes: an in-memory database dies with
  its connection, and a `SqliteConnection` is not thread-safe.
- **Frontend:** [`link-service.ts`](angular/src/app/link-service.ts), the
  service's contract — `create(url)` and `list()`, no implementation.

On a fresh skeleton everything passes. **Uncommenting STEP 1 is what gets you
your first red** — in C# that red is a build error,
`CS1061: 'UrlShortener' does not contain a definition for 'Shorten'`, before a
single test runs; in Angular it is `expected undefined to be true`, because the
page has no short-URL box yet.

## Session flow

**Backend track** ([`UrlShortenerTests.cs`](csharp/UrlShortenerTests.cs)):

| Phase | What happens |
|-------|--------------|
| **STEP 0** | Write the test LIST first — as many skipped Facts as the mob can name. A test here is a *sequence*: you can't resolve a code you didn't shorten. |
| **STEP 1** | Uncomment the given assertion, watch the **build** fail, minimum green (a constant code and a constant URL are legal). |
| **Invent tests** | Promote todos — a second URL, codes that differ, an unknown code, the same URL twice. ONE red at a time. |
| **CHECKPOINT #1 · pattern menu** | First the forcing prompt: *write the test that proves a link survives a restart.* They can't. **That** is what earns the seam. Then the mob picks **ONE** pattern from (a)–(f). |
| **CHECKPOINT #2 · negative & edge** | Blank input, case sensitivity, and the collision that silently destroys a link while every test stays green. Ends on the cliffhanger. |
| **CHECKPOINT #3 · is it true?** | The real database: SQLite behind the same interface. Predict what breaks *before* running. Something green goes red. |
| **CHECKPOINT #4 · the front door** | People use a *page*. Real routes, driven by a real `HttpClient` against an in-process server: status codes, what the transport catches that nothing else can, and the test pyramid derived by counting. |
| **EXTENDED STORY** | [The table](UrlShortenerExtendedStory.md): every link, newest first — a clock becomes a collaborator, and ordering becomes a promise both stores must keep. |

**Frontend track** ([`shortener-page.spec.ts`](angular/src/app/shortener-page.spec.ts)):

| Phase | What happens |
|-------|--------------|
| **STEP 0** | Write the test LIST first — as many `it.todo()`s as the mob can name. |
| **STEP 1** | Uncomment the given assertion, watch it fail, minimum green (one read-only input). |
| **Invent tests** | The first Create test can't pass without a service: **fake or mock?** Then what the page asks for, what it shows, blank input. ONE red at a time, through the DOM. |
| **CHECKPOINT #1 · pattern menu** | On green: mob picks **ONE** pattern from (a)–(e). The DOM tests stay green throughout, proving each refactor safe. |
| **CHECKPOINT #2 · negative & edge** | The page while it waits (a stand-in that answers only when told), a failing service, editing the long URL after Create, the same URL twice. |
| **EXTENDED STORY** | [The table](UrlShortenerExtendedStory.md): the page now owns a list — loaded, added to, shown — until the **smart/dumb split** earns its place. |

**Timing.** Each track's story and first two checkpoints fit the standard 60
minutes. On the backend, checkpoint #3 is the 0:40–0:50 stretch slot if the mob
moved fast, or the start of a second session — protect it, because it is the
payoff; checkpoint #4 and the extended story are separate sittings. On the
frontend, the extended story is the stretch slot.

## The contract (decide this as a mob)

The story leaves gaps on purpose — choose, then encode each as a test:

- **The same URL twice** — same short link back, or a fresh one? Both ship in the
  real world. Discuss it before deciding: it becomes a status code on the
  backend and a row count in the table.
- **A code nobody minted** — throw? hand back nothing? Apply it consistently.
- **A URL that isn't one** — is `"   "` valid? `"banana"`? And is checking even
  *your* half's job?
- **Case** — is `K3F9Q2` the same link as `k3f9q2`? Then the real question:
  *where is that written down?*
- **A code collision** — nothing guarantees a generator never repeats. What
  happens if it does, and **would you find out?**
- **The long URL after Create** — stays beside its short link, or clears? And if
  it's edited afterwards?

## Test taxonomy

| Category | What it checks | Backend examples | Frontend examples |
|----------|----------------|------------------|-------------------|
| **Positive** | correct behavior for valid use | the round trip; a second link resolving to its own URL | Create shows the short URL; the service is asked for what was typed |
| **Boundary** | the edges of a rule | two links proving codes are generated, not constant; two links in one instant | the moment Create is disabled, and enabled again |
| **Edge** | ambiguous / contract-defining states | an unknown code; the same URL twice; case mismatch | the empty short box; Enter while waiting; editing after Create |
| **Negative** | invalid use rejected, not mishandled | blank input; **a collision that must not lose a link** | blank input asks nothing; a failing service |

## The pattern menus (checkpoint #1)

The mob **decides which ONE to implement first**; the coach enforces
one-pattern-at-a-time.

**Backend:**

| Item | Pattern | The move |
|------|---------|----------|
| (a) | **Port + hand-written fake** | storage moves behind an interface the *domain* owns, with an in-memory class behind it. One item on purpose — you can't extract an interface and stay green with nothing implementing it |
| (b) | **Injected code generator** | the only unpredictable thing becomes a constructor argument. Payoff: assert the *exact* code, no regex |
| (c) | **Shared contract tests** | one abstract suite every store must pass. Honestly ceremony today — you have one store. Write down *when* it pays, then decline it |
| (d) | **Try-pattern instead of throwing** | `TryResolve(code, out var url)`, the .NET idiom (cf. the Stack kata's menu item (d)) |
| (e) | **Domain vs infrastructure errors** | whatever the store throws is translated at the boundary. Nothing forces this today — checkpoint #3 will |
| (f) | **Short code as a value object** | one home for the alphabet, the length, and the case decision |

Weigh and likely decline: a **mocking library** (you just wrote a working fake in
a dozen lines — a configured mock can only tell you what you told it, which is
precisely the failure checkpoint #3 is about), a generic `IRepository<T>` (it
can't express *"is this code taken?"*), async everywhere, unit-of-work.

Two of these are declined at checkpoint #1 and **earned later** — (c) by the
real database, (e) by HTTP. Watching a pattern you passed on become the obvious
call is the point of declining it out loud.

**Frontend:**

| Item | Pattern | The move |
|------|---------|----------|
| (a) | **Smart/dumb split** | a presentational bar (`input()`/`output()`); the page keeps state and the service call. Pays off when the table arrives |
| (b) | **Signal store** | lift state and the service call into an injectable store; the component becomes glue |
| (c) | **Computed derived state** | "may Create be pressed?", "what does the short box show?" as `computed()`, not fields a handler must remember to update |
| (d) | **Observables** | RxJS (`exhaustMap` for one-request-at-a-time) instead of async/await |
| (e) | **A reusable stand-in** | move a fake out of the spec and give it the powers tests keep reaching for — remember, fail, hold its answer. A fake can even run the page for a demo; a mock can't |

Weigh and likely decline: **NgRx** (one bar), **`HttpClient`** (the page talks to
a contract, not a URL), **`vi.mock()`** on your own modules (the Angular test
builder refuses it — "Please use Angular TestBed for mocking dependencies").

## Running the tests

| Track | From | Install | Run |
|-------|------|---------|-----|
| Backend | `csharp/` | — | `dotnet test` |
| Frontend | `angular/` | `npm install` | `npm test` |

**Backend:** .NET SDK 8+. The first run restores `Microsoft.Data.Sqlite` and
`Microsoft.AspNetCore.TestHost`, which needs network once; after that it is
offline like everything else. Neither needs anything installed — SQLite bundles
its native binaries, and the test server runs in-process without opening a port.
A fresh skeleton is all green — **1 passed, 2 skipped**.

**Frontend:** Node 20.19+/22.12+/24+ and Angular v21 (installed via
`npm install`). `npm test` watches for changes in a terminal; `npx ng test
--watch=false` runs once. A fresh skeleton is all green — **1 passed, 2 todo**.
`npm start` serves the page, but there is no service behind it until you provide
one: the contract has no implementation, so wire a fake into `app.config.ts` if
you want to click around.

## Revealing the solutions

**Backend — two complete solutions, and the second one is the retro.** Both
satisfy the whole story, the extended story and every R-rule in
`facilitator/RULES.md`; they disagree about nearly everything else, because
different kinds of test drove them.

| | [Solution 1 — Isolated](solutions/Solution1-Isolated.md) | [Solution 2 — Integrated](solutions/Solution2-Integrated.md) |
|---|---|---|
| Direction | inside-out, domain first | outside-in, endpoint first |
| Doubles | a fake repository, a stub generator, a fake clock | a stub generator and a fake clock, each for a named test |
| `IUrlRepository` | yes | **none — nothing ever forced one** |
| Files / test classes / tests | 6 / 3 / 39 | 3 / 1 / 19 |
| Fast subset | **29 tests in ~10 ms** | none — every test starts a host |
| Proves "survives a restart" | no | **yes** |
| Can simulate a DB failure | yes | no |
| Found a concurrency defect | no | **yes** |

Solution 1 is what the backend checkpoints are written to produce: menu items
**(a)** and **(b)** taken, **(d)** and **(f)** declined, **(c)** and **(e)** picked
up where the real database and HTTP earn them. Solution 2 exists because the
obvious objection to all of that — *in a real system everything is already wired
together, so why not just test it that way?* — is a good one with a real school
of thought behind it. [solutions/README.md](solutions/README.md) compares them
with measured numbers, and each `Solution*.md` gives its own test order,
rationale, pros and cons.

**Frontend — one solution**, in [solutions/angular/](solutions/angular/): the
page, a hand-written fake for the service, and the extended story's table.
Menu items **(c)** and **(e)** taken, **(a)** earned by the table, **(b)** and
**(d)** declined — with reasons in the file headers.

The references' contract choices — a fresh code per shortening, throwing on an
unknown code, case-sensitive codes, the long URL staying beside its short link —
are *one* set of answers, not a mandate. If your mob chose differently, nothing
drops in verbatim, and the interesting part of the retro is arguing about why.

```bash
# from katas/url-shortener/, over a scratch copy to keep the skeletons pristine

# Backend, solution 1 — isolated:
cp solutions/csharp-isolated/*.cs csharp/ && (cd csharp && dotnet test)   # 39 passing

# Backend, solution 2 — integrated, in a DIFFERENT fresh copy (the two collide).
# It has no UrlShortenerTests.cs to replace the skeleton's, which would not
# compile against its constructor, so that goes first:
rm csharp/UrlShortenerTests.cs && cp solutions/csharp-integrated/*.cs csharp/ \
  && (cd csharp && dotnet test)                                            # 19 passing

# Frontend — the page, its fake, and the table, with both specs.
cp solutions/angular/*.ts angular/src/app/ && (cd angular && npx ng test --watch=false)   # 20 passing
```

`SqliteTestDatabase.cs` and `link-service.ts` deliberately live only in the
skeletons, not in any solution folder, so the copies above leave them in place
rather than creating copies that drift apart. Every solution uses them.

Keep them closed until the retro. The solutions deliberately *show their
judgment* — which patterns they took, which they declined, and why — so the retro
can argue with them.

## The whole product, running (both tracks together)

Want to click it? From [`smoke/`](smoke/), `npm start` builds the reference page,
starts the integrated reference backend behind it, and serves both at
**<http://127.0.0.1:5179>** — paste a URL, press Create, watch the table (bordered,
with a shaded header, via Bootstrap). The database is in memory, so each start
is empty. [smoke/README.md](smoke/README.md) has the one-time setup.

### The smoke test

The tracks never meet, so neither can say whether the halves fit.
[`smoke/`](smoke/) is the **one** test that can: a Playwright test driving the
reference page against the integrated reference backend, served together from a
real host over a real database. It creates a link, checks the short URL resolves
at the shortener, reloads, and finds the link at the top of the table.

It is for the retro, not the mob — it runs only the reference solutions, and
Playwright lives in that folder alone. It has already earned its keep: it found
that `POST /links` answered with a bare code while the page's contract promised
the whole link, a gap every suite in both tracks was green through. That fix is
R24. See [smoke/README.md](smoke/README.md) to run it.
