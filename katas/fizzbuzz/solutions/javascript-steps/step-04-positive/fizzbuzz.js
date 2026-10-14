// FizzBuzz kata — step by step, STEP 4: the other [positive] cases — each word rule on its own.
// Snapshot at the END of the step; walkthrough in ../README.md

export class FizzBuzz {
  convert(n) {
    if (n % 3 === 0) return 'Fizz';
    if (n % 5 === 0) return 'Buzz';
    return String(n);
  }
}
