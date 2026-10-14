// Stack kata — step by step, STEP 6: [edge] — the empty contract.
// Snapshot at the END of the step; walkthrough in ../README.md

export class EmptyStackError extends Error {
  constructor(operation: string) {
    super(`cannot ${operation} an empty stack`);
    this.name = 'EmptyStackError';
  }
}

export class Stack {
  private items: any[] = [];
  private count = 0;

  push(item: any): void {
    this.items[this.count] = item;
    this.count++;
  }

  pop(): any {
    if (this.isEmpty()) throw new EmptyStackError('pop');
    this.count--;
    const item = this.items[this.count];
    this.items[this.count] = undefined; // let go: a popped item must not stay reachable
    return item;
  }

  peek(): any {
    if (this.isEmpty()) throw new EmptyStackError('peek');
    return this.items[this.count - 1];
  }

  size(): number {
    return this.count;
  }

  isEmpty(): boolean {
    return this.count === 0;
  }
}
