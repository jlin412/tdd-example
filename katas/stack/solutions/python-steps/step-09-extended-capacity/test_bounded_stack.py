"""Stack kata — step by step, STEP 9: extended R7 — a capacity fixed at creation.

Snapshot at the END of the step; walkthrough in ../README.md
"""
import pytest

from bounded_stack import BoundedStack
from stack import EmptyStackError


def bounded_stack_of(capacity, *items):
    stack = BoundedStack(capacity)
    for item in items:
        stack.push(item)
    return stack


class TestCapacity:
    """[positive] (R7)"""

    def test_reports_the_capacity_it_was_created_with(self):
        assert BoundedStack(3).capacity == 3


class TestAnInvalidCapacityIsRejectedAtConstruction:
    """[negative]"""

    def test_a_negative_capacity_is_a_value_error(self):
        with pytest.raises(ValueError):
            BoundedStack(-1)

    @pytest.mark.parametrize("capacity", [1.5, "2", None, True])
    def test_a_non_int_capacity_is_a_type_error(self, capacity):
        # Note for the retro: the Java and C# versions of this suite have no
        # equivalent test, because `int capacity` makes these lines
        # impossible to write. Here they are only impossible if you check.
        # (True is in the list on purpose — bool subclasses int.)
        with pytest.raises(TypeError):
            BoundedStack(capacity)


class TestEverythingThePlainStackDidItStillDoes:
    """[positive]"""

    def test_is_still_last_in_first_out(self):
        stack = bounded_stack_of(3, "a", "b", "c")
        assert [stack.pop(), stack.pop(), stack.pop()] == ["c", "b", "a"]

    def test_still_peeks_without_removing(self):
        stack = bounded_stack_of(3, "a", "b")
        assert stack.peek() == "b"
        assert stack.size() == 2

    def test_still_raises_on_an_empty_pop(self):
        # The wrapped contract is unchanged.
        with pytest.raises(EmptyStackError):
            BoundedStack(2).pop()

    def test_is_still_iterable_top_down(self):
        assert list(bounded_stack_of(3, "a", "b", "c")) == ["c", "b", "a"]

# ── Test list — still to promote (step 10, R8: a full stack refuses more) ──
# [R8]
@pytest.mark.skip(reason="todo")
def test_starts_empty_and_not_full():
    ...


@pytest.mark.skip(reason="todo")
def test_becomes_full_exactly_at_capacity():
    ...


@pytest.mark.skip(reason="todo")
def test_raises_when_pushing_past_capacity():
    ...


@pytest.mark.skip(reason="todo")
def test_does_not_grow_when_a_push_is_refused():
    ...


@pytest.mark.skip(reason="todo")
def test_accepts_a_push_again_once_something_is_popped():
    ...


@pytest.mark.skip(reason="todo")
def test_capacity_0_is_empty_and_full_at_the_same_time():
    ...


@pytest.mark.skip(reason="todo")
def test_capacity_0_refuses_the_very_first_push():
    ...

