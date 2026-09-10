// Todo List kata — reference solution (edit stretch): presentational item.
//
// This is pattern-menu item (b) SMART / DUMB SPLIT, now earned by R9–R13.
// TodoItem is fully CONTROLLED: the parent owns "which row is editing"
// (input `editing`) so only one row edits at a time (R13). The item knows
// nothing about the store — it renders a todo and emits intent:
//   toggle · remove · startEdit · save(text) · cancel
//
// The suppressBlur flag stops Escape's follow-up blur from committing a
// save (R11): cancel must NOT save.

import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  effect,
  input,
  output,
  signal,
  untracked,
  viewChild,
} from '@angular/core';

import { Todo } from './todo-store';

@Component({
  selector: 'app-todo-item',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <li [class.done]="todo().done">
      @if (editing()) {
        <input
          #editBox
          type="text"
          class="edit"
          [value]="draft()"
          [attr.aria-label]="'Edit ' + todo().text"
          (input)="draft.set(editBox.value)"
          (keyup.enter)="onEnter()"
          (keyup.escape)="onEscape()"
          (blur)="onBlur()"
        />
      } @else {
        <input
          type="checkbox"
          [checked]="todo().done"
          [attr.aria-label]="'Toggle ' + todo().text"
          (change)="toggle.emit()"
        />
        <span class="text" (dblclick)="startEdit.emit()">{{ todo().text }}</span>
        <button
          type="button"
          [attr.aria-label]="'Delete ' + todo().text"
          (click)="remove.emit()"
        >
          ✕
        </button>
      }
    </li>
  `,
})
export class TodoItem {
  readonly todo = input.required<Todo>();
  readonly editing = input(false);

  readonly toggle = output<void>();
  readonly remove = output<void>();
  readonly startEdit = output<void>();
  readonly save = output<string>();
  readonly cancel = output<void>();

  protected readonly draft = signal('');
  private readonly editBox = viewChild<ElementRef<HTMLInputElement>>('editBox');
  private suppressBlur = false;

  constructor() {
    // Seed the draft with the current text each time editing turns on.
    effect(() => {
      if (this.editing()) this.draft.set(untracked(() => this.todo().text));
    });
    // R9 — focus the edit box once it renders.
    effect(() => {
      if (this.editing()) this.editBox()?.nativeElement?.focus();
    });
  }

  protected onEnter(): void {
    this.suppressBlur = true;
    this.save.emit(this.draft());
  }

  protected onEscape(): void {
    this.suppressBlur = true;
    this.cancel.emit();
  }

  protected onBlur(): void {
    if (this.suppressBlur) {
      this.suppressBlur = false;
      return;
    }
    this.save.emit(this.draft()); // R10 — blur commits
  }
}
