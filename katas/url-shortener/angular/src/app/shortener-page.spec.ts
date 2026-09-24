// ════════════════════════════════════════════════════════════════════
// URL Shortener kata · FRONTEND track — YOU write the tests.
// ════════════════════════════════════════════════════════════════════
// The story is in ../../../UrlShortenerStory.md — read it first, walkthrough
// and all.
//
// This track builds the PAGE, and only the page: drive it through the DOM
// (render it, type, click) and assert on what a user would see. The service
// behind the page is GIVEN, as a contract with no implementation, in
// link-service.ts. You never build that service — your tests stand in for it.
// (The service itself is the C# track, in ../../../csharp/. The two tracks
// share the story and nothing else.)
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
// (Vitest globals — describe/it/expect/vi — are on by default here; no import.)

import { Provider } from '@angular/core';
import { TestBed } from '@angular/core/testing';

import { ShortenerPage } from './shortener-page';

// A tiny helper you can copy for your own tests. `providers` is where your
// stand-in for the service goes, once the page needs one:
//   await render([{ provide: LinkService, useValue: yourStandIn }]);
async function render(providers: Provider[] = []) {
  TestBed.configureTestingModule({ providers });
  const fixture = TestBed.createComponent(ShortenerPage);
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

it.todo('shows the short URL in a box nobody can type into');
it.todo('pressing Create shows the short URL the service hands back');

// Now your turn — keep adding. Walk the story's table row by row, then
// push PAST it into what the table doesn't show. Aim for a dozen or more
// before anyone touches production code, and cover all four types:
//   [positive] one per behavior — the bar's three controls, what Create
//              asks the service for, the short URL arriving
//   [boundary] the first and last moment of a request — when does Create
//              stop working, and when does it work again?
//   [edge]     the short box before anything is created; the page while
//              the service is still thinking; editing the long URL after
//   [negative] blank input; a service that says no
//
// Phrase them the way a USER would describe them, not the way the code
// works — "pressing Create shows the short link", not "create() sets a
// signal". DOM-level names survive the checkpoint-#1 refactor; internal
// ones don't.
//
// Then run `npm test`, read your list back, pick ONE, and turn it into a
// real test at STEP 1. Delete each todo as you promote it.
//
// (Keep the list alive: every time you think "what about…?" mid-session,
//  add an it.todo instead of chasing it and losing the red you're on.)

// ── STEP 1 · [edge] · the short-URL box · the RED → GREEN → REFACTOR loop, worked ──
// The assertion ships COMMENTED OUT — uncommenting it is your RED step.
it('shows the short URL in a box nobody can type into', async () => {
  const fixture = await render();
  const page = fixture.nativeElement as HTMLElement;

  // 1. RED — agree as a mob how a test will FIND the box (a <label>? an
  //    aria-label?), uncomment the two lines below, run `npm test`, and watch
  //    it fail: the template is empty. 'Short URL' is just a PLACEHOLDER — the
  //    story leaves the wording on screen to you.
  //    (Heads up: the reference solution in solutions/ uses that label, so
  //     different wording means its tests won't match yours verbatim.)
  // const shortBox = page.querySelector<HTMLInputElement>('input[aria-label="Short URL"]');
  // expect(shortBox?.readOnly).toBe(true);

  // 2. GREEN — add the MINIMUM to pass: one input, labelled, readonly. No
  //    service, no signals — nothing has asked for them yet.

  // 3. REFACTOR — nothing to tidy yet. The NEXT test (press Create, see a
  //    short URL) is the one that needs a backend — and there isn't one.
});

// ─────────────────────────────────────────────────────────────────────
// From here on, the tests are yours. ONE red at a time.
// Query the DOM the same way STEP 1 does: render(), then read/act on
// fixture.nativeElement. To simulate a user: set input.value and
// dispatch a new Event('input'); call button.click(); then
// `await fixture.whenStable()` before you assert.
//
// One trap, because your stand-in answers with a Promise: whenStable()
// only waits for work ANGULAR knows about. If a test looks at the page
// before your stand-in's answer has landed, let pending promises run
// first —
//     await new Promise((resolve) => setTimeout(resolve));
//     await fixture.whenStable();
// — and notice you just found a helper worth extracting.
// ─────────────────────────────────────────────────────────────────────

// [positive] — press Create, see a short URL. Promote it from STEP 0.
//   This is the test you CANNOT pass on your own: the short URL comes from a
//   service, and there isn't one — only the contract in link-service.ts.
//   So, before writing it, the mob's first design decision:
//
//   THE STAND-IN — A FAKE, OR A MOCK?
//     · A FAKE is a small class that really works: it `extends LinkService`,
//       keeps its links in an array, hands back a short URL, and remembers
//       what it was asked. You wrote it, so you can read it.
//     · A MOCK is vi.fn() with a canned answer —
//           const create = vi.fn().mockResolvedValue({ url, shortUrl, createdAt });
//           await render([{ provide: LinkService, useValue: { create } }]);
//       — and later `expect(create).toHaveBeenCalledWith(...)`.
//   Pick one and go. Keep a note of what it makes easy and what it makes
//   awkward; the retro will ask. Either way it reaches the page through DI,
//   and the page should never be able to tell which one it got.

// [positive] — what did the page ASK for?
//   How does your stand-in let a test see exactly what it was handed? Is
//   that exactly what the user typed?

// [edge] — and the long URL, once the short one appears?
//   Stay in its box, beside its short link, or clear for the next one? Look
//   at the layout the story draws before you decide. Then test the choice.

// [negative] — blank input.
//   What should the service be asked? How does a test prove NOTHING
//   happened — and which kind of stand-in makes that easy to say?

// ═════════════════════════════════════════════════════════════════════
// CHECKPOINT #1 · REFACTOR — THE PATTERN MENU  (only when green)
// ═════════════════════════════════════════════════════════════════════
// By now the page is holding the typed URL, the short link, the call to the
// service AND a template — and the spec is holding a stand-in. That crowding
// is the signal to refactor.
//
// Housekeeping first (hygiene, not a pattern): NAME the magic bits — the
// labels, the message text.
//
// Then the menu. As a mob, DECIDE which ONE pattern to implement:
//
//   (a) SMART / DUMB SPLIT — extract a presentational bar: input() for what
//       it shows, output() for "create this". The page keeps the state and
//       the service call. Payoff: the bar is testable with no stand-in at
//       all. (Pays off big once something else shares the page — see the
//       extended story.)
//   (b) SIGNAL STORE — lift the state and the call to the service out of
//       the component into an injectable store (signal() fields + methods).
//       The component becomes thin glue over it.
//   (c) COMPUTED DERIVED STATE — anything the template shows that could be
//       WORKED OUT from other state ("may Create be pressed?", "what does the
//       short box show right now?") becomes a computed() instead of a field
//       some event handler has to remember to update.
//   (d) OBSERVABLES — wrap the service in RxJS (from(), exhaustMap for
//       one-request-at-a-time) instead of async/await.
//   (e) A REUSABLE STAND-IN — if you wrote a fake, move it out of this file
//       into its own, and give it the powers your tests keep reaching for:
//       remember what it was asked, fail on demand, hold its answer back.
//       A fake is a real implementation, so it could even run the page for a
//       demo with `npm start`. A mock can't — it has nothing to remember.
//
// COACH'S RULE — ONE PATTERN AT A TIME:
//   implement the chosen pattern → all green → ask "did it pay for
//   itself? do we want another?" → only then pick the next item.
//   Never two patterns mid-flight. A pattern that changes structure ((a),
//   (b), (e)) is still driven test-first: the DOM tests you already wrote
//   must stay green THROUGHOUT — that's your proof the refactor is safe.
//
// DISCUSS: which patterns are RIGHT for this page — and which are
// over-engineering? Options, not obligations.
// (Weigh and likely DECLINE: NgRx — one bar does not need a Redux store.
//  HttpClient — this page talks to a CONTRACT, not to a URL; how the real
//  service is reached is somebody else's track. vi.mock() on a module —
//  the Angular test builder refuses it outright for your own files ("Please
//  use Angular TestBed for mocking dependencies"), which is the framework
//  telling you where the seam belongs.)
//
// Tests are code too: a type() / pressCreate() helper to extract? A
// settle() for the promise trap above?

// ═════════════════════════════════════════════════════════════════════
// CHECKPOINT #2 · NEGATIVE & EDGE — pin the contract down
// ═════════════════════════════════════════════════════════════════════
// Back to RED work: the corners the story left unsettled. ONE at a time.

// [edge] — the page while the service is still thinking.
//   What stops a second press from asking twice? And Enter in the text box
//   — does a disabled button stop THAT? To test "while it's thinking" you
//   need a stand-in that hasn't answered yet. How do you build one that
//   answers only when the test says so?

// [negative] — the service says no, or can't be reached.
//   What does the user see? What's left in the short box? And when the next
//   attempt works — does the message go away?

// [edge] — edit the long URL after Create.
//   Is the old short link still sitting beside a URL it wasn't made for?
//   DECIDE what the short box should show, then test it.

// [edge] — "   " and "  https://example.com  ".
//   What does the service get handed for each? Pin both.

// [negative] contract — is "banana" a URL?
//   Should the page stop it, or let the service decide? (The backend's
//   answer to that is its own business — what would you do if it said no?)

// [edge] contract — the same URL twice.
//   Same short link back, or a new one? Whose decision is that — the page's,
//   or the service's? What should the page do in each case?

// REFACTOR (on green):
//   · Does the component read as "show state + forward events", or is it
//     still doing bookkeeping the template could derive?
//   · Does any test reach past the DOM into the component's insides? Those
//     are the ones the next refactor will break.

// ── WHAT NEXT · the extended story, the table ────────────────────────
//   Shipped the bar? The product owner wants a table of every link, newest
//   first, under it. See ../../../UrlShortenerExtendedStory.md — and notice
//   the contract in link-service.ts already has the method it needs.
//   Watch it push you toward the smart/dumb split (a).
