// FizzBuzz kata — step by step, STEP 5: [boundary] — both word rules fire at once.
// Snapshot at the END of the step; walkthrough in ../README.md

export class FizzBuzz {
  convert(n) {
    let word = '';
    if (n % 3 === 0) word += 'Fizz';
    if (n % 5 === 0) word += 'Buzz';
    return word || String(n);
  }
}
