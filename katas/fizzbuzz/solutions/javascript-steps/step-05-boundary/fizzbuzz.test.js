// FizzBuzz kata — step by step, STEP 5: [boundary] — both word rules fire at once.
// Snapshot at the END of the step; walkthrough in ../README.md
import { describe, it, expect } from 'vitest';

import { FizzBuzz } from './fizzbuzz.js';

const fizzBuzz = new FizzBuzz();

describe('non-multiples [positive]', () => {
  it.each([[1, '1'], [2, '2'], [4, '4']])('convert(%i) === "%s"', (n, expected) => {
    expect(fizzBuzz.convert(n)).toBe(expected);
  });
});

describe('word rules [positive]/[boundary]', () => {
  it.each([
    [3, 'Fizz'], [9, 'Fizz'],
    [5, 'Buzz'], [20, 'Buzz'],
    [15, 'FizzBuzz'], [45, 'FizzBuzz'],
  ])('convert(%i) === "%s"', (n, expected) => {
    expect(fizzBuzz.convert(n)).toBe(expected);
  });
});

// Test list — still to promote
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
