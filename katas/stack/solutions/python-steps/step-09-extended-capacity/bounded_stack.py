"""Stack kata — step by step, STEP 9: extended R7 — a capacity fixed at creation.

Snapshot at the END of the step; walkthrough in ../README.md

Menu item (c) DECORATOR: a BoundedStack wraps a plain Stack instead of
editing it — stack.py is not opened in steps 9 or 10. The limit is not
enforced yet; step 10 (R8) does that.
"""
from __future__ import annotations

from typing import Generic, Iterator, TypeVar

from stack import Stack

T = TypeVar("T")


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

    # Everything else is the wrapped stack's job, unchanged.
    def push(self, item: T) -> None:
        self._stack.push(item)

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
