package kata;

import java.util.List;
import java.util.stream.Collectors;

// FizzBuzz kata — step by step, STEP 9: extended R7 — even numbers with no word print "*".
// Snapshot at the END of the step; walkthrough in ../README.md
//
// R7 adds a fallback: an even number that matched no word returns "*",
// expressed as the "nothing matched" branch of convert() so a Fizz/Buzz word
// always wins. Fizz is still the three times table here; step 10 changes that.
public class FizzBuzz {

    // Menu items (a)+(b) — rules as data, each rule a tiny value object
    // that decides for itself (Tell, Don't Ask).
    public record Rule(int divisor, String word) {
        public boolean matches(int n) {
            return n % divisor == 0;
        }
    }

    private static final List<Rule> RULES = List.of(
            new Rule(3, "Fizz"),
            new Rule(5, "Buzz")
    );

    private final List<Rule> rules;

    public FizzBuzz() {
        this(RULES);
    }

    // Menu item (d) — injectable rules (defensively copied).
    public FizzBuzz(List<Rule> rules) {
        this.rules = List.copyOf(rules);
    }

    public String convert(int n) {
        if (n < 1) {
            throw new IllegalArgumentException("convert expects n >= 1, got " + n);
        }
        String matched = rules.stream()
                .filter(rule -> rule.matches(n))
                .map(Rule::word)
                .collect(Collectors.joining());
        if (!matched.isEmpty()) {
            return matched;
        }
        return n % 2 == 0 ? "*" : Integer.toString(n); // R7: even fallback → "*"
    }
}
