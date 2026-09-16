// ════════════════════════════════════════════════════════════════════
// Stack kata — YOU write the tests.
// ════════════════════════════════════════════════════════════════════
// The story is in ../StackStory.md — read it first, walkthrough and all.
//
// STEP 0 is the test LIST: before any code, name as many tests as the mob
// can think of. STEP 1 then works one of them through the loop:
//   RED      — write ONE failing test. Run it. Watch it fail for the
//              right reason before writing any production code.
//   GREEN    — write the MINIMUM code in stack.ts that passes.
//   REFACTOR — on green only. Then loop.
//
// STEP 1's three steps are labelled inside the test. Every test after it is
// yours to invent — the prompts suggest WHAT to think about, not which exact
// cases to write.
//
// Test-type legend:  [positive] [boundary] [edge] [negative]
//
// TYPESCRIPT NOTE — you have TWO safety nets here, and they run separately:
//   `npm test`           runs the tests. Vitest strips types WITHOUT checking
//                        them, so a type error will not fail this.
//   `npm run typecheck`  checks the types and nothing else.
// Get in the habit of running both. CHECKPOINT #2 makes you prove why.

import { it, expect } from 'vitest';

import { Stack } from './stack';

// ═════════════════════════════════════════════════════════════════════
// STEP 0 · THE TEST LIST — do this BEFORE you write any code
// ═════════════════════════════════════════════════════════════════════
// The first move in TDD isn't a test, it's a LIST of the tests you want.
// As a mob, out loud, name as many as you can — go for QUANTITY now and
// prune later. You are translating the story into test language.
//
// This kata has a twist the earlier ones don't: a stack has STATE, so a
// test is a SEQUENCE of operations, not a single call. Name the whole
// sequence — "popping after two pushes returns the second one", not
// "pop works".
//
// `it.todo()` takes a name and no body, so `npm test` prints your whole
// list back as pending work. Two to start you off:

it.todo('a brand-new stack is empty');
it.todo('pushing one item then popping returns that item');

// Now your turn — keep adding. Walk the story's table row by row, then
// push PAST it into what the table doesn't show. Aim for a dozen or more
// before anyone touches production code, and cover all four types:
//   [positive] push, pop, peek, size — each behavior on its own
//   [boundary] the FIRST case that proves last-in-first-out (how many
//              pushes does that take?); the pop that empties it again
//   [edge]     popping or peeking an empty stack; peek must NOT remove
//   [negative] the story leaves this one open on purpose — for a stack
//              that holds anything, what IS invalid input?
//
// Then run `npm test`, read your list back, pick ONE, and turn it into a
// real test at STEP 1. Delete each todo as you promote it.
//
// (Keep the list alive: every time you think "what about…?" mid-session,
//  add an it.todo instead of chasing it and losing the red you're on.)

// ── STEP 1 · [edge] · the empty case · the RED → GREEN → REFACTOR loop, worked ──
// The assertion ships COMMENTED OUT — uncommenting it is your RED step.
it('a brand-new stack is empty', () => {
  const stack = new Stack();

  // 1. RED — agree as a mob how "is it empty?" gets asked (isEmpty()?
  //    size()? length?), put it in the assertion below, uncomment it, and
  //    run `npm test`. It fails: the class has no such method. Run
  //    `npm run typecheck` too and notice you get a SECOND, different red —
  //    one at runtime, one at compile time. That pair is the whole reason
  //    this kata is in TypeScript.
  //    `isEmpty` is just a PLACEHOLDER — the story leaves the API shape to
  //    you. (Heads up: the reference solution in solutions/ uses isEmpty()
  //    and size(), so other names stop it being a literal drop-in.)
  // expect(stack.isEmpty()).toBe(true);

  // 2. GREEN — add that method with the MINIMUM to pass
  //    (`return true` is legitimate; let the next example force more).

  // 3. REFACTOR — generalize: the next test (push something, then ask
  //    again) is what makes a hard-coded `true` wrong. Let it.
});

// ─────────────────────────────────────────────────────────────────────
// From here on, the tests are yours. ONE red at a time.
// New in this kata: every test needs an ARRANGE — the pushes that set up
// the state you are about to assert on. Watch that setup grow, and when
// it starts repeating, remember that tests are code too.
// ─────────────────────────────────────────────────────────────────────

// [positive] — push one item, then get it back. Promote it from STEP 0.
//   What is the smallest test that proves an item actually went in?

// [boundary] — last in, FIRST out.
//   How many items must you push before a pop can PROVE the order? One
//   proves nothing: a stack and a queue behave identically when there is
//   only one item in them. That is the whole lesson of this test.

// [positive] — looking at the top WITHOUT taking it.
//   Two assertions are hiding in here: what it hands back, and what it
//   leaves behind. One test or two? Your call — but don't lose the second.

// [boundary] — how many items are in there?
//   Track it across a whole sequence: empty → push → push → pop. Does your
//   count follow the operations, or only the pushes?

// ═════════════════════════════════════════════════════════════════════
// CHECKPOINT #1 · REFACTOR — THE PATTERN MENU  (only when green)
// ═════════════════════════════════════════════════════════════════════
// Housekeeping first (hygiene, not a pattern): is the backing array
// private? Does anything in the public API hand it out?
//
// Then the menu. As a mob, DECIDE which ONE pattern to implement:
//
//   (a) GENERIC TYPE PARAMETER — Stack<T> instead of a stack of `any`.
//       The enabler here, and the reason this kata is in TypeScript.
//       Payoff check: push a number, pop it into a `string` variable, and
//       run `npm run typecheck`. Does it complain? (If `npm test` still
//       passes, you have just learned what each tool is for.)
//   (b) SWAPPABLE BACKING STORE — hide the array behind an interface and
//       implement a linked-list version alongside it. The point isn't the
//       linked list; it's that YOUR TESTS MUST NOT CHANGE. If they do,
//       they were testing the implementation, not the behavior.
//   (c) DECORATOR — wrap a Stack in something that adds behavior without
//       editing it (logging, counting, a size limit). The Extended story
//       makes this one earn its place.
//   (d) RESULT / OPTION INSTEAD OF THROWING — a tryPop(): T | undefined,
//       or a discriminated union { ok: true, value: T } | { ok: false }.
//       Worth it if you hit the `undefined` ambiguity in checkpoint #2.
//   (e) ITERATOR — implement [Symbol.iterator]() so callers can walk the
//       stack (for…of, spread) without ever touching the internals.
//   (f) IMMUTABLE / PERSISTENT STACK — push() returns a NEW stack instead
//       of mutating. A genuinely different design: what happens to your
//       tests? What happens to your arrange steps?
//
// COACH'S RULE — ONE PATTERN AT A TIME:
//   implement the chosen pattern → all green → ask "did it pay for
//   itself? do we want another?" → only then pick the next item.
//   Never two patterns mid-flight. Structural changes ((b), (c), (f)) are
//   still driven test-first, and the tests you already have must stay
//   green throughout — that's your proof the refactor is safe.
//
// DISCUSS: which patterns are RIGHT here — and which are over-engineering?
// Options, not obligations.
// (Weigh and likely DECLINE: a linked list for its own sake — an array is
//  already O(1) at the end; a Proxy to trap access; elaborate generic
//  constraints (`T extends ...`) when plain T does the job.)
//
// Tests are code too: by now the arrange steps repeat. Extract a helper
// (a stackOf('a', 'b', 'c')?) — and notice that a good helper makes the
// NEXT test easier to write, which is the real payoff.

// ═════════════════════════════════════════════════════════════════════
// CHECKPOINT #2 · NEGATIVE & EDGE — pin the contract down
// ═════════════════════════════════════════════════════════════════════
// Back to RED work: the corners the story deliberately left open. ONE
// failing test at a time.

// [edge] — popping an empty stack. And peeking one.
//   The story does NOT tell you what should happen. Throw? Return
//   undefined? Something else? DECIDE as a mob, then encode the decision
//   as a test. (Every language answers this differently — Python raises,
//   Java throws, JavaScript's own Array.pop() quietly returns undefined.)

// [edge] — the ambiguity that decision creates.
//   Suppose you chose "return undefined when empty". Now push undefined
//   into the stack and pop it. Can a caller tell those two cases apart?
//   Write the test that exposes it, then decide what to do about it —
//   this is exactly what menu item (d) is for.

// [negative] — the leak.
//   Can a caller reach your internal array and mutate the stack behind
//   its back? Try it: get whatever your public API hands out, change it,
//   and then ask the stack what it contains. If the stack changed, you
//   have a bug that no other test in this file would ever have caught.

// [edge] — the two safety nets, proven.
//   Deliberately break a type: pop into a variable of the wrong type, or
//   push a string onto a Stack<number>. Run `npm test` — does it pass?
//   Run `npm run typecheck` — does it fail? Retro question: which of your
//   bugs today would each net have caught?

// REFACTOR (on green):
//   · Does the public API read as a stack — or does it leak how it's stored?
//   · Is the empty-stack decision expressed in ONE place, or repeated at
//     every entry point?

// ── WHAT NEXT · the Extended story ───────────────────────────────────
//   Shipped the stack? The requirements change in ../StackExtendedStory.md
//   — the stack gains a maximum size. Work out what changed, and predict
//   which of your tests will break BEFORE you run them.
