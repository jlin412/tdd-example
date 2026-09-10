// Todo List kata — reference test suite (standard R1–R8, one possible end state).
// Drives the component through the DOM, plus a few pure store unit tests to
// show the extracted store (pattern (a)) is testable without Angular at all.

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TodoList } from './todo-list';
import { TodoStore } from './todo-store';

type Fixture = ComponentFixture<TodoList>;

async function render(): Promise<Fixture> {
  const fixture = TestBed.createComponent(TodoList);
  await fixture.whenStable();
  return fixture;
}

const page = (f: Fixture) => f.nativeElement as HTMLElement;
const allText = (f: Fixture) => page(f).textContent ?? '';
const rows = (f: Fixture) => [...page(f).querySelectorAll('li')] as HTMLLIElement[];
const rowTexts = (f: Fixture) =>
  rows(f).map((li) => li.querySelector('.text')?.textContent?.trim() ?? '');
const hasButton = (f: Fixture, label: string) =>
  [...page(f).querySelectorAll('button')].some(
    (b) => (b.textContent ?? '').trim() === label,
  );

function clickButton(f: Fixture, label: string): void {
  const button = [...page(f).querySelectorAll('button')].find(
    (b) => (b.textContent ?? '').trim() === label,
  ) as HTMLButtonElement | undefined;
  if (!button) throw new Error(`button "${label}" not found`);
  button.click();
  f.detectChanges();
}

function addTodo(f: Fixture, value: string): void {
  const input = page(f).querySelector('input[type=text]') as HTMLInputElement;
  input.value = value;
  input.dispatchEvent(new Event('input'));
  f.detectChanges(); // render the typed value (a real app runs CD on input)
  clickButton(f, 'Add');
}

function toggleRow(f: Fixture, todoText: string): void {
  const checkbox = page(f).querySelector(
    `input[aria-label="Toggle ${todoText}"]`,
  ) as HTMLInputElement;
  checkbox.dispatchEvent(new Event('change'));
  f.detectChanges();
}

function deleteRow(f: Fixture, todoText: string): void {
  const button = page(f).querySelector(
    `button[aria-label="Delete ${todoText}"]`,
  ) as HTMLButtonElement;
  button.click();
  f.detectChanges();
}

describe('TodoList component (through the DOM)', () => {
  it('shows an empty-state message when there are no todos [edge]', async () => {
    const f = await render();
    expect(allText(f)).toContain('No todos yet');
    expect(rows(f)).toHaveLength(0);
  });

  it('adds a todo and clears the input [positive]', async () => {
    const f = await render();
    addTodo(f, 'Buy milk');
    expect(rowTexts(f)).toEqual(['Buy milk']);
    expect(allText(f)).not.toContain('No todos yet');
    const input = page(f).querySelector('input[type=text]') as HTMLInputElement;
    expect(input.value).toBe('');
  });

  it('ignores blank / whitespace-only text [negative]', async () => {
    const f = await render();
    addTodo(f, '   ');
    expect(rows(f)).toHaveLength(0);
    expect(allText(f)).toContain('No todos yet');
  });

  it('toggles a todo done [positive]', async () => {
    const f = await render();
    addTodo(f, 'Buy milk');
    toggleRow(f, 'Buy milk');
    const li = rows(f)[0];
    expect(li.classList.contains('done')).toBe(true);
    expect((li.querySelector('input[type=checkbox]') as HTMLInputElement).checked).toBe(true);
  });

  it('deletes a todo [positive]', async () => {
    const f = await render();
    addTodo(f, 'Buy milk');
    addTodo(f, 'Walk dog');
    deleteRow(f, 'Buy milk');
    expect(rowTexts(f)).toEqual(['Walk dog']);
  });

  it('counts remaining active todos, pluralized [boundary]', async () => {
    const f = await render();
    addTodo(f, 'a');
    addTodo(f, 'b');
    expect(allText(f)).toContain('2 items left');
    toggleRow(f, 'a');
    expect(allText(f)).toContain('1 item left');
    toggleRow(f, 'b');
    expect(allText(f)).toContain('0 items left');
  });

  it('filters by all / active / completed [positive][boundary]', async () => {
    const f = await render();
    addTodo(f, 'active one');
    addTodo(f, 'done one');
    toggleRow(f, 'done one');

    clickButton(f, 'active');
    expect(rowTexts(f)).toEqual(['active one']);

    clickButton(f, 'completed');
    expect(rowTexts(f)).toEqual(['done one']);

    clickButton(f, 'all');
    expect(rowTexts(f)).toEqual(['active one', 'done one']);
  });

  it('clears completed and hides the button when none are done [edge]', async () => {
    const f = await render();
    addTodo(f, 'keep');
    addTodo(f, 'remove');
    expect(hasButton(f, 'Clear completed')).toBe(false);

    toggleRow(f, 'remove');
    expect(hasButton(f, 'Clear completed')).toBe(true);

    clickButton(f, 'Clear completed');
    expect(rowTexts(f)).toEqual(['keep']);
    expect(hasButton(f, 'Clear completed')).toBe(false);
  });
});

describe('TodoStore (unit — no DOM, thanks to pattern (a)) [SRP]', () => {
  it('adds trimmed todos and assigns ids', () => {
    const store = new TodoStore();
    store.add('  hello  ');
    expect(store.todos()).toEqual([{ id: 1, text: 'hello', done: false }]);
  });

  it('shrugs off unknown ids [contract]', () => {
    const store = new TodoStore();
    store.add('x');
    expect(() => {
      store.toggle(999);
      store.remove(999);
    }).not.toThrow();
    expect(store.todos()).toHaveLength(1);
  });

  it('derives remaining and the visible list', () => {
    const store = new TodoStore();
    store.add('a');
    store.add('b');
    store.toggle(1);
    expect(store.remaining()).toBe(1);
    store.setFilter('completed');
    expect(store.visible().map((t) => t.text)).toEqual(['a']);
  });
});
