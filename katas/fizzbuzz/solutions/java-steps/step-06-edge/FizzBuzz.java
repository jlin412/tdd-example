package kata;

// FizzBuzz kata — step by step, STEP 6: [edge] — 0 and negatives are out of the game.
// Snapshot at the END of the step; walkthrough in ../README.md
public class FizzBuzz {
    public String convert(int n) {
        if (n < 1) {
            throw new IllegalArgumentException("convert expects n >= 1, got " + n);
        }
        String matched = "";
        if (n % 3 == 0) {
            matched += "Fizz";
        }
        if (n % 5 == 0) {
            matched += "Buzz";
        }
        return matched.isEmpty() ? Integer.toString(n) : matched;
    }
}
