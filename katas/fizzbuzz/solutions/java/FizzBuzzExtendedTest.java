package kata;

import java.util.List;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvSource;
import org.junit.jupiter.params.provider.ValueSource;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

// FizzBuzz kata — reference test suite (extended: R1–R6 with R7 + R8).
// See FizzBuzzExtendedStory.md for the requirement.
class FizzBuzzExtendedTest {

    private final FizzBuzzExtended fizzBuzz = new FizzBuzzExtended();

    @ParameterizedTest
    @CsvSource({"1,1", "7,7", "9,9", "11,11"})
    @DisplayName("[positive] numbers with no word return the number")
    void numbersWithNoWord(int n, String expected) {
        assertEquals(expected, fizzBuzz.convert(n));
    }

    @ParameterizedTest
    @ValueSource(ints = {3, 13, 23, 33})
    @DisplayName("[positive] R8 — Fizz = digits contain a 3")
    void fizzWhenDigitsContainA3(int n) {
        assertEquals("Fizz", fizzBuzz.convert(n));
    }

    @ParameterizedTest
    @ValueSource(ints = {5, 10, 20, 50})
    @DisplayName("[positive] Buzz = multiples of 5")
    void buzzMultiplesOfFive(int n) {
        assertEquals("Buzz", fizzBuzz.convert(n));
    }

    @ParameterizedTest
    @ValueSource(ints = {30, 35, 130})
    @DisplayName("[boundary] FizzBuzz = contains a 3 AND multiple of 5")
    void fizzBuzzContains3AndMultipleOf5(int n) {
        assertEquals("FizzBuzz", fizzBuzz.convert(n));
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
        assertEquals("Fizz", fizzBuzz.convert(32));      // even, but contains a 3
        assertEquals("Buzz", fizzBuzz.convert(10));      // even, but ÷5
        assertEquals("FizzBuzz", fizzBuzz.convert(30));  // even, but a word fires
    }

    @ParameterizedTest
    @ValueSource(ints = {0, -1, -15})
    @DisplayName("[edge] contract: n must be >= 1")
    void valuesBelowOneAreRejected(int n) {
        assertThrows(IllegalArgumentException.class, () -> fizzBuzz.convert(n));
    }

    @Test
    @DisplayName("rules engine: custom predicate rules are injectable")
    void usesInjectedRulesInsteadOfTheDefaults() {
        FizzBuzzExtended evens =
                new FizzBuzzExtended(List.of(new FizzBuzzExtended.Rule(n -> n % 2 == 0, "Even")));
        assertEquals("Even", evens.convert(4));
        assertEquals("3", evens.convert(3)); // default rules not in play; 3 is odd → itself
    }
}
