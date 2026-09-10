# Todo List — the Story (standard)

Build the humble to-do list every framework demo starts with — but grow it
**test-first**, one behavior at a time, driving a real Angular component through
the DOM. It's a **frontend** kata: your tests render the component, type into it,
and click things, then assert on what a user would see.

## The app

A single page holding a list of things you mean to do.

There's a text box and an "Add" control. Type something, add it, and it joins the
list — and the box empties, ready for the next one. Adding blank text does
nothing at all.

Each item in the list can be **ticked off**, and a ticked item is visibly marked
as done. Each item can also be **deleted** outright.

Somewhere the page tells you **how many items are still outstanding**. Three
**filters** — All, Active, Completed — narrow what the list shows. And once
anything is done, a **"Clear completed"** control appears that removes every
done item in one go; while nothing is done, that control isn't there at all.

The component is `TodoList` (selector `app-todo-list`). Signals-first: prefer
`signal()` / `computed()` for state. The exact wording on screen — and how you
structure the innards — is the mob's call, not the story's.

## What it looks like

```
┌───────────────────────────────────────────────┐
│ [ what needs doing?              ]   ( Add )    │
├───────────────────────────────────────────────┤
│ [x] Buy milk                              ✕     │
│ [ ] Walk the dog                          ✕     │
├───────────────────────────────────────────────┤
│ 1 item left      All  Active  Completed         │
│                              ( Clear completed )│
└───────────────────────────────────────────────┘
```

## A walk through it

| you do this | you should see |
|---|---|
| open the page | an empty-state message, no rows |
| add "Buy milk" | one row; the message gone; the box empty; `1 item left` |
| add "Walk the dog" | two rows; `2 items left` |
| add `"   "` | still two rows; nothing changed |
| tick "Buy milk" | it's marked done; `1 item left`; "Clear completed" appears |
| click **Active** | only "Walk the dog" |
| click **Completed** | only "Buy milk" |
| click **All** | both rows again |
| click **Clear completed** | "Buy milk" gone; one row; the control disappears |
| delete "Walk the dog" | no rows; the empty-state message is back |

## What the app doesn't say

Some things nobody specified. They're yours to decide as a mob — then encode each
decision as a test:

- **Plurals** — `1 item left` but `2 items left`. What about zero?
- **Blank adds** — does `"   "` count as blank? Does the box clear after an
  ignored add, or keep what you typed?
- **A stale id** — toggling or deleting something that's already gone: shrug it
  off, or throw?

There is no single correct contract. What matters is that you **choose, and your
tests express the choice.**

## The loop

1. **RED** — write ONE failing test. Run it. Watch it fail for the right reason.
2. **GREEN** — write the *minimum* to pass (for a component, often just a scrap
   of template). Nothing more.
3. **REFACTOR** — improve the code (and tests) while staying green. Then repeat.

Before any of that, though: write the **test list** (STEP 0 in the spec file).

## Where things live

- Skeleton to fill in: [angular/src/app/todo-list.ts](angular/src/app/todo-list.ts)
  + [angular/src/app/todo-list.spec.ts](angular/src/app/todo-list.spec.ts)
- Reference solution (reveal at the end):
  [solutions/angular/todo-list.ts](solutions/angular/todo-list.ts)
- Run the tests: from `angular/`, `npm install` then `npm test`.

## What next

Shipped the standard list? The requirements grow. See
[TodoListEditStory.md](TodoListEditStory.md) — inline editing, which pushes you
toward splitting out a presentational item component.
