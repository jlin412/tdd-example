"""Stack kata — step by step, STEP 7: [negative] — how private is private, in Python?

Snapshot at the END of the step; walkthrough in ../README.md
"""

INITIAL_CAPACITY = 4


class EmptyStackError(IndexError):
    """pop/peek was called on an empty stack.

    Subclasses IndexError on purpose: that is what list.pop() raises, so a
    caller who already handles "took from an empty container" keeps working.
    """

    def __init__(self, operation: str):
        super().__init__(f"cannot {operation} an empty stack")


class Stack:
    def __init__(self) -> None:
        self._items = [None] * INITIAL_CAPACITY
        self._count = 0

    def push(self, item: object) -> None:
        if self._count == len(self._items):
            self._grow()
        self._items[self._count] = item
        self._count += 1

    def pop(self) -> object:
        if self.is_empty():
            raise EmptyStackError("pop")
        self._count -= 1
        item = self._items[self._count]
        self._items[self._count] = None  # let go: a popped item must not stay reachable
        return item

    def peek(self) -> object:
        if self.is_empty():
            raise EmptyStackError("peek")
        return self._items[self._count - 1]

    def size(self) -> int:
        return self._count

    def is_empty(self) -> bool:
        return self._count == 0

    def _grow(self) -> None:
        # The list is used as a fixed-size array: never append()ed to. So
        # make a bigger one and copy the items across by hand.
        bigger = [None] * (len(self._items) * 2)
        for i in range(self._count):
            bigger[i] = self._items[i]
        self._items = bigger
