// FizzBuzz kata — step by step, STEP 6: [edge] — 0 and negatives are out of the game.
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

describe('contract: n >= 1 [edge]', () => {
  it.each([[0], [-1], [-15]])('rejects %i', (n) => {
    expect(() => fizzBuzz.convert(n)).toThrow(RangeError);
  });
});

// Test list — still to promote
// [negative]
it.todo('"3" is a string, not a number — TypeError');
it.todo('3.5 is not a whole number — TypeError');
it.todo('null — TypeError');
it.todo('undefined — TypeError');
it.todo('NaN — TypeError');
it.todo('an object ({}) — TypeError');
