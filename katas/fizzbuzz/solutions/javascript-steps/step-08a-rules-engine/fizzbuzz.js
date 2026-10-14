// FizzBuzz kata — step by step, STEP 8a: pattern (a) RULES ENGINE — the rules become data.
// Snapshot at the END of the step; walkthrough in ../README.md

const RULES = [
  [3, 'Fizz'],
  [5, 'Buzz'],
];

export class FizzBuzz {
  convert(n) {
    this.#validate(n);
    const word = RULES
      .filter(([divisor]) => n % divisor === 0)
      .map(([, ruleWord]) => ruleWord)
      .join('');
    return word || String(n);
  }

  #validate(n) {
    if (!Number.isInteger(n)) {
      throw new TypeError(`convert expects an integer, got ${typeof n}: ${String(n)}`);
    }
    if (n < 1) {
      throw new RangeError(`convert expects n >= 1, got ${n}`);
    }
  }
}
