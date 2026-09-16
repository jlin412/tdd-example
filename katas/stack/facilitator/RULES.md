# Stack — facilitator's answer key

**Open this DURING the session, not before.** It is not a spoiler in the way
`solutions/` is — that folder stays closed until the retro, this one you open at
the test-list checkpoint to play product owner.

The stories ([StackStory.md](../StackStory.md),
[StackExtendedStory.md](../StackExtendedStory.md)) describe the structure in
prose and show a walkthrough; they deliberately do **not** hand over a numbered
rule list. Producing that list — as a list of tests — is the mob's first job.
This file is the canonical list to compare theirs against, and it is what the
`R`-numbers in `solutions/` refer to.

## Classic — R1–R6

- **R1.** A brand-new stack is **empty** (nothing in it; size 0).
- **R2.** **push** puts an item on top; the size grows by one.
- **R3.** **pop** removes the top item and returns it; the size shrinks by one.
- **R4.** **LIFO** — with more than one item in, `pop` returns the **most
  recently pushed** one. (Needs ≥2 pushes to demonstrate; one proves nothing.)
- **R5.** **peek** returns the top item **without** removing it — the size is
  unchanged and a second `peek` returns the same thing.
- **R6.** `pop`/`peek` on an **empty** stack is a defined, deliberate behavior.
  *The mob picks which* — throwing and returning a sentinel are both defensible;
  what matters is that it's chosen and tested, and applied consistently to both.

## Extended — R7–R8

- **R7.** A stack is created with a **maximum size** and never exceeds it.
- **R8.** Pushing onto a **full** stack is refused (mob picks how), and fullness
  is observable. Popping frees room again.

## Playing product owner

Pre-decided answers to the questions a mob reliably asks. Give a one-line ruling
and move on — do not debate:

| They ask | Ruling |
|---|---|
| What should `pop` do when empty? | **Their call** — but pick ONE and apply it to `peek` too. |
| Reference's choice? | It throws/raises, in all four languages. Say so only if they ask *after* deciding. |
| What does *our language* do? | Python `list.pop()` raises `IndexError`; JS/TS `Array.pop()` returns `undefined`; `java.util.Stack.pop()` throws `EmptyStackException`; .NET `Stack<T>.Pop()` throws `InvalidOperationException`. Offer this only once they've committed — it ends the debate too early. |
| May we push `None` / `null` / `undefined`? | **Yes.** That's exactly why "hand back nothing when empty" is a shaky contract — let them find it. (In C#, the sharper version is `default(int) == 0` on a stack of numbers.) |
| Does `peek` remove? | **No.** That's the whole point of having it. |
| Must it be generic (`Stack<T>`)? | Not to start. It's pattern-menu item (a) — reach it by refactor, not up front. |
| What can it hold — one type or anything? | **Their call.** Both are valid designs; the retro compares them. |
| Capacity of 0? | **Legal** — a stack that is full from birth. Good edge test. |
| Negative capacity? | **Reject it.** Creating one is the error, not the first push. |
| Does `size` count or recompute? | Implementation detail — don't let them test it. |
| Thread safety? Persistence? Undo? | Out of scope. Parking lot. |

**Out of scope, say no:** thread safety, serialization/persistence, a `Queue`
too, language-specific exotica (`Symbol.species`, custom `__reduce__`,
`ICloneable`), iterators *unless* they arrive via menu item (e), performance
benchmarking.

## Coverage check

Instead of ticking off R-numbers, ask for four boxes:

- **[positive]** one per behavior — push, pop, peek, size/empty,
- **[boundary]** a pop that proves LIFO (≥2 items), and the pop that empties it,
- **[edge]** pop/peek on empty; peek leaving the stack untouched,
- **[negative]** the internals don't leak — a caller can't mutate the stack
  through anything the public API handed out.

Two extras specific to this kata:

- **Are the arrange steps repeating?** By the fifth test they will be. If nobody
  has extracted a helper, prompt for it — that's the "tests are code too" beat.
- **Do they know which safety nets they're actually running?** This differs per
  language and most mobs never notice:
  - **TypeScript** — have they run `npm run typecheck` *at all*? Vitest does not
    type-check. A mob that only runs `npm test` has been using one net all
    session without noticing the second exists.
  - **Java / C#** — they've been getting compiler reds for free. Ask which of
    their negative tests the compiler made redundant (pushing the wrong type),
    and which it could never have caught (pop on empty, the internals leaking).
  - **Python** — there is no second net. Ask what it would take to get one, and
    whether any bug today would have been caught by it.

## Language-specific beats worth pointing out

Only if the mob doesn't get there itself, and only at the retro:

| Language | The beat |
|---|---|
| Python | type hints are documentation — `Stack[int]().push("x")` runs fine. Also: `bool` subclasses `int`, so `True` sneaks past a capacity check. |
| TypeScript | the same type error passes `npm test` and fails `npm run typecheck` — two nets, one habit. |
| Java | generics are **erased**: `Stack<String>` and `Stack<Integer>` are the same class at runtime. And `Optional` cannot hold `null`, so menu item (d) can't express "I popped a null". |
| C# | generics are **reified**: those two are different types. And `default(int)` is `0`, which is the sharpest version of the empty-sentinel ambiguity. |

## Retro prompt

> Diff the test list you wrote at the start against this one. What did you miss?
> Then: your LIFO test — how many items did it take to prove the order, and why
> would one not have been enough? And finally, which of today's bugs did your
> language catch for you before a test ran, and which could only ever have been
> found by a test you wrote? Would your answer change in a different language?
