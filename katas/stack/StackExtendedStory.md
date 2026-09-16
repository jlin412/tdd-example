# Stack — the Extended Story (bounded)

You've shipped [the stack](StackStory.md). Now it grows a limit — and some of
your existing tests will stop even compiling. That's the point: drive the change
test-first, and update the old expectations on purpose.

## What changes

A stack now has a **maximum size, fixed when you create it**. You say how big it
can get, and it never grows past that.

Once it's **full**, it refuses further items. *How* it refuses is your call —
throw? hand back a false? quietly drop the item? push out the bottom one to make
room? — but it must not silently grow. Whether it's currently full is something
a caller can ask.

Everything else about the stack is exactly as it was, and everything the first
story left unsettled is still unsettled the same way.

## A walk through it

| you do this | you get back | the stack is now |
|---|---|---|
| make a new stack that holds 2 | — | *(empty)*, room for 2 |
| push `"a"` | — | `a` |
| push `"b"` | — | `a b` — **full** |
| push `"c"` | *refused* | `a b` |
| pop | `"b"` | `a` — room again |
| push `"c"` | — | `a c` |

## The design question this forces

The obvious move is to open up your existing stack and add a limit to it. Resist
that for a moment and look at the other option: **wrap** it. A bounded stack that
holds a plain stack inside it and guards the way in adds the new behavior without
touching a line of the old class — which means every test you already wrote stays
green, untouched, as proof.

That's pattern-menu item **(c) Decorator**, and this is the requirement that
earns it. Which way you go is the mob's decision — but make it a *decision*, and
be able to say why.

## Your job, in order

1. **Update your test list first.** Which existing tests does a required capacity
   break? Which new ones do you need? Write the list before touching code.
2. **Predict the damage** — mark the tests you expect to fail, and say why.
   Your language changes the *shape* of the damage: in Java and C# the old
   `new Stack()` calls won't fail, they'll refuse to build; in TypeScript
   they'll pass `npm test` and fail `npm run typecheck`; in Python they'll
   sail through construction and blow up somewhere later. Predict which.
3. **Run them.** Were you right?
4. **Now drive the change test-first**, one red at a time.

## Where things live

The reference solution, per language — reveal it at the retro, not before:

| Language | Solution |
|---|---|
| Python | [solutions/python/bounded_stack.py](solutions/python/bounded_stack.py) |
| TypeScript | [solutions/typescript/boundedStack.ts](solutions/typescript/boundedStack.ts) |
| Java | [solutions/java/BoundedStack.java](solutions/java/BoundedStack.java) |
| C# | [solutions/csharp/BoundedStack.cs](solutions/csharp/BoundedStack.cs) |
