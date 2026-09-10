// Todo List kata — reference test suite (edit stretch, R9–R13).
// Drives the container (which now composes <app-todo-item>) through the DOM.
// A couple of base behaviors are re-checked to prove the smart/dumb split
// didn't regress anything.

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TodoListEdit } from './todo-list-edit';

type Fixture = ComponentFixture<TodoListEdit>;

async function render(): Promise<Fixture> {
  const fixture = TestBed.createComponent(TodoListEdit);
  await fixture.whenStable();
  return fixture;
}

const page = (f: Fixture) => f.nativeElement as HTMLElement;
const rowTexts = (f: Fixture) =>
  [...page(f).querySelectorAll('li .text')].map((s) => s.textContent?.trim() ?? '');
const editInput = (f: Fixture) =>
  page(f).querySelector('input.edit') as HTMLInputElement | null;

function addTodo(f: Fixture, value: string): void {
  const input = page(f).querySelector('input[type=text]') as HTMLInputElement;
  input.value = value;
  input.dispatchEvent(new Event('input'));
  f.detectChanges();
  const add = [...page(f).querySelectorAll('button')].find(
    (b) => (b.textContent ?? '').trim() === 'Add',
  ) as HTMLButtonElement;
  add.click();
  f.detectChanges();
}

function startEdit(f: Fixture, todoText: string): void {
  const span = [...page(f).querySelectorAll('li .text')].find(
    (s) => s.textContent?.trim() === todoText,
  ) as HTMLElement;
  span.dispatchEvent(new MouseEvent('dblclick'));
  f.detectChanges();
}

function typeEdit(f: Fixture, value: string): void {
  const input = editInput(f)!;
  input.value = value;
  input.dispatchEvent(new Event('input'));
  f.detectChanges();
}

function pressKey(f: Fixture, key: string): void {
  editInput(f)!.dispatchEvent(new KeyboardEvent('keyup', { key }));
  f.detectChanges();
}

describe('TodoListEdit — inline editing (R9–R13)', () => {
  it('double-click opens an edit box prefilled with the text [R9]', async () => {
    const f = await render();
    addTodo(f, 'Buy milk');
    startEdit(f, 'Buy milk');
    expect(editInput(f)).not.toBeNull();
    expect(editInput(f)!.value).toBe('Buy milk');
  });

  it('Enter saves the trimmed new text [R10]', async () => {
    const f = await render();
    addTodo(f, 'Buy milk');
    startEdit(f, 'Buy milk');
    typeEdit(f, '  Buy oat milk  ');
    pressKey(f, 'Enter');
    expect(editInput(f)).toBeNull();
    expect(rowTexts(f)).toEqual(['Buy oat milk']);
  });

  it('Escape cancels — the original text stays [R11]', async () => {
    const f = await render();
    addTodo(f, 'Buy milk');
    startEdit(f, 'Buy milk');
    typeEdit(f, 'throwaway');
    pressKey(f, 'Escape');
    expect(editInput(f)).toBeNull();
    expect(rowTexts(f)).toEqual(['Buy milk']);
  });

  it('saving blank text deletes the todo [R12]', async () => {
    const f = await render();
    addTodo(f, 'Buy milk');
    startEdit(f, 'Buy milk');
    typeEdit(f, '   ');
    pressKey(f, 'Enter');
    expect(rowTexts(f)).toEqual([]);
  });

  it('only one row is editable at a time [R13]', async () => {
    const f = await render();
    addTodo(f, 'first');
    addTodo(f, 'second');
    startEdit(f, 'first');
    startEdit(f, 'second');
    const boxes = page(f).querySelectorAll('input.edit');
    expect(boxes).toHaveLength(1);
    expect((boxes[0] as HTMLInputElement).value).toBe('second');
  });

  it('still adds and toggles after the split [regression]', async () => {
    const f = await render();
    addTodo(f, 'a');
    addTodo(f, 'b');
    expect(rowTexts(f)).toEqual(['a', 'b']);
    const cb = page(f).querySelector('input[aria-label="Toggle a"]') as HTMLInputElement;
    cb.dispatchEvent(new Event('change'));
    f.detectChanges();
    expect(page(f).textContent).toContain('1 item left');
  });
});
