"""FizzBuzz kata — step by step, STEP 8b: pattern (b) VALUE OBJECT — each rule decides for itself.

Snapshot at the END of the step; walkthrough in ../README.md
"""


class Rule:
    def __init__(self, divisor: int, word: str):
        self.divisor = divisor
        self.word = word

    def matches(self, n: int) -> bool:
        return n % self.divisor == 0


RULES = [Rule(3, "Fizz"), Rule(5, "Buzz")]


class FizzBuzz:
    def convert(self, n: int) -> str:
        self._validate(n)
        matched = "".join(rule.word for rule in RULES if rule.matches(n))
        return matched or str(n)

    @staticmethod
    def _validate(n: int) -> None:
        # bool is a subclass of int in Python — reject it explicitly.
        if isinstance(n, bool) or not isinstance(n, int):
            raise TypeError(f"convert expects an int, got {type(n).__name__}")
        if n < 1:
            raise ValueError(f"convert expects n >= 1, got {n}")
