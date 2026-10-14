// FizzBuzz kata — step by step, STEP 3: REFACTOR — generalize the hard-coded answer.
// Snapshot at the END of the step; walkthrough in ../README.md
import { describe, it, expect } from 'vitest';

import { FizzBuzz } from './fizzbuzz.js';

const fizzBuzz = new FizzBuzz();

describe('non-multiples [positive]', () => {
  it.each([[1, '1'], [2, '2'], [4, '4']])('convert(%i) === "%s"', (n, expected) => {
    expect(fizzBuzz.convert(n)).toBe(expected);
  });
});

// Test list — still to promote
// [positive]
it.todo('3 is a multiple of three, so it prints "Fizz"');
it.todo('9 is a bigger multiple of three, so it still prints "Fizz"');
it.todo('5 is a multiple of five, so it prints "Buzz"');
it.todo('20 is a bigger multiple of five, so it still prints "Buzz"');
// [boundary]
it.todo('15 is the first multiple of both, so it prints "FizzBuzz" — Fizz first');
it.todo('45 is another multiple of both, so it prints "FizzBuzz"');
// [edge]
it.todo('0 is out of the game (counting starts at one) — RangeError, though 0 % 3 === 0');
it.todo('-1 is negative, so it is rejected with a RangeError');
it.todo('-15 is negative, so it is rejected even though it is a multiple of both');
// [negative]
it.todo('"3" is a string, not a number — TypeError');
it.todo('3.5 is not a whole number — TypeError');
it.todo('null — TypeError');
it.todo('undefined — TypeError');
it.todo('NaN — TypeError');
it.todo('an object ({}) — TypeError');
