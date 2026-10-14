"""FizzBuzz kata — step by step, STEP 6: [edge] — 0 and negatives are out of the game.

Snapshot at the END of the step; walkthrough in ../README.md
"""


class FizzBuzz:
    def convert(self, n: int) -> str:
        if n < 1:
            raise ValueError(f"convert expects n >= 1, got {n}")
        matched = ""
        if n % 3 == 0:
            matched += "Fizz"
        if n % 5 == 0:
            matched += "Buzz"
        return matched or str(n)
