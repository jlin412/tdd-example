using Xunit;

namespace Kata;

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
//   GREEN    — write the MINIMUM code in Stack.cs that passes.
//   REFACTOR — on green only. Then loop.
//
// STEP 1's three steps are labelled inside the test. Every test after it is
// yours to invent — the prompts suggest WHAT to think about, not which exact
// cases to write.
//
// Test-type legend:  [positive] [boundary] [edge] [negative]
//
// C# NOTE — you have TWO safety nets, and the first one runs before your
// tests do. `dotnet test` BUILDS, then runs: a type mistake never gets as
// far as an assertion, because the compiler refuses to produce an assembly.
// So "does not build" is a legitimate RED here — and it's the one you're
// about to get. CHECKPOINT #2 asks what that net does and doesn't cover.
public class StackTests
{
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
    // A skipped Fact is a todo that `dotnet test` prints back to you.
    // Two to start you off:

    [Fact(Skip = "todo")]
    public void ABrandNewStackIsEmpty()
    {
    }

    [Fact(Skip = "todo")]
    public void PushingOneItemThenPoppingReturnsThatItem()
    {
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
    // Then run `dotnet test`, read your list back, pick ONE, and turn it
    // into a real test at STEP 1. Drop the Skip as you promote it.
    //
    // (Keep the list alive: every time you think "what about…?" mid-session,
    //  add a skipped Fact instead of chasing it and losing your red.)

    // ── STEP 1 · [edge] · the empty case · the RED → GREEN → REFACTOR loop, worked ──
    // The assertion ships COMMENTED OUT — uncommenting it is your RED step.
    [Fact]
    public void ABrandNewStackReportsThatItIsEmpty()
    {
        var stack = new Stack();

        // 1. RED — agree as a mob how "is it empty?" gets asked (IsEmpty()?
        //    Count? Any()?), put it in the assertion below, uncomment it,
        //    and run `dotnet test`. It won't even BUILD — the class has no
        //    such member. In a statically typed language that IS your red,
        //    and you get it without running a single test.
        //    `IsEmpty` is just a PLACEHOLDER — the story leaves the API
        //    shape to you. (Heads up: the reference solution in solutions/
        //    uses IsEmpty() and Size(), so other names stop it being a
        //    literal drop-in.)
        // Assert.True(stack.IsEmpty());

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
    // private and readonly? Does anything in the public API hand it out?
    //
    // Then the menu. As a mob, DECIDE which ONE pattern to implement:
    //
    //   (a) GENERIC TYPE PARAMETER — Stack<T> instead of a stack of
    //       object. Payoff check: push a string, pop it into an int, and
    //       watch the build fail before a test ever runs. Then the harder
    //       question: at RUNTIME, does a Stack<string> still know what T
    //       was? (In C# it does — generics are REIFIED. Compare with the
    //       Java version of this kata, where they are erased.)
    //   (b) SWAPPABLE BACKING STORE — hide the list behind an interface
    //       and implement a linked-list version alongside it. The point
    //       isn't the linked list; it's that YOUR TESTS MUST NOT CHANGE.
    //       If they do, they were testing the implementation, not the
    //       behavior.
    //   (c) DECORATOR — wrap a Stack in something that adds behavior
    //       without editing it (logging, counting, a size limit). The
    //       Extended story makes this one earn its place.
    //   (d) THE TRY-PATTERN INSTEAD OF THROWING — bool TryPop(out T value),
    //       the idiom .NET uses everywhere (including on its own
    //       Stack<T>). Worth it if you hit the `default` ambiguity in
    //       checkpoint #2.
    //   (e) IENUMERABLE<T> — implement it with `yield return` so callers
    //       can walk the stack (foreach, LINQ) without ever touching the
    //       internals.
    //   (f) IMMUTABLE / PERSISTENT STACK — Push() returns a NEW stack
    //       instead of mutating (this is what
    //       System.Collections.Immutable.ImmutableStack<T> does). A
    //       genuinely different design: what happens to your tests? What
    //       happens to your arrange steps?
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
    // (Weigh and likely DECLINE: a linked list for its own sake — a
    //  List<T> is already O(1) at the end; elaborate generic constraints
    //  (where T : IComparable<T>) when plain T does the job; inheriting
    //  from List<T>, which hands callers every method you never wanted to
    //  support — is a stack a list, or does a stack HAVE a list?)
    //
    // Tests are code too: by now the arrange steps repeat. Extract a
    // helper (a StackOf("a", "b", "c")?) — and notice that a good helper
    // makes the NEXT test easier to write, which is the real payoff.

    // ═════════════════════════════════════════════════════════════════
    // CHECKPOINT #2 · NEGATIVE & EDGE — pin the contract down
    // ═════════════════════════════════════════════════════════════════
    // Back to RED work: the corners the story deliberately left open. ONE
    // failing test at a time.

    // [edge] — popping an empty stack. And peeking one.
    //   The story does NOT tell you what should happen. Throw? Return
    //   default? Return a bool and an out parameter? DECIDE as a mob, then
    //   encode the decision as a test (Assert.Throws<T> is the tool).
    //   Every language answers this differently — .NET's own Stack<T>
    //   throws InvalidOperationException (and offers TryPop alongside),
    //   Python raises IndexError, JavaScript's Array.pop() quietly hands
    //   back undefined.

    // [edge] — the ambiguity that decision creates.
    //   Suppose you chose "return default(T) when empty". Now make a
    //   Stack<int>, push 0, and pop it. `default(int)` IS 0 — so can a
    //   caller tell an empty stack from a stack holding a zero? (Same trap
    //   with null on a Stack<string>.) Write the test that exposes it,
    //   then decide what to do about it — this is exactly what menu item
    //   (d) is for.

    // [negative] — the leak.
    //   Can a caller reach your internal list and mutate the stack behind
    //   its back? Try it: get whatever your public API hands out, change
    //   it, and then ask the stack what it contains. If the stack changed,
    //   you have a bug that no other test in this file would ever have
    //   caught. (If you implemented (e), check that path too — does the
    //   IEnumerable<T> you return expose the list itself?)

    // [edge] — what the compiler will NOT catch for you.
    //   The build already rejected half your negative tests — pushing the
    //   wrong type simply doesn't compile. But it has nothing to say about
    //   popping an empty stack, or about a property that hands out the
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
