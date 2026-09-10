// Todo List kata — reference solution (edit stretch): the container.
//
// Same shell as the standard todo-list.ts, but the rows are now
// <app-todo-item> presentational components (pattern (b)). The container
// owns editingId so only ONE row edits at a time (R13) and routes item
// events to the store. store.edit() deletes on blank text (R12).

import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';

import { TODO_FILTERS, TodoStore } from './todo-store';
import { TodoItem } from './todo-item';

@Component({
  selector: 'app-todo-list',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [TodoStore],
  imports: [TodoItem],
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
            <app-todo-item
              [todo]="todo"
              [editing]="editingId() === todo.id"
              (toggle)="store.toggle(todo.id)"
              (remove)="store.remove(todo.id)"
              (startEdit)="editingId.set(todo.id)"
              (save)="onSave(todo.id, $event)"
              (cancel)="editingId.set(null)"
            />
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
export class TodoListEdit {
  protected readonly store = inject(TodoStore);
  protected readonly draft = signal('');
  protected readonly editingId = signal<number | null>(null);
  protected readonly filters = TODO_FILTERS;

  protected add(): void {
    this.store.add(this.draft());
    this.draft.set('');
  }

  protected onSave(id: number, text: string): void {
    this.store.edit(id, text); // blank text deletes (R12)
    this.editingId.set(null);
  }
}
