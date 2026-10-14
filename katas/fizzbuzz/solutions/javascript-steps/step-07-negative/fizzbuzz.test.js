// FizzBuzz kata — step by step, STEP 7: [negative] — input that is not a whole number.
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

describe('runtime type checking [negative]', () => {
  it.each([['3'], [3.5], [null], [undefined], [NaN], [{}]])('rejects %s', (bad) => {
    expect(() => fizzBuzz.convert(bad)).toThrow(TypeError);
  });
});
