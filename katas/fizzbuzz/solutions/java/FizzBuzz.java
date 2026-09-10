package kata;

import java.util.List;
import java.util.stream.Collectors;

/**
 * FizzBuzz kata — reference solution (classic R1–R6).
 *
 * One possible end state. From the checkpoint-#1 pattern menu this team took
 * (a) RULES ENGINE, (b) VALUE OBJECT and (d) DEPENDENCY INJECTION — added ONE
 * at a time, per the coach's rule, with all tests green between patterns.
 *
 * See FizzBuzzStory.md for the requirement. The R7/R8 variant lives in
 * FizzBuzzExtended.java (FizzBuzzExtendedStory.md).
 */
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
        return matched.isEmpty() ? Integer.toString(n) : matched;
    }
}
