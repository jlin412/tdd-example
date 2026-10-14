"""Stack kata — step by step, STEP 4: the other [positive] cases — push/pop, peek, size.

Snapshot at the END of the step; walkthrough in ../README.md
"""
import pytest

from stack import Stack


# The arrange helper the mob extracts once the setup starts repeating.
def stack_of(*items):
    stack = Stack()
    for item in items:
        stack.push(item)
    return stack


class TestANewStack:
    """[edge]"""

    def test_is_empty(self):
        assert Stack().is_empty() is True


class TestPushAndPop:
    """[positive]"""

    def test_is_not_empty_after_a_push(self):
        assert stack_of("a").is_empty() is False

    def test_hands_back_the_item_that_was_pushed(self):
        assert stack_of("a").pop() == "a"

    def test_is_empty_again_once_the_only_item_is_popped(self):
        stack = stack_of("a")
        stack.pop()
        assert stack.is_empty() is True


class TestPeek:
    """[positive]"""

    def test_returns_the_top_item(self):
        assert stack_of("a", "b").peek() == "b"

    def test_does_not_remove_it(self):
        stack = stack_of("a", "b")
        assert stack.peek() == "b"
        assert stack.peek() == "b"
        assert stack.size() == 2


# ── Test list — still to promote ──
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
