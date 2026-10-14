// FizzBuzz kata — step by step, STEP 9: extended R7 — even numbers with no word print "*".
// Snapshot at the END of the step; walkthrough in ../README.md
//
// R7 adds a fallback: an even number that matched no word returns '*'. That is
// NOT a word rule (it never concatenates) — it lives in convert() as the
// "nothing matched" branch, so a Fizz/Buzz word always wins. Fizz is still the
// three times table here; step 10 changes that.

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
    if (word) return word;
    return n % 2 === 0 ? '*' : String(n); // R7: even fallback → '*', else the number
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
