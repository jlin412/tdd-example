using System.Linq;

namespace Kata;

// FizzBuzz kata — step by step, STEP 8b: pattern (b) VALUE OBJECT — each rule decides for itself.
// Snapshot at the END of the step; walkthrough in ../README.md
public class FizzBuzz
{
    private record Rule(int Divisor, string Word)
    {
        public bool Matches(int n) => n % Divisor == 0;
    }

    private static readonly Rule[] Rules =
    {
        new(3, "Fizz"),
        new(5, "Buzz"),
    };

    public string Convert(int n)
    {
        if (n < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(n), n, "Convert expects n >= 1");
        }

        var matched = string.Concat(
            Rules.Where(rule => rule.Matches(n)).Select(rule => rule.Word));
        return matched.Length == 0 ? n.ToString() : matched;
    }
}
