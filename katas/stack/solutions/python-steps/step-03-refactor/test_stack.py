"""Stack kata — step by step, STEP 3: REFACTOR — generalize the hard-coded answer.

Snapshot at the END of the step; walkthrough in ../README.md
"""
import pytest

from stack import Stack


class TestANewStack:
    """[edge]"""

    def test_is_empty(self):
        assert Stack().is_empty() is True


class TestPushAndPop:
    """[positive]"""

    def test_is_not_empty_after_a_push(self):
        stack = Stack()
        stack.push("a")
        assert stack.is_empty() is False


# ── Test list — still to promote ──
# [positive]
@pytest.mark.skip(reason="todo")
def test_pushing_one_item_then_popping_returns_that_item():
    ...


@pytest.mark.skip(reason="todo")
def test_popping_the_only_item_empties_the_stack_again():
    ...


@pytest.mark.skip(reason="todo")
def test_peek_returns_the_top_item():
    ...


@pytest.mark.skip(reason="todo")
def test_peek_does_not_remove_the_item():
    ...


# [boundary]
@pytest.mark.skip(reason="todo")
def test_a_brand_new_stack_has_size_0():
    ...


@pytest.mark.skip(reason="todo")
def test_after_pushing_a_then_b_pop_returns_b():
    ...


@pytest.mark.skip(reason="todo")
def test_popping_three_times_returns_the_items_in_reverse_order():
    ...


@pytest.mark.skip(reason="todo")
def test_size_grows_on_push_and_shrinks_on_pop():
    ...


@pytest.mark.skip(reason="todo")
def test_a_stack_holds_as_many_items_as_you_push():
    ...


# [edge]
@pytest.mark.skip(reason="todo")
def test_popping_an_empty_stack_raises():
    ...


@pytest.mark.skip(reason="todo")
def test_peeking_an_empty_stack_raises():
    ...


@pytest.mark.skip(reason="todo")
def test_popping_again_after_the_stack_has_been_emptied_raises():
    ...


@pytest.mark.skip(reason="todo")
def test_an_empty_pop_is_catchable_as_an_index_error():
    ...


@pytest.mark.skip(reason="todo")
def test_none_can_be_pushed_and_popped_without_meaning_empty():
    ...


# [negative]
@pytest.mark.skip(reason="todo")
def test_the_backing_list_cannot_be_tampered_with_from_outside():
    ...
