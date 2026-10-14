"""Stack kata — reference solution (extended: R7 + R8, a bounded stack).

See StackExtendedStory.md for the requirement.

This is pattern-menu item (c) DECORATOR, and the capacity requirement is
what earns it. Note what did NOT happen: stack.py was not opened. Every
test in test_stack.py still passes, untouched, because the new behavior
wraps the old class instead of editing it — the Open/Closed Principle with
a green suite as the receipt.

The alternative — adding a `capacity` argument to Stack itself — is a
legitimate choice too, but it makes capacity a concern of EVERY stack,
forces an argument on callers who never wanted a limit, and puts the old
tests at risk. Wrapping keeps "a stack" and "a stack with a ceiling" as
separate ideas.

FULL CONTRACT (R8): this team raises, matching the empty-stack decision in
stack.py — a refused push is exceptional, not an expected outcome the
caller should have to remember to check. Returning False is the other
defensible option; evicting the bottom item is a THIRD behavior (a ring
buffer) that happens to be a different data structure wearing a stack's
clothes, which is worth saying out loud in the retro.
"""
from __future__ import annotations

from typing import Generic, Iterator, TypeVar

from stack import Stack

T = TypeVar("T")


class FullStackError(Exception):
    """push() was called on a stack that is already at capacity."""

    def __init__(self, capacity: int):
        super().__init__(f"cannot push onto a full stack (capacity {capacity})")


class BoundedStack(Generic[T]):
    def __init__(self, capacity: int) -> None:
        # bool is a subclass of int in Python — reject it explicitly, the
        # same trap the FizzBuzz kata's validation runs into.
        if isinstance(capacity, bool) or not isinstance(capacity, int):
            raise TypeError(
                f"capacity must be an int, got {type(capacity).__name__}"
            )
        if capacity < 0:
            raise ValueError(f"capacity must be non-negative, got {capacity}")
        self._capacity = capacity
        self._stack: Stack[T] = Stack()

    @property
    def capacity(self) -> int:
        return self._capacity

    def is_full(self) -> bool:
        return self._stack.size() >= self._capacity

    def push(self, item: T) -> None:
        if self.is_full():
            raise FullStackError(self._capacity)
        self._stack.push(item)

    # Everything else is the wrapped stack's job, unchanged.
    def pop(self) -> T:
        return self._stack.pop()

    def peek(self) -> T:
        return self._stack.peek()

    def size(self) -> int:
        return self._stack.size()

    def is_empty(self) -> bool:
        return self._stack.is_empty()

    def __iter__(self) -> Iterator[T]:
        return iter(self._stack)

    def __len__(self) -> int:
        return self._stack.size()
