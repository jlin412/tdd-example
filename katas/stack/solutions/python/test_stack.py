"""Stack kata — reference test suite (classic R1–R6, one possible end state).

This is what a mob's suite might look like after the whole session.
"""
import pytest

from stack import EmptyStackError, Stack


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

    def test_hands_back_the_item_that_was_pushed(self):
        assert stack_of("a").pop() == "a"

    def test_is_empty_again_once_the_only_item_is_popped(self):
        stack = stack_of("a")
        stack.pop()
        assert stack.is_empty() is True


class TestLastInFirstOut:
    """[boundary] — two items is the SMALLEST case that tells a stack from a queue."""

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


class TestTheEmptyContract:
    """[edge]"""

    def test_raises_when_popping_an_empty_stack(self):
        with pytest.raises(EmptyStackError):
            Stack().pop()

    def test_raises_when_peeking_an_empty_stack(self):
        with pytest.raises(EmptyStackError):
            Stack().peek()

    def test_raises_again_after_a_stack_is_emptied_by_popping(self):
        stack = stack_of("a")
        stack.pop()
        with pytest.raises(EmptyStackError):
            stack.pop()

    def test_the_error_is_catchable_as_an_index_error(self):
        # Deliberate: list.pop() raises IndexError, so existing callers who
        # already handle "took from an empty container" keep working.
        with pytest.raises(IndexError):
            Stack().pop()


class TestNothingShapedItemsAreStillItems:
    """[edge] — precisely the case that would be ambiguous if pop() returned
    None to signal "empty". Because this stack raises instead, it is not."""

    def test_can_hold_none_without_that_meaning_empty(self):
        stack = stack_of(None)
        assert stack.is_empty() is False
        assert stack.pop() is None
        assert stack.is_empty() is True


class TestTheInternalsDoNotLeak:
    """[negative]"""

    def test_cannot_be_mutated_through_anything_the_public_api_hands_out(self):
        stack = stack_of("a", "b")
        seen = list(stack)  # iterating gives a copy, not the list itself
        seen.append("c")
        seen.clear()
        assert stack.size() == 2
        assert stack.peek() == "b"


class TestIterable:
    """[positive] — menu item (e)"""

    def test_walks_the_items_top_down_without_removing_them(self):
        stack = stack_of("a", "b", "c")
        assert list(stack) == ["c", "b", "a"]
        assert stack.size() == 3

    def test_supports_len(self):
        assert len(stack_of("a", "b")) == 2


class TestTheNetPythonDoesNotGiveYou:
    """[negative] — the type hints are documentation, not enforcement."""

    def test_a_stack_annotated_for_ints_still_accepts_a_string(self):
        # Stack[int] is a HINT. Nothing at runtime rejects this, and no
        # separate check runs in this kata — so the mistake surfaces
        # wherever the value is finally used, far from the push that caused
        # it. The Java, C# and TypeScript solutions all reject the
        # equivalent line before it ever runs. That difference is the point.
        stack: Stack[int] = Stack()
        stack.push("not an int")  # type: ignore[arg-type]
        assert stack.pop() == "not an int"
