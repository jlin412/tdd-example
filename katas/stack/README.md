# Kata: Stack (TypeScript)

## The brief

Build a stack — push, pop, peek, and a size — test-first. The full requirement is
in [StackStory.md](StackStory.md). Once that ships,
[StackExtendedStory.md](StackExtendedStory.md) gives it a maximum size.

Where the other katas in this repo sit either side of it: **FizzBuzz** is a pure
function (one input, one output, no state), and **Todo List** is stateful but
driven through the DOM. A stack is the shape in between and the most common one
in everyday code — **a stateful plain object tested through its own API**.

That changes what the tests look like, which is the point:

- **Every test needs an arrange sequence.** You cannot test `pop` without
  `push`ing first. The setup is unavoidable here, so "tests are code too" stops
  being advice and starts being necessary.
- **Behavior is order-dependent.** One push and one pop prove nothing about
  LIFO — a stack and a queue are indistinguishable with a single item in them.
  Finding the smallest test that *does* prove it is the exercise.
- **The empty-stack contract has no universal answer.** Python raises, Java
  throws, JavaScript's own `Array.pop()` quietly returns `undefined`. The mob
  has to choose, and then live with the consequence.

## Why TypeScript

It's the only language in the repo where the two safety nets come apart:

| | Catches |
|---|---|
| `npm test` | runtime behavior. Vitest transpiles types away with esbuild and **does not check them** — a type error will not fail this. |
| `npm run typecheck` | types only (`tsc --noEmit`), and nothing about behavior. |

So TypeScript sits *between* the repo's dynamic languages (Python/JS — runtime
guards, tests are the only net) and its static ones (Java/C# — the compiler
refuses to build). Checkpoint #2 makes the mob discover this rather than being
told, and uncommenting STEP 1 produces **two different reds**: a `TypeError` at
runtime and a `TS2339` at compile time.

It also makes **generics** a genuine pattern-menu item rather than the no-op
they'd be in plain JS.

## How the skeleton works

The `typescript/` folder hands the mob exactly three things:

1. **The requirement — in the story, not the code.** [`stack.ts`](typescript/stack.ts)
   points at [StackStory.md](StackStory.md), which describes the structure in
   prose with a walkthrough table and never lists numbered rules.
2. **An empty class** — no methods. Nothing is added until a red test demands it.
3. **A test list to write first** (STEP 0) and **ONE worked example** (STEP 1),
   plus comment prompts. STEP 0 asks the mob to name as many tests as it can as
   `it.todo()` placeholders before any code is written. STEP 1 ships with its
   assertion **commented out** and the RED → GREEN → REFACTOR steps labelled
   inside. Every test after the first is yours to invent — the prompts ask
   questions, they don't hand you cases.

On a fresh skeleton everything passes: STEP 1 is commented out and the STEP 0
todos report as pending. **Uncommenting STEP 1 is what gets you your first red.**

## Session flow

| Phase | What happens |
|-------|--------------|
| **STEP 0** | Write the test LIST first — as many `it.todo()`s as the mob can name. Name whole *sequences*, not single calls. |
| **STEP 1** | Uncomment the given assertion, watch it fail (twice — run both commands), minimum green (`return true` is legal). |
| **Invent tests** | Promote todos off the list — push/pop round-trip, the LIFO proof, peek-doesn't-remove, size across a sequence. ONE red at a time. |
| **CHECKPOINT #1 · pattern menu** | On green: mob picks **ONE** pattern from (a)–(f). Coach's rule: one at a time — green + a payoff check between picks. |
| **CHECKPOINT #2 · negative & edge** | The empty contract; the `undefined` ambiguity it creates; the internals leak; and proving what each safety net catches. |
| **EXTENDED** | [StackExtendedStory.md](StackExtendedStory.md): a maximum size. Breaks existing construction on purpose — predict which tests before running. |

## The contract (decide this as a mob)

The story leaves gaps on purpose — choose, then encode each as a test:

- **Empty `pop` / `peek`** — throw? return nothing? Pick one, apply it to both.
- **Nothing-shaped items** — may you push `undefined`? If you also chose "return
  `undefined` when empty", how does a caller tell those apart? (This clash is
  the best argument in the kata for menu item (d).)
- **The internals** — can a caller mutate the stack through anything the public
  API handed out?

## Test taxonomy

| Category | What it checks | Stack examples |
|----------|----------------|----------------|
| **Positive** | correct behavior for valid use | push then pop returns the item; peek returns the top |
| **Boundary** | the edges of a rule | two items proving LIFO; the pop that empties it; capacity exactly reached |
| **Edge** | ambiguous / contract-defining states | pop on empty; peek not removing; capacity 0 |
| **Negative** | invalid use rejected, not mishandled | the internals don't leak; a negative capacity is refused |

## The pattern menu (checkpoint #1)

The mob **decides which ONE to implement first**; the coach enforces
one-pattern-at-a-time. Same *shape* as the other katas' menus, re-flavored for a
data structure — and the right picks differ, which is the lesson.

| Item | Pattern | The move |
|------|---------|----------|
| (a) | **Generic type parameter** | `Stack<T>` instead of `any`. The enabler, and why this kata is TS. Payoff: does `npm run typecheck` reject popping a `number` into a `string`? |
| (b) | **Swappable backing store** | array vs linked list behind one interface — *the tests must not change*, which proves they were behavioral |
| (c) | **Decorator** | wrap a stack to add behavior without editing it — earned by the Extended story |
| (d) | **Result / Option** | `tryPop(): T \| undefined` or a discriminated union, instead of throwing |
| (e) | **Iterator** | `[Symbol.iterator]()` — traverse without exposing internals |
| (f) | **Immutable / persistent** | `push` returns a *new* stack; watch what it does to your arrange steps |

Weigh and likely decline: a linked list for its own sake (an array is already
O(1) at the end), a `Proxy`, elaborate generic constraints when plain `T` does.

## Running the tests

| From | Install | Run |
|------|---------|-----|
| `typescript/` | `npm install` | `npm test` — and `npm run typecheck` |

Requires Node 18+. Run **both** commands; they catch different things, and the
kata is partly about noticing that.

## Revealing the solution

`solutions/typescript/` holds one possible end state:

- **Classic** ([`stack.ts`](solutions/typescript/stack.ts)) — pattern-menu picks
  **(a) generics** and **(e) iterator**; (b), (d) and (f) declined with reasons
  in the header comment. It **throws** on empty, and the header says why.
- **Extended** ([`boundedStack.ts`](solutions/typescript/boundedStack.ts)) — the
  capacity limit as a **(c) Decorator** that wraps the plain stack rather than
  editing it, so the classic suite stays green untouched.

The reference uses `isEmpty()` / `size()` and throws on empty. Those are *one*
set of choices, not a mandate — the story leaves the API shape to the mob. If
yours matches, the classic files are drop-in:

```bash
# from katas/stack/, over a scratch copy if you want to keep the skeleton pristine
cp solutions/typescript/*.ts typescript/
(cd typescript && npm test && npm run typecheck)
```

Keep them closed until the retro. The solutions deliberately *show their
judgment* — which patterns they took, which they declined, and why — so the retro
can argue with them.
