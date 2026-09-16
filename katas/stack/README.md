# Kata: Stack

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
- **The empty-stack contract has no universal answer.** The mob has to choose,
  and then live with the consequence.

## Four languages, three answers

The kata ships in Python, TypeScript, Java and C#. Same story, same menu, same
checkpoints — but this is the one kata in the repo where the *language itself*
is part of the lesson, because a stack is where a type system starts to earn
its keep. Run the same mistake past each toolchain and you get a different
answer:

| Language | What catches `stack.pop()` assigned to the wrong type? | So your first RED is… |
|---|---|---|
| **Python** | nothing — type hints are documentation, and there is no check step in this kata | an `AttributeError` at runtime |
| **TypeScript** | `npm run typecheck` only. Vitest strips types with esbuild and **does not check them**, so `npm test` will happily pass | **two** different reds — a `TypeError` and a `TS2339` |
| **Java** | `javac`. `mvn test` compiles before it runs, so the mistake never reaches an assertion | a compile error — "cannot find symbol" |
| **C#** | the compiler. `dotnet test` builds before it runs | a build error — `CS1061` |

TypeScript is the interesting middle: it has a type-checker, but it doesn't run
unless you ask. Checkpoint #2 in every language asks the mob which of its bugs
that day each net would have caught — and the honest answer differs per
language, which is the whole reason to notice.

The empty-stack contract splits the same way, and every language's own standard
library already made a different choice:

| | Its own stack says |
|---|---|
| Python | `list.pop()` raises `IndexError` |
| JavaScript / TypeScript | `Array.prototype.pop()` quietly returns `undefined` |
| Java | `java.util.Stack.pop()` throws `EmptyStackException` |
| C# | `Stack<T>.Pop()` throws `InvalidOperationException` — and offers `TryPop` alongside |

Generics land differently too: they're enforced-then-**erased** in Java,
**reified** in C#, checked only on demand in TypeScript, and pure documentation
in Python. Each reference solution has a test pinning its language's answer.

## How the skeleton works

Each language folder hands the mob exactly three things:

1. **The requirement — in the story, not the code.** The production skeleton
   points at [StackStory.md](StackStory.md), which describes the structure in
   prose with a walkthrough table and never lists numbered rules.
2. **An empty class** — no methods. Nothing is added until a red test demands it.
3. **A test list to write first** (STEP 0) and **ONE worked example** (STEP 1),
   plus comment prompts. STEP 0 asks the mob to name as many tests as it can as
   pending/todo placeholders before any code is written. STEP 1 ships with its
   assertion **commented out** and the RED → GREEN → REFACTOR steps labelled
   inside. Every test after the first is yours to invent — the prompts ask
   questions, they don't hand you cases.

On a fresh skeleton everything passes: STEP 1 is commented out and the STEP 0
todos report as pending. **Uncommenting STEP 1 is what gets you your first red.**

## Session flow

| Phase | What happens |
|-------|--------------|
| **STEP 0** | Write the test LIST first — as many pending tests as the mob can name. Name whole *sequences*, not single calls. |
| **STEP 1** | Uncomment the given assertion, watch it fail (see the table above for what "fail" looks like in your language), minimum green (`return true` is legal). |
| **Invent tests** | Promote todos off the list — push/pop round-trip, the LIFO proof, peek-doesn't-remove, size across a sequence. ONE red at a time. |
| **CHECKPOINT #1 · pattern menu** | On green: mob picks **ONE** pattern from (a)–(f). Coach's rule: one at a time — green + a payoff check between picks. |
| **CHECKPOINT #2 · negative & edge** | The empty contract; the nothing-shaped-item ambiguity it creates; the internals leak; and proving what each safety net catches. |
| **EXTENDED** | [StackExtendedStory.md](StackExtendedStory.md): a maximum size. Breaks existing construction on purpose — predict which tests before running. |

## The contract (decide this as a mob)

The story leaves gaps on purpose — choose, then encode each as a test:

- **Empty `pop` / `peek`** — throw? return nothing? Pick one, apply it to both.
- **Nothing-shaped items** — may you push `None` / `null` / `undefined`? If you
  also chose "hand back nothing when empty", how does a caller tell those apart?
  (This clash is the best argument in the kata for menu item (d). In C#, ask the
  sharper version: `default(int)` is `0`, so what about a stack holding a zero?)
- **The internals** — can a caller mutate the stack through anything the public
  API handed out?

## Test taxonomy

| Category | What it checks | Stack examples |
|----------|----------------|----------------|
| **Positive** | correct behavior for valid use | push then pop returns the item; peek returns the top |
| **Boundary** | the edges of a rule | two items proving LIFO; the pop that empties it; capacity exactly reached |
| **Edge** | ambiguous / contract-defining states | pop on empty; peek not removing; capacity 0 |
| **Negative** | invalid use rejected, not mishandled | the internals don't leak; a negative capacity is refused |

In Java and C# a chunk of the negative column is written for you by the
compiler — pushing the wrong type simply doesn't build, so there is nothing to
assert. In Python nothing is written for you. Noticing *which* of your tests
the compiler made redundant is part of checkpoint #2.

## The pattern menu (checkpoint #1)

The mob **decides which ONE to implement first**; the coach enforces
one-pattern-at-a-time. Same *shape* as the other katas' menus, re-flavored for a
data structure — and the right picks differ, which is the lesson.

| Item | Pattern | The move |
|------|---------|----------|
| (a) | **Generic type parameter** | `Stack<T>` / `Stack(Generic[T])` instead of a stack of anything. The enabler. Payoff check: push a string, pop it into a number — who complains, and when? |
| (b) | **Swappable backing store** | array vs linked list behind one interface — *the tests must not change*, which proves they were behavioral |
| (c) | **Decorator** | wrap a stack to add behavior without editing it — earned by the Extended story |
| (d) | **Result / Option instead of throwing** | a `try_pop` / `tryPop` / `TryPop(out T)` — the idiom differs per language, the question doesn't |
| (e) | **Iteration protocol** | `__iter__` · `[Symbol.iterator]()` · `Iterable<T>` · `IEnumerable<T>` — traverse without exposing internals |
| (f) | **Immutable / persistent** | `push` returns a *new* stack; watch what it does to your arrange steps |

Weigh and likely decline: a linked list for its own sake (the built-in list is
already O(1) at the end), elaborate generic constraints when plain `T` does, and
inheriting from the built-in list type — is a stack a list, or does a stack
*have* a list?

Each skeleton's menu is written in its own language's idiom; item (d) in
particular is where they diverge most (Java's `Optional` famously **cannot hold
null**, which settles — or dodges — the contract argument).

## Running the tests

| From | Install | Run |
|------|---------|-----|
| `python/` | `pip install -r requirements.txt` | `pytest` |
| `typescript/` | `npm install` | `npm test` — **and `npm run typecheck`** |
| `java/` | — | `mvn test` |
| `csharp/` | — | `dotnet test` |

Python 3.9+, Node 18+, JDK 17+, .NET SDK 8+. In TypeScript, run **both**
commands; they catch different things, and the kata is partly about noticing
that.

## Revealing the solution

`solutions/<language>/` holds one possible end state per language — all four
converge on the same design so they can be compared side by side:

- **Classic** — pattern-menu picks **(a) generics** and **(e) the iteration
  protocol**; (b), (d) and (f) declined with reasons in each header comment.
  It **throws/raises** on empty, and the header says why.
- **Extended** — the capacity limit as a **(c) Decorator** that wraps the plain
  stack rather than editing it, so the classic suite stays green untouched.

Each uses `is_empty()`/`isEmpty()`/`IsEmpty()` and `size()`/`Size()` and
throws on empty. Those are *one* set of choices, not a mandate — the story
leaves the API shape to the mob. If yours matches, they're drop-in:

```bash
# from katas/stack/, over a scratch copy if you want to keep the skeletons pristine
cp solutions/python/*.py      python/      && (cd python && pytest)
cp solutions/typescript/*.ts  typescript/  && (cd typescript && npm test && npm run typecheck)
cp solutions/java/Stack.java solutions/java/EmptyStackException.java \
   solutions/java/BoundedStack.java solutions/java/FullStackException.java  java/src/main/java/kata/
cp solutions/java/StackTest.java solutions/java/BoundedStackTest.java       java/src/test/java/kata/
(cd java && mvn test)
cp solutions/csharp/*.cs      csharp/      && (cd csharp && dotnet test)
```

Keep them closed until the retro. The solutions deliberately *show their
judgment* — which patterns they took, which they declined, and why — so the retro
can argue with them.
