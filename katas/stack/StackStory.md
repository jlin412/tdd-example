# Stack — the Story (classic)

Build a stack — the pile-of-plates data structure — but grow it **test-first**,
one behavior at a time. Everything you need is below; nothing here is a numbered
requirement list, because turning this into a list of tests is your first job.

## The structure

A stack is a pile. You add to the **top**, and you take from the **top** — so the
last thing you put in is the first thing you get back out. Nothing reaches into
the middle.

Four things you can do with one:

- **push** — put an item on top.
- **pop** — take the top item off and hand it back.
- **peek** — look at the top item *without* taking it off.
- ask **how many** items it's holding, or simply whether it's empty.

`push`, `pop` and `peek` are the ordinary names for these — that's the vocabulary
of a stack, so feel free to keep them. Everything else is the mob's call: what
the class is called, whether "how many" is a `size()`, a `length`, a `count()`,
or just an `isEmpty()`, and what type of thing a stack is allowed to hold.

## A walk through it

| you do this | you get back | the stack is now (bottom → top) |
|---|---|---|
| make a new stack | — | *(empty)* |
| push `"a"` | — | `a` |
| push `"b"` | — | `a b` |
| peek | `"b"` | `a b` |
| pop | `"b"` | `a` |
| push `"c"` | — | `a c` |
| pop | `"c"` | `a` |
| pop | `"a"` | *(empty)* |

Read that table carefully before writing anything. Two of those rows are the
only reason you can tell a stack from a queue.

## What the story doesn't say

Deliberate gaps. They're yours to decide as a mob — then encode each decision as
a test:

- **An empty stack.** What should `pop` do when there's nothing left? What about
  `peek`? Throw? Hand back nothing? Something else? There is no universal answer
  here — languages genuinely disagree.
- **Nothing-shaped items.** May you push `null` or `undefined`? And if you chose
  "return `undefined` when empty" above — how would a caller tell an empty stack
  apart from a stack whose top item *is* `undefined`?
- **How big can it get?** The story says nothing about a limit.
- **The internals.** If a caller gets hold of whatever your stack stores things
  in, can they change the stack behind its back? Should they be able to?

There is no single correct contract. What matters is that you **choose, and your
tests express the choice.**

## The loop

1. **RED** — write ONE failing test. Run it. Watch it fail for the right reason.
2. **GREEN** — write the *minimum* production code to pass. Nothing more.
3. **REFACTOR** — improve the code (and tests) while staying green. Then repeat.

Before any of that, though: write the **test list** (STEP 0 in the spec file).

One thing that's new here: a stack has **state**, so a test is a *sequence* of
operations, not a single call. Every test needs its arrange steps — the pushes
that set up the situation you're about to assert on.

## Where things live

- Skeleton to fill in: [typescript/stack.ts](typescript/stack.ts) +
  [typescript/stack.spec.ts](typescript/stack.spec.ts)
- Reference solution (reveal at the end):
  [solutions/typescript/stack.ts](solutions/typescript/stack.ts)
- Run the tests: from `typescript/`, `npm install` then `npm test` —
  **and `npm run typecheck`**, which is a separate net that catches different
  bugs.

## What next

Shipped the stack? The requirements change. See
[StackExtendedStory.md](StackExtendedStory.md).
