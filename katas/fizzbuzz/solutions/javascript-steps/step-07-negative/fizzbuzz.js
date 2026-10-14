// FizzBuzz kata — step by step, STEP 7: [negative] — input that is not a whole number.
// Snapshot at the END of the step; walkthrough in ../README.md

export class FizzBuzz {
  convert(n) {
    this.#validate(n);
    let word = '';
    if (n % 3 === 0) word += 'Fizz';
    if (n % 5 === 0) word += 'Buzz';
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
