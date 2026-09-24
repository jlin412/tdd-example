// ════════════════════════════════════════════════════════════════════
// ShortenerPage — production skeleton (Angular, signals-first)
// ════════════════════════════════════════════════════════════════════
// The requirement lives in the story, not here:
//   · The product:      ../../../UrlShortenerStory.md
//   · Then, the table:  ../../../UrlShortenerExtendedStory.md
//
// This is the FRONTEND track: the page, and only the page. The service behind
// it is GIVEN as a contract with no implementation (link-service.ts). You never
// build it — your tests stand in for it.
//
// TDD rules for this file:
//   · Add NOTHING here until a RED test in shortener-page.spec.ts demands it.
//   · Write the MINIMUM that makes the current red test green.
//     (For a component that often means the smallest bit of TEMPLATE —
//      e.g. a single <input readonly> — not just class code.)
//   · Refactor only on green (prompts for that live in the spec file).

import { Component } from '@angular/core';

@Component({
  selector: 'app-shortener-page',
  // Intentionally empty. Your first failing test (STEP 1) asks the DOM for
  // something that isn't here yet — the template grows one green step at a time.
  template: ``,
})
export class ShortenerPage {
  // Intentionally empty. Your first failing test tells you what to build.
}
