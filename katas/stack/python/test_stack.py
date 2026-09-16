"""Stack kata — YOU write the tests.

The story is in ../StackStory.md — read it first, walkthrough and all.

STEP 0 is the test LIST: before any code, name as many tests as the mob
can think of. STEP 1 then works one of them through the loop:
  RED      — write ONE failing test. Run it. Watch it fail for the
             right reason before writing any production code.
  GREEN    — write the MINIMUM code in stack.py that passes.
  REFACTOR — on green only. Then loop.

STEP 1's three steps are labelled inside the test. Every test after it is
yours to invent — the prompts suggest WHAT to think about, not which exact
cases to write.

Test-type legend:  [positive] [boundary] [edge] [negative]

PYTHON NOTE — you have exactly ONE safety net here, and it's this file.
Nothing checks types for you: a stack built to hold ints will accept a
string without a murmur, and you'll find out somewhere else entirely, much
later. The Java, C# and TypeScript versions of this kata each get a second
net for free. CHECKPOINT #2 asks you what that costs you.
"""
import pytest

from stack import Stack


# ═════════════════════════════════════════════════════════════════════
# STEP 0 · THE TEST LIST — do this BEFORE you write any code
# ═════════════════════════════════════════════════════════════════════
# The first move in TDD isn't a test, it's a LIST of the tests you want.
# As a mob, out loud, name as many as you can — go for QUANTITY now and
# prune later. You are translating the story into test language.
#
# This kata has a twist the earlier ones don't: a stack has STATE, so a
# test is a SEQUENCE of operations, not a single call. Name the whole
# sequence — "popping after two pushes returns the second one", not
# "pop works".
#
# A skipped test is a todo that `pytest` prints back to you. Two to start:

@pytest.mark.skip(reason="todo")
def test_a_brand_new_stack_is_empty():
    ...


@pytest.mark.skip(reason="todo")
def test_pushing_one_item_then_popping_returns_that_item():
    ...


# Now your turn — keep adding. Walk the story's table row by row, then
# push PAST it into what the table doesn't show. Aim for a dozen or more
# before anyone touches production code, and cover all four types:
#   [positive] push, pop, peek, size — each behavior on its own
#   [boundary] the FIRST case that proves last-in-first-out (how many
#              pushes does that take?); the pop that empties it again
#   [edge]     popping or peeking an empty stack; peek must NOT remove
#   [negative] the story leaves this one open on purpose — for a stack
#              that holds anything, what IS invalid input?
#
# Then run `pytest`, read your list back, pick ONE, and turn it into a
# real test at STEP 1. Drop the skip marker as you promote it.
#
# (Keep the list alive: every time you think "what about…?" mid-session,
#  add a skipped test instead of chasing it and losing the red you're on.)


# ── STEP 1 · [edge] · the empty case · the RED → GREEN → REFACTOR loop, worked ──
# The assertion ships COMMENTED OUT — uncommenting it is your RED step.
def test_a_brand_new_stack_reports_that_it_is_empty():
    stack = Stack()

    # 1. RED — agree as a mob how "is it empty?" gets asked (is_empty()?
    #    size()? len()?), put it in the assertion below, uncomment it, and
    #    run `pytest`. It fails with an AttributeError: the class has no
    #    such method. `is_empty` is just a PLACEHOLDER — the story leaves
    #    the API shape to you. (Heads up: the reference solution in
    #    solutions/ uses is_empty() and size(), so other names stop it
    #    being a literal drop-in.)
    # assert stack.is_empty() is True

    # 2. GREEN — add that method with the MINIMUM to pass
    #    (`return True` is legitimate; let the next example force more).

    # 3. REFACTOR — generalize: the next test (push something, then ask
    #    again) is what makes a hard-coded True wrong. Let it.


# ─────────────────────────────────────────────────────────────────────
# From here on, the tests are yours. ONE red at a time.
# New in this kata: every test needs an ARRANGE — the pushes that set up
# the state you are about to assert on. Watch that setup grow, and when
# it starts repeating, remember that tests are code too.
# ─────────────────────────────────────────────────────────────────────

# [positive] — push one item, then get it back. Promote it from STEP 0.
#   What is the smallest test that proves an item actually went in?

# [boundary] — last in, FIRST out.
#   How many items must you push before a pop can PROVE the order? One
#   proves nothing: a stack and a queue behave identically when there is
#   only one item in them. That is the whole lesson of this test.

# [positive] — looking at the top WITHOUT taking it.
#   Two assertions are hiding in here: what it hands back, and what it
#   leaves behind. One test or two? Your call — but don't lose the second.

# [boundary] — how many items are in there?
#   Track it across a whole sequence: empty → push → push → pop. Does your
#   count follow the operations, or only the pushes?

# ═════════════════════════════════════════════════════════════════════
# CHECKPOINT #1 · REFACTOR — THE PATTERN MENU  (only when green)
# ═════════════════════════════════════════════════════════════════════
# Housekeeping first (hygiene, not a pattern): is the backing list
# private-by-convention (a leading underscore)? Does anything in the
# public API hand it out?
#
# Then the menu. As a mob, DECIDE which ONE pattern to implement:
#
#   (a) GENERIC TYPE PARAMETER — Stack(Generic[T]) instead of a stack of
#       "whatever". In Python this is DOCUMENTATION, not enforcement.
#       Payoff check: annotate a Stack[int], push a string onto it, and
#       run `pytest`. Does anything complain? (No. That answer is the
#       point — the Java, C# and TypeScript versions of this kata all
#       catch it. Discuss what you'd have to add to catch it here.)
#   (b) SWAPPABLE BACKING STORE — hide the list behind a small interface
#       (an ABC, or just a Protocol) and implement a linked-list version
#       alongside it. The point isn't the linked list; it's that YOUR
#       TESTS MUST NOT CHANGE. If they do, they were testing the
#       implementation, not the behavior.
#   (c) DECORATOR — wrap a Stack in something that adds behavior without
#       editing it (logging, counting, a size limit). The Extended story
#       makes this one earn its place.
#   (d) RESULT / OPTION INSTEAD OF RAISING — a try_pop() that returns
#       None, or a small Result value. Careful: None is also a perfectly
#       good thing to PUSH — see checkpoint #2.
#   (e) DUNDER PROTOCOLS — __iter__ so callers can walk the stack
#       (for…in, list(), unpacking) without touching the internals;
#       __len__ so len(stack) works. How much of your public API does
#       Python already have a name for?
#   (f) IMMUTABLE / PERSISTENT STACK — push() returns a NEW stack instead
#       of mutating. A genuinely different design: what happens to your
#       tests? What happens to your arrange steps?
#
# COACH'S RULE — ONE PATTERN AT A TIME:
#   implement the chosen pattern → all green → ask "did it pay for
#   itself? do we want another?" → only then pick the next item.
#   Never two patterns mid-flight. Structural changes ((b), (c), (f)) are
#   still driven test-first, and the tests you already have must stay
#   green throughout — that's your proof the refactor is safe.
#
# DISCUSS: which patterns are RIGHT here — and which are over-engineering?
# Options, not obligations.
# (Weigh and likely DECLINE: a linked list for its own sake — a Python
#  list is already O(1) at the end; __getattr__ trickery; inheriting from
#  list, which hands callers every list method you never wanted to
#  support — is a stack a list, or does a stack HAVE a list?)
#
# Tests are code too: by now the arrange steps repeat. Extract a helper
# (a stack_of("a", "b", "c")? a fixture?) — and notice that a good helper
# makes the NEXT test easier to write, which is the real payoff.

# ═════════════════════════════════════════════════════════════════════
# CHECKPOINT #2 · NEGATIVE & EDGE — pin the contract down
# ═════════════════════════════════════════════════════════════════════
# Back to RED work: the corners the story deliberately left open. ONE
# failing test at a time.

# [edge] — popping an empty stack. And peeking one.
#   The story does NOT tell you what should happen. Raise? Return None?
#   Something else? DECIDE as a mob, then encode the decision as a test
#   (pytest.raises is the tool). Every language answers this differently —
#   Python's own list.pop() raises IndexError, Java throws, JavaScript's
#   Array.pop() quietly hands back undefined.

# [edge] — the ambiguity that decision creates.
#   Suppose you chose "return None when empty". Now push None into the
#   stack and pop it. Can a caller tell those two cases apart? Write the
#   test that exposes it, then decide what to do about it — this is
#   exactly what menu item (d) is for.

# [negative] — the leak.
#   Can a caller reach your internal list and mutate the stack behind its
#   back? Try it: get whatever your public API hands out, change it, and
#   then ask the stack what it contains. If the stack changed, you have a
#   bug that no other test in this file would ever have caught.

# [negative] — the net you don't have.
#   Push a string onto a stack you meant to hold ints. Nothing stops you:
#   there is no compile step and no type-check step in this kata. Where
#   does that mistake actually surface — and how far from the push? Write
#   the test that catches it HERE instead, and ask what it cost you to
#   have to write it by hand. (Retro question: which of today's bugs would
#   a type checker have found for free?)

# REFACTOR (on green):
#   · Does the public API read as a stack — or does it leak how it's stored?
#   · Is the empty-stack decision expressed in ONE place, or repeated at
#     every entry point?

# ── WHAT NEXT · the Extended story ───────────────────────────────────
#   Shipped the stack? The requirements change in ../StackExtendedStory.md
#   — the stack gains a maximum size. Work out what changed, and predict
#   which of your tests will break BEFORE you run them.
