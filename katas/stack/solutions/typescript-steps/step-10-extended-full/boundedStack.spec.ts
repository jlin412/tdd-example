// Stack kata — reference test suite (extended: R7 + R8, a bounded stack).
// See StackExtendedStory.md for the requirement.
import { describe, it, expect } from 'vitest';

import { BoundedStack, FullStackError } from './boundedStack';
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

  it('starts empty and not full', () => {
    const stack = new BoundedStack<string>(2);
    expect(stack.isEmpty()).toBe(true);
    expect(stack.isFull()).toBe(false);
  });

  it('becomes full exactly at capacity', () => {
    const stack = boundedStackOf(2, 'a');
    expect(stack.isFull()).toBe(false);
    stack.push('b');
    expect(stack.isFull()).toBe(true);
  });
});

describe('a full stack refuses more [edge] (R8)', () => {
  it('throws when pushing past capacity', () => {
    const stack = boundedStackOf(2, 'a', 'b');
    expect(() => stack.push('c')).toThrow(FullStackError);
  });

  it('does not grow when a push is refused', () => {
    const stack = boundedStackOf(2, 'a', 'b');
    expect(() => stack.push('c')).toThrow();
    expect(stack.size()).toBe(2);
    expect(stack.peek()).toBe('b');
  });

  it('accepts a push again once something is popped', () => {
    const stack = boundedStackOf(2, 'a', 'b');
    expect(stack.pop()).toBe('b');
    expect(stack.isFull()).toBe(false);
    stack.push('c');
    expect(stack.peek()).toBe('c');
    expect(stack.size()).toBe(2);
  });
});

describe('capacity 0 is legal — full from birth [boundary]', () => {
  it('is empty and full at the same time', () => {
    const stack = new BoundedStack<string>(0);
    expect(stack.isEmpty()).toBe(true);
    expect(stack.isFull()).toBe(true);
  });

  it('refuses the very first push', () => {
    expect(() => new BoundedStack<string>(0).push('a')).toThrow(FullStackError);
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
