// FizzBuzz kata — reference solution (classic R1–R6).
// One possible end state. From the checkpoint-#1 pattern menu this team took
// (a) RULES ENGINE, (b) VALUE OBJECT and (d) DEPENDENCY INJECTION — added ONE
// at a time, per the coach's rule, with all tests green between patterns.
//
// See FizzBuzzStory.md for the requirement. The R7/R8 variant lives in
// fizzbuzzExtended.js (FizzBuzzExtendedStory.md).

export class Rule {
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
  #rules;

  // Menu item (d) — the rules list is injectable, with a sensible default,
  // so tests can probe the engine with tiny rule sets (Open/Closed by config).
  constructor(rules = RULES) {
    this.#rules = rules;
  }

  convert(n) {
    this.#validate(n);
    const word = this.#rules
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
