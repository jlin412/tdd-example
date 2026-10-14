// FizzBuzz kata — step by step, STEP 9: extended R7 — even numbers with no word print "*".
// Snapshot at the END of the step; walkthrough in ../README.md
import { describe, it, expect } from 'vitest';

import { FizzBuzz, Rule } from './fizzbuzz.js';

const fizzBuzz = new FizzBuzz();

describe('numbers with no word [positive]', () => {
  // odd, not ÷3, not ÷5 → the number itself
  it.each([[1, '1'], [7, '7'], [11, '11']])('convert(%i) === "%s"', (n, expected) => {
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

describe('even numbers with no word → "*" (R7) [positive]', () => {
  it.each([[2], [4], [8], [14], [16]])('convert(%i) === "*"', (n) => {
    expect(fizzBuzz.convert(n)).toBe('*');
  });
});

describe('a word beats "*" [edge]', () => {
  it('6 is even but a multiple of 3 → Fizz, not "*"', () => {
    expect(fizzBuzz.convert(6)).toBe('Fizz');
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

describe('rules engine: custom rules are injectable', () => {
  it('uses injected rules instead of the defaults', () => {
    const evens = new FizzBuzz([new Rule(2, 'Even')]);
    expect(evens.convert(4)).toBe('Even');
    expect(evens.convert(3)).toBe('3'); // default rules not in play
  });
});

// Test list — still to promote (step 10, R8: Fizz = the digits contain a 3)
it.todo('13 contains a 3, so it prints "Fizz"');
it.todo('23 contains a 3, so it prints "Fizz"');
it.todo('33 contains a 3, so it prints "Fizz"');
it.todo('35 contains a 3 and is a multiple of 5, so it prints "FizzBuzz"');
it.todo('130 contains a 3 and is a multiple of 5, so it prints "FizzBuzz"');
it.todo('32 is even but contains a 3, so it prints "Fizz", not "*"');
it.todo('50 is a multiple of 5 with no 3 in it, so it prints "Buzz"');
