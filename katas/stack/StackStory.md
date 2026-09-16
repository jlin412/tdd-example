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
- **Nothing-shaped items.** May you push whatever your language's *nothing* is —
  `None`, `null`, `undefined`? And if you chose "hand back nothing when empty"
  above, how would a caller tell an empty stack apart from a stack whose top
  item *is* nothing? (In a language with default values, ask the same question
  about a stack of numbers holding a legitimate `0`.)
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

Pick a language — the kata is the same in all four. The skeleton to fill in, the
reference solution to reveal at the end, and how to run the tests:

| Language | Skeleton | Solution | Run |
|---|---|---|---|
| Python | [python/](python/) | [solutions/python/](solutions/python/) | `pip install -r requirements.txt`, then `pytest` |
| TypeScript | [typescript/](typescript/) | [solutions/typescript/](solutions/typescript/) | `npm install`, then `npm test` — **and `npm run typecheck`** |
| Java | [java/](java/) | [solutions/java/](solutions/java/) | `mvn test` |
| C# | [csharp/](csharp/) | [solutions/csharp/](solutions/csharp/) | `dotnet test` |

Your language decides how much help you get on the gaps above. Java and C#
refuse to build code whose types don't line up; TypeScript checks types only
when you ask it to (`npm run typecheck` — the test run won't); Python checks
nothing at all. None of them will decide the empty-stack contract for you.

## What next

Shipped the stack? The requirements change. See
[StackExtendedStory.md](StackExtendedStory.md).
