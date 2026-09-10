# Kata: Todo List (Angular)

## The brief

Build a to-do list as an Angular component: add, toggle, delete, filter, a
remaining-count, and clear-completed. The full requirement is in
[TodoListStory.md](TodoListStory.md) (the standard list). Once that ships,
[TodoListEditStory.md](TodoListEditStory.md) adds inline editing.

Unlike the polyglot katas, this one is **TypeScript / Angular only** — on
purpose. Its value is the thing the pure-logic katas can't teach: **testing a
framework** — components, templates, DOM interaction, dependency injection, and
reactive (signal) state — not just an algorithm.

## How the skeleton works

The `angular/` workspace hands the mob exactly three things:

1. **The requirement — in the story, not the code.** The component file points to
   [TodoListStory.md](TodoListStory.md), which describes the app in prose — the
   mob turns it into a list of tests (STEP 0) before writing any code.
2. **An empty component** — [`todo-list.ts`](angular/src/app/todo-list.ts) has a
   blank template and no logic. Nothing is added until a red test demands it.
3. **A test list to write first** (STEP 0) and **ONE worked example** (STEP 1),
   plus comment prompts. STEP 0 asks the mob to name as many tests as it can —
   as `it.todo()` placeholders — before any code is written. STEP 1 ships with
   its assertion **commented out** and the RED → GREEN → REFACTOR steps labelled
   inside. Every test after the first is yours to invent — the prompts ask
   questions, they don't hand you cases.

Your first RED is deliberate: **uncomment STEP 1's assertion** and run the tests.
It fails because the component renders nothing yet. Minimum green is a literal
empty-state message in the template — then the *next* test (add a todo) forces
real state. (A fresh skeleton is all green: STEP 1 is commented out and the
STEP 0 todos report as pending — uncommenting is the move.)

## Session flow

| Phase | What happens |
|-------|--------------|
| **STEP 0** | Write the test LIST first — as many `it.todo()`s as the mob can name, translating the story into test language. |
| **STEP 1** | Uncomment the given assertion, watch it fail, minimum green (a literal empty-state message). |
| **Invent tests** | Promote todos off the STEP 0 list — `[positive]`/`[boundary]` for add, toggle, delete, count, filters — ONE red at a time, driven through the DOM. |
| **CHECKPOINT #1 · pattern menu** | On green: mob picks **ONE** pattern from (a)–(f). Coach's rule: one at a time — green + a payoff check between picks. The DOM tests stay green throughout, proving each refactor safe. |
| **CHECKPOINT #2 · negative & edge** | Blank-add ignored, plurals, clear-completed visibility, unknown-id contract. |
| **EDIT STRETCH** | [TodoListEditStory.md](TodoListEditStory.md): inline edit — double-click, Enter saves, Escape cancels, blank deletes. Deliberately grows an item's behavior until the **smart/dumb split** earns its place. |

## The contract (decide this as a mob)

The story leaves gaps on purpose — choose, then encode each as a test:

- **Pluralization** — `1 item left` vs `2 items left`; what about `0`?
- **Blank add** — does `"   "` count as blank? Does the input clear after an
  ignored add?
- **Unknown id** — toggling/deleting a missing id: shrug it off, or throw?

## Test taxonomy

| Category | What it checks | Todo examples |
|----------|----------------|---------------|
| **Positive** | correct behavior for valid use | add shows a row; toggle marks done; delete removes |
| **Boundary** | the edges of a rule | count `2 → 1 → 0`; `1 item` vs `2 items`; filter partitions |
| **Edge** | ambiguous / contract-defining states | empty-state message; input clears after add; clear-completed hidden when none done |
| **Negative** | invalid input rejected, not mishandled | blank / whitespace-only add is ignored |

## The pattern menu (checkpoint #1)

The mob **decides which ONE to implement first**; the coach enforces
one-pattern-at-a-time. Same *shape* as the FizzBuzz menu, re-flavored for a
component — and the right picks differ, which is the lesson.

| Item | Pattern | The move |
|------|---------|----------|
| (a) | **Signal store** | lift state + mutations into an injectable `TodoStore` (signals). State becomes testable without the DOM. Enabler. |
| (b) | **Smart/dumb split** | extract a presentational `TodoItem` (`input()`/`output()`). Pays off once an item gains behavior — see the edit stretch. |
| (c) | **Computed derived state** | `visible` list and `remaining` count as `computed()` signals, not hand-maintained fields. |
| (d) | **Immutable updaters** | every change returns a new array via `update(list => …)`; OnPush-friendly. |
| (e) | **Filter strategy** | All/Active/Completed as a `{ name: predicate }` lookup (the cousin of FizzBuzz's rules engine). |
| (f) | **Dependency injection** | provide the store; inject a fake/preloaded one to drive the component from a known state in tests. |

Weigh and likely decline: **NgRx / Redux-style store** (the signal store *is* the
lightweight answer here), a custom **`*todoFilter` pipe** (a `computed()` is
simpler), **routing** the filters (over-engineering for a kata).

## A note on frontend testing

- **Test through the DOM.** Render with `TestBed.createComponent`, `await
  fixture.whenStable()`, then read/act on `fixture.nativeElement`. Asserting on
  what a *user* sees is what lets you refactor internals (checkpoint #1) without
  touching the tests.
- **Signals-first, zoneless.** Angular v21 defaults to zoneless change detection;
  prefer `signal()`/`computed()` for state and call `fixture.detectChanges()`
  after simulating an event before asserting.

## Running the tests

| From | Install | Run |
|------|---------|-----|
| `angular/` | `npm install` | `npm test` |

Requires Node 20.19+/22.12+/24+ and Angular v21 (installed via `npm install`).
On a fresh skeleton everything passes: STEP 1's assertion is commented out and
the STEP 0 todos report as pending. Uncomment STEP 1 to get your first red.

## Revealing the solution

`solutions/angular/` holds one possible end state:

- **Standard** ([`todo-list.ts`](solutions/angular/todo-list.ts) +
  [`todo-store.ts`](solutions/angular/todo-store.ts)) — the standard list, with pattern-menu
  picks **(a) store, (c) computed, (d) immutable updaters**; **(b), (e), (f)
  declined** with reasons in the header comments.
- **Edit stretch** ([`todo-item.ts`](solutions/angular/todo-item.ts) +
  [`todo-list-edit.ts`](solutions/angular/todo-list-edit.ts)) — inline editing, where the
  **(b) smart/dumb split** finally earns its place.

The reference uses the same component name as the skeleton, and its own copy
("No todos yet", `1 item left`). That copy is one choice, not a mandate — the
mob picks its own wording. If yours matches, the standard files are drop-in:

```bash
# from katas/todo-list/, over a scratch copy if you want to keep the skeleton pristine
cp solutions/angular/todo-store.ts solutions/angular/todo-list.ts angular/src/app/
cp solutions/angular/todo-list.spec.ts angular/src/app/
(cd angular && npm test)
```

Keep them closed until the retro. The solutions deliberately *show their
judgment* — which patterns they took, which they declined, and why — so the retro
can argue with them.
