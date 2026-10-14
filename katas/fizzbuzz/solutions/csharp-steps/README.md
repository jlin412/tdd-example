# FizzBuzz (C#), step by step

**Spoilers.** Like the rest of `solutions/`, keep this closed until the retro.

This folder shows one mob's path through the kata, saved after each step. Each
`step-*` folder holds `FizzBuzz.cs` and `FizzBuzzTests.cs` as they stand at the
**end** of that step. Two of them are the reference solutions:
- **Step 8c** ([`step-08c-dependency-injection/`](step-08c-dependency-injection/))
  is the finished classic game.
- **Step 10** ([`step-10-extended-digit-three/`](step-10-extended-digit-three/))
  is the finished extended game, worked **in place** in the same `FizzBuzz.cs`.

## Running a step

The steps have no `.csproj` of their own. Copy a step over the skeleton **in a
scratch copy of the repo**, so the real skeleton stays pristine:

```bash
# from katas/fizzbuzz/
cp solutions/csharp-steps/step-05-boundary/*.cs csharp/ && (cd csharp && dotnet test)
```

To see exactly what one step changed, from this folder:

```bash
diff -u step-04-positive step-05-boundary
```

**`step-01-red` doesn't compile, on purpose.** In a statically typed language,
"does not compile" is a legitimate first RED.

| Step | Folder | `dotnet test` |
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

(A `[Theory]` counts once per `[InlineData]` row.)

The order follows the skeleton's prompts: positives and the boundary, then
checkpoint #1 (edge, then negative), then checkpoint #2 (the pattern menu), then
the extended story's two changes, one at a time.

Steps 0–2 are the skeleton files plus only the mob's move. From step 3 on, the
skeleton's coaching comments are gone, and any todos not yet promoted wait at the
bottom of the test class as skipped `[Fact]`s. Each RED quoted below comes from a
real run: step N's tests against step N−1's code.

**What's different in C#:** the compiler is part of the test suite. It gives the
first RED (step 1). It replaces the whole [negative] group (step 7). And it
catches two of the reds that JavaScript only finds at runtime (steps 8c and 10).

---

## Step 0 · the test list

Before any code, the class gets 13 skipped `[Fact]`s: the 2 the skeleton seeds
plus 11 more, grouped as [positive], [boundary], [edge] and [negative]. The
[negative] group is a single open question ("`"3"`, 3.5, null, or does the
compiler do it for us?") that step 7 answers.

## Step 1 · RED

Uncomment the STEP 1 assertion and delete the skipped `[Fact]` it promotes. It
doesn't compile:

```
error CS1061: 'FizzBuzz' does not contain a definition for 'Convert' …
```

## Step 2 · GREEN

```csharp
public string Convert(int n)
{
    return "1";
}
```

## Step 3 · REFACTOR: generalize

Generalizing takes one more red, a second example. xUnit reports
`Assert.Equal() Failure: Strings differ` for 2 and 4, because both still get
`"1"`. The fix is `return n.ToString();`. The tests get a refactor too: a
`_fizzBuzz` field, and a `[Theory]` with `[InlineData]` rows.

## Step 4 · the other positives

```
✗ WordRules(n: 3, expected: "Fizz")    Assert.Equal() Failure: Strings differ
✗ WordRules(n: 9, expected: "Fizz")    …
✗ WordRules(n: 5, expected: "Buzz")    …
✗ WordRules(n: 20, expected: "Buzz")   …
```

In the session these go one at a time: 3 alone could be passed with `n == 3`, and
9 forces `n % 3 == 0`.

## Step 5 · boundary

```
✗ WordRules(n: 15, expected: "FizzBuzz")   Assert.Equal() Failure: Strings differ
✗ WordRules(n: 45, expected: "FizzBuzz")   …
```

Both returned `"Fizz"`. The minimum GREEN is a special case for 15; the REFACTOR
joins the words into a `matched` string instead.

## Step 6 · edge (checkpoint #1)

```
✗ ValuesBelowOneAreRejected(n: 0)     Assert.Throws() Failure: No exception was thrown
                                      Expected: typeof(System.ArgumentOutOfRangeException)
  … and the same for -1 and -15
```

Step 5's code returned `"FizzBuzz"` for 0 and -15: the `0 % 3 == 0` trap.

```csharp
if (n < 1)
{
    throw new ArgumentOutOfRangeException(nameof(n), n, "Convert expects n >= 1");
}
```

## Step 7 · negative (checkpoint #1): the compiler's step

The mob promotes the last skipped `[Fact]` and tries to write it:

```csharp
Assert.Throws<ArgumentException>(() => _fizzBuzz.Convert("3"));
Assert.Throws<ArgumentException>(() => _fizzBuzz.Convert(3.5));
Assert.Throws<ArgumentException>(() => _fizzBuzz.Convert(null));
```

It never runs. The compiler rejects every line:

```
error CS1503: Argument 1: cannot convert from 'string' to 'int'
error CS1503: Argument 1: cannot convert from 'double' to 'int'
error CS1503: Argument 1: cannot convert from '<null>' to 'int'
```

That compile error **is** the red, and there is no GREEN to write. The test can't
exist, so it becomes a comment saying why. The production code doesn't change.

*Discuss:* which safety net caught more today, the compiler or the tests? What
can't `int` rule out? (Answer: 0 and negatives. That's why step 6 is still yours.)

## Step 8 · the pattern menu (checkpoint #2), one pattern at a time

**8a · (a) rules engine.** The skeleton warns that in C# "a table needs an element
type, which quietly pulls in (b)". A value tuple keeps the two moves apart:

```csharp
private static readonly (int Divisor, string Word)[] Rules =
{
    (3, "Fizz"),
    (5, "Buzz"),
};
// …
var matched = string.Concat(
    Rules.Where(rule => n % rule.Divisor == 0).Select(rule => rule.Word));
```

This is a pure refactor, still 12 green.

**8b · (b) value object.** The tuple can hold data but can't decide anything, so
a nested `private record Rule(int Divisor, string Word)` with `Matches(n)` takes
over. It is private, because no test needs it yet.

**8c · (d) dependency injection**, driven test-first. The test that wishes the API
existed doesn't compile:

```
error CS1729: 'FizzBuzz' does not contain a constructor that takes 1 arguments
error CS0246: The type or namespace name 'Rule' could not be found …
```

The GREEN moves `Rule` to a public top-level record and adds
`FizzBuzz(IReadOnlyList<Rule> rules)`, with the parameterless constructor
delegating to the defaults. This is the finished classic solution.

## Step 9 · extended R7: even numbers with no word print `*`

1. **Update the test list first.** Step 10's 7 skipped `[Fact]`s wait at the bottom.
2. **Predict the damage:** only 2 and 4 in the non-multiples rows.
3. **RED:** `EvenNumbersWithNoWordReturnStar` fails for 2, 4, 8, 14 and 16
   (`Strings differ`, because each still prints its number).
4. **GREEN:** `if (matched.Length > 0) return matched;`, then
   `return n % 2 == 0 ? "*" : n.ToString();`.
5. **`AWordBeatsStar`** (6, 10, 30) passes on arrival. Check for even first, and
   **3** tests go red: `AWordBeatsStar`, `WordRules(20)`, and the DI test. Then
   revert.
6. **REFACTOR:** the non-multiples rows become `NumbersWithNoWord`: 1, 7 and 11.

## Step 10 · extended R8: Fizz means "contains a 3"

1. **Predict the damage, then make it real.** Rewrite 9 → `"9"`, 15 → `"Buzz"`,
   45 → `"Buzz"`, and 6 → `"*"` (6 moves to the star rows). These four go red.
2. **RED:** `FizzWhenDigitsContainA3(13)` fails.
3. **GREEN.** `Rule` becomes `record Rule(Func<int, bool> Matches, string Word)`:
   Strategy (c), earned now. **The compiler** catches the surprise:
   ```
   error CS1503: Argument 1: cannot convert from 'int' to 'System.Func<int, bool>'
   ```
   `new Rule(2, "Even")` no longer fits. Rewrite it as
   `new Rule(n => n % 2 == 0, "Even")`.
4. **Promote the rest:** 23 and 33 → Fizz; 35 and 130 → FizzBuzz; 32 → Fizz (in
   `AWordBeatsStar`, replacing 6); 50 → Buzz.
5. **REFACTOR:** split `WordRules` into Fizz, Buzz and FizzBuzz theories.

Step 10's tests (minus the DI test, which won't compile against the old `Rule`)
fail 9 times against step 9's code. 30 and 33 pass under both Fizz rules, so they
can't tell them apart; 13, 23, 35 and 130 can.
