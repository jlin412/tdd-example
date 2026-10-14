// Stack kata — reference solution (classic R1–R6).
//
// One possible end state. From the checkpoint-#2 pattern menu this team took
// (a) GENERIC TYPE PARAMETER and (e) ITERATOR — added one at a time, per the
// coach's rule, with all tests green between them.
//
// NO NATIVE STACK: the backing store is a plain array used ONLY by index —
// items[count] = x to push, items[--count] to pop — and the stack keeps its own
// count. No push/pop/splice/shift, no `.length`: an array's push and pop ARE a
// stack, and leaning on them would hand the interesting part of the kata to the
// runtime. A popped slot is cleared, so the stack never keeps an item alive
// after handing it back (the "obsolete reference" bug in Effective Java).
//
// Deliberately NOT taken (all good retro topics):
//   · (b) SWAPPABLE BACKING STORE — the store is already hand-rolled on a plain
//     array. A linked list behind an interface would be ceremony with no payoff
//     at this size; the interface is worth introducing the day a SECOND
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

  // Menu item (e) — callers can walk the stack (for…of, spread) top-down
  // without ever getting a reference to the array itself.
  *[Symbol.iterator](): IterableIterator<T> {
    for (let i = this.#count - 1; i >= 0; i--) {
      yield this.#items[i] as T;
    }
  }
}
