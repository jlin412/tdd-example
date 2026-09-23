# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this repo is

A practice repo of **TDD mob/ensemble-programming katas**. It ships runnable
*skeletons* for a human mob to fill in live, plus a *facilitator answer key*
and *reference solutions* to reveal afterward. There is no application to run
and no production feature work — changes here are almost always to the kata
content itself (story text, skeleton comments/prompts, or reference
solutions), not to app logic.

Four katas exist today, each deliberately teaching a different *shape* of
testing:

- **`katas/fizzbuzz/`** — polyglot: Python, JavaScript, Java, C#. Same kata,
  four language skeletons. A **pure function** — one input, one output, no state.
- **`katas/todo-list/`** — Angular-only, on purpose (it teaches *framework*
  testing: components, DOM, DI, signals — not just an algorithm).
- **`katas/stack/`** — polyglot: Python, TypeScript, Java, C# (note the
  TypeScript, *not* JavaScript — generics are a pattern-menu item here). A
  **stateful plain object tested through its own API**: every test is a
  sequence (you can't `pop` without `push`ing). Unlike FizzBuzz, the four
  language versions are deliberately *not* interchangeable in their lesson:
  how many safety nets a language gives you is itself the teaching material
  (Python none, TypeScript one you must ask for, Java/C# a compiler), and each
  skeleton's checkpoint #2 and each solution's tests pin its own language's
  answer. Keep that per-language divergence when editing.

- **`katas/url-shortener/`** — C#-only, on purpose. The only kata whose subject
  under test has a **collaborator**: a service that depends on storage. It
  teaches seam discovery under test pressure, hand-written fakes, and shared
  contract tests. It breaks house convention in three deliberate ways —
  **do not "fix" any of them**:
  1. It adds two packages beyond the test framework — `Microsoft.Data.Sqlite`
     and `Microsoft.AspNetCore.TestHost` — plus a `FrameworkReference` to
     `Microsoft.AspNetCore.App`. Every other kata uses only its test framework.
     Here a real database that really enforces a `PRIMARY KEY`, and a real
     server that really routes, *are* the subject matter; simulating either
     would destroy the point being made. The reference is raw ADO.NET, not EF
     Core, on purpose (EF's InMemory provider does not enforce constraints at
     all). Both ship in the skeleton `.csproj` from day one even though stories
     1–2 never touch them, because `solutions/` carries no build files.
  2. `csharp/SqliteTestDatabase.cs` is **given** infrastructure the mob does not
     write, and it deliberately does **not** exist in either solution folder, so
     the documented `cp solutions/csharp-*/*.cs csharp/` leaves it in place.
  3. It is the repo's first **three-story** kata. Story 3 drives a real
     in-process server with a real `HttpClient`, but deliberately uses **neither
     `Microsoft.NET.Sdk.Web` nor `WebApplicationFactory`** — the routes live in
     an extension method on `IEndpointRouteBuilder` and the tests build their
     own host with `UseTestServer()`. That keeps the project on a plain
     `Microsoft.NET.Sdk` with no `Program.cs`, which matters because the web SDK
     demands an entry point and it would collide with the one
     `Microsoft.NET.Test.Sdk` generates. Don't "simplify" this to
     `WebApplicationFactory<Program>`.

  4. It is the only kata with **two complete solutions**, and they are not
     variants of one design — they are opposing answers to "isolated or
     integrated tests?". `solutions/csharp-isolated/` (6 files, 3 test classes,
     30 tests) and `solutions/csharp-integrated/` (3 files, 1 test class, 13
     tests). The integrated one deliberately has **no `IUrlRepository` and no
     fake**, because outside-in TDD against a real database never forces a seam
     — that absence is the teaching point, so don't "complete" it by adding one.
     Each has a `solutions/Solution*.md` giving its test order, rationale and
     pros/cons. They each define `UrlShortener`, so only one may be copied over
     the skeleton at a time.

  **Three claims here are empirically verified and must stay that way.** All are
  documented with their exact output in `solutions/README.md` and the
  `Solution*.md` files; re-run them if you touch the relevant code:
  - Reverting the isolated fake's `TryAdd` guard to `_links[code] = url` fails
    **4** tests, all on the in-memory path and none on SQLite.
  - Changing the route `"/links"` to `"/lnks"` fails **7** tests, all in
    `UrlEndpointsTests` and none in the service or contract suites.
  - The measured costs the solution docs argue from: real SQLite ≈ **0.2 ms**
    per test, the web host ≈ **29–40 ms** per test. The whole "which ratio?"
    discussion rests on that ~170× gap, so re-measure rather than reword it.

  One trap to leave alone: the integrated `UrlShortener` takes a connection
  **string** and opens a connection per operation. Do not "simplify" it to take
  a shared `SqliteConnection` — that is not thread-safe, and under the
  concurrency test it throws `NullReferenceException` from inside the driver,
  *intermittently*. An early 200-request probe passed by luck, which is exactly
  how undefined behaviour behaves. `SqliteTestDatabase` therefore exposes both
  `Connection` (single-threaded use, solution 1) and `ConnectionString`
  (concurrent use, solution 2) over a uniquely-named shared-cache in-memory
  database.

## Commands

Run from inside the relevant language folder.

| Language | Setup | Test |
|---|---|---|
| Python | `pip install -r requirements.txt` | `pytest` |
| JavaScript | `npm install` | `npm test` (= `vitest run`; `npm run test:watch` for watch mode) |
| Java | — | `mvn test` |
| C# | — | `dotnet test` |
| C# (url-shortener) | — | `dotnet test` (first run restores `Microsoft.Data.Sqlite`; needs network once) |
| TypeScript (stack) | `npm install` | `npm test` (= `vitest run`) **and `npm run typecheck`** (= `tsc --noEmit`) |
| Angular | `npm install` | `npm test` (= `ng test`, runs Vitest via `@angular/build`) |

Angular needs Node 20.19+/22.12+/24+; the other JS/TS skeletons need Node 18+.
Java needs JDK 17+; C# needs .NET SDK 8+.

**Vitest does not type-check.** It transpiles TypeScript with esbuild and strips
types without verifying them, so a type error will not fail `npm test` — only
`npm run typecheck` catches it. In the stack kata this is deliberate teaching
material (uncommenting the TypeScript STEP 1 produces two different reds, a
runtime `TypeError` and a compile-time `TS2339`, where Python gets one
`AttributeError` and Java/C# refuse to build at all), so don't "fix" it by
wiring typechecking into the test script. When changing TypeScript in that
kata, run both commands.

**Solutions have no build files of their own.** `katas/*/solutions/<lang>/`
holds only source — no `package.json`/`pom.xml`/`.csproj`. To run a solution
suite, either point the sibling skeleton's test runner at it (e.g. from
`katas/fizzbuzz/javascript/`: `npx vitest run --root ../solutions/javascript`)
or copy the solution files over the skeleton in a scratch copy of the repo
(each kata's root `README.md` has the exact `cp` commands). Don't overwrite a
real skeleton with a solution unless asked.

## Architecture: how a kata is assembled

Every kata (`katas/<name>/`) follows the same five-piece structure. When
editing one kata, keep the others (and the sibling pieces within the same
kata) consistent with it — they're deliberately parallel.

1. **Story file(s) at the kata root** (e.g. `FizzBuzzStory.md`,
   `TodoListStory.md`) — the only spec a mob gets. These are **narrative,
   not a numbered requirements list on purpose**: the story describes the
   problem in prose with worked examples (a table of inputs → outputs, or a
   UI walkthrough) but never says "R1, R2, R3...". Producing the requirements
   list *is* the exercise — see STEP 0 below. A kata may have a second,
   later story (`*ExtendedStory.md` / `*EditStory.md`) that changes the
   requirements and deliberately breaks some existing tests.

2. **Empty production skeleton** (e.g. `javascript/fizzbuzz.js`,
   `angular/src/app/todo-list.ts`) — an empty class/component. Its header
   comment points at the story file(s) and states the TDD rule: add nothing
   until a red test demands it. It does **not** restate the requirements.

3. **Test skeleton** (e.g. `javascript/fizzbuzz.test.js`,
   `angular/src/app/todo-list.spec.ts`) — the mob-facing heart of the kata:
   - **STEP 0 — the test list.** Before any code, the file seeds two
     pending/todo tests (`it.todo(...)` in JS/TS, `@pytest.mark.skip` in
     Python, `@Disabled` in Java, `[Fact(Skip=...)]` in C#) and prompts the
     mob to keep naming more, translating the story into test names. A
     fresh skeleton run is **all green** — nothing is failing yet.
   - **STEP 1 — one worked example**, with its assertion shipped
     **commented out**. Uncommenting it is the mob's deliberate first RED.
     The three RED → GREEN → REFACTOR steps are labelled as comments
     *inside* the test body.
   - Everything after STEP 1 is comment **prompts, not answers** —
     questions like "which number proves it?", never "test that 3 returns
     Fizz". Prompts are tagged `[positive]` / `[boundary]` / `[edge]` /
     `[negative]`.
   - A **CHECKPOINT #1 pattern menu**: several named design patterns
     (rules engine, value object, strategy, DI, factory, null object, or a
     component-flavored equivalent — signal store, smart/dumb split,
     computed, immutable updaters, filter strategy). The mob picks ONE at a
     time; the file explains the payoff of each and which ones to likely
     decline, and why.
   - A **CHECKPOINT #2** for negative/edge cases (invalid input, boundary
     contract decisions the story deliberately left open).

4. **`facilitator/RULES.md`** — the canonical numbered rule list (R1, R2,
   ...) and pre-decided "product owner" answers to the questions a mob will
   ask, meant to be opened **during** the session (not before) once the mob
   has produced its own list. This is where R-numbers legitimately live —
   they do not appear in the story or skeleton files.

5. **`solutions/<lang>/`** — one possible finished end state per pattern
   choice made, with a complete test suite, kept closed until the retro.
   Method/component names used here (e.g. `convert`, the exact empty-state
   copy) are *one* choice the reference happened to make, not a mandate —
   the story explicitly leaves naming/wording to the mob, and skeleton
   comments flag that a different choice means the solution won't drop in
   verbatim.

`katas/<name>/README.md` documents the session flow, pattern menu, and test
taxonomy for that kata in prose; `FACILITATION.md` (repo root) covers the
mob mechanics that apply across all katas (driver/navigator roles, 4–5 min
rotation, strong-style pairing, timeboxing, anti-patterns to watch for).

## Conventions to preserve when editing kata content

- Never put numbered requirements (R1, R2, ...) in a story file or skeleton
  comment — that defeats the "mob derives the list" exercise. Numbered
  rules belong only in `facilitator/RULES.md` and in `solutions/`.
- Keep STEP 1's assertion commented out in skeletons; a fresh skeleton
  must pass with everything green (STEP 1 skipped-via-comment, STEP 0
  todos pending) — the mob's own uncomment is what produces the first red.
- Don't hardcode a specific method/component name or UI copy as though it
  were required, when the story leaves it to the mob — flag it as a
  placeholder instead (see existing skeleton comments for the pattern).
- When a change to a skeleton or story affects mob-facing prompts, update
  the corresponding `facilitator/RULES.md` and any `README.md` sections
  that describe session flow, so they don't drift out of sync.
