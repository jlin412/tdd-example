"""FizzBuzz kata — step by step, STEP 7: [negative] — input that is not a whole number.

Snapshot at the END of the step; walkthrough in ../README.md
"""


class FizzBuzz:
    def convert(self, n: int) -> str:
        self._validate(n)
        matched = ""
        if n % 3 == 0:
            matched += "Fizz"
        if n % 5 == 0:
            matched += "Buzz"
        return matched or str(n)

    @staticmethod
    def _validate(n: int) -> None:
        # bool is a subclass of int in Python — reject it explicitly.
        if isinstance(n, bool) or not isinstance(n, int):
            raise TypeError(f"convert expects an int, got {type(n).__name__}")
        if n < 1:
            raise ValueError(f"convert expects n >= 1, got {n}")
