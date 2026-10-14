// Stack kata — step by step, STEP 4: the other [positive] cases — push/pop, peek, size.
// Snapshot at the END of the step; walkthrough in ../README.md

export class Stack {
  private count = 0;
  private top: any;

  push(item: any): void {
    this.top = item;
    this.count++;
  }

  pop(): any {
    this.count--;
    return this.top;
  }

  peek(): any {
    return this.top;
  }

  size(): number {
    return this.count;
  }

  isEmpty(): boolean {
    return this.count === 0;
  }
}
