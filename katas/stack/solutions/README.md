# Stack — reference solutions

**Spoilers.** Keep this folder closed until the retro.

Each language ships **two** finished implementations, each with a complete test
suite. These are **one possible end state**, not the only correct answer — the
point of the retro is to compare your mob's design against this and argue with
its choices.

- **Classic** (`Stack` / `stack`) — per [StackStory.md](../StackStory.md).
- **Extended** (`BoundedStack` / `bounded_stack`) — the capacity limit, per
  [StackExtendedStory.md](../StackExtendedStory.md).

| Language | Classic | Extended |
|---|---|---|
| Python | [stack.py](python/stack.py) | [bounded_stack.py](python/bounded_stack.py) |
| TypeScript | [stack.ts](typescript/stack.ts) | [boundedStack.ts](typescript/boundedStack.ts) |
| Java | [Stack.java](java/Stack.java) | [BoundedStack.java](java/BoundedStack.java) |
| C# | [Stack.cs](csharp/Stack.cs) | [BoundedStack.cs](csharp/BoundedStack.cs) |

## Design shown here — classic

All four languages converge on the same shape, so they can be read side by side.
From the checkpoint-#1 pattern menu, this "team" implemented — one at a time,
per the coach's rule:

- **(a) Generic type parameter** — `Stack<T>`, so a caller knows what comes back
  out of `pop()`. What that buys you differs per language: see below.
- **(e) The iteration protocol** — `__iter__` · `[Symbol.iterator]()` ·
  `Iterable<T>` · `IEnumerable<T>`, so callers can walk the stack without ever
  getting a reference to the backing list.

And deliberately **declined** (reasons in each file's header comment):

- **(b) Swappable backing store** — every one of these languages' built-in lists
  is already O(1) at the end. The interface is worth introducing the day a
  *second* implementation exists.
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

These files use `is_empty()`/`isEmpty()`/`IsEmpty()` and `size()`/`Size()` and
throw on empty. Those are one set of choices, not a mandate — if your mob picked
the same shape, they're drop-in:

```bash
# from katas/stack/
cp solutions/python/*.py      python/      && (cd python && pytest)
cp solutions/typescript/*.ts  typescript/  && (cd typescript && npm test && npm run typecheck)
cp solutions/java/Stack.java solutions/java/EmptyStackException.java \
   solutions/java/BoundedStack.java solutions/java/FullStackException.java  java/src/main/java/kata/
cp solutions/java/StackTest.java solutions/java/BoundedStackTest.java       java/src/test/java/kata/
(cd java && mvn test)
cp solutions/csharp/*.cs      csharp/      && (cd csharp && dotnet test)
```

In TypeScript both commands matter: `npm test` covers behavior, `npm run
typecheck` covers the generics. Vitest does not type-check.

(Use a scratch copy of the repo if you want to keep the skeletons pristine — the
copies above overwrite them.)
