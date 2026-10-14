package kata;

import java.util.List;

import org.junit.jupiter.api.Disabled;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvSource;
import org.junit.jupiter.params.provider.ValueSource;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

// FizzBuzz kata — step by step, STEP 9: extended R7 — even numbers with no word print "*".
// Snapshot at the END of the step; walkthrough in ../README.md
class FizzBuzzTest {

    private final FizzBuzz fizzBuzz = new FizzBuzz();

    @ParameterizedTest
    @CsvSource({"1,1", "7,7", "11,11"})
    @DisplayName("[positive] numbers with no word return the number")
    void numbersWithNoWord(int n, String expected) {
        assertEquals(expected, fizzBuzz.convert(n));
    }

    @ParameterizedTest
    @CsvSource({"3,Fizz", "9,Fizz", "5,Buzz", "20,Buzz", "15,FizzBuzz", "45,FizzBuzz"})
    @DisplayName("[positive]/[boundary] word rules")
    void wordRules(int n, String expected) {
        assertEquals(expected, fizzBuzz.convert(n));
    }

    @ParameterizedTest
    @ValueSource(ints = {2, 4, 8, 14, 16})
    @DisplayName("[positive] R7 — even numbers with no word return \"*\"")
    void evenNumbersWithNoWordReturnStar(int n) {
        assertEquals("*", fizzBuzz.convert(n));
    }

    @Test
    @DisplayName("[edge] a word beats \"*\"")
    void aWordBeatsStar() {
        assertEquals("Fizz", fizzBuzz.convert(6));       // even, but ÷3
        assertEquals("Buzz", fizzBuzz.convert(10));      // even, but ÷5
        assertEquals("FizzBuzz", fizzBuzz.convert(30));  // even, but a word fires
    }

    @ParameterizedTest
    @ValueSource(ints = {0, -1, -15})
    @DisplayName("[edge] contract: n must be >= 1")
    void valuesBelowOneAreRejected(int n) {
        assertThrows(IllegalArgumentException.class, () -> fizzBuzz.convert(n));
    }

    // [negative] runtime type tests are unnecessary in Java — the
    // compiler rejects convert("3") outright. See the kata README.

    @Test
    @DisplayName("rules engine: custom rules are injectable")
    void usesInjectedRulesInsteadOfTheDefaults() {
        FizzBuzz evens = new FizzBuzz(List.of(new FizzBuzz.Rule(2, "Even")));
        assertEquals("Even", evens.convert(4));
        assertEquals("3", evens.convert(3)); // default rules not in play
    }

    // ── Test list — still to promote (step 10, R8: Fizz = the digits contain a 3) ──

    @Test
    @Disabled("todo")
    @DisplayName("13 contains a 3, so it prints \"Fizz\"")
    void thirteenContainsA3() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("23 contains a 3, so it prints \"Fizz\"")
    void twentyThreeContainsA3() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("33 contains a 3, so it prints \"Fizz\"")
    void thirtyThreeContainsA3() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("35 contains a 3 and is a multiple of 5, so it prints \"FizzBuzz\"")
    void thirtyFiveContainsA3AndIsAMultipleOf5() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("130 contains a 3 and is a multiple of 5, so it prints \"FizzBuzz\"")
    void oneThirtyContainsA3AndIsAMultipleOf5() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("32 is even but contains a 3, so it prints \"Fizz\", not \"*\"")
    void thirtyTwoIsEvenButContainsA3() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("50 is a multiple of 5 with no 3 in it, so it prints \"Buzz\"")
    void fiftyIsAMultipleOf5WithNo3() {
    }
}
