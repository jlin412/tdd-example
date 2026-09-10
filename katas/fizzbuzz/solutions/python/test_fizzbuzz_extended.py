"""FizzBuzz kata — reference test suite (extended: R1–R6 with R7 + R8).

See FizzBuzzExtendedStory.md for the requirement.
"""
import pytest

from fizzbuzz_extended import FizzBuzz, Rule

fizz_buzz = FizzBuzz()


# [positive] numbers with no word (odd, no "3" digit, not ÷5) → the number
@pytest.mark.parametrize("n, expected", [(1, "1"), (7, "7"), (9, "9"), (11, "11")])
def test_numbers_with_no_word(n, expected):
    assert fizz_buzz.convert(n) == expected


# [positive] R8 — Fizz = digits contain a 3
@pytest.mark.parametrize("n", [3, 13, 23, 33])
def test_fizz_when_digits_contain_a_3(n):
    assert fizz_buzz.convert(n) == "Fizz"


# [positive] Buzz = multiples of 5
@pytest.mark.parametrize("n", [5, 10, 20, 50])
def test_buzz_multiples_of_five(n):
    assert fizz_buzz.convert(n) == "Buzz"


# [boundary] FizzBuzz = contains a 3 AND multiple of 5
@pytest.mark.parametrize("n", [30, 35, 130])
def test_fizzbuzz_contains_3_and_multiple_of_5(n):
    assert fizz_buzz.convert(n) == "FizzBuzz"


# [positive] R7 — even numbers with no word → "*"
@pytest.mark.parametrize("n", [2, 4, 8, 14, 16])
def test_even_numbers_with_no_word_return_star(n):
    assert fizz_buzz.convert(n) == "*"


# [edge] a word beats "*"
def test_a_word_beats_star():
    assert fizz_buzz.convert(32) == "Fizz"      # even, but contains a 3
    assert fizz_buzz.convert(10) == "Buzz"      # even, but ÷5
    assert fizz_buzz.convert(30) == "FizzBuzz"  # even, but a word fires


# [edge] contract: n must be >= 1
@pytest.mark.parametrize("n", [0, -1, -15])
def test_values_below_one_are_rejected(n):
    with pytest.raises(ValueError):
        fizz_buzz.convert(n)


# [negative] runtime type checking (incl. the bool-is-an-int gotcha)
@pytest.mark.parametrize("bad", ["3", 3.5, None, True, [3]])
def test_non_integer_input_is_rejected(bad):
    with pytest.raises(TypeError):
        fizz_buzz.convert(bad)


# rules engine: custom predicate rules are injectable
def test_uses_injected_rules_instead_of_the_defaults():
    evens = FizzBuzz([Rule(lambda n: n % 2 == 0, "Even")])
    assert evens.convert(4) == "Even"
    assert evens.convert(3) == "3"  # default rules not in play; 3 is odd → itself
