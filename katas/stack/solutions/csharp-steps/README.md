# Stack (C#), step by step

**Spoilers.** Like the rest of `solutions/`, keep this closed until the retro.

This folder shows one mob's path through the kata, saved after each step. Each
`step-*` folder holds `Stack.cs` and `StackTests.cs` as they stand at the **end**
of that step. Steps 9 and 10 add `BoundedStack.cs` and `BoundedStackTests.cs`.
Two of the steps are the reference solutions:
- **Step 8b** ([`step-08b-iterator/`](step-08b-iterator/)) is the finished
  classic stack.
- **Step 10** ([`step-10-extended-full/`](step-10-extended-full/)) is the
  finished bounded stack, a Decorator around step 8b's untouched `Stack`.

**No native stack.** There is no `Stack<T>` from .NET, no `List<T>`, and no
`Array.Resize`. The store is a plain array, read and written only by index, with
the stack keeping its own count. A C# array has a fixed length, so when it is full
the stack copies everything into a bigger one by hand. `_items.Length` means "how
big is the array", never "how many items".

## Running a step

The steps have no `.csproj`. Copy a step over the skeleton **in a scratch copy of
the repo**:

```bash
# from katas/stack/
cp solutions/csharp-steps/step-05-boundary/*.cs csharp/ && (cd csharp && dotnet test)
```

To see what one step changed, from this folder: `diff -u step-04-positive step-05-boundary`.

**`step-01-red` doesn't build, on purpose.**

| Step | Folder | `dotnet test` |
|---|---|---|
| 0 · test list | `step-00-test-list` | 1 passed, 17 skipped |
| 1 · RED | `step-01-red` | **build error** |
| 2 · GREEN | `step-02-green` | 1 passed, 16 skipped |
| 3 · REFACTOR (generalize) | `step-03-refactor` | 2 passed, 15 skipped |
| 4 · other positives | `step-04-positive` | 6 passed, 11 skipped |
| 5 · boundary | `step-05-boundary` | 11 passed, 6 skipped |
| 6 · edge | `step-06-edge` | 16 passed, 1 skipped |
| 7 · negative | `step-07-negative` | 16 passed |
| 8a · generics | `step-08a-generics` | 17 passed |
| 8b · iterator | `step-08b-iterator` | 19 passed |
| 9 · extended R7, capacity | `step-09-extended-capacity` | 26 passed, 7 skipped |
| 10 · extended R8, full | `step-10-extended-full` | 19 + 14 = **33** passed |

The order follows the skeleton's prompts: positives and the boundary, then
checkpoint #1 (edge, then negative), then checkpoint #2 (the pattern menu), then
the extended story's two requirements. Until step 8a the stack holds `object?`.
Each RED below comes from a real run: step N's tests against step N−1's code.

---

## Step 0 · the test list

The class gets 17 skipped `[Fact]`s: the skeleton's 2 plus 15 more, named as
**sequences**. C#-specific ones:
- an empty pop is catchable as an `InvalidOperationException`, which is what
  .NET's own stack throws
- **zero** is an item, not "empty": `default(int)` is `0`, the sharp version of
  the null ambiguity

## Step 1 · RED

```
error CS1061: 'Stack' does not contain a definition for 'IsEmpty' …
```

## Steps 2–4 · GREEN, generalize, positives

- **Step 2:** `return true;`.
- **Step 3:** "is not empty after a push" doesn't build (CS1061 for `Push`). The
  minimum is a `_count` field that `Push` increments. **`Push` stores nothing
  yet.**
- **Step 4:** `Pop`, `Peek` and `Size` don't build. **Still no array.** One
  `object? _top` field passes all four positive tests. The REFACTOR extracts the
  `StackOf(params object?[])` helper.

## Step 5 · boundary: LIFO, and as many items as you push

Three of the five new tests pass on arrival against the single field. Two go red:

```
✗ PopsTheRestInReverseOrderOfPushing                    Assert.Equal() Failure: Collections differ
✗ HoldsAsManyItemsAsYouPushAndGivesThemBackInReverse    Assert.Equal() Failure: Values differ
```

**The array arrives:** `new object?[InitialCapacity]` (4), indexed by `_count`.
That fixes LIFO, and the growth test stays red for a new reason:

```
✗ HoldsAsManyItemsAsYouPushAndGivesThemBackInReverse    System.IndexOutOfRangeException : Index was outside the bounds of the array.
```

So `Push` grows the array when it's full, copying into one twice the size by hand.
`Pop` resets the slot it just emptied, so the stack doesn't keep popped objects
alive.

## Step 6 · edge (checkpoint #1): the empty contract

The first red is the compiler's (CS0246: `EmptyStackException` could not be
found). With the class in place, three tests are red: the array **already**
refuses an empty pop, but with `IndexOutOfRangeException`.

```
✗ TheEmptyContractPopAndPeekBothThrow                Assert.Throws() Failure: Exception type was not an exact match
✗ ThrowsAgainAfterAStackIsEmptiedByPopping           Assert.Throws() Failure: Exception type was not an exact match
✗ TheErrorIsCatchableAsAnInvalidOperationException   Assert.IsAssignableFrom() Failure: Value is an incompatible type
```

The GREEN is `EmptyStackException : InvalidOperationException` and one guard each
in `Pop` and `Peek`. Both "nothing-shaped" tests, null and **zero**, pass on
arrival, because throwing already avoids the `default(T)` ambiguity.

## Step 7 · negative (checkpoint #1): the compiler keeps the array private

The mob tries to write the tamper test:

```csharp
var stack = StackOf("a", "b");
stack._items[1] = "x";
```

```
error CS0122: 'Stack._items' is inaccessible due to its protection level
```

It never builds, so **the compiler is this test.** There is no production
change, and the test becomes a comment saying why. Reflection with
`BindingFlags.NonPublic` can still break in, but that's deliberate, not an
accident.

## Step 8 · the pattern menu (checkpoint #2), one pattern at a time

**8a · (a) `Stack<T>`, and the native stack that was hiding all along.** Run the
generic tests against step 7's non-generic class. You'd expect "`Stack` is not
generic". Instead:

```
error CS1061: 'Stack<string>' does not contain a definition for 'IsEmpty' …
error CS1061: 'Stack<int>' does not contain a definition for 'Size' …
```

`Stack<string>` compiled. With no generic `Kata.Stack<T>` to find, the name
resolved through the implicit usings to **.NET's own
`System.Collections.Generic.Stack<T>`**, the very native stack this kata
forbids. It only fails because the mob called its methods `IsEmpty` and `Size`,
where .NET says `Count`. A mob that had picked .NET's names would have had its
tests silently running against .NET's stack.

The GREEN is `Stack<T>` over a real `T[]` (C# generics are reified, so this is
allowed, unlike Java), plus the pin test
`Assert.NotSame(typeof(Stack<string>), typeof(Stack<int>))`.

*Discuss:* which net caught this, and what would have caught it if the names had
matched?

**8b · (e) `IEnumerable<T>`.** The tests call `ToList()` and `ToArray()`, which
don't build until the stack is enumerable (CS1061). `GetEnumerator` uses
`yield return` over the indexes from `_count - 1` down to `0`, so it hands out
values and never the array. This is the finished classic solution, with 19 tests.

## Step 9 · extended R7: a capacity fixed at creation

**Predicted damage: none.** `BoundedStack<T>` wraps a private `Stack<T>`, and
`Stack.cs` and `StackTests.cs` are byte-identical to step 8b. The first red is
CS0246 (`BoundedStack<>` could not be found). Only the capacity's **value** needs
a test (`-1`, `-42` → `ArgumentOutOfRangeException`); `int capacity` rules out
the rest.

## Step 10 · extended R8: a full stack refuses more

The R8 tests don't build: CS1061 for `IsFull`, and CS0246 for
`FullStackException`. The GREEN is `IsFull()` plus a guard in `Push`. The total is
19 classic + 14 bounded = **33** tests.
