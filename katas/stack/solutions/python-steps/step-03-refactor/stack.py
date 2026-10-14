"""Stack kata — step by step, STEP 3: REFACTOR — generalize the hard-coded answer.

Snapshot at the END of the step; walkthrough in ../README.md
"""


class Stack:
    def __init__(self) -> None:
        self._count = 0

    def push(self, item) -> None:
        self._count += 1

    def is_empty(self) -> bool:
        return self._count == 0
