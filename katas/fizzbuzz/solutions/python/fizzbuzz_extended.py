"""FizzBuzz kata — reference solution (extended: R1–R6 with R7 + R8).

See FizzBuzzExtendedStory.md for the requirement.

R8 replaces R2: Fizz fires when the decimal digits CONTAIN a "3" (not
divisibility). That is no longer a (divisor, word) pair, so rules become
PREDICATES — menu item (c) STRATEGY, which the classic solution declined as
YAGNI but this requirement genuinely needs.

R7 adds a fallback: an even number that matched no word returns "*". That is
NOT a word rule (it never concatenates) — it lives in convert() as the
"nothing matched" branch, so a Fizz/Buzz word always wins.
"""
from __future__ import annotations

from typing import Callable


class Rule:
    def __init__(self, matches: Callable[[int], bool], word: str):
        self.matches = matches  # predicate: (n) -> bool
        self.word = word


RULES = [
    Rule(lambda n: "3" in str(n), "Fizz"),  # R8: digit 3, not divisibility
    Rule(lambda n: n % 5 == 0, "Buzz"),
]


class FizzBuzz:
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
