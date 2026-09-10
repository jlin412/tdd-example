# Kata: FizzBuzz

## The brief

Build a `FizzBuzz` class that converts a positive integer into a string:

- multiples of **3** → `"Fizz"`
- multiples of **5** → `"Buzz"`
- multiples of **both** → `"FizzBuzz"`
- anything else → the number itself, as a string (`4` → `"4"`)

Sounds trivial — and the happy path is. The value of this kata is in
**inventing your own tests from a requirement**, the **contract decisions**,
and a **team-chosen design-pattern refactor** — not the algorithm.

The full brief lives in [FizzBuzzStory.md](FizzBuzzStory.md) (the classic game).
Once that ships, [FizzBuzzExtendedStory.md](FizzBuzzExtendedStory.md) changes
the requirements and breaks some existing tests on purpose.

## How the skeletons work

Each language folder hands the mob exactly three things:

1. **The requirement — in the story, not the code.** The production file points
   to [FizzBuzzStory.md](FizzBuzzStory.md); every test you write should trace to
   one of its rules.
2. **An empty production class** — no methods. Nothing gets added until a red
   test demands it.
3. **A test list to write first** (STEP 0) and **ONE worked example** (STEP 1),
   plus comment prompts. STEP 0 asks the mob to name as many tests as it can —
   as skipped/todo placeholders — before any code is written. STEP 1
   ships with its assertion **commented out** and the RED → GREEN → REFACTOR
   steps labelled inside it. Every test after the first is yours to invent — the
   prompts ask questions ("which number proves the first word rule?"), they
   don't hand you cases.

Your first RED is deliberate: **uncomment STEP 1's assertion** and run the tests.

- **Python / JavaScript** — it now fails at runtime: the method doesn't exist yet.
- **Java / C#** — it now **doesn't even compile**. In a statically typed
  language, "does not compile" is a legitimate first RED.

(A fresh, still-commented skeleton passes as a no-op — uncommenting is the move.)

## Session flow

| Phase | What happens |
|-------|--------------|
| **STEP 0** | Write the test LIST first — as many todos as the mob can name, translating the story into test language. |
| **STEP 1** | Uncomment the given assertion, watch it fail for the right reason, minimum green (`return "1"` is legal). |
| **Invent tests** | Promote todos off the STEP 0 list — `[positive]` / `[boundary]` for the word rules, ONE red at a time. |
| **CHECKPOINT #1 · pattern menu** | On green: mob picks **ONE** pattern from (a)–(f) and implements it. Coach's rule: one pattern at a time — green and a payoff check between picks. |
| **CHECKPOINT #2 · negative & edge** | Back to red work: `0`/negatives (mind the `0 % 3 == 0` trap), plus runtime type guards in Python/JS. |
| **EXTENDED** | [FizzBuzzExtendedStory.md](FizzBuzzExtendedStory.md): Fizz becomes "the digits *contain a 3*", and even numbers with no word print `"*"`. These deliberately break existing tests — predict which, then drive the change test-first. |

## The contract (decide this as a mob)

FizzBuzz is usually specified only for the happy path. Part of the exercise is
making the edges explicit, then encoding each decision as a test:

- **What about 0?** Mathematically `0 % 3 == 0`, so naive code returns
  `"FizzBuzz"`. The story says counting starts at one — but *which* error to raise is
  the mob's call.
- **What about non-integer input** (`"3"`, `3.5`, `null`)? Dynamically typed
  languages reject it at runtime; statically typed ones already refuse to
  compile it — see [the type-checking note](#a-note-on-type-checking).

There is no single "correct" contract. What matters is that you **choose and
your tests express the choice.**

## Test taxonomy

| Category | What it checks | FizzBuzz examples |
|----------|----------------|-------------------|
| **Positive** | correct output for valid input | `1→"1"`, `3→"Fizz"`, `5→"Buzz"` |
| **Boundary** | the edges of each rule | first multiples `3, 5, 15`; lowest valid input `1` |
| **Edge** | ambiguous / contract-defining inputs | `0`, negatives |
| **Negative** | invalid input is rejected, not silently mishandled | `"3"`, `3.5`, `null` (Python/JS at runtime; Java/C#: the compiler is this test) |

## The pattern menu (checkpoint #1)

The mob **decides which ONE to implement first**; the coach enforces
one-pattern-at-a-time (implement → green → "did it pay for itself?" → next).

| Item | Pattern | The move |
|------|---------|----------|
| (a) | **Rules engine** | if/else chain → a data table of (divisor, word) rules the code walks. Every other item gets easier after this one. |
| (b) | **Value Object (OO)** | each rule becomes a tiny type with `matches(n)` — Tell, Don't Ask. |
| (c) | **Strategy / Polymorphism** | rules that differ in kind (predicates or subclasses) — composition vs inheritance, a.k.a. Specification. |
| (d) | **Dependency Injection** | constructor accepts the rules, sensible defaults — Open/Closed by config. |
| (e) | **Factory** | named constructors like `classic()` hide the rule wiring. |
| (f) | **Null Object** | the fallback as an always-matching last rule — trickier than it looks. |

Weigh and likely decline (retro fodder): **Builder** (a three-item list doesn't
need one), **Chain of Responsibility** (crowns one winner; FizzBuzz
concatenates — it fights the requirement), **Template Method** (the inheritance
twin of (c)).

## A note on type checking

A deliberate cross-language teaching moment:

- **Python & JavaScript** are dynamically typed. Passing `"3"` is a call the
  runtime will happily attempt, so the mob must **guard types at runtime** and
  write `[negative]` tests demanding it (`Number.isInteger(n)`,
  `isinstance(n, int)` — and remember `bool` is a subclass of `int` in Python).
- **Java & C#** are statically typed. Passing `"3"` **won't compile** — the
  type system is a test you didn't have to write. Their checkpoint #2 focuses
  on **value** validation (`n < 1` → exception), and their very first RED is a
  compile error rather than a failing assertion.

Retro question: which safety net caught more today — the compiler or the tests?

## Running the tests

| Language | From | Command |
|----------|------|---------|
| Python | `python/` | `pip install -r requirements.txt` then `pytest` |
| JavaScript | `javascript/` | `npm install` then `npm test` |
| Java | `java/` | `mvn test` |
| C# | `csharp/` | `dotnet test` |

On a fresh skeleton everything passes: STEP 1's assertion is commented out and
the STEP 0 todos report as skipped. Uncommenting STEP 1 is what gets you your
first red (or, in Java/C#, your first compile failure) — that's your starting
point.

## Revealing the solution

`solutions/<language>/` holds **two** end states per language:

- **Classic** (`FizzBuzz` / `fizzbuzz`) — the classic game, with pattern-menu picks
  (a) rules engine, (b) value object, (d) injection; (c), (e) and (f) declined.
- **Extended** (`FizzBuzzExtended` / `fizzbuzz_extended`) — the same, plus the
  digit-3 and `"*"` changes.
  Here "contains a 3" forces predicate rules, so menu item **(c) Strategy** —
  YAGNI for the classic kata — finally earns its place.

The reference solutions name their method `convert` (`Convert` in C#). That is
one choice, not a mandate — the mob names its own. If yours agreed on the same
name, the classic files are drop-in; the extended files sit alongside them:

```bash
# from katas/fizzbuzz/
cp solutions/python/*.py       python/       && (cd python && pytest)
cp solutions/javascript/*.js   javascript/   && (cd javascript && npm test)
cp solutions/java/FizzBuzz.java solutions/java/FizzBuzzExtended.java         java/src/main/java/kata/
cp solutions/java/FizzBuzzTest.java solutions/java/FizzBuzzExtendedTest.java java/src/test/java/kata/ && (cd java && mvn test)
cp solutions/csharp/*.cs       csharp/       && (cd csharp && dotnet test)
```

Keep them closed until the retro. The solutions deliberately *show their
judgment* — which patterns they took, which they declined, and why — so the
retro can argue with them.
