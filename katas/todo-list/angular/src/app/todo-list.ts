// ════════════════════════════════════════════════════════════════════
// Todo List — production skeleton (Angular, signals-first)
// ════════════════════════════════════════════════════════════════════
// The requirement lives in the story, not here:
//   · Standard:      ../../TodoListStory.md
//   · Edit stretch:  ../../TodoListEditStory.md
//
// TDD rules for this file:
//   · Add NOTHING here until a RED test in todo-list.spec.ts demands it.
//   · Write the MINIMUM that makes the current red test green.
//     (For a component that often means the smallest bit of TEMPLATE —
//      e.g. a literal <p>No todos yet</p> — not just class code.)
//   · Refactor only on green (prompts for that live in the spec file).

import { Component } from '@angular/core';

@Component({
  selector: 'app-todo-list',
  // Intentionally empty. Your first failing test (STEP 1) asks the DOM for
  // text that isn't here yet — the template grows one green step at a time.
  template: ``,
})
export class TodoList {
  // Intentionally empty. Your first failing test tells you what to build.
}
