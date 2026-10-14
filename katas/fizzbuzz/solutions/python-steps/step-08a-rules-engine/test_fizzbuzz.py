"""FizzBuzz kata — step by step, STEP 8a: pattern (a) RULES ENGINE — the rules become data.

Snapshot at the END of the step; walkthrough in ../README.md
"""
import pytest

from fizzbuzz import FizzBuzz

fizz_buzz = FizzBuzz()


# [positive] non-multiples return the number
@pytest.mark.parametrize("n, expected", [(1, "1"), (2, "2"), (4, "4")])
def test_non_multiples_return_the_number(n, expected):
    assert fizz_buzz.convert(n) == expected


# [positive]/[boundary] word rules
@pytest.mark.parametrize("n, expected", [
    (3, "Fizz"), (9, "Fizz"),
    (5, "Buzz"), (20, "Buzz"),
    (15, "FizzBuzz"), (45, "FizzBuzz"),
])
def test_word_rules(n, expected):
    assert fizz_buzz.convert(n) == expected


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
