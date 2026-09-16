package kata;

import org.junit.jupiter.api.Disabled;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertTrue;

// ════════════════════════════════════════════════════════════════════
// Stack kata — YOU write the tests.
// ════════════════════════════════════════════════════════════════════
// The story is in katas/stack/StackStory.md — read it first, walkthrough
// and all.
//
// STEP 0 is the test LIST: before any code, name as many tests as the mob
// can think of. STEP 1 then works one of them through the loop:
//   RED      — write ONE failing test. Run it. Watch it fail for the
//              right reason before writing any production code.
//   GREEN    — write the MINIMUM code in Stack.java that passes.
//   REFACTOR — on green only. Then loop.
//
// STEP 1's three steps are labelled inside the test. Every test after it is
// yours to invent — the prompts suggest WHAT to think about, not which exact
// cases to write.
//
// Test-type legend:  [positive] [boundary] [edge] [negative]
//
// JAVA NOTE — you have TWO safety nets, and the first one runs before your
// tests do. `mvn test` COMPILES, then runs: a type mistake never gets as far
// as an assertion, because javac refuses to build it. So "does not compile"
// is a legitimate RED here — and it's the one you're about to get.
// CHECKPOINT #2 asks what that net does and doesn't cover.
class StackTest {

    // ═════════════════════════════════════════════════════════════════
    // STEP 0 · THE TEST LIST — do this BEFORE you write any code
    // ═════════════════════════════════════════════════════════════════
    // The first move in TDD isn't a test, it's a LIST of the tests you
    // want. As a mob, out loud, name as many as you can — go for QUANTITY
    // now and prune later. You are translating the story into test
    // language.
    //
    // This kata has a twist the earlier ones don't: a stack has STATE, so
    // a test is a SEQUENCE of operations, not a single call. Name the
    // whole sequence — "popping after two pushes returns the second one",
    // not "pop works".
    //
    // A @Disabled test is a todo that `mvn test` prints back to you as
    // skipped. Two to start you off:

    @Test
    @Disabled("todo")
    @DisplayName("a brand-new stack is empty")
    void aBrandNewStackIsEmpty() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("pushing one item then popping returns that item")
    void pushingOneItemThenPoppingReturnsThatItem() {
    }

    // Now your turn — keep adding. Walk the story's table row by row, then
    // push PAST it into what the table doesn't show. Aim for a dozen or
    // more before anyone touches production code, and cover all four
    // types:
    //   [positive] push, pop, peek, size — each behavior on its own
    //   [boundary] the FIRST case that proves last-in-first-out (how many
    //              pushes does that take?); the pop that empties it again
    //   [edge]     popping or peeking an empty stack; peek must NOT remove
    //   [negative] the story leaves this one open on purpose — for a stack
    //              that holds anything, what IS invalid input?
    //
    // Then run `mvn test`, read your list back, pick ONE, and turn it into
    // a real test at STEP 1. Drop the @Disabled as you promote it.
    //
    // (Keep the list alive: every time you think "what about…?" mid-session,
    //  add a @Disabled test instead of chasing it and losing your red.)

    // ── STEP 1 · [edge] · the empty case · the RED → GREEN → REFACTOR loop, worked ──
    // The assertion ships COMMENTED OUT — uncommenting it is your RED step.
    @Test
    void aBrandNewStackReportsThatItIsEmpty() {
        Stack stack = new Stack();

        // 1. RED — agree as a mob how "is it empty?" gets asked (isEmpty()?
        //    size()? isBlank()?), put it in the assertion below, uncomment
        //    it, and run `mvn test`. It won't even COMPILE — the class has
        //    no such method. In a statically typed language that IS your
        //    red, and you get it without running a single test.
        //    `isEmpty` is just a PLACEHOLDER — the story leaves the API
        //    shape to you. (Heads up: the reference solution in solutions/
        //    uses isEmpty() and size(), so other names stop it being a
        //    literal drop-in.)
        // assertTrue(stack.isEmpty());

        // 2. GREEN — add that method with the MINIMUM to pass
        //    (`return true;` is legitimate; let the next example force more).

        // 3. REFACTOR — generalize: the next test (push something, then ask
        //    again) is what makes a hard-coded true wrong. Let it.
    }

    // ─────────────────────────────────────────────────────────────────
    // From here on, the tests are yours. ONE red at a time.
    // New in this kata: every test needs an ARRANGE — the pushes that set
    // up the state you are about to assert on. Watch that setup grow, and
    // when it starts repeating, remember that tests are code too.
    // ─────────────────────────────────────────────────────────────────

    // [positive] — push one item, then get it back. Promote it from STEP 0.
    //   What is the smallest test that proves an item actually went in?

    // [boundary] — last in, FIRST out.
    //   How many items must you push before a pop can PROVE the order? One
    //   proves nothing: a stack and a queue behave identically when there
    //   is only one item in them. That is the whole lesson of this test.

    // [positive] — looking at the top WITHOUT taking it.
    //   Two assertions are hiding in here: what it hands back, and what it
    //   leaves behind. One test or two? Your call — but don't lose the second.

    // [boundary] — how many items are in there?
    //   Track it across a whole sequence: empty → push → push → pop. Does
    //   your count follow the operations, or only the pushes?

    // ═════════════════════════════════════════════════════════════════
    // CHECKPOINT #1 · REFACTOR — THE PATTERN MENU  (only when green)
    // ═════════════════════════════════════════════════════════════════
    // Housekeeping first (hygiene, not a pattern): is the backing list
    // private and final? Does anything in the public API hand it out?
    //
    // Then the menu. As a mob, DECIDE which ONE pattern to implement:
    //
    //   (a) GENERIC TYPE PARAMETER — Stack<T> instead of a stack of
    //       Object. Payoff check: push a String, pop it into an Integer,
    //       and watch javac refuse before a test ever runs. Then ask the
    //       harder question: at RUNTIME, can a Stack<String> tell you what
    //       T was? (It cannot — generics are ERASED. Compare with the C#
    //       version of this kata, where they are not.)
    //   (b) SWAPPABLE BACKING STORE — hide the list behind an interface
    //       and implement a linked-list version alongside it. The point
    //       isn't the linked list; it's that YOUR TESTS MUST NOT CHANGE.
    //       If they do, they were testing the implementation, not the
    //       behavior.
    //   (c) DECORATOR — wrap a Stack in something that adds behavior
    //       without editing it (logging, counting, a size limit). The
    //       Extended story makes this one earn its place.
    //   (d) OPTIONAL INSTEAD OF THROWING — an Optional<T> tryPop().
    //       Watch for the trap: Optional CANNOT hold null, so if your
    //       stack lets callers push null, Optional literally cannot
    //       express "I popped a null". Does that settle the contract
    //       argument in checkpoint #2, or dodge it?
    //   (e) ITERABLE — implement Iterable<T> so callers can walk the stack
    //       (for-each, streams) without ever touching the internals.
    //   (f) IMMUTABLE / PERSISTENT STACK — push() returns a NEW stack
    //       instead of mutating. A genuinely different design: what
    //       happens to your tests? What happens to your arrange steps?
    //
    // COACH'S RULE — ONE PATTERN AT A TIME:
    //   implement the chosen pattern → all green → ask "did it pay for
    //   itself? do we want another?" → only then pick the next item.
    //   Never two patterns mid-flight. Structural changes ((b), (c), (f))
    //   are still driven test-first, and the tests you already have must
    //   stay green throughout — that's your proof the refactor is safe.
    //
    // DISCUSS: which patterns are RIGHT here — and which are
    // over-engineering? Options, not obligations.
    // (Weigh and likely DECLINE: a linked list for its own sake — an
    //  ArrayList is already O(1) at the end; elaborate bounded type
    //  parameters (<T extends Comparable<T>>) when plain T does the job;
    //  extending java.util.Stack or ArrayList, which hands callers every
    //  method you never wanted to support — is a stack a list, or does a
    //  stack HAVE a list?)
    //
    // Tests are code too: by now the arrange steps repeat. Extract a
    // helper (a stackOf("a", "b", "c")?) — and notice that a good helper
    // makes the NEXT test easier to write, which is the real payoff.

    // ═════════════════════════════════════════════════════════════════
    // CHECKPOINT #2 · NEGATIVE & EDGE — pin the contract down
    // ═════════════════════════════════════════════════════════════════
    // Back to RED work: the corners the story deliberately left open. ONE
    // failing test at a time.

    // [edge] — popping an empty stack. And peeking one.
    //   The story does NOT tell you what should happen. Throw? Return
    //   null? Return an Optional? DECIDE as a mob, then encode the
    //   decision as a test (assertThrows is the tool). Every language
    //   answers this differently — java.util.Stack throws
    //   EmptyStackException, Python raises IndexError, JavaScript's
    //   Array.pop() quietly hands back undefined.

    // [edge] — the ambiguity that decision creates.
    //   Suppose you chose "return null when empty". Now push null into the
    //   stack and pop it. Can a caller tell those two cases apart? Write
    //   the test that exposes it, then decide what to do about it — this
    //   is exactly what menu item (d) is for.

    // [negative] — the leak.
    //   Can a caller reach your internal list and mutate the stack behind
    //   its back? Try it: get whatever your public API hands out, change
    //   it, and then ask the stack what it contains. If the stack changed,
    //   you have a bug that no other test in this file would ever have
    //   caught. (If you implemented (e), check the iterator too — what
    //   does its remove() do?)

    // [edge] — what the compiler will NOT catch for you.
    //   javac already rejected half your negative tests — pushing the
    //   wrong type simply doesn't build. But it has nothing to say about
    //   popping an empty stack, or about an iterator that hands out the
    //   live list. Retro question: which of today's bugs did the compiler
    //   find, and which could only ever have been found by a test?

    // REFACTOR (on green):
    //   · Does the public API read as a stack — or does it leak how it's
    //     stored?
    //   · Is the empty-stack decision expressed in ONE place, or repeated
    //     at every entry point?

    // ── WHAT NEXT · the Extended story ────────────────────────────────
    //   Shipped the stack? The requirements change in
    //   katas/stack/StackExtendedStory.md — the stack gains a maximum
    //   size. Work out what changed, and predict which of your tests will
    //   break BEFORE you run them.
}
