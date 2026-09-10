# FizzBuzz — the Story (classic)

You are printing the numbers `1, 2, 3, …` — but with a twist the whole team has
to agree on. Grow it **test-first**, one rule at a time.

## The game

Count upwards from one and print each number — except that some numbers are
replaced by a word.

A **multiple of three** prints `"Fizz"`. A **multiple of five** prints
`"Buzz"`. A number that is a multiple of *both* prints both words, run together,
Fizz first. Every other number prints as itself.

## What you're building

One function: it takes a single whole number and returns the text to print for
that number.

What to **call** it — and where it lives — is the mob's call, not the story's.
Agree on a name before you start, and keep it consistent.

## Examples

| n | prints | n | prints | n | prints |
|---|--------|---|--------|---|--------|
| 1 | `"1"` | 6 | `"Fizz"` | 11 | `"11"` |
| 2 | `"2"` | 7 | `"7"` | 12 | `"Fizz"` |
| 3 | `"Fizz"` | 8 | `"8"` | 13 | `"13"` |
| 4 | `"4"` | 9 | `"Fizz"` | 14 | `"14"` |
| 5 | `"Buzz"` | 10 | `"Buzz"` | 15 | `"FizzBuzz"` |

## What the game doesn't say

The counting starts at one. Everything else is yours to decide as a mob — then
encode each decision as a test:

- **`0`** — is it in the game? Careful: zero divides by three.
- **Negative numbers** — same question.
- **Something that isn't a whole number at all** — `"3"`, `3.5`, `null`, `NaN`.
  Should the function refuse? And *how* should it refuse?

There is no single correct contract. What matters is that you **choose, and your
tests express the choice.**

## The loop

1. **RED** — write ONE failing test. Run it. Watch it fail for the right reason.
2. **GREEN** — write the *minimum* production code to pass. Nothing more.
3. **REFACTOR** — improve the code (and tests) while staying green. Then repeat.

Before any of that, though: write the **test list** (STEP 0 in the spec file).

## Where things live

- Skeleton to fill in: [javascript/fizzbuzz.js](javascript/fizzbuzz.js) +
  [javascript/fizzbuzz.test.js](javascript/fizzbuzz.test.js)
- Reference solution (reveal at the end):
  [solutions/javascript/fizzbuzz.js](solutions/javascript/fizzbuzz.js)

## What next

Shipped classic FizzBuzz? The requirements change. See
[FizzBuzzExtendedStory.md](FizzBuzzExtendedStory.md).
