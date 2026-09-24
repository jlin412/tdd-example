# TDD Mob Kata

A hands-on, multi-language practice repo for learning **Test-Driven Development**
in a **mob / ensemble programming** setting. Everyone works on one problem, one
screen, rotating the keyboard on a timer.

Each kata ships as a **runnable skeleton** in four languages: the feature
requirement lives in a **story file**, the production class starts **empty**,
and the test file gives **one worked STEP 1** (its assertion commented out —
uncommenting it is your first RED) with the RED → GREEN → REFACTOR steps
labelled inside. Every test after that is the mob's to invent — guided by
comment prompts and two checkpoints (a team-chosen **design-pattern menu**,
then **negative & edge** contract work). A **reference solution** per language
is there to reveal once the session is done.

## Katas

| Kata | Focus | Status |
|------|-------|--------|
| [FizzBuzz](katas/fizzbuzz/) — [Story](katas/fizzbuzz/FizzBuzzStory.md) · [Extended Story](katas/fizzbuzz/FizzBuzzExtendedStory.md) | The rhythm of TDD; inventing positive/boundary/edge/negative tests from a requirement; a team-chosen pattern refactor (rules engine, value object, DI, factory…); validation & type checking | ✅ Ready |
| [Stack](katas/stack/) — [Story](katas/stack/StackStory.md) · [Extended Story](katas/stack/StackExtendedStory.md) | A **stateful object** tested through its own API: tests as sequences, proving LIFO, the empty-stack contract, and generics — plus how many safety nets your language actually gives you, which is a different answer in each of the four | ✅ Ready |
| [URL Shortener (C# backend · Angular frontend)](katas/url-shortener/) — [Story](katas/url-shortener/UrlShortenerStory.md) · [Extended Story](katas/url-shortener/UrlShortenerExtendedStory.md) | The kata with a **collaborator**, from both sides of the wire. One product story, two tracks — pick either or both. **Backend:** discovering a seam because a test can't be written, writing your own fake instead of importing a mocking library, and the day that fake lies to you — caught by one contract suite run against it and a real database. **Frontend:** driving a component through the DOM against a given service contract — fake or mock, and a stand-in that answers only when the test says so | ✅ Ready |
| String Calculator | Parsing, custom delimiters, richer negative/edge cases | ⏳ Planned |

## Languages & tooling

| Language | Test framework | Run tests |
|----------|----------------|-----------|
| Python | pytest | `pytest` |
| JavaScript | Vitest | `npm install && npm test` |
| Java | JUnit 5 (Maven) | `mvn test` |
| C# | xUnit | `dotnet test` |
| TypeScript | Vitest + `tsc` | `npm install && npm test`, plus `npm run typecheck` |
| Angular (TypeScript) | Vitest (`@angular/build`) | `npm install && npm test` |
| End-to-end (URL Shortener only) | Playwright | from `katas/url-shortener/smoke/`: `npm install && npm test` |

For the polyglot katas — **FizzBuzz** and **Stack** — pick whichever language
your mob is most comfortable in; the kata is identical across all four. Stack
adds a twist worth knowing about: the four languages disagree about how much a
type system does for you, and it turns that disagreement into the exercise, so
running it twice in two languages is a genuinely different session. **URL
Shortener** is different again: its two tracks are two halves of one product,
not two languages for one exercise — the backend track is C# because its subject
is a seam between a service and a real database, and the frontend track is
Angular because it teaches *framework* testing (components, DOM, DI, signals).
Prerequisites: Python 3.9+, Node 18+ (the Angular track needs
20.19+/22.12+/24+), JDK 17+ / Maven, .NET SDK 8+.

Every kata uses only its language's test framework, with one deliberate
exception: URL Shortener's backend track adds `Microsoft.Data.Sqlite` and
`Microsoft.AspNetCore.TestHost`, because a real database that really enforces
constraints — and a real server that really routes — *are* that track's subject
matter. Neither needs anything installed: SQLite bundles its own native
binaries, and the test server runs in-process without opening a port. (URL
Shortener also has a single Playwright smoke test for the retro, running both
reference halves together; it lives in its own folder, and neither track
installs it. The same folder runs the finished product for you to try by hand:
see [katas/url-shortener/smoke/README.md](katas/url-shortener/smoke/README.md).)

## The loop (TL;DR)

1. **RED** — write ONE failing test. Run it. Watch it fail for the right reason.
2. **GREEN** — write the *minimum* production code to pass. Nothing more.
3. **REFACTOR** — improve the code (and tests) while staying green.
4. Repeat.

Full mob mechanics — roles, rotation, timeboxing — are in
[FACILITATION.md](FACILITATION.md).

## Solutions

Each kata has a `solutions/` folder with reference implementations and complete
test suites per language. FizzBuzz and Stack ship **two** per language — a
**classic** drop-in matching the skeleton's names and public API, plus an
**extended** one covering the second story alongside it. URL Shortener ships
**two** backend solutions (isolated and integrated — opposing designs, not
variants) and **one** frontend solution, each covering both of its stories. A
facilitator can reveal or run the finished state at the end. Keep them closed
until then.
