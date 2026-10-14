// Stack kata — step by step, STEP 3: REFACTOR — generalize the hard-coded answer.
// Snapshot at the END of the step; walkthrough in ../README.md

export class Stack {
  private count = 0;

  push(item: any): void {
    this.count++;
  }

  isEmpty(): boolean {
    return this.count === 0;
  }
}
