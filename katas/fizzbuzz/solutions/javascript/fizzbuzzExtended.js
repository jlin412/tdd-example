// FizzBuzz kata — reference solution (extended: R1–R6 with R7 + R8).
// See FizzBuzzExtendedStory.md for the requirement.
//
// R8 replaces R2: Fizz fires when the decimal digits CONTAIN a '3' (not
// divisibility). That is no longer expressible as a [divisor, word] pair, so
// rules become PREDICATES ((n) => boolean) — menu item (c) STRATEGY, which the
// classic solution declined as YAGNI but this requirement genuinely needs.
//
// R7 adds a fallback: an even number that matched no word returns '*'. That is
// NOT a word rule (it never concatenates) — it lives in convert() as the
// "nothing matched" branch, so a Fizz/Buzz word always wins.

export class Rule {
  constructor(matches, word) {
    this.matches = matches; // predicate: (n) => boolean
    this.word = word;
  }
}

const RULES = [
  new Rule((n) => String(n).includes('3'), 'Fizz'), // R8: digit 3, not divisibility
  new Rule((n) => n % 5 === 0, 'Buzz'),
];

export class FizzBuzz {
  #rules;

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
