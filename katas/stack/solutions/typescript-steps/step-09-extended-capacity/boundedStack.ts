// Stack kata — step by step, STEP 9: extended R7 — a capacity fixed at creation.
// Snapshot at the END of the step; walkthrough in ../README.md
//
// Menu item (c) DECORATOR: a BoundedStack wraps a plain Stack instead of
// editing it — stack.ts is not opened in steps 9 or 10. The limit is not
// enforced yet; step 10 (R8) does that.

import { Stack } from './stack';

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

  // Everything else is the wrapped stack's job, unchanged.
  push(item: T): void {
    this.#stack.push(item);
  }

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
