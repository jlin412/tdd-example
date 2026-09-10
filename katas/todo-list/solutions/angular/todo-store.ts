// Todo List kata — reference solution: the signal store.
//
// Pattern-menu picks embodied here:
//   (a) SIGNAL STORE     — all state + mutations live here, not in the
//                          component; unit-testable without the DOM.
//   (c) COMPUTED         — `visible`, `remaining`, `hasCompleted` derive
//                          themselves; nothing is hand-maintained.
//   (d) IMMUTABLE UPDATERS — every change returns a NEW array.
//
// Contract choices (see TodoListStory.md): add() ignores blank/whitespace
// text; toggle()/remove()/edit() shrug off an unknown id (no throw).

import { Injectable, computed, signal } from '@angular/core';

export interface Todo {
  readonly id: number;
  readonly text: string;
  readonly done: boolean;
}

export type TodoFilter = 'all' | 'active' | 'completed';
export const TODO_FILTERS: readonly TodoFilter[] = ['all', 'active', 'completed'];

@Injectable()
export class TodoStore {
  private readonly _todos = signal<Todo[]>([]);
  private readonly _filter = signal<TodoFilter>('all');
  private nextId = 1;

  readonly todos = this._todos.asReadonly();
  readonly filter = this._filter.asReadonly();

  readonly visible = computed<Todo[]>(() => {
    const filter = this._filter();
    return this._todos().filter((todo) =>
      filter === 'all' ? true : filter === 'active' ? !todo.done : todo.done,
    );
  });

  readonly remaining = computed(
    () => this._todos().filter((todo) => !todo.done).length,
  );

  readonly hasCompleted = computed(() =>
    this._todos().some((todo) => todo.done),
  );

  add(text: string): void {
    const trimmed = text.trim();
    if (trimmed === '') return; // R3 — ignore blank / whitespace-only
    this._todos.update((list) => [
      ...list,
      { id: this.nextId++, text: trimmed, done: false },
    ]);
  }

  toggle(id: number): void {
    this._todos.update((list) =>
      list.map((todo) => (todo.id === id ? { ...todo, done: !todo.done } : todo)),
    );
  }

  remove(id: number): void {
    this._todos.update((list) => list.filter((todo) => todo.id !== id));
  }

  setFilter(filter: TodoFilter): void {
    this._filter.set(filter);
  }

  clearCompleted(): void {
    this._todos.update((list) => list.filter((todo) => !todo.done));
  }

  // Used by the edit stretch (R10/R12): save trimmed text; blank deletes.
  edit(id: number, text: string): void {
    const trimmed = text.trim();
    if (trimmed === '') {
      this.remove(id);
      return;
    }
    this._todos.update((list) =>
      list.map((todo) => (todo.id === id ? { ...todo, text: trimmed } : todo)),
    );
  }
}
