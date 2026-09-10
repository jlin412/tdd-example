package kata;

import org.junit.jupiter.api.Disabled;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

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
//   GREEN    — write the MINIMUM code in FizzBuzz.java that passes.
//   REFACTOR — on green only. Then loop.
//
// STEP 1's three steps are labelled inside the test. Every test after it is
// yours to invent — the prompts suggest WHAT to think about, not which exact
// cases to write.
//
// Test-type legend:  [positive] [boundary] [edge] [negative]
class FizzBuzzTest {

    // ═════════════════════════════════════════════════════════════════
    // STEP 0 · THE TEST LIST — do this BEFORE you write any code
    // ═════════════════════════════════════════════════════════════════
    // The first move in TDD isn't a test, it's a LIST of the tests you
    // want. As a mob, out loud, name as many as you can — go for QUANTITY
    // now and prune later. You are translating the story into test
    // language: every requirement you can state in English should become a
    // name on this list.
    //
    // A @Disabled test is a todo that `mvn test` prints back to you as
    // skipped. Two to start you off:

    @Test
    @Disabled("todo")
    @DisplayName("1 has no special meaning, so it prints \"1\"")
    void oneHasNoSpecialMeaning() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("3 is a multiple of three, so it prints \"Fizz\"")
    void threeIsAMultipleOfThree() {
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
    // Then run `mvn test`, read your list back, pick ONE, and turn it into
    // a real test at STEP 1. Drop the @Disabled as you promote it.
    //
    // (Keep the list alive: every time you think "what about…?" mid-session,
    //  add a @Disabled test instead of chasing it and losing your red.)

    // ── STEP 1 · [positive] · the plain case · the RED → GREEN → REFACTOR loop, worked ──
    // The assertion ships COMMENTED OUT — uncommenting it is your RED step.
    @Test
    void returnsTheNumberAsStringWhenNothingSpecialApplies() {
        FizzBuzz fizzBuzz = new FizzBuzz();

        // 1. RED — agree your method name as a mob, put it in the assertion
        //    below, uncomment it, and run `mvn test`; it won't even COMPILE
        //    (the class is empty). In a statically typed language, "does not
        //    compile" is a valid RED. `convert` here is just a PLACEHOLDER —
        //    rename it to whatever the mob chose, then stay consistent.
        //    (Heads up: the reference solution in solutions/ uses `convert`,
        //     so another name stops it being a literal drop-in.)
        // assertEquals("1", fizzBuzz.convert(1));

        // 2. GREEN — add that method with the MINIMUM to pass
        //    (`return "1";` is legitimate; let the next example force more).

        // 3. REFACTOR — generalize: replace the hard-coded answer with real
        //    logic, proven by more examples, e.g.
        //    assertEquals("2", fizzBuzz.convert(2));
        //    assertEquals("7", fizzBuzz.convert(7));
        //    assertEquals("11", fizzBuzz.convert(11));
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
    //       Java nuance: a table needs an element type — which quietly
    //       pulls in (b). Many Java mobs do (a)+(b) as one move.)
    //   (b) VALUE OBJECT (OO) — a tiny type per rule:
    //       record Rule(int divisor, String word) with a matches(n)
    //       method. The rule DECIDES for itself ("Tell, Don't Ask") and
    //       the method reads as intent: rules.stream().filter(r -> r.matches(n)).
    //   (c) STRATEGY / POLYMORPHISM — rules that DIFFER in kind: a
    //       Predicate<Integer> field, or a Rule interface with
    //       implementations (DivisibilityRule, ContainsDigitRule…) —
    //       frees rules from divisibility ("contains a 3" → Fizz?).
    //       Strategy = composition, subclassing = inheritance — which
    //       wins, and why? (DDD calls this the Specification pattern.)
    //   (d) DEPENDENCY INJECTION — a constructor accepts List<Rule>
    //       (the no-arg constructor delegates to sensible defaults), so
    //       tests can probe the engine with tiny rule sets (Open/Closed
    //       by config).
    //   (e) FACTORY — named constructors that hide the rule wiring:
    //       a static classic() vs a custom preset. Callers stop caring how
    //       a FizzBuzz is assembled. And what should plain
    //       `new FizzBuzz()` mean once presets exist?
    //   (f) NULL OBJECT — make the Integer.toString(n) fallback itself a
    //       rule that "always matches, says n". Then the method is JUST
    //       "run the rules". Trickier than it looks: it must fire ONLY
    //       when nothing else matched — can you express "last resort"
    //       without smuggling the conditional back in?
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
    //  crying out for .withRule(…)? CHAIN OF RESPONSIBILITY — FizzBuzz
    //  CONCATENATES matches while CoR crowns one winner: it fights the
    //  requirement. TEMPLATE METHOD — the inheritance twin of (c); why
    //  does modern code prefer composition?)
    //
    // Tests are code too: repeated setup to extract? Try
    // @ParameterizedTest with @CsvSource for the value tables.

    // ═════════════════════════════════════════════════════════════════
    // CHECKPOINT #2 · NEGATIVE & EDGE — pin the contract down
    // ═════════════════════════════════════════════════════════════════
    // Back to RED work: the corners the story left dark on purpose. Same
    // loop as ever — ONE failing test at a time.

    // [edge] The story says counting starts at one — so what about 0?
    //   Negative numbers? Careful: 0 % 3 == 0. What would naive code return
    //   for a call with 0? DECIDE as a mob which exception "refuse it" means
    //   (IllegalArgumentException is the convention), then encode the
    //   decision as tests (assertThrows is the tool).

    // [negative] — already half-written for you, by the compiler:
    //   passing "3" or null simply won't compile. Try it —
    //   uncomment a call like that and watch javac refuse. That's the
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
