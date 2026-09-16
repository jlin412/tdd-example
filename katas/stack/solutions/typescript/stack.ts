// Stack kata — reference solution (classic R1–R6).
//
// One possible end state. From the checkpoint-#1 pattern menu this team took
// (a) GENERIC TYPE PARAMETER and (e) ITERATOR — added one at a time, per the
// coach's rule, with all tests green between them.
//
// Deliberately NOT taken (all good retro topics):
//   · (b) SWAPPABLE BACKING STORE — a JS array already pushes and pops at the
//     end in O(1). A linked list behind an interface would be ceremony with no
//     payoff at this size; the interface is worth introducing the day a SECOND
//     implementation actually exists.
//   · (d) RESULT / OPTION — this team chose to throw on empty (see below), and
//     throwing is unambiguous. tryPop() earns its place only if you picked the
//     "return undefined" contract and then hit the undefined-vs-empty clash.
//   · (f) IMMUTABLE STACK — a fine design, but a different one; it changes the
//     public API from `void push` to `Stack<T> push` and every caller with it.
//
// THE EMPTY CONTRACT (R6): this team throws. The alternative — returning
// undefined, the way Array.prototype.pop() does — is defensible right up until
// you push `undefined` into the stack, at which point the caller cannot tell an
// empty stack from a stack whose top item is undefined. Throwing has no such
// blind spot, so the ambiguity decided it.

export class EmptyStackError extends Error {
  constructor(operation: string) {
    super(`cannot ${operation} an empty stack`);
    this.name = 'EmptyStackError';
  }
}

export class Stack<T> {
  // Menu item (a) — the type parameter. The array is private and never
  // handed out, so a caller cannot mutate the stack behind its back.
  readonly #items: T[] = [];

  push(item: T): void {
    this.#items.push(item);
  }

  pop(): T {
    if (this.isEmpty()) throw new EmptyStackError('pop');
    return this.#items.pop() as T;
  }

  peek(): T {
    if (this.isEmpty()) throw new EmptyStackError('peek');
    return this.#items[this.#items.length - 1] as T;
  }

  size(): number {
    return this.#items.length;
  }

  isEmpty(): boolean {
    return this.#items.length === 0;
  }

  // Menu item (e) — callers can walk the stack (for…of, spread) top-down
  // without ever getting a reference to the array itself.
  *[Symbol.iterator](): IterableIterator<T> {
    for (let i = this.#items.length - 1; i >= 0; i--) {
      yield this.#items[i] as T;
    }
  }
}
