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
| Reference's choice? | It throws. Say so only if they ask *after* deciding. |
| May we push `null` / `undefined`? | **Yes.** That's exactly why "return `undefined` when empty" is a shaky contract — let them find it. |
| Does `peek` remove? | **No.** That's the whole point of having it. |
| Must it be generic (`Stack<T>`)? | Not to start. It's pattern-menu item (a) — reach it by refactor, not up front. |
| What can it hold — one type or anything? | **Their call.** Both are valid designs; the retro compares them. |
| Capacity of 0? | **Legal** — a stack that is full from birth. Good edge test. |
| Negative capacity? | **Reject it.** Creating one is the error, not the first push. |
| Does `size` count or recompute? | Implementation detail — don't let them test it. |
| Thread safety? Persistence? Undo? | Out of scope. Parking lot. |

**Out of scope, say no:** thread safety, serialization/persistence, a `Queue`
too, `Symbol.species`, iterators *unless* they arrive via menu item (e),
performance benchmarking.

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
- **Have they run `npm run typecheck` at all?** Vitest does not type-check. A
  mob that only runs `npm test` has been using one safety net all session
  without noticing the other exists.

## Retro prompt

> Diff the test list you wrote at the start against this one. What did you miss?
> Then: your LIFO test — how many items did it take to prove the order, and why
> would one not have been enough? And finally, which bugs today would the
> type-checker have caught, and which only the tests could?
