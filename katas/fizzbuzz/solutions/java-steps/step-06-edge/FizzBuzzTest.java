package kata;

import org.junit.jupiter.api.Disabled;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvSource;
import org.junit.jupiter.params.provider.ValueSource;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

// FizzBuzz kata — step by step, STEP 6: [edge] — 0 and negatives are out of the game.
// Snapshot at the END of the step; walkthrough in ../README.md
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

    // ── Test list — still to promote ──

    // [negative]
    @Test
    @Disabled("todo")
    @DisplayName("\"3\", 3.5 and null are rejected — or does the compiler do it for us?")
    void nonIntegerInputIsRejected() {
    }
}
