"""Stack kata — step by step, STEP 5: [boundary] — last in, first out; size; as many items as you push.

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

    def test_has_size_0(self):
        assert Stack().size() == 0


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


class TestLastInFirstOut:
    """[boundary] — two items is the SMALLEST case that tells a stack from a
    queue, but only a second pop proves the order (see the walkthrough)."""

    def test_pops_the_most_recently_pushed_item_first(self):
        assert stack_of("a", "b").pop() == "b"

    def test_pops_the_rest_in_reverse_order_of_pushing(self):
        stack = stack_of("a", "b", "c")
        assert [stack.pop(), stack.pop(), stack.pop()] == ["c", "b", "a"]


class TestPeek:
    """[positive]"""

    def test_returns_the_top_item(self):
        assert stack_of("a", "b").peek() == "b"

    def test_does_not_remove_it(self):
        stack = stack_of("a", "b")
        assert stack.peek() == "b"
        assert stack.peek() == "b"
        assert stack.size() == 2


class TestSizeTracksTheWholeSequence:
    """[boundary]"""

    def test_grows_on_push_and_shrinks_on_pop(self):
        stack = Stack()
        assert stack.size() == 0
        stack.push("a")
        assert stack.size() == 1
        stack.push("b")
        assert stack.size() == 2
        stack.pop()
        assert stack.size() == 1


class TestHoldsAsManyItemsAsYouPush:
    """[boundary] — the story never sets a limit, and a fixed-size list has to
    grow somehow. This is the test that makes it."""

    def test_keeps_every_item_in_order_however_many_are_pushed(self):
        stack = Stack()
        for i in range(100):
            stack.push(i)
        assert stack.size() == 100
        assert [stack.pop() for _ in range(100)] == list(range(99, -1, -1))
        assert stack.is_empty() is True


# ── Test list — still to promote ──
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
