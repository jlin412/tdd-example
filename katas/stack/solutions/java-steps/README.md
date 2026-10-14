# Stack (Java), step by step

**Spoilers.** Like the rest of `solutions/`, keep this closed until the retro.

This folder shows one mob's path through the kata, saved after each step. Each
`step-*` folder holds `Stack.java` and `StackTest.java` as they stand at the
**end** of that step. Step 6 adds `EmptyStackException.java`; steps 9 and 10 add
`BoundedStack.java`, `BoundedStackTest.java` and, at 10, `FullStackException.java`.
Two of the steps are the reference solutions:
- **Step 8b** ([`step-08b-iterator/`](step-08b-iterator/)) is the finished
  classic stack.
- **Step 10** ([`step-10-extended-full/`](step-10-extended-full/)) is the
  finished bounded stack, a Decorator around step 8b's untouched `Stack`.

**No native stack.** There is no `java.util.Stack`, no `Deque`, no `ArrayList`,
and no `Arrays.copyOf`. The store is a plain `Object[]`, read and written only by
index, with the stack keeping its own count. A Java array has a fixed length, so
when it is full the stack copies everything into a bigger one by hand. Reading
`items.length` means "how big is the array", never "how many items".

## Running a step

The steps have no `pom.xml`. Copy a step over the skeleton **in a scratch copy of
the repo**:

```bash
# from katas/stack/
S=solutions/java-steps/step-05-boundary
cp $S/*Test.java java/src/test/java/kata/
for f in $S/*.java; do case $f in *Test.java) ;; *) cp $f java/src/main/java/kata/;; esac; done
(cd java && mvn test)
```

To see what one step changed, from this folder: `diff -u step-04-positive step-05-boundary`.

**`step-01-red` doesn't compile, on purpose.**

| Step | Folder | `mvn test` |
|---|---|---|
| 0 · test list | `step-00-test-list` | 1 passed, 15 skipped |
| 1 · RED | `step-01-red` | **compile error** |
| 2 · GREEN | `step-02-green` | 1 passed, 14 skipped |
| 3 · REFACTOR (generalize) | `step-03-refactor` | 2 passed, 13 skipped |
| 4 · other positives | `step-04-positive` | 6 passed, 9 skipped |
| 5 · boundary | `step-05-boundary` | 11 passed, 4 skipped |
| 6 · edge | `step-06-edge` | 14 passed, 1 skipped |
| 7 · negative | `step-07-negative` | 14 passed |
| 8a · generics | `step-08a-generics` | 15 passed |
| 8b · iterator | `step-08b-iterator` | 18 passed |
| 9 · extended R7, capacity | `step-09-extended-capacity` | 18 + 7 passed, 7 skipped |
| 10 · extended R8, full | `step-10-extended-full` | 18 + 14 = **32** passed |

The order follows the skeleton's prompts: positives and the boundary, then
checkpoint #1 (edge, then negative), then checkpoint #2 (the pattern menu), then
the extended story's two requirements. Until step 8a the stack holds `Object`,
because generics are a menu item reached by refactor. Each RED below comes from a
real run: step N's tests against step N−1's code. Most Java reds are **compile
errors**, and that is the language's lesson.

---

## Step 0 · the test list

The class gets 15 `@Disabled` tests: the skeleton's 2 plus 13 more, named as
**sequences**. There is one [negative] item, "the backing array cannot be
tampered with from outside", which step 7 answers.

## Step 1 · RED

```
StackTest.java: cannot find symbol
  symbol:   method isEmpty()
  location: variable stack of type kata.Stack
```

## Steps 2–4 · GREEN, generalize, positives

- **Step 2:** `return true;`.
- **Step 3:** "a stack is not empty after a push" doesn't compile (`cannot find
  symbol: push`). The minimum is a `count` field that `push` increments.
  **`push` stores nothing yet.**
- **Step 4:** `pop`, `peek` and `size` don't compile. **Still no array.** One
  `Object top` field passes all four positive tests. The REFACTOR extracts the
  `stackOf(Object...)` helper.

## Step 5 · boundary: LIFO, and as many items as you push

Three of the five new tests pass on arrival against the single field: size 0,
size across a sequence, and "pops the most recently pushed item first". Two go red:

```
✗ popsTheRestInReverseOrder   expected: <[c, b, a]> but was: <[c, c, c]>
✗ holdsAsManyItemsAsYouPush   expected: <98> but was: <99>
```

**The array arrives:** `new Object[INITIAL_CAPACITY]` (4), indexed by `count`.
That fixes LIFO, but the growth test stays red, now for a different reason:

```
✗ holdsAsManyItemsAsYouPush » ArrayIndexOutOfBounds Index 4 out of bounds for length 4
```

A Java array can't grow, so the stack makes a bigger one and copies across by hand:

```java
private void grow() {
    Object[] bigger = new Object[items.length * 2];
    for (int i = 0; i < count; i++) {
        bigger[i] = items[i];
    }
    items = bigger;
}
```

`pop` also nulls the slot it just emptied. Without that, the array keeps every
popped object reachable. This is the "obsolete reference" bug: *Effective Java*
(item 7) found it in exactly this kind of hand-rolled stack.

## Step 6 · edge (checkpoint #1): the empty contract

The first red is the compiler's (`cannot find symbol: class EmptyStackException`).
With the class in place, the tests run, and Java turns out to **already** refuse
an empty pop, just with the wrong exception:

```
✗ popAndPeekOnEmptyBothThrow    Unexpected exception type thrown,
                                expected: <kata.EmptyStackException> but was: <java.lang.ArrayIndexOutOfBoundsException>
✗ throwsAgainAfterBeingEmptied  (the same)
```

The array's own bounds check fired at `items[-1]`. Compare Python, where `-1`
silently wraps to the last slot, and TypeScript, where it quietly returns
`undefined`. The GREEN is one guard each in `pop` and `peek`. The `null` test
passes on arrival, because throwing already avoids the "null or empty?" ambiguity.

## Step 7 · negative (checkpoint #1): the compiler keeps the array private

The mob tries to write the tamper test:

```java
Stack stack = stackOf("a", "b");
stack.items[1] = "x";
```

```
items has private access in kata.Stack
```

It never compiles, so **the compiler is this test.** There is no production
change, and the test becomes a comment saying why. (Reflection with
`setAccessible(true)` can still break in. That's a deliberate break-in, not an
accident.) Compare TypeScript, whose `private` let the same test run and wreck
the stack, and Python, which can't stop it at all.

## Step 8 · the pattern menu (checkpoint #2), one pattern at a time

**8a · (a) `Stack<T>`.** Its pin is the erasure test,
`assertSame(new Stack<String>().getClass(), new Stack<Integer>().getClass())`.
Against step 7 it doesn't compile:

```
type kata.Stack does not take parameters
```

Erasure shapes the implementation too. Java can't create a `T[]`, so the array
stays an `Object[]` and each read casts back to `T` (`@SuppressWarnings("unchecked")`).

**8b · (e) `Iterable<T>`.** The tests pass the stack to a `toList(Iterable<T>)`
helper, and that doesn't compile until `Stack` is `Iterable`:

```
method toList in class kata.StackTest cannot be applied to given types;
```

The iterator walks the indexes from `count - 1` down to `0` and hands out values,
never the array. Its `remove()` is the interface default, which throws
`UnsupportedOperationException`, and a test pins that. This is the finished
classic solution, with 18 tests.

## Step 9 · extended R7: a capacity fixed at creation

**Predicted damage: none.** `BoundedStack<T>` wraps a private `Stack<T>`, and
`Stack.java` and `StackTest.java` are byte-identical to step 8b. The first red is
the compiler's (`cannot find symbol: class BoundedStack`).

Only the capacity's **value** needs a test (`-1`, `-42` →
`IllegalArgumentException`); `int capacity` already rules out `1.5` and `"2"`.

## Step 10 · extended R8: a full stack refuses more

The R8 tests don't compile (`cannot find symbol: isFull`, `FullStackException`).
The GREEN is `isFull()` plus a guard in `push`. The total is 18 classic + 14
bounded = **32** tests.
