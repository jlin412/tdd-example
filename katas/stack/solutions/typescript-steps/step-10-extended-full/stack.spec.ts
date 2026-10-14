// Stack kata — reference test suite (classic R1–R6, one possible end state).
// This is what a mob's suite might look like after the whole session.
import { describe, it, expect } from 'vitest';

import { EmptyStackError, Stack } from './stack';

// The arrange helper the mob extracts once the setup starts repeating.
function stackOf<T>(...items: T[]): Stack<T> {
  const stack = new Stack<T>();
  for (const item of items) stack.push(item);
  return stack;
}

describe('a new stack [edge]', () => {
  it('is empty', () => {
    expect(new Stack<string>().isEmpty()).toBe(true);
  });

  it('has size 0', () => {
    expect(new Stack<string>().size()).toBe(0);
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
    const stack = new Stack<string>();
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
    const stack = new Stack<number>();
    for (let i = 0; i < 100; i++) stack.push(i);
    expect(stack.size()).toBe(100);
    for (let i = 99; i >= 0; i--) expect(stack.pop()).toBe(i);
    expect(stack.isEmpty()).toBe(true);
  });
});

describe('the empty contract [edge]', () => {
  it('throws when popping an empty stack', () => {
    expect(() => new Stack<string>().pop()).toThrow(EmptyStackError);
  });

  it('throws when peeking an empty stack', () => {
    expect(() => new Stack<string>().peek()).toThrow(EmptyStackError);
  });

  it('throws again after a stack is emptied by popping', () => {
    const stack = stackOf('a');
    stack.pop();
    expect(() => stack.pop()).toThrow(EmptyStackError);
  });
});

describe('nothing-shaped items are still items [edge]', () => {
  // Precisely the case that would be ambiguous if pop() returned undefined
  // to signal "empty". Because this stack throws instead, it is not.
  it('can hold undefined without that meaning "empty"', () => {
    const stack = stackOf<string | undefined>(undefined);
    expect(stack.isEmpty()).toBe(false);
    expect(stack.pop()).toBeUndefined();
    expect(stack.isEmpty()).toBe(true);
  });
});

describe('the internals do not leak [negative]', () => {
  it('cannot be tampered with from outside, even with a cast', () => {
    const stack = stackOf('a', 'b');
    // TypeScript's `private` is checked by the compiler only — at runtime the
    // field is an ordinary property. Wreck anything array-shaped we can reach.
    for (const field of Object.values(stack)) {
      if (Array.isArray(field)) field.length = 0;
    }
    expect(stack.size()).toBe(2);
    expect(stack.peek()).toBe('b');
  });

  it('cannot be mutated through anything the public API hands out', () => {
    const stack = stackOf('a', 'b');
    const seen = [...stack]; // the iterator gives a copy, not the array
    seen.push('c');
    seen.length = 0;
    expect(stack.size()).toBe(2);
    expect(stack.peek()).toBe('b');
  });
});

describe('types are checked by `npm run typecheck`, not by `npm test` [edge]', () => {
  it('lets a wrong-typed push through at runtime — only the type checker objects', () => {
    const stack = new Stack<number>();
    // `npm run typecheck` fails if the next line ever STOPS being a type error.
    // @ts-expect-error — a string is not a number
    stack.push('a');
    expect(stack.pop()).toBe('a'); // vitest strips the types and never checks them
  });
});

describe('iterable [positive]', () => {
  it('walks the items top-down without removing them', () => {
    const stack = stackOf('a', 'b', 'c');
    expect([...stack]).toEqual(['c', 'b', 'a']);
    expect(stack.size()).toBe(3);
  });
});
