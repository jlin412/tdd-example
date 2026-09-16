# Stack — reference solutions

**Spoilers.** Keep this folder closed until the retro.

Two finished implementations, each with a complete test suite. These are **one
possible end state**, not the only correct answer — the point of the retro is to
compare your mob's design against this and argue with its choices.

- **Classic** ([`stack.ts`](typescript/stack.ts)) — per
  [StackStory.md](../StackStory.md).
- **Extended** ([`boundedStack.ts`](typescript/boundedStack.ts)) — the capacity
  limit, per [StackExtendedStory.md](../StackExtendedStory.md).

## Design shown here — classic

From the checkpoint-#1 pattern menu, this "team" implemented — one at a time,
per the coach's rule:

- **(a) Generic type parameter** — `Stack<T>`, so the compiler knows what comes
  back out of `pop()`.
- **(e) Iterator** — `[Symbol.iterator]()`, so callers can walk the stack with
  `for…of` or spread it without ever getting a reference to the array.

And deliberately **declined** (reasons in the file's header comment):

- **(b) Swappable backing store** — a JS array is already O(1) at the end. The
  interface is worth introducing the day a *second* implementation exists.
- **(d) Result / Option** — this team throws, and throwing is unambiguous.
  `tryPop()` earns its place only under the "return undefined" contract.
- **(f) Immutable stack** — a fine design, but a different one: it changes the
  public API and every caller with it.

### The empty contract, and why

The reference **throws** (`EmptyStackError`). The alternative — returning
`undefined`, the way `Array.prototype.pop()` does — is perfectly defensible
right up until you push `undefined` *into* the stack, at which point a caller
cannot tell an empty stack from a stack whose top item is `undefined`.

That ambiguity is the argument, and there's a test for it
(`can hold undefined without that meaning "empty"`). A mob that chose the
sentinel contract should look at that test and decide whether they care.

## What the extended solution adds

The capacity requirement is what **earns menu item (c) Decorator**. Note what
did *not* happen: `stack.ts` was never opened. `BoundedStack` holds a plain
`Stack` and guards the way in, so every test in `stack.spec.ts` still passes,
untouched — the Open/Closed Principle with a green suite as the receipt.

Adding a `capacity` field to `Stack` itself is a legitimate alternative, but it
makes capacity a concern of *every* stack, forces a constructor argument on
callers who never wanted a limit, and puts the existing tests at risk.

A third option worth naming in the retro: **evicting the bottom item** when full.
That isn't a bounded stack at all — it's a ring buffer wearing a stack's clothes.

## Running the finished state

These files use `isEmpty()` / `size()` and throw on empty. Those are one set of
choices, not a mandate — if your mob picked the same shape, they're drop-in:

```bash
# from katas/stack/
cp solutions/typescript/*.ts typescript/
(cd typescript && npm test && npm run typecheck)
```

Both commands matter: `npm test` covers behavior, `npm run typecheck` covers the
generics. Vitest does not type-check.

(Use a scratch copy of the repo if you want to keep the skeleton pristine — the
copy above overwrites `typescript/stack.ts` and `typescript/stack.spec.ts`.)
