// FizzBuzz kata — step by step, STEP 8b: pattern (b) VALUE OBJECT — each rule decides for itself.
// Snapshot at the END of the step; walkthrough in ../README.md

class Rule {
  constructor(divisor, word) {
    this.divisor = divisor;
    this.word = word;
  }

  matches(n) {
    return n % this.divisor === 0;
  }
}

const RULES = [
  new Rule(3, 'Fizz'),
  new Rule(5, 'Buzz'),
];

export class FizzBuzz {
  convert(n) {
    this.#validate(n);
    const word = RULES
      .filter((rule) => rule.matches(n))
      .map((rule) => rule.word)
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
