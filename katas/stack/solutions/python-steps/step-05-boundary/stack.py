"""Stack kata — step by step, STEP 5: [boundary] — last in, first out; size; as many items as you push.

Snapshot at the END of the step; walkthrough in ../README.md
"""

INITIAL_CAPACITY = 4


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
        self._count -= 1
        item = self._items[self._count]
        self._items[self._count] = None  # let go: a popped item must not stay reachable
        return item

    def peek(self) -> object:
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
