# Todo List — reference solution

**Spoilers.** Keep this folder closed until the retro.

One possible end state, in two parts:

- **Standard** — [`todo-store.ts`](angular/todo-store.ts) +
  [`todo-list.ts`](angular/todo-list.ts) + [`todo-list.spec.ts`](angular/todo-list.spec.ts)
- **Edit stretch** — [`todo-item.ts`](angular/todo-item.ts) +
  [`todo-list-edit.ts`](angular/todo-list-edit.ts) +
  [`todo-list-edit.spec.ts`](angular/todo-list-edit.spec.ts)

## Design & pattern-menu picks

The **standard** solution took, one at a time:

- **(a) Signal store** — all state + mutations in `TodoStore`; unit-testable
  without the DOM (see the store tests in `todo-list.spec.ts`).
- **(c) Computed derived state** — `visible`, `remaining`, `hasCompleted`.
- **(d) Immutable updaters** — every change returns a new array; the component
  runs OnPush.

It **declined** (reasons in the file headers):

- **(b) Smart/dumb split** — one component is fine while a row just shows +
  toggles + deletes.
- **(e) Filter strategy** — three fixed filters read fine as one computed.
- **(f) DI of a fake store** — the DOM tests drive the real store happily.

Those picks **differ from FizzBuzz's** (which took DI + factory). Same menu,
different problem — the lesson of the pattern menu.

The **edit stretch** is where **(b) finally earns its place**: `TodoItem` is a
controlled presentational component (parent owns `editing`, so "one row at a
time" falls out for free), emitting `toggle`/`remove`/`startEdit`/`save`/`cancel`.
The `suppressBlur` flag keeps Escape from committing a save.

## Running the finished state

```bash
# from katas/todo-list/ (use a scratch copy to keep the skeleton pristine)
# Standard:
cp solutions/angular/todo-store.ts solutions/angular/todo-list.ts \
   solutions/angular/todo-list.spec.ts angular/src/app/
(cd angular && npm test)

# Edit stretch (add the item + container + its spec):
cp solutions/angular/todo-item.ts solutions/angular/todo-list-edit.ts \
   solutions/angular/todo-list-edit.spec.ts angular/src/app/
(cd angular && npm test)
```

Verified with Angular v21 + Vitest: standard suite 11/11, edit stretch 6/6.
