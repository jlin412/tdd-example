// Stack kata — step by step, STEP 5: [boundary] — last in, first out; size across a sequence.
// Snapshot at the END of the step; walkthrough in ../README.md

export class Stack {
  private items: any[] = [];
  private count = 0;

  push(item: any): void {
    this.items[this.count] = item;
    this.count++;
  }

  pop(): any {
    this.count--;
    const item = this.items[this.count];
    this.items[this.count] = undefined; // let go: a popped item must not stay reachable
    return item;
  }

  peek(): any {
    return this.items[this.count - 1];
  }

  size(): number {
    return this.count;
  }

  isEmpty(): boolean {
    return this.count === 0;
  }
}
