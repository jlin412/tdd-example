// ════════════════════════════════════════════════════════════════════
// Todo List kata — YOU write the tests.
// ════════════════════════════════════════════════════════════════════
// The story is in ../../TodoListStory.md — read it first, walkthrough and all.
//
// This is a FRONTEND kata: drive the component through the DOM (render it,
// type, click) and assert on what a user would see. Testing behavior
// through the UI is what lets you refactor the internals freely later.
//
// STEP 0 is the test LIST: before any code, name as many tests as the mob
// can think of. STEP 1 then works one of them through the loop:
//   RED      — write ONE failing test. Run it. Watch it fail for the
//              right reason before writing any production code.
//   GREEN    — write the MINIMUM (often just template) that passes.
//   REFACTOR — on green only. Then loop.
//
// STEP 1's three steps are labelled inside the test. Every test after it is
// yours to invent — the prompts suggest WHAT to think about, not which exact
// cases to write.
//
// Test-type legend:  [positive] [boundary] [edge] [negative]
//
// (Vitest globals — describe/it/expect — are on by default here; no import.)

import { TestBed } from '@angular/core/testing';

import { TodoList } from './todo-list';

// A tiny helper you can copy for your own tests.
async function render() {
  const fixture = TestBed.createComponent(TodoList);
  await fixture.whenStable();
  return fixture;
}

// ═════════════════════════════════════════════════════════════════════
// STEP 0 · THE TEST LIST — do this BEFORE you write any code
// ═════════════════════════════════════════════════════════════════════
// The first move in TDD isn't a test, it's a LIST of the tests you want.
// As a mob, out loud, name as many as you can — go for QUANTITY now and
// prune later. You are translating the story into test language: every
// behavior you can state in English should become a name on this list.
//
// `it.todo()` takes a name and no body, so `npm test` prints your whole
// list back as pending work. Two to start you off:

it.todo('with no todos, it shows an empty-state message');
it.todo('adding "Buy milk" puts one row in the list');

// Now your turn — keep adding. Walk the story's table row by row, then
// push PAST it into what the table doesn't show. Aim for a dozen or more
// before anyone touches production code, and cover all four types:
//   [positive] one per behavior — add, tick off, delete, count, filters
//   [boundary] the count's singular/plural flip; what each filter includes
//   [edge]     the empty state; the box clearing; "Clear completed"
//              appearing and vanishing
//   [negative] blank / whitespace-only text adds nothing
//
// Phrase them the way a USER would describe them, not the way the code
// works — "ticking an item marks it done", not "toggle() flips the flag".
// DOM-level names survive the checkpoint-#1 refactor; internal ones don't.
//
// Then run `npm test`, read your list back, pick ONE, and turn it into a
// real test at STEP 1. Delete each todo as you promote it.
//
// (Keep the list alive: every time you think "what about…?" mid-session,
//  add an it.todo instead of chasing it and losing the red you're on.)

// ── STEP 1 · [edge] · the empty state · the RED → GREEN → REFACTOR loop, worked ──
// The assertion ships COMMENTED OUT — uncommenting it is your RED step.
it('shows an empty-state message when there are no todos', async () => {
  const fixture = await render();
  const page = fixture.nativeElement as HTMLElement;

  // 1. RED — agree your empty-state wording as a mob, put it in the
  //    assertion below, uncomment it, run `npm test`, and watch it fail
  //    (the component template is empty). 'No todos yet' is just a
  //    PLACEHOLDER — the story leaves the exact copy to you.
  //    (Heads up: the reference solution in solutions/ uses that wording,
  //     so different copy means its tests won't match yours verbatim.)
  // expect(page.textContent).toContain('No todos yet');

  // 2. GREEN — add the MINIMUM to pass. A literal <p>No todos yet</p> in
  //    the template is legitimate; let the next example force real state.

  // 3. REFACTOR — the next test (add a todo) will make that message wrong.
  //    Generalize: show it ONLY when there are no todos.
});

// ─────────────────────────────────────────────────────────────────────
// From here on, the tests are yours. ONE red at a time.
// Query the DOM the same way STEP 1 does: render(), then read/act on
// fixture.nativeElement. To simulate a user: set input.value and
// dispatch a new Event('input'); call button.click(); then
// fixture.detectChanges() before you assert.
// ─────────────────────────────────────────────────────────────────────

// [positive] — add a todo. Promote it from your STEP 0 list.
//   After typing text and activating "Add", does the item show in the
//   list? (It should also banish the "No todos yet" message — is that one
//   test with two assertions, or two smaller tests? Your call.)

// [positive] — tick a todo off (its done state).
//   How will you SEE "done" in the DOM so a test can assert it — a checked
//   checkbox? a CSS class on the row? Pick something observable, then test it.

// [positive] — delete a todo.
//   Add two, delete one — which one remains? How do you target the right
//   row's delete control?

// [boundary] — the outstanding-items count.
//   Add a few todos; pin the EXACT text. (Pluralization is waiting for you
//   in checkpoint #2.)

// [positive] [boundary] — the All / Active / Completed filters.
//   With a mix of active and completed todos, clicking "Active" shows
//   which rows? "Completed"? "All"? What is "active" vs "completed"?

// ═════════════════════════════════════════════════════════════════════
// CHECKPOINT #1 · REFACTOR — THE PATTERN MENU  (only when green)
// ═════════════════════════════════════════════════════════════════════
// By now the component is holding an array, a filter, mutation methods AND
// a template. That crowding is the signal to refactor.
//
// Housekeeping first (hygiene, not a pattern): NAME the magic bits — the
// filter values, the empty-state text.
//
// Then the menu. As a mob, DECIDE which ONE pattern to implement:
//
//   (a) SIGNAL STORE — lift the todos/filter state and the mutations out
//       of the component into an injectable TodoStore (signal() fields +
//       methods). The component becomes thin glue over it.
//       Payoff: state logic is testable WITHOUT the DOM — a plain unit
//       test of the store. (Every other item gets easier after this one.)
//   (b) SMART / DUMB SPLIT — extract a presentational TodoItemComponent:
//       input() todo, output() toggle / remove. The list orchestrates; the
//       item just renders + emits. (Pays off big once an item grows
//       behavior — see the edit stretch.)
//   (c) COMPUTED DERIVED STATE — replace any hand-maintained "visible
//       list" or "remaining count" with computed() signals that recompute
//       themselves. Declarative derivation vs imperative bookkeeping.
//   (d) IMMUTABLE UPDATERS — every change returns a NEW array via
//       todos.update(list => [...]) instead of mutating in place. Pure,
//       predictable, and friendly to OnPush change detection.
//   (e) FILTER STRATEGY — express All/Active/Completed as a
//       { name: predicate } lookup instead of a switch (the direct cousin
//       of the FizzBuzz rules engine). Adding a filter = adding an entry.
//   (f) DEPENDENCY INJECTION — provide the store so a test can inject a
//       fake or pre-loaded store and drive the component from a known
//       state. Open/Closed for swapping where state comes from.
//
// COACH'S RULE — ONE PATTERN AT A TIME:
//   implement the chosen pattern → all green → ask "did it pay for
//   itself? do we want another?" → only then pick the next item.
//   Never two patterns mid-flight. A pattern that changes structure ((a),
//   (b), (f)) is still driven test-first: the DOM tests you already wrote
//   must stay green THROUGHOUT — that's your proof the refactor is safe.
//
// DISCUSS: which patterns are RIGHT for this requirement — and which are
// over-engineering? (This menu is the SAME shape as the FizzBuzz kata's,
// but the right picks differ — same tools, different problem.) Options,
// not obligations.
// (Weigh and likely DECLINE: NgRx / a full Redux-style store — the signal
//  store IS the lightweight answer at this size. A custom *todoFilter PIPE
//  — a computed() is simpler and unit-testable. ROUTING the filters — nice
//  in a real app, over-engineering for a kata.)
//
// Tests are code too: a render()+addTodo() helper to extract? it.each()
// for the pluralization cases?

// ═════════════════════════════════════════════════════════════════════
// CHECKPOINT #2 · NEGATIVE & EDGE — pin the contract down
// ═════════════════════════════════════════════════════════════════════
// Back to RED work: the corners the story left unsettled. ONE at a time.

// [negative] — blank / whitespace-only text is ignored (no todo added).
//   What exactly counts as blank — "" ? "   " ? "\t" ? Write the test that
//   pins it, then guard in the add path. Does the count stay put?

// [edge] — after a successful add, the input clears. And after an
//   ignored (blank) add — clear, or leave the text? DECIDE, then test it.

// [boundary] — plurals: "1 item left" vs "2 items left", and
//   "0 items left" when everything's done. Write the boundary tests.

// [edge] — "Clear completed" removes done todos AND is hidden when
//   nothing is completed. Test all three: hidden at the start, appears
//   once something is done, gone again after you clear.

// [edge] contract — toggling or deleting an id that no longer exists:
//   does your store shrug it off, or throw? DECIDE and encode the choice.

// REFACTOR (on green):
//   · Does the component read as "render state + forward events", with the
//     real logic living in the store?
//   · Is every state change an immutable update, or did a sneaky
//     array.push()/splice() creep back in?

// ── WHAT NEXT · the Edit stretch ─────────────────────────────────────
//   Shipped the standard todo list? The requirements grow: double-click a
//   todo to edit it — Enter saves, Esc cancels, blank text deletes it.
//   See ../../TodoListEditStory.md. Watch it push you toward the
//   smart/dumb split (b): an item that owns its own edit state.
