// Stack kata — step by step, STEP 9: extended R7 — a capacity fixed at creation.
// Snapshot at the END of the step; walkthrough in ../README.md
import { describe, it, expect } from 'vitest';

import { BoundedStack } from './boundedStack';
import { EmptyStackError } from './stack';

function boundedStackOf<T>(capacity: number, ...items: T[]): BoundedStack<T> {
  const stack = new BoundedStack<T>(capacity);
  for (const item of items) stack.push(item);
  return stack;
}

describe('capacity [positive] (R7)', () => {
  it('reports the capacity it was created with', () => {
    expect(new BoundedStack<string>(3).capacity).toBe(3);
  });
});

describe('an invalid capacity is rejected at construction [negative]', () => {
  it.each([-1, 1.5, Number.NaN])('rejects capacity %s', (capacity) => {
    expect(() => new BoundedStack<string>(capacity)).toThrow(RangeError);
  });
});

describe('everything the plain stack did, it still does [positive]', () => {
  it('is still last-in-first-out', () => {
    const stack = boundedStackOf(3, 'a', 'b', 'c');
    expect([stack.pop(), stack.pop(), stack.pop()]).toEqual(['c', 'b', 'a']);
  });

  it('still peeks without removing', () => {
    const stack = boundedStackOf(3, 'a', 'b');
    expect(stack.peek()).toBe('b');
    expect(stack.size()).toBe(2);
  });

  it('still throws on an empty pop — the wrapped contract is unchanged', () => {
    expect(() => new BoundedStack<string>(2).pop()).toThrow(EmptyStackError);
  });

  it('is still iterable, top-down', () => {
    expect([...boundedStackOf(3, 'a', 'b', 'c')]).toEqual(['c', 'b', 'a']);
  });
});

// ── Test list — still to promote (step 10, R8: a full stack refuses more) ──
it.todo('starts empty and not full');
it.todo('becomes full exactly at capacity');
it.todo('throws when pushing past capacity');
it.todo('does not grow when a push is refused');
it.todo('accepts a push again once something is popped');
it.todo('capacity 0 is empty and full at the same time');
it.todo('capacity 0 refuses the very first push');
