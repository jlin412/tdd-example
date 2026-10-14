# FizzBuzz (Java), step by step

**Spoilers.** Like the rest of `solutions/`, keep this closed until the retro.

This folder shows one mob's path through the kata, saved after each step. Each
`step-*` folder holds `FizzBuzz.java` and `FizzBuzzTest.java` as they stand at the
**end** of that step. Two of them are the reference solutions:
- **Step 8c** ([`step-08c-dependency-injection/`](step-08c-dependency-injection/))
  is the finished classic game.
- **Step 10** ([`step-10-extended-digit-three/`](step-10-extended-digit-three/))
  is the finished extended game, worked **in place** in the same `FizzBuzz.java`.

## Running a step

The steps have no `pom.xml` of their own. Copy a step over the skeleton **in a
scratch copy of the repo**, so the real skeleton stays pristine:

```bash
# from katas/fizzbuzz/
STEP=solutions/java-steps/step-05-boundary
cp $STEP/FizzBuzz.java     java/src/main/java/kata/
cp $STEP/FizzBuzzTest.java java/src/test/java/kata/
(cd java && mvn test)
```

To see exactly what one step changed, from this folder:

```bash
diff -u step-04-positive step-05-boundary
```

**`step-01-red` doesn't compile, on purpose.** In a statically typed language,
"does not compile" is a legitimate first RED.

| Step | Folder | `mvn test` |
|---|---|---|
| 0 · test list | `step-00-test-list` | 1 passed, 13 skipped |
| 1 · RED | `step-01-red` | **compile error** |
| 2 · GREEN | `step-02-green` | 1 passed, 12 skipped |
| 3 · REFACTOR (generalize) | `step-03-refactor` | 3 passed, 10 skipped |
| 4 · other positives | `step-04-positive` | 7 passed, 6 skipped |
| 5 · boundary | `step-05-boundary` | 9 passed, 4 skipped |
| 6 · edge | `step-06-edge` | 12 passed, 1 skipped |
| 7 · negative | `step-07-negative` | 12 passed |
| 8a · rules engine | `step-08a-rules-engine` | 12 passed |
| 8b · value object | `step-08b-value-object` | 12 passed |
| 8c · dependency injection | `step-08c-dependency-injection` | 13 passed |
| 9 · extended R7, the star | `step-09-extended-star` | 19 passed, 7 skipped |
| 10 · extended R8, contains a 3 | `step-10-extended-digit-three` | 28 passed |

(A parameterized test counts once per row, so 3 passed means three rows.)

The order follows the skeleton's prompts: positives and the boundary, then
checkpoint #1 (edge, then negative), then checkpoint #2 (the pattern menu), then
the extended story's two changes, one at a time.

Steps 0–2 are the skeleton files plus only the mob's move. From step 3 on, the
skeleton's coaching comments are gone, and any todos not yet promoted wait at the
bottom of the test class as `@Disabled` tests. Each RED quoted below comes from a
real run: step N's tests against step N−1's code.

**What's different in Java:** the compiler is part of the test suite. It gives
the first RED (step 1). It replaces the whole [negative] group (step 7). And it
catches two of the reds that JavaScript only finds at runtime (steps 8c and 10).

---

## Step 0 · the test list

Before any code, the class gets 13 `@Disabled` tests: the 2 the skeleton seeds
plus 11 more, grouped as [positive], [boundary], [edge] and [negative]. The
[negative] group is a single open question ("`"3"`, 3.5 and null are rejected,
or does the compiler do it for us?") that step 7 answers.

## Step 1 · RED

Uncomment the STEP 1 assertion and delete the disabled test it promotes. It
doesn't compile:

```
FizzBuzzTest.java: cannot find symbol
  symbol:   method convert(int)
  location: variable fizzBuzz of type kata.FizzBuzz
```

## Step 2 · GREEN

```java
public String convert(int n) {
    return "1";
}
```

## Step 3 · REFACTOR: generalize

Generalizing takes one more red, a second example:

```
✗ nonMultiplesReturnTheNumber   expected: <2> but was: <1>
✗ nonMultiplesReturnTheNumber   expected: <4> but was: <1>
```

The fix is `return Integer.toString(n);`. The tests get a refactor too: a
`fizzBuzz` field, and a `@ParameterizedTest` with `@CsvSource`.

## Step 4 · the other positives

```
✗ wordRules   expected: <Fizz> but was: <3>
✗ wordRules   expected: <Fizz> but was: <9>
✗ wordRules   expected: <Buzz> but was: <5>
✗ wordRules   expected: <Buzz> but was: <20>
```

In the session these go one at a time: 3 alone could be passed with `n == 3`, and
9 forces `n % 3 == 0`.

## Step 5 · boundary

```
✗ wordRules   expected: <FizzBuzz> but was: <Fizz>    (15)
✗ wordRules   expected: <FizzBuzz> but was: <Fizz>    (45)
```

The minimum GREEN is a special case, `if (n % 15 == 0) return "FizzBuzz";`. The
REFACTOR joins the words into a `matched` string instead.

## Step 6 · edge (checkpoint #1)

```
✗ valuesBelowOneAreRejected   Expected java.lang.IllegalArgumentException to be thrown, but nothing was thrown.
  … three times, for 0, -1 and -15
```

Step 5's code returned `"FizzBuzz"` for 0 and -15: the `0 % 3 == 0` trap.

```java
if (n < 1) {
    throw new IllegalArgumentException("convert expects n >= 1, got " + n);
}
```

## Step 7 · negative (checkpoint #1): the compiler's step

The mob promotes the last disabled test and tries to write it:

```java
assertThrows(IllegalArgumentException.class, () -> fizzBuzz.convert("3"));
assertThrows(IllegalArgumentException.class, () -> fizzBuzz.convert(3.5));
assertThrows(IllegalArgumentException.class, () -> fizzBuzz.convert(null));
```

It never runs. `javac` rejects every line:

```
incompatible types: java.lang.String cannot be converted to int
incompatible types: possible lossy conversion from double to int
incompatible types: <nulltype> cannot be converted to int
```

That compile error **is** the red, and there is no GREEN to write. The test can't
exist, so it becomes a comment saying why. The production code doesn't change.
Python and JavaScript need a runtime type guard and a table of [negative] tests
here; Java gets that for free.

*Discuss:* which safety net caught more today, the compiler or the tests? What
can't `int` rule out? (Answer: 0 and negatives. That's why step 6 is still yours.)

## Step 8 · the pattern menu (checkpoint #2), one pattern at a time

**8a · (a) rules engine.** The skeleton warns that in Java "a table needs an
element type, which quietly pulls in (b)". To keep the two moves apart, this
table borrows a type that already exists:

```java
private static final List<Map.Entry<Integer, String>> RULES = List.of(
        Map.entry(3, "Fizz"),
        Map.entry(5, "Buzz")
);
```

(Not `Map.of(...)`: its iteration order is unspecified, and "FizzBuzz" could come
out as "BuzzFizz".) This is a pure refactor, still 12 green.

**8b · (b) value object.** `Map.Entry`'s `getKey()` and `getValue()` say nothing
about the domain, so a `private record Rule(int divisor, String word)` with
`matches(n)` replaces them. It is private, because no test needs it yet.

**8c · (d) dependency injection**, driven test-first. The test that wishes the API
existed doesn't compile, twice over:

```
kata.FizzBuzz.Rule has private access in kata.FizzBuzz
```

Make `Rule` public, and the next compile error is the missing constructor:

```
constructor FizzBuzz in class kata.FizzBuzz cannot be applied to given types;
  required: no arguments
  found:    java.util.List<kata.FizzBuzz.Rule>
```

The GREEN is `FizzBuzz(List<Rule> rules)` (defensively copied), with the no-arg
constructor delegating to the defaults. This is the finished classic solution.

## Step 9 · extended R7: even numbers with no word print `*`

1. **Update the test list first.** Step 10's 7 disabled tests wait at the bottom.
2. **Predict the damage:** only 2 and 4 in the non-multiples rows.
3. **RED:**
   ```
   ✗ evenNumbersWithNoWordReturnStar   expected: <*> but was: <2>
   … and the same for 4, 8, 14 and 16
   ```
4. **GREEN:** after the stream, `if (!matched.isEmpty()) return matched;`, then
   `return n % 2 == 0 ? "*" : Integer.toString(n);`.
5. **`aWordBeatsStar`** (6, 10, 30) passes on arrival. Check for even first, and
   **3** tests go red: `aWordBeatsStar`, `wordRules` for 20, and the DI test.
   Then revert.
6. **REFACTOR:** the non-multiples rows become `numbersWithNoWord`: 1, 7 and 11.

## Step 10 · extended R8: Fizz means "contains a 3"

1. **Predict the damage, then make it real.** Rewrite 9 → `"9"`, 15 → `"Buzz"`,
   45 → `"Buzz"`, and 6 → `"*"` (6 moves to the star rows). These four go red.
2. **RED:** 13 → `expected: <Fizz> but was: <13>`.
3. **GREEN.** A rule becomes `record Rule(Predicate<Integer> predicate, String
   word)`: Strategy (c), earned now. This time **the compiler** catches the
   surprise, not a test run:
   ```
   incompatible types: int cannot be converted to java.util.function.Predicate<java.lang.Integer>
   ```
   `new FizzBuzz.Rule(2, "Even")` no longer fits the record. Rewrite it as
   `new FizzBuzz.Rule(n -> n % 2 == 0, "Even")`.
4. **Promote the rest:** 23 and 33 → Fizz; 35 and 130 → FizzBuzz; 32 → Fizz (in
   `aWordBeatsStar`, replacing 6); 50 → Buzz.
5. **REFACTOR:** split `wordRules` into Fizz, Buzz and FizzBuzz tests.

Step 10's tests (minus the DI test, which won't compile against the old `Rule`)
fail 9 times against step 9's code. 30 and 33 pass under both Fizz rules, so they
can't tell them apart; 13, 23, 35 and 130 can.
