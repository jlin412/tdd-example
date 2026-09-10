package kata;

import java.util.List;
import java.util.function.Predicate;
import java.util.stream.Collectors;

/**
 * FizzBuzz kata — reference solution (extended: R1–R6 with R7 + R8).
 * See FizzBuzzExtendedStory.md for the requirement.
 *
 * R8 replaces R2: Fizz fires when the decimal digits CONTAIN a '3' (not
 * divisibility), so a rule is now a PREDICATE, not a (divisor, word) pair —
 * menu item (c) STRATEGY, which the classic solution declined as YAGNI but
 * this requirement genuinely needs.
 *
 * R7 adds a fallback: an even number that matched no word returns "*",
 * expressed as the "nothing matched" branch of convert() so a Fizz/Buzz word
 * always wins.
 */
public class FizzBuzzExtended {

    public record Rule(Predicate<Integer> predicate, String word) {
        public boolean matches(int n) {
            return predicate.test(n);
        }
    }

    private static final List<Rule> RULES = List.of(
            new Rule(n -> Integer.toString(n).contains("3"), "Fizz"), // R8: digit 3
            new Rule(n -> n % 5 == 0, "Buzz")
    );

    private final List<Rule> rules;

    public FizzBuzzExtended() {
        this(RULES);
    }

    public FizzBuzzExtended(List<Rule> rules) {
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
