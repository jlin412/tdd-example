package kata;

import java.util.List;
import java.util.stream.Collectors;

// FizzBuzz kata — step by step, STEP 8b: pattern (b) VALUE OBJECT — each rule decides for itself.
// Snapshot at the END of the step; walkthrough in ../README.md
public class FizzBuzz {

    private record Rule(int divisor, String word) {
        boolean matches(int n) {
            return n % divisor == 0;
        }
    }

    private static final List<Rule> RULES = List.of(
            new Rule(3, "Fizz"),
            new Rule(5, "Buzz")
    );

    public String convert(int n) {
        if (n < 1) {
            throw new IllegalArgumentException("convert expects n >= 1, got " + n);
        }
        String matched = RULES.stream()
                .filter(rule -> rule.matches(n))
                .map(Rule::word)
                .collect(Collectors.joining());
        return matched.isEmpty() ? Integer.toString(n) : matched;
    }
}
