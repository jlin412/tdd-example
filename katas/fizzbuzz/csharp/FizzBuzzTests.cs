using Xunit;

namespace Kata;

// ════════════════════════════════════════════════════════════════════
// FizzBuzz kata — YOU write the tests.
// ════════════════════════════════════════════════════════════════════
// The story is in katas/fizzbuzz/FizzBuzzStory.md — read it first,
// examples and all.
//
// STEP 0 is the test LIST: before any code, name as many tests as the mob
// can think of. STEP 1 then works one of them through the loop:
//   RED      — write ONE failing test. Run it. Watch it fail for the
//              right reason before writing any production code.
//   GREEN    — write the MINIMUM code in FizzBuzz.cs that passes.
//   REFACTOR — on green only. Then loop.
//
// STEP 1's three steps are labelled inside the test. Every test after it is
// yours to invent — the prompts suggest WHAT to think about, not which exact
// cases to write.
//
// Test-type legend:  [positive] [boundary] [edge] [negative]
public class FizzBuzzTests
{
    // ═════════════════════════════════════════════════════════════════
    // STEP 0 · THE TEST LIST — do this BEFORE you write any code
    // ═════════════════════════════════════════════════════════════════
    // The first move in TDD isn't a test, it's a LIST of the tests you
    // want. As a mob, out loud, name as many as you can — go for QUANTITY
    // now and prune later. You are translating the story into test
    // language: every requirement you can state in English should become a
    // name on this list.
    //
    // A skipped Fact is a todo that `dotnet test` prints back to you.
    // Two to start you off:

    [Fact(Skip = "todo")]
    public void OneHasNoSpecialMeaningSoItPrints1()
    {
    }

    [Fact(Skip = "todo")]
    public void ThreeIsAMultipleOfThreeSoItPrintsFizz()
    {
    }

    // Now your turn — keep adding. Work through the story's example table
    // first, then push PAST it into what the table doesn't show. Aim for a
    // dozen or more before anyone touches production code, and cover all
    // four types:
    //   [positive] the ordinary cases — each word rule, on its own
    //   [boundary] the first number where BOTH rules fire at once
    //   [edge]     0, negatives — the corners the story leaves to you
    //   [negative] input the type system can't reject for you
    //
    // Then run `dotnet test`, read your list back, pick ONE, and turn it
    // into a real test at STEP 1. Drop the Skip as you promote it.
    //
    // (Keep the list alive: every time you think "what about…?" mid-session,
    //  add a skipped Fact instead of chasing it and losing your red.)

    // ── STEP 1 · [positive] · the plain case · the RED → GREEN → REFACTOR loop, worked ──
    // The assertion ships COMMENTED OUT — uncommenting it is your RED step.
    [Fact]
    public void ReturnsTheNumberAsStringWhenNothingSpecialApplies()
    {
        var fizzBuzz = new FizzBuzz();

        // 1. RED — agree your method name as a mob, put it in the assertion
        //    below, uncomment it, and run `dotnet test`; it won't even
        //    COMPILE (the class is empty). "Does not compile" is a legitimate
        //    first RED in a statically typed language. `Convert` here is just
        //    a PLACEHOLDER — rename it to whatever the mob chose, then stay
        //    consistent. (Heads up: the reference solution in solutions/ uses
        //    `Convert`, so another name stops it being a literal drop-in.)
        // Assert.Equal("1", fizzBuzz.Convert(1));

        // 2. GREEN — add that method with the MINIMUM to pass
        //    (`return "1";` is legitimate; let the next example force more).

        // 3. REFACTOR — generalize: replace the hard-coded answer with real
        //    logic, proven by more examples, e.g.
        //    Assert.Equal("2", fizzBuzz.Convert(2));
        //    Assert.Equal("7", fizzBuzz.Convert(7));
        //    Assert.Equal("11", fizzBuzz.Convert(11));
    }

    // ─────────────────────────────────────────────────────────────────
    // From here on, the tests are yours. ONE red at a time.
    // ─────────────────────────────────────────────────────────────────

    // [positive] — the first word rule. Promote it from your STEP 0 list.
    //   Which number proves it? Does one example convince you, or do you
    //   want a second (6? 9?) to force real logic instead of a hard-coded
    //   answer?

    // [positive] — the other word rule. Same questions.

    // [boundary] — a number where BOTH word rules fire at once.
    //   What is the FIRST such number? Why is that the interesting one?
    //   Does your production code join the words, or did you special-case it?

    // ═════════════════════════════════════════════════════════════════
    // CHECKPOINT #1 · REFACTOR — THE PATTERN MENU  (only when green)
    // ═════════════════════════════════════════════════════════════════
    // Housekeeping first (hygiene, not a pattern): NAMED CONSTANTS —
    // do 3, 5, "Fizz", "Buzz" deserve names?
    //
    // Then the menu. As a mob, DECIDE which ONE pattern to implement:
    //
    //   (a) RULES ENGINE — rules as DATA: the if/else chain becomes a
    //       table the code walks, joining every word whose divisor
    //       divides n. Payoff check: can you add a rule WITHOUT editing
    //       the method? (Every other item gets easier after this one.
    //       C# nuance: a table needs an element type — which quietly
    //       pulls in (b). Many C# mobs do (a)+(b) as one move.)
    //   (b) VALUE OBJECT (OO) — a tiny type per rule:
    //       record Rule(int Divisor, string Word) with a Matches(n)
    //       method. The rule DECIDES for itself ("Tell, Don't Ask") and
    //       the method reads as intent: rules.Where(r => r.Matches(n)).
    //   (c) STRATEGY / POLYMORPHISM — rules that DIFFER in kind: a
    //       Func<int, bool> predicate, or an IRule interface with
    //       implementations (DivisibilityRule, ContainsDigitRule…) —
    //       frees rules from divisibility ("contains a 3" → Fizz?).
    //       Strategy = composition, subclassing = inheritance — which
    //       wins, and why? (DDD calls this the Specification pattern.)
    //   (d) DEPENDENCY INJECTION — a constructor accepts the rules
    //       (the parameterless one delegates to sensible defaults), so
    //       tests can probe the engine with tiny rule sets (Open/Closed
    //       by config).
    //   (e) FACTORY — named constructors that hide the rule wiring:
    //       a static Classic() vs a custom preset. Callers stop caring how
    //       a FizzBuzz is assembled. And what should plain
    //       `new FizzBuzz()` mean once presets exist?
    //   (f) NULL OBJECT — make the n.ToString() fallback itself a rule
    //       that "always matches, says n". Then the method is JUST "run
    //       the rules". Trickier than it looks: it must fire ONLY when
    //       nothing else matched — can you express "last resort" without
    //       smuggling the conditional back in?
    //
    // COACH'S RULE — ONE PATTERN AT A TIME:
    //   implement the chosen pattern → all green → ask "did it pay for
    //   itself? do we want another?" → only then pick the next item.
    //   Never two patterns mid-flight. If a pattern changes the public
    //   API ((d), (e)), drive it TEST-FIRST like any feature: write the
    //   test that wishes the API existed, watch it go red, build it.
    //
    // DISCUSS: which patterns are RIGHT for this requirement — and which
    // are over-engineering? Patterns are options, not obligations.
    // (Weigh and likely DECLINE: BUILDER — is a three-item list really
    //  crying out for .WithRule(…)? CHAIN OF RESPONSIBILITY — FizzBuzz
    //  CONCATENATES matches while CoR crowns one winner: it fights the
    //  requirement. TEMPLATE METHOD — the inheritance twin of (c); why
    //  does modern code prefer composition?)
    //
    // Tests are code too: repeated setup to extract? Try [Theory] with
    // [InlineData] for the value tables.

    // ═════════════════════════════════════════════════════════════════
    // CHECKPOINT #2 · NEGATIVE & EDGE — pin the contract down
    // ═════════════════════════════════════════════════════════════════
    // Back to RED work: the corners the story left dark on purpose. Same
    // loop as ever — ONE failing test at a time.

    // [edge] The story says counting starts at one — so what about 0?
    //   Negative numbers? Careful: 0 % 3 == 0. What would naive code return
    //   for a call with 0? DECIDE as a mob which exception "refuse it" means
    //   (ArgumentOutOfRangeException is the convention), then encode the
    //   decision as tests (Assert.Throws<T> is the tool).

    // [negative] — already half-written for you, by the compiler:
    //   passing "3" or null simply won't compile. Try it —
    //   type a call like that and watch the build refuse. That's the
    //   type system acting as a test you didn't have to write. (Compare
    //   with the Python/JS versions of this kata, which need runtime
    //   type-guard tests here.) The VALUE checks above are still yours.

    // REFACTOR (on green):
    //   · GUARD CLAUSES — does validation read as one clean fail-fast
    //     gate at the top of the method, one early exit per bad shape?
    //   · Would a thrown message that NAMES the bad input help a caller?

    // ── WHAT NEXT · the Extended story ────────────────────────────────
    //   Shipped classic FizzBuzz? The requirements change in
    //   katas/fizzbuzz/FizzBuzzExtendedStory.md — work out what changed, and
    //   predict which of your tests will break BEFORE you run them.
}
