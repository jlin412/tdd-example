// Stack kata — step by step, STEP 3: REFACTOR — generalize the hard-coded answer.
// Snapshot at the END of the step; walkthrough in ../README.md
import { describe, it, expect } from 'vitest';

import { Stack } from './stack';

describe('a new stack [edge]', () => {
  it('is empty', () => {
    expect(new Stack().isEmpty()).toBe(true);
  });
});

describe('push and pop [positive]', () => {
  it('is not empty after a push', () => {
    const stack = new Stack();
    stack.push('a');
    expect(stack.isEmpty()).toBe(false);
  });
});

// ── Test list — still to promote ─────────────────────────────────────
// [positive]
it.todo('pushing one item then popping returns that item');
it.todo('popping the only item empties the stack again');
it.todo('peek returns the top item');
it.todo('peek does not remove the item — peeking twice agrees, and the size is unchanged');

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
