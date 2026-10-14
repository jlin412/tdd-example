// Stack kata — step by step, STEP 4: the other [positive] cases — push/pop, peek, size.
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

// ── Test list — still to promote ─────────────────────────────────────
// [boundary]
it.todo('a brand-new stack has size 0');
it.todo('after pushing a then b, pop returns b');
it.todo('popping three times returns the items in reverse order');
it.todo('size grows on push and shrinks on pop');
it.todo('a stack holds as many items as you push — 100 in, 100 back out in reverse');

// [edge]
it.todo('popping an empty stack throws');
it.todo('peeking an empty stack throws');
it.todo('popping again after the stack has been emptied throws');
it.todo('undefined can be pushed and popped — it does not mean "empty"');

// [negative]
it.todo('the backing array cannot be tampered with from outside, even with a cast');
