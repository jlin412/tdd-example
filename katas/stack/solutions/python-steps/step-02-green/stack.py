# ════════════════════════════════════════════════════════════════════
# Stack — production skeleton (Python)
# ════════════════════════════════════════════════════════════════════
# The requirement lives in the story, not here:
#   · Classic:  ../StackStory.md
#   · Extended: ../StackExtendedStory.md
#
# TDD rules for this file:
#   · Add NOTHING here until a RED test in test_stack.py demands it.
#   · Write the MINIMUM that makes the current red test green.
#   · Refactor only on green (prompts for that live in the test file).
#
# One thing to know about Python here: it has NO separate type-checking
# step. Whatever type hints you write are hints — nothing enforces them
# at runtime. Your tests are the only safety net you have.


class Stack:
    def is_empty(self):
        return True
