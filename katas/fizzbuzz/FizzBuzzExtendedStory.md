# FizzBuzz — the Extended Story

You've shipped [classic FizzBuzz](FizzBuzzStory.md). Now the game changes — and
some of your existing tests will go red. That's the point: drive the change
test-first, watch old expectations fail, and update them on purpose.

## What changes

Two things.

**Fizz is now about the digit, not the times table.** A number gets `"Fizz"`
when you can *see* a three in it — when its digits contain a `3`. Being a
multiple of three no longer counts for anything. So `6`, `9` and `12` stop being
Fizz, while `13`, `23` and `33` start.

**Even numbers with nothing to say get a star.** If a number earns no word at
all and it's even, it prints `*` instead of the number.

Buzz is unchanged — still the five times table. And a **word always wins**: if a
number earned a word, it prints the word, never the star.

Everything the classic game left unsettled is still unsettled the same way.

## Examples

| n | prints | why |
|---|--------|-----|
| 2 | `"*"` | even, no word |
| 3 | `"Fizz"` | contains a 3 |
| 5 | `"Buzz"` | multiple of 5 |
| 6 | `"*"` | no 3 in it, and even |
| 7 | `"7"` | no word, and odd |
| 9 | `"9"` | no 3 in it, and odd |
| 10 | `"Buzz"` | even, but a word wins |
| 13 | `"Fizz"` | contains a 3 |
| 15 | `"Buzz"` | no 3 in it — **not** FizzBuzz any more |
| 30 | `"FizzBuzz"` | contains a 3 **and** multiple of 5 |
| 32 | `"Fizz"` | even, but a word wins |

## Your job, in order

1. **Update your test list first.** Which existing tests are now wrong? Which new
   ones do you need? Write the list before you touch the code.
2. **Predict the damage** — mark the tests you expect to fail, and say why.
3. **Run them.** Were you right? A surprise here is worth more than a green bar.
4. **Now drive the change test-first**, one red at a time.

## Design shift this forces

"Contains a three" is **not** a divisibility check, so a rule can no longer be a
simple `[divisor, word]` pair. What does a rule have to become? And the star
isn't a word that joins the others — it's what happens when nothing else fired.
Where does that belong, if not in the rule list?

## Reference solution

Reveal at the end:
[solutions/javascript/fizzbuzzExtended.js](solutions/javascript/fizzbuzzExtended.js).
