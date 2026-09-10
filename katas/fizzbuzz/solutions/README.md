# FizzBuzz — reference solutions

**Spoilers.** Keep this folder closed until the retro.

Each language ships **two** finished implementations, each with a complete test
suite. These are **one possible end state**, not the only correct answer — the
point of the retro is to compare your mob's design against this and argue with
its choices.

- **Classic** (`FizzBuzz` / `fizzbuzz`) — the classic game, per
  [FizzBuzzStory.md](../FizzBuzzStory.md).
- **Extended** (`FizzBuzzExtended` / `fizzbuzz_extended`) — the same plus the
  digit-3 and `"*"` changes,
  per [FizzBuzzExtendedStory.md](../FizzBuzzExtendedStory.md).

## Design shown here — classic

All four languages converge on the same shape. From the checkpoint-#1 pattern
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

These name their method `convert` (`Convert` in C#) — if your mob chose the
same, the classic files drop straight in; the extended files sit alongside them.
Copy over and run:

```bash
# from katas/fizzbuzz/
cp solutions/python/*.py       python/       && (cd python && pytest)
cp solutions/javascript/*.js   javascript/   && (cd javascript && npm test)
cp solutions/java/FizzBuzz.java solutions/java/FizzBuzzExtended.java         java/src/main/java/kata/
cp solutions/java/FizzBuzzTest.java solutions/java/FizzBuzzExtendedTest.java java/src/test/java/kata/ && (cd java && mvn test)
cp solutions/csharp/*.cs       csharp/       && (cd csharp && dotnet test)
```

(Use a scratch copy of the repo if you want to keep the skeletons pristine.)
