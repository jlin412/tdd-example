// Stack kata — step by step, STEP 5: [boundary] — last in, first out; size across a sequence.
// Snapshot at the END of the step; walkthrough in ../README.md
import { describe, it, expect } from 'vitest';

import { Stack } from './stack';

// The arrange helper the mob extracts once the setup starts repeating.
function stackOf(...items: any[]): Stack {
  const stack = new Stack();
  for (const item of items) stack.push(item);
  return stack;
}

describe('a new stack [edge]', () => {
  it('is empty', () => {
    expect(new Stack().isEmpty()).toBe(true);
  });

  it('has size 0', () => {
    expect(new Stack().size()).toBe(0);
  });
});

describe('push and pop [positive]', () => {
  it('is not empty after a push', () => {
    expect(stackOf('a').isEmpty()).toBe(false);
  });

  it('hands back the item that was pushed', () => {
    expect(stackOf('a').pop()).toBe('a');
  });

  it('is empty again once the only item is popped', () => {
    const stack = stackOf('a');
    stack.pop();
    expect(stack.isEmpty()).toBe(true);
  });
});

describe('last in, first out [boundary]', () => {
  // Two items is the SMALLEST case that can tell a stack from a queue —
  // but only a second pop proves the order (see the walkthrough, step 5).
  it('pops the most recently pushed item first', () => {
    expect(stackOf('a', 'b').pop()).toBe('b');
  });

  it('pops the rest in reverse order of pushing', () => {
    const stack = stackOf('a', 'b', 'c');
    expect([stack.pop(), stack.pop(), stack.pop()]).toEqual(['c', 'b', 'a']);
  });
});

describe('peek [positive]', () => {
  it('returns the top item', () => {
    expect(stackOf('a', 'b').peek()).toBe('b');
  });

  it('does NOT remove it — size is unchanged and peeking twice agrees', () => {
    const stack = stackOf('a', 'b');
    expect(stack.peek()).toBe('b');
    expect(stack.peek()).toBe('b');
    expect(stack.size()).toBe(2);
  });
});

describe('size tracks the whole sequence [boundary]', () => {
  it('grows on push and shrinks on pop', () => {
    const stack = new Stack();
    expect(stack.size()).toBe(0);
    stack.push('a');
    expect(stack.size()).toBe(1);
    stack.push('b');
    expect(stack.size()).toBe(2);
    stack.pop();
    expect(stack.size()).toBe(1);
  });
});

describe('holds as many items as you push [boundary]', () => {
  // The story never sets a limit. A JavaScript array grows by itself, so this
  // passes on arrival here — in Java, C# and Python it is the test that forces
  // the stack to grow its own array.
  it('keeps every item, in order, however many are pushed', () => {
    const stack = new Stack();
    for (let i = 0; i < 100; i++) stack.push(i);
    expect(stack.size()).toBe(100);
    for (let i = 99; i >= 0; i--) expect(stack.pop()).toBe(i);
    expect(stack.isEmpty()).toBe(true);
  });
});

// ── Test list — still to promote ─────────────────────────────────────
// [edge]
it.todo('popping an empty stack throws');
it.todo('peeking an empty stack throws');
it.todo('popping again after the stack has been emptied throws');
it.todo('undefined can be pushed and popped — it does not mean "empty"');

// [negative]
it.todo('the backing array cannot be tampered with from outside, even with a cast');
