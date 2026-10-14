"""FizzBuzz kata — step by step, STEP 8a: pattern (a) RULES ENGINE — the rules become data.

Snapshot at the END of the step; walkthrough in ../README.md
"""

RULES = [(3, "Fizz"), (5, "Buzz")]


class FizzBuzz:
    def convert(self, n: int) -> str:
        self._validate(n)
        matched = "".join(word for divisor, word in RULES if n % divisor == 0)
        return matched or str(n)

    @staticmethod
    def _validate(n: int) -> None:
        # bool is a subclass of int in Python — reject it explicitly.
        if isinstance(n, bool) or not isinstance(n, int):
            raise TypeError(f"convert expects an int, got {type(n).__name__}")
        if n < 1:
            raise ValueError(f"convert expects n >= 1, got {n}")
