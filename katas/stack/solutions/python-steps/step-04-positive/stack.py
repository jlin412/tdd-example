"""Stack kata — step by step, STEP 4: the other [positive] cases — push/pop, peek, size.

Snapshot at the END of the step; walkthrough in ../README.md
"""


class Stack:
    def __init__(self) -> None:
        self._count = 0
        self._top = None

    def push(self, item) -> None:
        self._top = item
        self._count += 1

    def pop(self):
        self._count -= 1
        return self._top

    def peek(self):
        return self._top

    def size(self) -> int:
        return self._count

    def is_empty(self) -> bool:
        return self._count == 0
