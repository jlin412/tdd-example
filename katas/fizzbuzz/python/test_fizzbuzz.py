"""FizzBuzz kata — YOU write the tests.

The story is in ../FizzBuzzStory.md — read it first, examples and all.

STEP 0 is the test LIST: before any code, name as many tests as the mob
can think of. STEP 1 then works one of them through the loop:
  RED      — write ONE failing test. Run it. Watch it fail for the
             right reason before writing any production code.
  GREEN    — write the MINIMUM code in fizzbuzz.py that passes.
  REFACTOR — on green only. Then loop.

STEP 1's three steps are labelled inside the test. Every test after it is
yours to invent — the prompts suggest WHAT to think about, not which exact
cases to write.

Test-type legend:  [positive] [boundary] [edge] [negative]
"""
import pytest

from fizzbuzz import FizzBuzz


# ═════════════════════════════════════════════════════════════════════
# STEP 0 · THE TEST LIST — do this BEFORE you write any code
# ═════════════════════════════════════════════════════════════════════
# The first move in TDD isn't a test, it's a LIST of the tests you want.
# As a mob, out loud, name as many as you can — go for QUANTITY now and
# prune later. You are translating the story into test language: every
# requirement you can state in English should become a name on this list.
#
# A skipped test is a todo that `pytest` prints back to you. Two to start:

@pytest.mark.skip(reason="todo")
def test_1_has_no_special_meaning_so_it_prints_1():
    ...


@pytest.mark.skip(reason="todo")
def test_3_is_a_multiple_of_three_so_it_prints_fizz():
    ...


# Now your turn — keep adding. Work through the story's example table
# first, then push PAST it into what the table doesn't show. Aim for a
# dozen or more before anyone touches production code, and cover all four
# types:
#   [positive] the ordinary cases — each word rule, on its own
#   [boundary] the first number where BOTH rules fire at once
#   [edge]     0, negatives — the corners the story leaves to you
#   [negative] input that isn't a positive whole number: "3", 3.5, None…
#
# Then run `pytest`, read your list back, pick ONE, and turn it into a
# real test at STEP 1. Drop the skip marker as you promote it.
#
# (Keep the list alive: every time you think "what about…?" mid-session,
#  add a skipped test instead of chasing it and losing the red you're on.)


# ── STEP 1 · [positive] · the plain case · the RED → GREEN → REFACTOR loop, worked ──
# The assertion ships COMMENTED OUT — uncommenting it is your RED step.
def test_returns_the_number_as_a_string_when_nothing_special_applies():
    fizz_buzz = FizzBuzz()

    # 1. RED — agree your method name as a mob, put it in the assertion
    #    below, uncomment it, run `pytest`, and watch it fail (the class
    #    is empty). `convert` here is just a PLACEHOLDER — rename it to
    #    whatever the mob chose, then stay consistent from here on.
    #    (Heads up: the reference solution in solutions/ uses `convert`, so
    #     if you pick another name it stops being a literal drop-in.)
    # assert fizz_buzz.convert(1) == "1"

    # 2. GREEN — add that method with the MINIMUM to pass
    #    (`return "1"` is legitimate; let the next example force more).

    # 3. REFACTOR — generalize: replace the hard-coded answer with real
    #    logic, proven by more examples, e.g.
    #    assert fizz_buzz.convert(2) == "2"
    #    assert fizz_buzz.convert(7) == "7"
    #    assert fizz_buzz.convert(11) == "11"


# ─────────────────────────────────────────────────────────────────────
# From here on, the tests are yours. ONE red at a time.
# ─────────────────────────────────────────────────────────────────────

# [positive] — the first word rule. Promote it from your STEP 0 list.
#   Which number proves it? Does one example convince you, or do you want
#   a second (6? 9?) to force real logic instead of a hard-coded answer?

# [positive] — the other word rule. Same questions.

# [boundary] — a number where BOTH word rules fire at once.
#   What is the FIRST such number? Why is that the interesting one?
#   Does your production code join the words, or did you special-case it?

# ═════════════════════════════════════════════════════════════════════
# CHECKPOINT #1 · REFACTOR — THE PATTERN MENU  (only when green)
# ═════════════════════════════════════════════════════════════════════
# Housekeeping first (hygiene, not a pattern): NAMED CONSTANTS —
# do 3, 5, "Fizz", "Buzz" deserve names?
#
# Then the menu. As a mob, DECIDE which ONE pattern to implement:
#
#   (a) RULES ENGINE — rules as DATA: the if/elif chain becomes a table
#       like [(3, "Fizz"), (5, "Buzz")] that the method walks, joining
#       every word whose divisor divides n.
#       Payoff check: can you add a rule WITHOUT editing the method?
#       (Every other item on this menu gets easier after this one.)
#   (b) VALUE OBJECT (OO) — promote each rule to a tiny class or
#       @dataclass: Rule(3, "Fizz") with a matches(n) method. The rule
#       DECIDES for itself ("Tell, Don't Ask") and the method reads as
#       intent: [r.word for r in rules if r.matches(n)].
#   (c) STRATEGY / POLYMORPHISM — rules that DIFFER in kind: a callable
#       predicate (lambda n: …) or subclasses (DivisibilityRule,
#       ContainsDigitRule…) free rules from divisibility ("contains a
#       3" → Fizz?). Strategy = composition, subclassing = inheritance —
#       which wins, and why? (DDD calls this the Specification pattern.)
#   (d) DEPENDENCY INJECTION — __init__ accepts a rules list with a
#       sensible default, so tests can probe the engine with tiny rule
#       sets (Open/Closed by config). Python trap: never use a MUTABLE
#       default argument — reach for a None sentinel.
#   (e) FACTORY — named constructors that hide the rule wiring:
#       a classic() preset vs a custom one as @classmethods. Callers
#       stop caring how a FizzBuzz is assembled. And what should plain
#       FizzBuzz() mean once presets exist?
#   (f) NULL OBJECT — make the str(n) fallback itself a rule that
#       "always matches, says n". Then the method is JUST "run the
#       rules". Trickier than it looks: it must fire ONLY when nothing
#       else matched — can you express "last resort" without smuggling
#       the conditional back in?
#
# COACH'S RULE — ONE PATTERN AT A TIME:
#   implement the chosen pattern → all green → ask "did it pay for
#   itself? do we want another?" → only then pick the next item.
#   Never two patterns mid-flight. If a pattern changes the public API
#   ((d), (e)), drive it TEST-FIRST like any feature: write the test
#   that wishes the API existed, watch it go red, build it.
#
# DISCUSS: which patterns are RIGHT for this requirement — and which
# are over-engineering? Patterns are options, not obligations.
# (Weigh and likely DECLINE: BUILDER — is a three-item list really
#  crying out for .with_rule(…)? CHAIN OF RESPONSIBILITY — FizzBuzz
#  CONCATENATES matches while CoR crowns one winner: it fights the
#  requirement. TEMPLATE METHOD — the inheritance twin of (c); why does
#  modern code prefer composition?)
#
# Tests are code too: repeated setup to extract? @pytest.mark.parametrize?

# ═════════════════════════════════════════════════════════════════════
# CHECKPOINT #2 · NEGATIVE & EDGE — pin the contract down
# ═════════════════════════════════════════════════════════════════════
# Back to RED work: the corners the story left dark on purpose. Same loop
# as ever — ONE failing test at a time.

# [edge] The story says counting starts at one — so what about 0? Negative
#   numbers? Careful: 0 % 3 == 0. What would naive code return for a call
#   with 0? DECIDE as a mob what "refuse it" means precisely (raise
#   what — ValueError? something custom?), then encode the decision as
#   tests (pytest.raises is the tool).

# [negative] — nothing in Python stops a caller passing "3", 3.5 or None.
#   What SHOULD each do? Write tests that pin the behavior down, then
#   guard at runtime (isinstance is a friend).
#   THE GOTCHA: bool is a subclass of int — isinstance(True, int) is
#   True. Is True valid input? Write the test that answers it.

# REFACTOR (on green):
#   · GUARD CLAUSES — do the validations read as one clean fail-fast
#     gate at the top of the method, one early exit per bad shape?
#   · Would extracting a _validate(n) helper keep the method telling
#     a single story at one level of detail?
#   · Would a raised message that NAMES the bad input help a caller?

# ── WHAT NEXT · the Extended story ───────────────────────────────────
#   Shipped classic FizzBuzz? The requirements change in
#   ../FizzBuzzExtendedStory.md — work out what changed, and predict which
#   of your tests will break BEFORE you run them.
