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
| [Todo List (Angular)](katas/todo-list/) — [Story](katas/todo-list/TodoListStory.md) · [Edit stretch](katas/todo-list/TodoListEditStory.md) | **Frontend** TDD: driving a component through the DOM, signals-first state, DI, a team-chosen pattern refactor (signal store, smart/dumb split, computed, immutability) | ✅ Ready |
| String Calculator | Parsing, custom delimiters, richer negative/edge cases | ⏳ Planned |

## Languages & tooling

| Language | Test framework | Run tests |
|----------|----------------|-----------|
| Python | pytest | `pytest` |
| JavaScript | Vitest | `npm install && npm test` |
| Java | JUnit 5 (Maven) | `mvn test` |
| C# | xUnit | `dotnet test` |
| Angular (TypeScript) | Vitest (`@angular/build`) | `npm install && npm test` |

For the polyglot katas (e.g. FizzBuzz) pick whichever language your mob is most
comfortable in — the kata is identical across all four. The **Todo List** kata is
Angular-only on purpose: it teaches *framework* testing (components, DOM, DI,
signals). Prerequisites: Python 3.9+, Node 18+ (Angular kata needs 20.19+/22.12+/24+),
JDK 17+ / Maven, .NET SDK 8+.

## The loop (TL;DR)

1. **RED** — write ONE failing test. Run it. Watch it fail for the right reason.
2. **GREEN** — write the *minimum* production code to pass. Nothing more.
3. **REFACTOR** — improve the code (and tests) while staying green.
4. Repeat.

Full mob mechanics — roles, rotation, timeboxing — are in
[FACILITATION.md](FACILITATION.md).

## Solutions

Each kata has a `solutions/` folder with reference implementations and complete
test suites per language. FizzBuzz ships two per language — a **classic** (R1–R6)
drop-in that matches the skeleton's names and public API, plus an **extended**
one (R7 + R8) alongside it. A facilitator can reveal or run the finished state at
the end. Keep them closed until then.
