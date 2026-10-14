// FizzBuzz kata — step by step, STEP 6: [edge] — 0 and negatives are out of the game.
// Snapshot at the END of the step; walkthrough in ../README.md

export class FizzBuzz {
  convert(n) {
    if (n < 1) {
      throw new RangeError(`convert expects n >= 1, got ${n}`);
    }
    let word = '';
    if (n % 3 === 0) word += 'Fizz';
    if (n % 5 === 0) word += 'Buzz';
    return word || String(n);
  }
}
