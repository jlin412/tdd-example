using System;
using System.Linq;

namespace Kata;

// FizzBuzz kata — reference solution (extended: R1–R6 with R7 + R8).
// See FizzBuzzExtendedStory.md for the requirement.
//
// R8 replaces R2: Fizz fires when the decimal digits CONTAIN a '3' (not
// divisibility), so a rule is now a PREDICATE, not a (divisor, word) pair —
// menu item (c) STRATEGY, which the classic solution declined as YAGNI but
// this requirement genuinely needs.
//
// R7 adds a fallback: an even number that matched no word returns "*",
// expressed as the "nothing matched" branch of Convert() so a Fizz/Buzz word
// always wins.
public class FizzBuzzExtended
{
    public record Rule(Func<int, bool> Matches, string Word);

    private static readonly Rule[] Rules =
    {
        new(n => n.ToString().Contains('3'), "Fizz"), // R8: digit 3
        new(n => n % 5 == 0, "Buzz"),
    };

    private readonly IReadOnlyList<Rule> _rules;

    public FizzBuzzExtended() : this(Rules)
    {
    }

    public FizzBuzzExtended(IReadOnlyList<Rule> rules) => _rules = rules;

    public string Convert(int n)
    {
        if (n < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(n), n, "Convert expects n >= 1");
        }

        var matched = string.Concat(
            _rules.Where(rule => rule.Matches(n)).Select(rule => rule.Word));
        if (matched.Length > 0)
        {
            return matched;
        }

        return n % 2 == 0 ? "*" : n.ToString(); // R7: even fallback → "*"
    }
}
