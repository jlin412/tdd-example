package kata;

import java.util.List;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvSource;
import org.junit.jupiter.params.provider.ValueSource;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

// FizzBuzz kata — reference test suite (classic R1–R6, one possible end state).
// This is what a mob's suite might look like after the whole session.
class FizzBuzzTest {

    private final FizzBuzz fizzBuzz = new FizzBuzz();

    @ParameterizedTest
    @CsvSource({"1,1", "2,2", "4,4"})
    @DisplayName("[positive] non-multiples return the number")
    void nonMultiplesReturnTheNumber(int n, String expected) {
        assertEquals(expected, fizzBuzz.convert(n));
    }

    @ParameterizedTest
    @CsvSource({"3,Fizz", "9,Fizz", "5,Buzz", "20,Buzz", "15,FizzBuzz", "45,FizzBuzz"})
    @DisplayName("[positive]/[boundary] word rules")
    void wordRules(int n, String expected) {
        assertEquals(expected, fizzBuzz.convert(n));
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
}
