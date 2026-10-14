using System.Linq;

namespace Kata;

// FizzBuzz kata — step by step, STEP 9: extended R7 — even numbers with no word print "*".
// Snapshot at the END of the step; walkthrough in ../README.md
//
// R7 adds a fallback: an even number that matched no word returns "*",
// expressed as the "nothing matched" branch of Convert() so a Fizz/Buzz word
// always wins. Fizz is still the three times table here; step 10 changes that.

// Menu items (a)+(b) — rules as data, each rule a tiny value object
// that decides for itself (Tell, Don't Ask).
public record Rule(int Divisor, string Word)
{
    public bool Matches(int n) => n % Divisor == 0;
}

public class FizzBuzz
{
    private static readonly Rule[] Rules =
    {
        new(3, "Fizz"),
        new(5, "Buzz"),
    };

    private readonly IReadOnlyList<Rule> _rules;

    public FizzBuzz() : this(Rules)
    {
    }

    // Menu item (d) — injectable rules.
    public FizzBuzz(IReadOnlyList<Rule> rules) => _rules = rules;

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
