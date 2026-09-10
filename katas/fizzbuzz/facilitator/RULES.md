# FizzBuzz — facilitator's answer key

**Open this DURING the session, not before.** It is not a spoiler in the way
`solutions/` is — that folder stays closed until the retro, this one you open at
the elicitation checkpoint (~minute 12) to play product owner.

The stories ([FizzBuzzStory.md](../FizzBuzzStory.md),
[FizzBuzzExtendedStory.md](../FizzBuzzExtendedStory.md)) deliberately do **not**
list numbered rules — producing that list is the mob's first job. This file is
the canonical list to compare theirs against, and it is what the `R`-numbers
throughout the rest of the kata refer to.

## Classic — R1–R6

- **R1.** `convert(n)` accepts a positive integer (`n >= 1`) and returns a
  **string**.
- **R2.** Multiples of **3** return `"Fizz"`.
- **R3.** Multiples of **5** return `"Buzz"`.
- **R4.** Multiples of **both 3 and 5** return `"FizzBuzz"`.
- **R5.** Any other valid `n` returns the number itself as a string (`4 → "4"`).
- **R6.** Invalid input is **rejected by throwing**. "Invalid" means `n < 1`, or
  not an integer at all (`'3'`, `3.5`, `null`, `NaN`, …). Dynamically typed
  languages need a runtime guard; Java and C# get the type half free from the
  compiler.

| n | out | n | out | n | out |
|---|-----|---|-----|---|-----|
| 1 | `"1"` | 6 | `"Fizz"` | 11 | `"11"` |
| 2 | `"2"` | 7 | `"7"` | 12 | `"Fizz"` |
| 3 | `"Fizz"` | 8 | `"8"` | 13 | `"13"` |
| 4 | `"4"` | 9 | `"Fizz"` | 14 | `"14"` |
| 5 | `"Buzz"` | 10 | `"Buzz"` | 15 | `"FizzBuzz"` |

## Extended — R7 + R8

- **R7 — the `'*'` fallback.** Any **even** number that matches **no word**
  returns `'*'`. A word always wins (`10 → "Buzz"`, `32 → "Fizz"`).
- **R8 — R2 is replaced.** Fizz now means **the decimal digits contain a `3`**,
  not divisibility. `6`/`9`/`12` stop being Fizz; `13`/`23`/`33` become Fizz;
  `15` is no longer `"FizzBuzz"`; `30`/`35` still are.

Resolution order for a valid `n`: build the word (`"Fizz"` if digits contain a
`3`, then `"Buzz"` if `n % 5 === 0`) → if a word was built, return it → else if
`n` is even return `'*'` → else return the number as a string. Invalid input
still throws (unchanged from R6).

## Playing product owner

Pre-decided answers to the questions a mob reliably asks. Give a one-line ruling
and move on — do not debate:

| They ask | Ruling |
|---|---|
| Does it print, or return? | **Returns.** One number in, one string out. |
| One number, or a whole range? | **One.** Looping is the caller's problem. |
| What about `0`? | **Invalid** — throw. (Watch for `0 % 3 === 0`.) |
| Negative numbers? | **Invalid** — throw. |
| `"3"`, `3.5`, `null`, `NaN`? | **Invalid** — throw. Which error type is *their* call. |
| Should `15` be `"FizzBuzz"` or `"BuzzFizz"`? | **`"FizzBuzz"`** — Fizz first. |
| Case sensitivity / different words? | Out of scope. Parking lot. |
| Should it cache / handle 1M inputs? | Out of scope. Parking lot. |

**Out of scope, say no:** i18n, printing/formatting, ranges, configuration,
persistence, performance work.

## Coverage check (~minute 35)

Instead of ticking off R-numbers, ask for four boxes:

- at least one **[positive]** per word rule,
- one **[boundary]** where two rules collide (the first is `15`),
- one **[edge]** at the low end of the range (`0`, negatives),
- one **[negative]** on input *shape* (`'3'`, `3.5`, `null`).

## Retro prompt

> Diff the list you wrote at minute 12 against this one. What did you invent that
> nobody asked for? What did you fill in from memory rather than from the story —
> and how would you have known, if this had been a problem you'd never seen?
