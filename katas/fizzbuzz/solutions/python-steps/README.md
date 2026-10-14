# FizzBuzz (Python), step by step

**Spoilers.** Like the rest of `solutions/`, keep this closed until the retro.

This folder shows one mob's path through the kata, saved after each step. Each
`step-*` folder holds `fizzbuzz.py` and `test_fizzbuzz.py` as they stand at the
**end** of that step. Two of them are the reference solutions:
- **Step 8c** ([`step-08c-dependency-injection/`](step-08c-dependency-injection/))
  is the finished classic game.
- **Step 10** ([`step-10-extended-digit-three/`](step-10-extended-digit-three/))
  is the finished extended game, worked **in place** in the same `fizzbuzz.py`.

## Running a step

Each folder runs on its own, with no copying. Install pytest once
(`pip install -r ../../python/requirements.txt`), then:

```bash
cd step-05-boundary && pytest
```

Run one folder at a time: every step has a `fizzbuzz.py`, so a single `pytest`
across all of them would mix the modules up.

To see exactly what one step changed, from this folder:

```bash
diff -u step-04-positive step-05-boundary
```

**`step-01-red` fails on purpose.** That failure is the RED.

| Step | Folder | `pytest` |
|---|---|---|
| 0 · test list | `step-00-test-list` | 1 passed, 17 skipped |
| 1 · RED | `step-01-red` | **1 failed**, 16 skipped |
| 2 · GREEN | `step-02-green` | 1 passed, 16 skipped |
| 3 · REFACTOR (generalize) | `step-03-refactor` | 3 passed, 14 skipped |
| 4 · other positives | `step-04-positive` | 7 passed, 10 skipped |
| 5 · boundary | `step-05-boundary` | 9 passed, 8 skipped |
| 6 · edge | `step-06-edge` | 12 passed, 5 skipped |
| 7 · negative | `step-07-negative` | 17 passed |
| 8a · rules engine | `step-08a-rules-engine` | 17 passed |
| 8b · value object | `step-08b-value-object` | 17 passed |
| 8c · dependency injection | `step-08c-dependency-injection` | 18 passed |
| 9 · extended R7, the star | `step-09-extended-star` | 24 passed, 7 skipped |
| 10 · extended R8, contains a 3 | `step-10-extended-digit-three` | 33 passed |

The order follows the skeleton's prompts: positives and the boundary, then
checkpoint #1 (edge, then negative), then checkpoint #2 (the pattern menu), then
the extended story's two changes, one at a time.

Steps 0–2 are the skeleton files plus only the mob's move. From step 3 on, the
skeleton's coaching comments are gone, and any todos not yet promoted wait at the
bottom of the test file as skipped tests. Each RED quoted below comes from a real
run: step N's tests run against step N−1's code.

---

## Step 0 · the test list

Before any code, the file gets 17 skipped tests: the 2 the skeleton seeds plus 15
more, grouped as [positive], [boundary], [edge] and [negative]. Each one becomes
one case in the finished classic suite. The [negative] group includes `True`,
the skeleton's "bool is an int" gotcha. The DI test is not on the list, because
it comes from a design decision in step 8c, not from the story.

## Step 1 · RED

Uncomment the STEP 1 assertion and delete the skipped test it promotes.

```
✗ test_returns_the_number_as_a_string_when_nothing_special_applies
    AttributeError: 'FizzBuzz' object has no attribute 'convert'
```

It fails for the right reason: the method doesn't exist yet.

## Step 2 · GREEN

```python
def convert(self, n):
    return "1"
```

The answer is hard-coded, and that is legitimate. Unlike JavaScript, Python
needs the `n` parameter even now: calling `convert(1)` on a method with no
parameter is itself a `TypeError`.

## Step 3 · REFACTOR: generalize

Generalizing takes one more red: a second example the hard-coded answer can't
pass. Step 2's code returns `'1'` for both 2 and 4:

```
✗ test_non_multiples_return_the_number[2-2]   assert '1' == '2'
✗ test_non_multiples_return_the_number[4-4]   assert '1' == '4'
```

The fix is `return str(n)`. Then the tests get a refactor too: `fizz_buzz` moves
to module scope, and the STEP 1 test becomes a `@pytest.mark.parametrize` table.

## Step 4 · the other positives

```
✗ test_word_rules[3-Fizz]    assert '3' == 'Fizz'
✗ test_word_rules[9-Fizz]    assert '9' == 'Fizz'
✗ test_word_rules[5-Buzz]    assert '5' == 'Buzz'
✗ test_word_rules[20-Buzz]   assert '20' == 'Buzz'
```

In the session these go one at a time. 3 alone could be passed with `n == 3`; 9 is
what forces `n % 3 == 0`. Buzz works the same way with 5, then 20.

## Step 5 · boundary

```
✗ test_word_rules[15-FizzBuzz]   assert 'Fizz' == 'FizzBuzz'
✗ test_word_rules[45-FizzBuzz]   assert 'Fizz' == 'FizzBuzz'
```

The minimum GREEN is a special case on top: `if n % 15 == 0: return "FizzBuzz"`.
The REFACTOR joins the words instead:

```python
matched = ""
if n % 3 == 0:
    matched += "Fizz"
if n % 5 == 0:
    matched += "Buzz"
return matched or str(n)
```

## Step 6 · edge (checkpoint #1)

```
✗ test_values_below_one_are_rejected[0]     Failed: DID NOT RAISE <class 'ValueError'>
✗ test_values_below_one_are_rejected[-1]    Failed: DID NOT RAISE <class 'ValueError'>
✗ test_values_below_one_are_rejected[-15]   Failed: DID NOT RAISE <class 'ValueError'>
```

Step 5's code returned `0 → 'FizzBuzz'`, `-1 → '-1'` and `-15 → 'FizzBuzz'`. That
is the `0 % 3 == 0` trap, made louder by step 5.

```python
if n < 1:
    raise ValueError(f"convert expects n >= 1, got {n}")
```

## Step 7 · negative (checkpoint #1)

This is the most surprising red in the Python track. **Only 2 of the 5 new tests
go red:**

```
✗ test_non_integer_input_is_rejected[3.5]    Failed: DID NOT RAISE <class 'TypeError'>
✗ test_non_integer_input_is_rejected[True]   Failed: DID NOT RAISE <class 'TypeError'>
```

Step 6's code returned `3.5 → '3.5'`, and `True → 'True'` because `True` is an
`int` equal to 1. The other three passed on arrival, **by accident**:

```
'3'  → raises TypeError: '<' not supported between instances of 'str' and 'int'
None → raises TypeError: '<' not supported between instances of 'NoneType' and 'int'
[3]  → raises TypeError: '<' not supported between instances of 'list' and 'int'
```

That `TypeError` comes from the range check's `<`, not from any decision the mob
made. Reorder the code, or compare differently, and it vanishes.

The GREEN is a real type guard, above the range check, and it must reject `bool`
explicitly:

```python
if isinstance(n, bool) or not isinstance(n, int):
    raise TypeError(f"convert expects an int, got {type(n).__name__}")
```

For the REFACTOR, both guards move into a `_validate(n)` static method.

*Discuss:* a test that passes on arrival hasn't proven anything yet. Three of
these were green for a reason nobody chose. How would the mob have found out?
(Try deleting the type guard and running step 7 again: the same three stay
green.)

## Step 8 · the pattern menu (checkpoint #2), one pattern at a time

The named-constants housekeeping is skipped: 8a's table already names 3, 5, Fizz
and Buzz, all in one place.

**8a · (a) rules engine.** This is a pure refactor: the tests are untouched and
still 17 green. The table is a plain list of tuples:

```python
RULES = [(3, "Fizz"), (5, "Buzz")]
# …
matched = "".join(word for divisor, word in RULES if n % divisor == 0)
```

**8b · (b) value object.** A `Rule` class with `matches(n)` decides for itself
(Tell, Don't Ask). Python has no export control, but no test uses `Rule` yet.

**8c · (d) dependency injection**, driven test-first because it changes the public
API. Add the test that wishes the API existed:

```
✗ test_uses_injected_rules_instead_of_the_defaults
    TypeError: FizzBuzz() takes no arguments
```

The GREEN is `__init__(self, rules=None)` with a **`None` sentinel**, the
skeleton's "never a mutable default argument" trap. The result is the finished
classic solution.

Declined, with the reasons from the reference header:
- **(c) Strategy:** YAGNI until step 10 earns it.
- **(e) Factory:** there is only one preset.
- **(f) Null Object:** smuggles the conditional back in.
- **Builder, Chain of Responsibility and Template Method:** see the skeleton's menu.

## Step 9 · extended R7: even numbers with no word print `*`

Fizz is still the three times table here.

1. **Update the test list first.** Step 10's 7 todos stay skipped at the bottom.
2. **Predict the damage:** only 2 and 4 in the non-multiples test.
3. **RED**, in a new star test with 2, 4, 8, 14 and 16:
   ```
   ✗ test_even_numbers_with_no_word_return_star[2]   assert '2' == '*'
   … and the same for 4, 8, 14 and 16
   ```
4. **GREEN:**
   ```python
   if matched:
       return matched
   return "*" if n % 2 == 0 else str(n)
   ```
5. **`test_a_word_beats_star`** (6 → Fizz, 10 → Buzz, 30 → FizzBuzz) passes on
   arrival. Break the code on purpose by checking for even before `matched`:
   **3** tests go red. They are the word-beats test, `test_word_rules[20-Buzz]`,
   and the DI test (`convert(4)` returns `'*'` instead of `'Even'`). Those last
   two were already guarding the order without saying so. Then revert.
6. **REFACTOR:** the non-multiples test becomes `test_numbers_with_no_word` with
   1, 7 and 11, since 2 is a non-multiple that now prints `*`.

## Step 10 · extended R8: Fizz means "contains a 3"

1. **Predict the damage, then make it real.** Rewrite the four expectations the
   story changes: 9 → `"9"`, 15 → `"Buzz"`, 45 → `"Buzz"`, and 6 → `"*"` (6 moves
   from the word-beats test to the star test). Exactly those four go red.
2. **RED**, the first new case: `13` → `'13'`, expected `'Fizz'`.
3. **GREEN.** A rule becomes a predicate, which is Strategy (c), earned now:
   `Rule(lambda n: "3" in str(n), "Fizz")`. Then the old DI test goes red:
   ```
   TypeError: 'int' object is not callable
   ```
   `Rule(2, "Even")` passed a number where the predicate goes. Rewrite it as
   `Rule(lambda n: n % 2 == 0, "Even")`.
4. **Promote the rest:**
   - 23 and 33 → Fizz
   - 35 and 130 → FizzBuzz
   - 32 → Fizz, not `*`, which replaces 6 in the word-beats test
   - 50 → Buzz
5. **REFACTOR:** split the word rules into Fizz, Buzz and FizzBuzz tests.

Running step 10's tests against step 9's code, 10 fail. **30 and 33 pass**: they
satisfy both Fizz rules, so they can't tell them apart. And several 3s still make
one Fizz: `33 → 'Fizz'`, `333 → 'Fizz'`, `330 → 'FizzBuzz'`.
