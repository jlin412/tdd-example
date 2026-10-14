using System.Linq;

namespace Kata;

// FizzBuzz kata — step by step, STEP 8a: pattern (a) RULES ENGINE — the rules become data.
// Snapshot at the END of the step; walkthrough in ../README.md
public class FizzBuzz
{
    private static readonly (int Divisor, string Word)[] Rules =
    {
        (3, "Fizz"),
        (5, "Buzz"),
    };

    public string Convert(int n)
    {
        if (n < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(n), n, "Convert expects n >= 1");
        }

        var matched = string.Concat(
            Rules.Where(rule => n % rule.Divisor == 0).Select(rule => rule.Word));
        return matched.Length == 0 ? n.ToString() : matched;
    }
}
