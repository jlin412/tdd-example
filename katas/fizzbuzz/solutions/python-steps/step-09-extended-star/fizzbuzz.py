"""FizzBuzz kata — step by step, STEP 9: extended R7 — even numbers with no word print "*".

Snapshot at the END of the step; walkthrough in ../README.md

R7 adds a fallback: an even number that matched no word returns "*". That is
NOT a word rule (it never concatenates) — it lives in convert() as the
"nothing matched" branch, so a Fizz/Buzz word always wins. Fizz is still the
three times table here; step 10 changes that.
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
        if matched:
            return matched
        return "*" if n % 2 == 0 else str(n)  # R7: even fallback → "*", else the number

    @staticmethod
    def _validate(n: int) -> None:
        # bool is a subclass of int in Python — reject it explicitly.
        if isinstance(n, bool) or not isinstance(n, int):
            raise TypeError(f"convert expects an int, got {type(n).__name__}")
        if n < 1:
            raise ValueError(f"convert expects n >= 1, got {n}")
