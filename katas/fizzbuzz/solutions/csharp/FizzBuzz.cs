using System.Linq;

namespace Kata;

// FizzBuzz kata — reference solution (classic R1–R6).
//
// One possible end state. From the checkpoint-#1 pattern menu this team took
// (a) RULES ENGINE, (b) VALUE OBJECT and (d) DEPENDENCY INJECTION — added ONE
// at a time, per the coach's rule, with all tests green between patterns.
//
// See FizzBuzzStory.md for the requirement. The R7/R8 variant lives in
// FizzBuzzExtended.cs (FizzBuzzExtendedStory.md).

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
        return matched.Length == 0 ? n.ToString() : matched;
    }
}
