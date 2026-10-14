package kata;

import org.junit.jupiter.api.Disabled;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvSource;

import static org.junit.jupiter.api.Assertions.assertEquals;

// FizzBuzz kata — step by step, STEP 5: [boundary] — both word rules fire at once.
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

    // ── Test list — still to promote ──

    // [edge]
    @Test
    @Disabled("todo")
    @DisplayName("0 is out of the game (counting starts at one), though 0 % 3 == 0")
    void zeroIsOutOfTheGame() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("-1 is negative, so it is rejected")
    void minusOneIsRejected() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("-15 is negative, so it is rejected even though it is a multiple of both")
    void minusFifteenIsRejected() {
    }

    // [negative]
    @Test
    @Disabled("todo")
    @DisplayName("\"3\", 3.5 and null are rejected — or does the compiler do it for us?")
    void nonIntegerInputIsRejected() {
    }
}
