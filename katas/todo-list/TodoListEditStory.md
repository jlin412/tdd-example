# Todo List — the Story (edit stretch)

Shipped the standard list ([TodoListStory.md](TodoListStory.md))? Good. The
product owner wants **inline editing**. This adds to what's already there — and
the existing tests should keep passing throughout.

## What changes

**Double-click an item's text and it becomes editable in place** — an input,
already holding the current text, with the cursor in it ready to type.

From there, **Enter commits** the change, and so does clicking away (losing
focus). **Escape abandons** it — the item keeps the text it had. If you commit
**empty** text, the item is **deleted** instead, which is the same instinct as
refusing to add a blank one in the first place.

Only **one** item is ever editable at a time.

## A walk through it

| you do this | you should see |
|---|---|
| double-click "Buy milk" | an input on that row, holding "Buy milk", focused |
| type "Buy oat milk", press Enter | the row now reads "Buy oat milk" |
| double-click it, type junk, press Escape | the row still reads "Buy oat milk" |
| double-click it, clear the box, press Enter | the row is gone entirely |
| double-click one row, then double-click another | only the second row is editable |
| double-click, retype, click elsewhere | the change is committed |

Whitespace is trimmed on the way in, and everything the standard list left
unsettled is still unsettled the same way.

## Why this earns a new pattern

A todo row now has its *own* state (am I being edited? what's the draft text?)
and its *own* events (save, cancel, delete). Cramming that into the list
component makes it bulge. This is the moment the **smart/dumb split** (pattern
menu item **(b)**) pays for itself:

- a **presentational** `TodoItem` component — `input()` for the todo, `output()`s
  for `toggle` / `remove` / `edit` — that owns its edit state and knows nothing
  about the store;
- the **container** `TodoList` wires items to the store.

Drive it test-first, and watch a pattern you may have *declined* in checkpoint #1
suddenly become the right call. Requirements, not fashion, justify a pattern.

## Your job, in order

1. **Update your test list first** — new tests to add, and any existing ones
   this makes wrong. Write the list before you touch the code.
2. **Run the suite.** Which existing tests still pass? Anything that breaks here
   is telling you something about how tightly your tests were coupled.
3. **Now drive the change test-first**, one red at a time.

## Where things live

- Reference solution: [solutions/angular/todo-item.ts](solutions/angular/todo-item.ts)
  + [solutions/angular/todo-list-edit.ts](solutions/angular/todo-list-edit.ts)
