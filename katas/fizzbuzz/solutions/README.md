# FizzBuzz — reference solutions

**Spoilers.** Keep this folder closed until the retro.

Each language ships as a **step-by-step trail**: one runnable snapshot after
every TDD step, from the test list to the finished extended game, with a
walkthrough that quotes the real failure behind each red. Two of the snapshots
are the finished implementations, each with a complete test suite:

- **Classic**, in `step-08c-dependency-injection/`: the classic game, per
  [FizzBuzzStory.md](../FizzBuzzStory.md).
- **Extended**, in `step-10-extended-digit-three/`: the same class plus the
  digit-3 and `"*"` changes, per
  [FizzBuzzExtendedStory.md](../FizzBuzzExtendedStory.md), worked in place.

These are **one possible end state**, not the only correct answer. The point of
the retro is to compare your mob's design against this and argue with its
choices.

| Language | Trail | Runs with |
|---|---|---|
| JavaScript | [javascript-steps/](javascript-steps/README.md) | the skeleton's vitest, via `--root` |
| Python | [python-steps/](python-steps/README.md) | `pytest` inside the step folder |
| Java | [java-steps/](java-steps/README.md) | copy over the skeleton in a scratch copy, then `mvn test` |
| C# | [csharp-steps/](csharp-steps/README.md) | copy over the skeleton in a scratch copy, then `dotnet test` |

In every language, `step-01-red` fails on purpose. In Java and C# it doesn't
even compile, which is a legitimate first RED in a statically typed language.

## Design shown here — classic

All four languages converge on the same shape. From the checkpoint-#2 pattern
menu, this "team" implemented — one at a time, per the coach's rule:

- **(a) Rules engine** — rules are data (a `(divisor, word)` table).
- **(b) Value Object** — a tiny `Rule` type (class / record) that answers
  `matches(n)` itself (Tell, Don't Ask).
- **(d) Dependency Injection** — the constructor accepts rules, with a sensible
  default.

And deliberately **declined** (reasons in each solution's header comment):

- **(c) Strategy** — only divisibility is required; predicate rules would be
  YAGNI. *(The extended solution flips this — see below.)*
- **(e) Factory** / **(f) Null Object** — not enough presets / reintroduces the
  conditional the pattern promises to remove.

## What the extended solution adds

The two changes deliberately break the classic design's assumptions:

- **Digit rule** — Fizz means "the digits **contain a 3**", not divisibility. A rule can
  no longer be a `(divisor, word)` pair, so rules become **predicates** — menu
  item **(c) Strategy**, now genuinely earned rather than YAGNI.
- **The star** — an even number with no word returns `"*"`. Not a word rule (it never
  concatenates); it lives in `convert()` as the "nothing matched" branch, so a
  Fizz/Buzz word always wins.

Everywhere, in both:

- **A stable public API** — a class with a single conversion method, named
  `convert` here (`Convert` in C#). The name is one choice among many; a mob
  that picked another name should expect these files not to drop in verbatim.
- **Input validation** — value guard (`n >= 1`) everywhere, plus a **runtime
  type guard** in Python and JavaScript (unnecessary in Java/C#, where the
  compiler enforces it).

## Running the finished state

These name their method `convert` (`Convert` in C#). If your mob chose the same
name, a step's files drop straight over the skeleton. Use a scratch copy of the
repo if you want to keep the skeletons pristine.

```bash
# from katas/fizzbuzz/javascript/ — no copying needed
npx vitest run --root ../solutions/javascript-steps/step-10-extended-digit-three

# from katas/fizzbuzz/solutions/python-steps/step-10-extended-digit-three/
pytest

# from katas/fizzbuzz/, in a scratch copy
S=solutions/java-steps/step-10-extended-digit-three
cp $S/FizzBuzz.java java/src/main/java/kata/ && cp $S/FizzBuzzTest.java java/src/test/java/kata/ && (cd java && mvn test)
cp solutions/csharp-steps/step-10-extended-digit-three/*.cs csharp/ && (cd csharp && dotnet test)
```

Swap `step-10-extended-digit-three` for any other step folder to run that step.
