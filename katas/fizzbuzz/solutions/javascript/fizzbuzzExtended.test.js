// FizzBuzz kata — reference test suite (extended: R1–R6 with R7 + R8).
// See FizzBuzzExtendedStory.md for the requirement.
import { describe, it, expect } from 'vitest';

import { FizzBuzz, Rule } from './fizzbuzzExtended.js';

const fizzBuzz = new FizzBuzz();

describe('numbers with no word [positive]', () => {
  // odd, no '3' digit, not ÷5 → the number itself
  it.each([[1, '1'], [7, '7'], [9, '9'], [11, '11']])('convert(%i) === "%s"', (n, expected) => {
    expect(fizzBuzz.convert(n)).toBe(expected);
  });
});

describe('Fizz = digits contain a 3 (R8) [positive]', () => {
  it.each([[3, 'Fizz'], [13, 'Fizz'], [23, 'Fizz'], [33, 'Fizz']])(
    'convert(%i) === "%s"',
    (n, expected) => {
      expect(fizzBuzz.convert(n)).toBe(expected);
    },
  );
});

describe('Buzz = multiples of 5 [positive]', () => {
  it.each([[5, 'Buzz'], [10, 'Buzz'], [20, 'Buzz'], [50, 'Buzz']])(
    'convert(%i) === "%s"',
    (n, expected) => {
      expect(fizzBuzz.convert(n)).toBe(expected);
    },
  );
});

describe('FizzBuzz = contains a 3 AND multiple of 5 [boundary]', () => {
  it.each([[30, 'FizzBuzz'], [35, 'FizzBuzz'], [130, 'FizzBuzz']])(
    'convert(%i) === "%s"',
    (n, expected) => {
      expect(fizzBuzz.convert(n)).toBe(expected);
    },
  );
});

describe('even numbers with no word → "*" (R7) [positive]', () => {
  it.each([[2], [4], [8], [14], [16]])('convert(%i) === "*"', (n) => {
    expect(fizzBuzz.convert(n)).toBe('*');
  });
});

describe('a word beats "*" [edge]', () => {
  it('32 contains a 3 → Fizz, not "*"', () => {
    expect(fizzBuzz.convert(32)).toBe('Fizz');
  });
  it('10 is even but a multiple of 5 → Buzz', () => {
    expect(fizzBuzz.convert(10)).toBe('Buzz');
  });
  it('30 → FizzBuzz, not "*"', () => {
    expect(fizzBuzz.convert(30)).toBe('FizzBuzz');
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

describe('rules engine: custom predicate rules are injectable', () => {
  it('uses injected rules instead of the defaults', () => {
    const evens = new FizzBuzz([new Rule((n) => n % 2 === 0, 'Even')]);
    expect(evens.convert(4)).toBe('Even');
    expect(evens.convert(3)).toBe('3'); // default rules not in play; 3 is odd → itself
  });
});
