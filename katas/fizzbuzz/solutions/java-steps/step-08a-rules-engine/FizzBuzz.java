package kata;

import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

// FizzBuzz kata — step by step, STEP 8a: pattern (a) RULES ENGINE — the rules become data.
// Snapshot at the END of the step; walkthrough in ../README.md
public class FizzBuzz {

    private static final List<Map.Entry<Integer, String>> RULES = List.of(
            Map.entry(3, "Fizz"),
            Map.entry(5, "Buzz")
    );

    public String convert(int n) {
        if (n < 1) {
            throw new IllegalArgumentException("convert expects n >= 1, got " + n);
        }
        String matched = RULES.stream()
                .filter(rule -> n % rule.getKey() == 0)
                .map(Map.Entry::getValue)
                .collect(Collectors.joining());
        return matched.isEmpty() ? Integer.toString(n) : matched;
    }
}
