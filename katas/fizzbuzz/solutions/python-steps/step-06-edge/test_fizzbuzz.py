"""FizzBuzz kata — step by step, STEP 6: [edge] — 0 and negatives are out of the game.

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


# ── Test list — still to promote ──────────────────────────────────────
# [negative]
@pytest.mark.skip(reason="todo")
def test_the_string_3_is_rejected_with_type_error():
    ...


@pytest.mark.skip(reason="todo")
def test_3_point_5_is_not_a_whole_number_type_error():
    ...


@pytest.mark.skip(reason="todo")
def test_none_is_rejected_with_type_error():
    ...


@pytest.mark.skip(reason="todo")
def test_true_is_a_bool_not_a_number_type_error():
    ...


@pytest.mark.skip(reason="todo")
def test_a_list_is_rejected_with_type_error():
    ...
