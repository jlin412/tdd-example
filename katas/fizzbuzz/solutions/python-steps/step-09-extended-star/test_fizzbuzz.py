"""FizzBuzz kata — step by step, STEP 9: extended R7 — even numbers with no word print "*".

Snapshot at the END of the step; walkthrough in ../README.md
"""
import pytest

from fizzbuzz import FizzBuzz, Rule

fizz_buzz = FizzBuzz()


# [positive] numbers with no word (odd, not ÷3, not ÷5) → the number
@pytest.mark.parametrize("n, expected", [(1, "1"), (7, "7"), (11, "11")])
def test_numbers_with_no_word(n, expected):
    assert fizz_buzz.convert(n) == expected


# [positive]/[boundary] word rules
@pytest.mark.parametrize("n, expected", [
    (3, "Fizz"), (9, "Fizz"),
    (5, "Buzz"), (20, "Buzz"),
    (15, "FizzBuzz"), (45, "FizzBuzz"),
])
def test_word_rules(n, expected):
    assert fizz_buzz.convert(n) == expected


# [positive] R7 — even numbers with no word → "*"
@pytest.mark.parametrize("n", [2, 4, 8, 14, 16])
def test_even_numbers_with_no_word_return_star(n):
    assert fizz_buzz.convert(n) == "*"


# [edge] a word beats "*"
def test_a_word_beats_star():
    assert fizz_buzz.convert(6) == "Fizz"       # even, but ÷3
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


# rules engine: custom rules are injectable
def test_uses_injected_rules_instead_of_the_defaults():
    evens = FizzBuzz([Rule(2, "Even")])
    assert evens.convert(4) == "Even"
    assert evens.convert(3) == "3"  # default rules not in play


# ── Test list — still to promote (step 10, R8: Fizz = the digits contain a 3) ──
@pytest.mark.skip(reason="todo")
def test_13_contains_a_3_so_it_prints_fizz():
    ...


@pytest.mark.skip(reason="todo")
def test_23_contains_a_3_so_it_prints_fizz():
    ...


@pytest.mark.skip(reason="todo")
def test_33_contains_a_3_so_it_prints_fizz():
    ...


@pytest.mark.skip(reason="todo")
def test_35_contains_a_3_and_is_a_multiple_of_5_so_it_prints_fizzbuzz():
    ...


@pytest.mark.skip(reason="todo")
def test_130_contains_a_3_and_is_a_multiple_of_5_so_it_prints_fizzbuzz():
    ...


@pytest.mark.skip(reason="todo")
def test_32_is_even_but_contains_a_3_so_it_prints_fizz_not_star():
    ...


@pytest.mark.skip(reason="todo")
def test_50_is_a_multiple_of_5_with_no_3_so_it_prints_buzz():
    ...
