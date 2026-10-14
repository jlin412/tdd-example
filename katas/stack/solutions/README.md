# Stack — reference solutions

**Spoilers.** Keep this folder closed until the retro.

Each language ships as a **step-by-step trail**: one runnable snapshot after
every TDD step, from the test list to the finished bounded stack, with a
walkthrough that quotes the real failure behind each red. Two of the snapshots
are the finished implementations, each with a complete test suite. These are
**one possible end state**, not the only correct answer — the point of the retro
is to compare your mob's design against this and argue with its choices.

- **Classic** (`Stack`), in `step-08b-iterator/` — per [StackStory.md](../StackStory.md).
- **Extended** (`BoundedStack`), in `step-10-extended-full/` — the capacity limit,
  per [StackExtendedStory.md](../StackExtendedStory.md).

| Language | Trail | Classic · extended tests |
|---|---|---|
| Python | [python-steps/](python-steps/README.md) | 21 · 17 |
| TypeScript | [typescript-steps/](typescript-steps/README.md) | 19 · 15 |
| Java | [java-steps/](java-steps/README.md) | 18 · 14 |
| C# | [csharp-steps/](csharp-steps/README.md) | 19 · 14 |

**No native stack, in any language.** Each `Stack` is built on a plain array
used **only by index**, with its own count: no `push`/`pop`/`append`, no
`ArrayList`, `List<T>`, `Deque` or `java.util.Stack`, and no
`Arrays.copyOf`/`Array.Resize`. A popped slot is cleared so the stack never keeps
an item alive. Python, Java and C# arrays are fixed-size, so those stacks grow by
copying into a bigger array by hand — a red forced by the [boundary] test "holds
as many items as you push". A JavaScript array grows by itself, so the same test
needs no growth code in TypeScript. `BoundedStack` holds no array at all: it
wraps a `Stack`.

## Design shown here — classic

All four languages converge on the same shape, so they can be read side by side.
From the checkpoint-#2 pattern menu, this "team" implemented — one at a time,
per the coach's rule:

- **(a) Generic type parameter** — `Stack<T>`, so a caller knows what comes back
  out of `pop()`. What that buys you differs per language: see below.
- **(e) The iteration protocol** — `__iter__` · `[Symbol.iterator]()` ·
  `Iterable<T>` · `IEnumerable<T>`, so callers can walk the stack without ever
  getting a reference to the backing list.

And deliberately **declined** (reasons in each file's header comment):

- **(b) Swappable backing store** — the store is already hand-rolled on a plain
  array. The interface is worth introducing the day a *second* implementation
  exists.
- **(d) Result / Option** — this team throws, and throwing is unambiguous. A
  `tryPop()` earns its place only under the "return nothing" contract.
- **(f) Immutable stack** — a fine design, but a different one: it changes the
  public API and every caller with it.

### The empty contract, and why

Every reference **throws** (`EmptyStackError` / `EmptyStackException`). The
alternative — handing back nothing, the way `Array.prototype.pop()` does — is
perfectly defensible right up until you push *nothing* into the stack, at which
point a caller cannot tell an empty stack from a stack whose top item is
`None`/`null`/`undefined`.

That ambiguity is the argument, and there's a test for it in every language. C#
gets a second, sharper one: `default(int)` is `0`, so a `Stack<int>` under the
sentinel contract cannot distinguish "empty" from "holding a legitimate zero" —
the case mobs miss, because a null feels special and a zero does not.

Two of the solutions also inherit from their language's conventional base so
existing `catch`/`except` clauses keep working: Python's `EmptyStackError`
subclasses `IndexError` (what `list.pop()` raises), C#'s subclasses
`InvalidOperationException` (what .NET's own `Stack<T>` throws).

### What each language's tests prove that the others' can't

The suites are deliberately near-identical, except where the language forces a
difference — those tests are the retro material:

| Language | The test only it can write |
|---|---|
| Python | a `Stack[int]` cheerfully accepts a string — the hints are documentation, and nothing runs to check them |
| Java | `new Stack<String>().getClass() == new Stack<Integer>().getClass()` — generics are **erased** |
| C# | `typeof(Stack<string>) != typeof(Stack<int>)` — generics are **reified**; plus the `default(int) == 0` trap |
| TypeScript | the type error that passes `npm test` and fails `npm run typecheck` |

The trails add three more cross-language moments, each one a step where the
languages genuinely diverge:

| Step | Python | TypeScript | Java | C# |
|---|---|---|---|---|
| 5 · growth | red: fixed list, `IndexError` | **passes** once the array arrives — it grows itself | red: `ArrayIndexOutOfBounds` | red: `IndexOutOfRangeException` |
| 6 · empty pop, before the guard | `items[-1]` **wraps** to the last slot → `None` | `undefined` | already throws — the wrong exception | already throws — the wrong exception |
| 7 · "can a caller tamper with the array?" | yes, and nothing can stop it → the test pins the convention | yes, through `private` → fixed with `#` fields | doesn't compile | doesn't compile |
| 8a · generics | no red in any net | typecheck-only red (TS2578) | doesn't compile | `Stack<string>` silently binds to **.NET's own** `Stack<T>` |

Conversely, the Python bounded-stack suite is the only one that has to test a
non-integer capacity (`1.5`, `"2"`, `None`, and `True` — bool subclasses int).
In Java and C#, `int capacity` makes those lines impossible to write.

## What the extended solution adds

The capacity requirement is what **earns menu item (c) Decorator**. Note what
did *not* happen: the classic `Stack` was never opened. `BoundedStack` holds a
plain `Stack` and guards the way in, so every test in the classic suite still
passes, untouched — the Open/Closed Principle with a green suite as the receipt.

Adding a `capacity` field to `Stack` itself is a legitimate alternative, but it
makes capacity a concern of *every* stack, forces a constructor argument on
callers who never wanted a limit, and puts the existing tests at risk.

A third option worth naming in the retro: **evicting the bottom item** when full.
That isn't a bounded stack at all — it's a ring buffer wearing a stack's clothes.

## Running the finished state

These use `is_empty()`/`isEmpty()`/`IsEmpty()` and `size()`/`Size()` and throw
on empty. Those are one set of choices, not a mandate — if your mob picked the
same shape, any step drops over the skeleton (use a scratch copy of the repo if
you want to keep the skeletons pristine):

```bash
# from katas/stack/ — the final step; swap the folder name for any other step
S=step-10-extended-full
(cd solutions/python-steps/$S && pytest)
cp solutions/typescript-steps/$S/*.ts typescript/ && (cd typescript && npm test && npm run typecheck)
cp solutions/java-steps/$S/*Test.java java/src/test/java/kata/
for f in solutions/java-steps/$S/*.java; do case $f in *Test.java) ;; *) cp $f java/src/main/java/kata/;; esac; done
(cd java && mvn test)
cp solutions/csharp-steps/$S/*.cs csharp/ && (cd csharp && dotnet test)
```

In TypeScript both commands matter: `npm test` covers behavior, `npm run
typecheck` covers the generics. Vitest does not type-check.
