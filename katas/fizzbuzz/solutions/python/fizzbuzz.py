"""FizzBuzz kata — reference solution (classic R1–R6).

One possible end state. From the checkpoint-#1 pattern menu this team took
(a) RULES ENGINE, (b) VALUE OBJECT and (d) DEPENDENCY INJECTION — added ONE
at a time, per the coach's rule, with all tests green between patterns.

See FizzBuzzStory.md for the requirement. The R7/R8 variant lives in
fizzbuzz_extended.py (FizzBuzzExtendedStory.md).
"""
from __future__ import annotations


class Rule:
    def __init__(self, divisor: int, word: str):
        self.divisor = divisor
        self.word = word

    def matches(self, n: int) -> bool:
        return n % self.divisor == 0


RULES = [Rule(3, "Fizz"), Rule(5, "Buzz")]


class FizzBuzz:
    # Menu item (d) — injectable rules with a sensible default. (None
    # sentinel — never a mutable default argument.)
    def __init__(self, rules: list[Rule] | None = None):
        self._rules = RULES if rules is None else rules

    def convert(self, n: int) -> str:
        self._validate(n)
        matched = "".join(rule.word for rule in self._rules if rule.matches(n))
        return matched or str(n)

    @staticmethod
    def _validate(n: int) -> None:
        # bool is a subclass of int in Python — reject it explicitly.
        if isinstance(n, bool) or not isinstance(n, int):
            raise TypeError(f"convert expects an int, got {type(n).__name__}")
        if n < 1:
            raise ValueError(f"convert expects n >= 1, got {n}")
