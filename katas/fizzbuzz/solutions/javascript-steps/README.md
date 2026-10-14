# FizzBuzz (JavaScript), step by step

**Spoilers.** Like the rest of `solutions/`, keep this closed until the retro.

This folder shows one mob's path through the kata, saved after each step. Each
`step-*` folder holds `fizzbuzz.js` and `fizzbuzz.test.js` as they stand at the
**end** of that step. Two of them are the reference solutions:
- **Step 8c** ([`step-08c-dependency-injection/`](step-08c-dependency-injection/))
  is the finished classic game.
- **Step 10** ([`step-10-extended-digit-three/`](step-10-extended-digit-three/))
  is the finished extended game, worked **in place** in the same `fizzbuzz.js`.

## Running a step

From `katas/fizzbuzz/javascript/` (after `npm install`):

```bash
npx vitest run --root ../solutions/javascript-steps/step-05-boundary
```

To see exactly what one step changed, from this folder:

```bash
diff -u step-04-positive step-05-boundary
```

**`step-01-red` fails on purpose.** That failure is the RED.

| Step | Folder | `vitest run` |
|---|---|---|
| 0 · test list | `step-00-test-list` | 1 passed, 18 todo |
| 1 · RED | `step-01-red` | **1 failed**, 17 todo |
| 2 · GREEN | `step-02-green` | 1 passed, 17 todo |
| 3 · REFACTOR (generalize) | `step-03-refactor` | 3 passed, 15 todo |
| 4 · other positives | `step-04-positive` | 7 passed, 11 todo |
| 5 · boundary | `step-05-boundary` | 9 passed, 9 todo |
| 6 · edge | `step-06-edge` | 12 passed, 6 todo |
| 7 · negative | `step-07-negative` | 18 passed |
| 8a · rules engine | `step-08a-rules-engine` | 18 passed |
| 8b · value object | `step-08b-value-object` | 18 passed |
| 8c · dependency injection | `step-08c-dependency-injection` | 19 passed |
| 9 · extended R7, the star | `step-09-extended-star` | 27 passed, 7 todo |
| 10 · extended R8, contains a 3 | `step-10-extended-digit-three` | 36 passed |

The order follows the skeleton's prompts: positives and the boundary, then
checkpoint #1 (edge, then negative), then checkpoint #2 (the pattern menu), then
the extended story's two changes, one at a time.

Steps 0–2 are the skeleton files plus only the mob's move. From step 3 on, the
skeleton's coaching comments are gone, and any todos not yet promoted wait at the
bottom of the test file. Each RED quoted below comes from a real run: step N's
tests run against step N−1's code. The output is trimmed to the test name and the
first line of the message.

---

## Step 0 · the test list

Before any code, the file gets 18 `it.todo`s: the 2 the skeleton seeds plus 16
more, grouped as [positive], [boundary], [edge] and [negative]. Each one becomes
one case in the finished classic suite. The DI test is not on the list, because it
comes from a design decision in step 8c, not from the story.

*Discuss:* which todos came from the story's example table, and which from "What
the game doesn't say"?

## Step 1 · RED

Uncomment the STEP 1 assertion and delete the todo it promotes.

```
✗ returns the number as a string when nothing special applies
    TypeError: fizzBuzz.convert is not a function
```

It fails for the right reason: the method doesn't exist yet. This is not an import
error or a typo in the test.

## Step 2 · GREEN

```js
convert() {
  return '1';
}
```

The answer is hard-coded, and that is legitimate: nothing demands more yet.

## Step 3 · REFACTOR: generalize

The skeleton calls this REFACTOR, but generalizing takes one more red: a second
example that the hard-coded answer can't pass.

```
✗ convert(2) === "2"  →  expected '1' to be '2'
✗ convert(4) === "4"  →  expected '1' to be '4'
```

The fix is `return String(n);`. Then the tests get a refactor too: `fizzBuzz` moves
to module scope, and the STEP 1 test becomes an `it.each` table.

## Step 4 · the other positives

```
✗ convert(3) === "Fizz"   →  expected '3' to be 'Fizz'
✗ convert(9) === "Fizz"   →  expected '9' to be 'Fizz'
✗ convert(5) === "Buzz"   →  expected '5' to be 'Buzz'
✗ convert(20) === "Buzz"  →  expected '20' to be 'Buzz'
```

In the session these go one at a time. 3 alone could be passed with `n === 3`; 9 is
what forces `n % 3 === 0`. Buzz works the same way with 5, then 20.

```js
if (n % 3 === 0) return 'Fizz';
if (n % 5 === 0) return 'Buzz';
return String(n);
```

## Step 5 · boundary

```
✗ convert(15) === "FizzBuzz"  →  expected 'Fizz' to be 'FizzBuzz'
✗ convert(45) === "FizzBuzz"  →  expected 'Fizz' to be 'FizzBuzz'
```

The minimum GREEN is a special case on top: `if (n % 15 === 0) return 'FizzBuzz';`.
For the REFACTOR, the skeleton asks whether the code joins the words or
special-cases them. It joins them:

```js
let word = '';
if (n % 3 === 0) word += 'Fizz';
if (n % 5 === 0) word += 'Buzz';
return word || String(n);
```

*Discuss:* the special case states the rules twice, since 15 is just 3 × 5. What
would a third rule cost under each version?

## Step 6 · edge (checkpoint #1)

```
✗ rejects 0    →  expected function to throw an error, but it didn't
✗ rejects -1   →  expected function to throw an error, but it didn't
✗ rejects -15  →  expected function to throw an error, but it didn't
```

Here is what the step-5 code actually returned: `0 → "FizzBuzz"`, `-1 → "-1"`,
`-15 → "FizzBuzz"`. That is the `0 % 3 === 0` trap, and step 5 made it louder,
because zero now earns both words.

```js
if (n < 1) {
  throw new RangeError(`convert expects n >= 1, got ${n}`);
}
```

The story says to refuse these inputs but not how. `RangeError` is the mob's
choice.

## Step 7 · negative (checkpoint #1)

```
✗ rejects 3          →  expected function to throw an error, but it didn't
✗ rejects 3.5        →  expected function to throw an error, but it didn't
✗ rejects null       →  expected error to be instance of TypeError
✗ rejects undefined  →  expected function to throw an error, but it didn't
✗ rejects NaN        →  expected function to throw an error, but it didn't
✗ rejects {}         →  expected function to throw an error, but it didn't
```

The first one is the string `'3'`; `it.each`'s `%s` drops the quotes. Step 6's code
returned:
- `'3' → "Fizz"`, because JavaScript coerced it
- `3.5 → "3.5"`
- `undefined → "undefined"`
- `NaN → "NaN"`
- `{} → "[object Object]"`

**`null` did throw, but a `RangeError`.** `null < 1` coerces to `0 < 1`, so it is
red for the wrong error type.

The GREEN is a type guard, and it goes **above** the range guard. `null` is the case
that forces that order.

```js
if (!Number.isInteger(n)) {
  throw new TypeError(`convert expects an integer, got ${typeof n}: ${String(n)}`);
}
```

For the REFACTOR, both guards move into `#validate(n)`, so `convert` reads as
"validate, then compute".

## Step 8 · the pattern menu (checkpoint #2), one pattern at a time

The named-constants housekeeping is skipped: 8a's table already names 3, 5, Fizz
and Buzz, all in one place.

**8a · (a) rules engine.** This is a pure refactor: the tests are untouched and
still 18 green.

```js
const RULES = [
  [3, 'Fizz'],
  [5, 'Buzz'],
];
// …
const word = RULES
  .filter(([divisor]) => n % divisor === 0)
  .map(([, ruleWord]) => ruleWord)
  .join('');
```

Payoff check: a new rule is one more row, and `convert` doesn't change.

**8b · (b) value object.** A `Rule` with `matches(n)` decides for itself (Tell,
Don't Ask). It is still not exported, because no test needs it yet.

**8c · (d) dependency injection**, driven test-first because it changes the public
API. First, add the test that wishes the API existed:

```
✗ uses injected rules instead of the defaults  →  TypeError: __vite_ssr_import_1__.Rule is not a constructor
```

`Rule` isn't exported. After exporting it, the test is red for the real reason:

```
✗ uses injected rules instead of the defaults  →  expected '4' to be 'Even'
```

The GREEN is a `#rules` field and `constructor(rules = RULES)`. This is the
finished classic solution.

Declined, with the reasons from the reference header:
- **(c) Strategy:** only divisibility exists so far, so it is YAGNI. Step 10 changes
  that.
- **(e) Factory:** there is only one preset.
- **(f) Null Object:** "fire only when nothing else did" smuggles the conditional
  back in.
- **Builder, Chain of Responsibility and Template Method:** see the skeleton's menu.

## Step 9 · extended R7: even numbers with no word print `*`

This is the extended story's first change. Fizz is still the three times table here.

1. **Update the test list first.** Write todos for both changes; the 7 for step 10
   stay pending at the bottom of the file.
2. **Predict the damage.** Only 2 and 4 in `non-multiples` should break. The DI test
   survives: 3 is odd, and 4 gets `'Even'`.
3. **RED.** 2 and 4 move to a new star block, and 8, 14 and 16 join them:
   ```
   ✗ convert(2) === "*"  →  expected '2' to be '*'
   ✗ convert(4) === "*"  →  expected '4' to be '*'
   … and the same for 8, 14 and 16
   ```
4. **GREEN:**
   ```js
   if (word) return word;
   return n % 2 === 0 ? '*' : String(n);
   ```
5. **"A word beats `*`"** (6 → Fizz, 10 → Buzz, 30 → FizzBuzz) passes on arrival. A
   test that has never failed hasn't proven anything, so break the code on purpose
   by checking for even before `word`. **Five** tests go red, not three:
   ```
   ✗ convert(20) === "Buzz"                         →  expected '*' to be 'Buzz'
   ✗ 6 is even but a multiple of 3 → Fizz, not "*"  →  expected '*' to be 'Fizz'
   ✗ 10 is even but a multiple of 5 → Buzz          →  expected '*' to be 'Buzz'
   ✗ 30 → FizzBuzz, not "*"                         →  expected '*' to be 'FizzBuzz'
   ✗ uses injected rules instead of the defaults    →  expected '*' to be 'Even'
   ```
   20 and the DI test were already guarding the order without saying so; the new
   block states that rule by name. Then revert.
6. **REFACTOR.** The title `non-multiples` is now wrong, because 2 is a non-multiple
   that prints `*`. It becomes `numbers with no word`: 1, 7, 11.

*Discuss:* the story asks where the star belongs. Not in the rule list, because it
never joins other words; it is what happens when nothing matched. That is also why
Null Object is declined again.

## Step 10 · extended R8: Fizz means "contains a 3"

1. **Predict the damage, then make it real.** Before touching the code, rewrite the
   four expectations the story changes:
   - 9 → `"9"`, which moves to `numbers with no word`
   - 15 → `"Buzz"` and 45 → `"Buzz"`
   - 6 → `"*"`: it lost its word, so it moves from "a word beats `*`" to the star block

   Run, and exactly those four go red:
   ```
   ✗ convert(9) === "9"      →  expected 'Fizz' to be '9'
   ✗ convert(15) === "Buzz"  →  expected 'FizzBuzz' to be 'Buzz'
   ✗ convert(45) === "Buzz"  →  expected 'FizzBuzz' to be 'Buzz'
   ✗ convert(6) === "*"      →  expected 'Fizz' to be '*'
   ```
   Nothing is deleted. A broken test is updated on purpose and keeps guarding.
2. **RED**, the first new case:
   `✗ convert(13) === "Fizz"  →  expected '13' to be 'Fizz'`.
3. **GREEN.** "Contains a 3" isn't a divisor, so a rule can no longer be a
   `(divisor, word)` pair. It becomes a predicate: menu item (c) Strategy, declined
   in step 8 and earned now.
   ```js
   new Rule((n) => String(n).includes('3'), 'Fizz')
   ```
   All five go green, and then the DI test goes red:
   ```
   ✗ uses injected rules instead of the defaults  →  TypeError: rule.matches is not a function
   ```
   `new Rule(2, 'Even')` passes a number where a predicate now goes. Did the mob
   predict this one? Rewrite it as `new Rule((n) => n % 2 === 0, 'Even')`.
4. **Promote the rest:**
   - 23 and 33 → Fizz
   - 35 and 130 → FizzBuzz
   - 32 → Fizz, not `*`. This is the "word beats" block's new even Fizz, now that 6
     has left it.
   - 50 → Buzz
5. **REFACTOR.** Split the word-rules table into one block per rule: Fizz, Buzz and
   FizzBuzz.

*Discuss:*
- Run step 10's tests against step 9's code and 10 of them fail, but **30 and 33
  pass**. They satisfy the old Fizz rule and the new one, so they can't tell the
  rules apart. 13, 23, 35 and 130 can.
- **Several 3s:** does 33 print `"FizzFizz"`? No. The story says "contains a 3", so a
  rule fires once or not at all, and 33 → `"Fizz"` in the Fizz block pins that. 333
  and 330 behave the same (`"Fizz"`, `"FizzBuzz"`).
