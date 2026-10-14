package kata;

import org.junit.jupiter.api.Disabled;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvSource;

import static org.junit.jupiter.api.Assertions.assertEquals;

// FizzBuzz kata — step by step, STEP 4: the other [positive] cases — each word rule on its own.
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
    @CsvSource({"3,Fizz", "9,Fizz", "5,Buzz", "20,Buzz"})
    @DisplayName("[positive] word rules")
    void wordRules(int n, String expected) {
        assertEquals(expected, fizzBuzz.convert(n));
    }

    // ── Test list — still to promote ──

    // [boundary]
    @Test
    @Disabled("todo")
    @DisplayName("15 is the first multiple of both, so it prints \"FizzBuzz\" — Fizz first")
    void fifteenIsTheFirstMultipleOfBoth() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("45 is another multiple of both, so it prints \"FizzBuzz\"")
    void fortyFiveIsAnotherMultipleOfBoth() {
    }

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
