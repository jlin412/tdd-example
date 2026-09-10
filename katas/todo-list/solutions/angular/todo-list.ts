// Todo List kata — reference solution (standard, R1–R8).
//
// One possible end state. From the checkpoint-#1 pattern menu this team took
// (a) SIGNAL STORE, (c) COMPUTED and (d) IMMUTABLE UPDATERS (all in
// todo-store.ts). The component below is thin: render store state, forward
// events. It uses OnPush — safe because every store update is immutable.
//
// Deliberately DECLINED here (all good retro topics):
//   · (b) SMART/DUMB SPLIT — one component is fine while a row just shows +
//     toggles + deletes. The moment a row gains its OWN behavior (inline
//     edit) the split pays off — see todo-list-edit.ts. Requirements, not
//     fashion, justify the pattern.
//   · (e) FILTER STRATEGY — three fixed filters read fine as one computed;
//     a predicate lookup would be machinery without payoff at this size.
//   · (f) DEPENDENCY INJECTION of a fake store — the DOM tests drive the
//     real store happily; inject a double only when a test needs to.
//
// Notice these picks DIFFER from FizzBuzz's (which took DI + factory). Same
// menu, different problem: here the win is clean signal state + derivation.

import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';

import { TODO_FILTERS, TodoStore } from './todo-store';

@Component({
  selector: 'app-todo-list',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [TodoStore],
  template: `
    <section>
      <input
        #box
        type="text"
        placeholder="What needs doing?"
        aria-label="New todo"
        [value]="draft()"
        (input)="draft.set(box.value)"
        (keyup.enter)="add()"
      />
      <button type="button" (click)="add()">Add</button>

      @if (store.todos().length === 0) {
        <p>No todos yet</p>
      } @else {
        <ul>
          @for (todo of store.visible(); track todo.id) {
            <li [class.done]="todo.done">
              <input
                type="checkbox"
                [checked]="todo.done"
                [attr.aria-label]="'Toggle ' + todo.text"
                (change)="store.toggle(todo.id)"
              />
              <span class="text">{{ todo.text }}</span>
              <button
                type="button"
                [attr.aria-label]="'Delete ' + todo.text"
                (click)="store.remove(todo.id)"
              >
                ✕
              </button>
            </li>
          }
        </ul>

        <footer>
          <span class="count">
            {{ store.remaining() }} {{ store.remaining() === 1 ? 'item' : 'items' }} left
          </span>
          <span class="filters">
            @for (name of filters; track name) {
              <button
                type="button"
                [class.selected]="store.filter() === name"
                (click)="store.setFilter(name)"
              >
                {{ name }}
              </button>
            }
          </span>
          @if (store.hasCompleted()) {
            <button type="button" class="clear" (click)="store.clearCompleted()">
              Clear completed
            </button>
          }
        </footer>
      }
    </section>
  `,
})
export class TodoList {
  protected readonly store = inject(TodoStore);
  protected readonly draft = signal('');
  protected readonly filters = TODO_FILTERS;

  protected add(): void {
    this.store.add(this.draft());
    this.draft.set(''); // R2 — input clears after a (successful or ignored) add
  }
}
