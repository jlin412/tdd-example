# Todo List — facilitator's answer key

**Open this DURING the session, not before.** It is not a spoiler in the way
`solutions/` is — that folder stays closed until the retro, this one you open at
the test-list checkpoint to play product owner.

The stories ([TodoListStory.md](../TodoListStory.md),
[TodoListEditStory.md](../TodoListEditStory.md)) describe the app in prose and
show what it looks like; they deliberately do **not** hand over a numbered rule
list. Producing that list — as a list of tests — is the mob's first job. This
file is the canonical list to compare theirs against, and it is what the
`R`-numbers in `solutions/` refer to.

## Standard — R1–R8

- **R1.** With no todos, the list shows an **empty-state message**
  ("No todos yet" in the reference).
- **R2.** **Add** a todo: entering text and activating "Add" appends it to the
  list, and the input **clears**.
- **R3.** **Blank or whitespace-only** text is **ignored** — no todo is added.
- **R4.** **Toggle** a todo's done state; completed todos are visibly marked.
- **R5.** **Delete** a todo removes it from the list.
- **R6.** A **remaining count** shows how many todos are still active — e.g.
  `2 items left`. Mind the singular: `1 item left`.
- **R7.** **Filters** — All / Active / Completed — change which todos are shown.
  (*Active* = not done; *Completed* = done.)
- **R8.** **Clear completed** removes all done todos, and is **hidden when
  nothing is completed**.

## Edit stretch — R9–R13

- **R9.** **Double-clicking** a todo's text puts that row into **edit mode**: an
  input, pre-filled with the current text, focused.
- **R10.** In edit mode, **Enter** (or blur) **saves** the trimmed text.
- **R11.** In edit mode, **Escape cancels** — the original text stays.
- **R12.** Saving **blank / whitespace-only** text **deletes** the todo (same
  spirit as R3).
- **R13.** Only **one** row is editable at a time.

## Playing product owner

Pre-decided answers to the questions a mob reliably asks. Give a one-line ruling
and move on — do not debate:

| They ask | Ruling |
|---|---|
| Exact empty-state wording? | **Their call.** Reference uses "No todos yet". |
| Exact count wording? | **Their call**, but singular/plural must differ. Reference: `1 item left` / `2 items left`. |
| `0 items left` or hide the count? | **Show `0 items left`.** |
| Is `"   "` blank? | **Yes** — whitespace-only counts as blank. |
| Does the input clear after an *ignored* blank add? | **Their call.** Reference clears it. |
| Toggle/delete an id that's gone? | **Shrug it off** — no throw. |
| Duplicate todo text allowed? | **Yes.** Two items may read the same. |
| Does it persist / reload? | Out of scope. Parking lot. |
| Should filters be routed (URL)? | Out of scope — over-engineering here. |
| Drag to reorder? Due dates? | Out of scope. Parking lot. |

**Out of scope, say no:** persistence, routing, reordering, due dates,
priorities, i18n, animation, accessibility audits (worth naming, not building).

## Coverage check

Instead of ticking off R-numbers, ask for four boxes:

- **[positive]** one per behavior — add, toggle, delete, count, each filter,
- **[boundary]** the count's singular/plural flip, and the filter partitions,
- **[edge]** empty state, input clearing, clear-completed appearing/vanishing,
- **[negative]** blank add ignored.

And one frontend-specific check: **are the tests driven through the DOM?** If a
test reaches into component internals, the checkpoint-#1 refactor will break it —
which is the whole lesson.

## Retro prompt

> Diff the test list you wrote at the start against this one. What did you miss,
> and what did you invent that nobody asked for? Then: how many of your tests
> survived the pattern refactor untouched — and what does that tell you about
> testing through the DOM?
