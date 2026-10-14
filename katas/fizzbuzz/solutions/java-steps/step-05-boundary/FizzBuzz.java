package kata;

// FizzBuzz kata — step by step, STEP 5: [boundary] — both word rules fire at once.
// Snapshot at the END of the step; walkthrough in ../README.md
public class FizzBuzz {
    public String convert(int n) {
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
