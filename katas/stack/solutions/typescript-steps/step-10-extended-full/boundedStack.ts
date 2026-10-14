// Stack kata — reference solution (extended: R7 + R8, a bounded stack).
// See StackExtendedStory.md for the requirement.
//
// This is pattern-menu item (c) DECORATOR, and the capacity requirement is
// what earns it. Note what did NOT happen: stack.ts was not opened. Every
// test in stack.spec.ts still passes, untouched, because the new behavior
// wraps the old class instead of editing it — the Open/Closed Principle with
// a green suite as the receipt.
//
// The alternative — adding a `capacity` field to Stack itself — is a
// legitimate choice too, but it makes capacity a concern of EVERY stack,
// forces a constructor argument on callers who never wanted a limit, and
// puts the old tests at risk. Wrapping keeps "a stack" and "a stack with a
// ceiling" as separate ideas.
//
// FULL CONTRACT (R8): this team throws, matching the empty-stack decision in
// stack.ts — a refused push is exceptional, not an expected outcome the
// caller should have to remember to check. Returning a boolean is the other
// defensible option; evicting the bottom item is a THIRD behavior (a ring
// buffer) that happens to be a different data structure wearing a stack's
// clothes, which is worth saying out loud in the retro.

import { Stack } from './stack';

export class FullStackError extends Error {
  constructor(capacity: number) {
    super(`cannot push onto a full stack (capacity ${capacity})`);
    this.name = 'FullStackError';
  }
}

export class BoundedStack<T> {
  readonly #stack = new Stack<T>();
  readonly #capacity: number;

  constructor(capacity: number) {
    if (!Number.isInteger(capacity) || capacity < 0) {
      throw new RangeError(`capacity must be a non-negative integer, got ${capacity}`);
    }
    this.#capacity = capacity;
  }

  get capacity(): number {
    return this.#capacity;
  }

  isFull(): boolean {
    return this.#stack.size() >= this.#capacity;
  }

  push(item: T): void {
    if (this.isFull()) throw new FullStackError(this.#capacity);
    this.#stack.push(item);
  }

  // Everything else is the wrapped stack's job, unchanged.
  pop(): T {
    return this.#stack.pop();
  }

  peek(): T {
    return this.#stack.peek();
  }

  size(): number {
    return this.#stack.size();
  }

  isEmpty(): boolean {
    return this.#stack.isEmpty();
  }

  [Symbol.iterator](): IterableIterator<T> {
    return this.#stack[Symbol.iterator]();
  }
}
