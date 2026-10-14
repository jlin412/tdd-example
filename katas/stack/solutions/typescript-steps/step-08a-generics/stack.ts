// Stack kata — step by step, STEP 8a: pattern (a) GENERIC TYPE PARAMETER — Stack<T>.
// Snapshot at the END of the step; walkthrough in ../README.md

export class EmptyStackError extends Error {
  constructor(operation: string) {
    super(`cannot ${operation} an empty stack`);
    this.name = 'EmptyStackError';
  }
}

export class Stack<T> {
  // Menu item (a) — the type parameter. The array is a runtime-private `#`
  // field (TypeScript's `private` is compile-time only) and is never handed
  // out, so a caller cannot mutate the stack behind its back.
  readonly #items: (T | undefined)[] = [];
  #count = 0;

  push(item: T): void {
    this.#items[this.#count] = item;
    this.#count++;
  }

  pop(): T {
    if (this.isEmpty()) throw new EmptyStackError('pop');
    this.#count--;
    const item = this.#items[this.#count] as T;
    this.#items[this.#count] = undefined; // let go: a popped item must not stay reachable
    return item;
  }

  peek(): T {
    if (this.isEmpty()) throw new EmptyStackError('peek');
    return this.#items[this.#count - 1] as T;
  }

  size(): number {
    return this.#count;
  }

  isEmpty(): boolean {
    return this.#count === 0;
  }
}
