namespace Kata;

// FizzBuzz kata — step by step, STEP 7: [negative] — the compiler already refuses non-integers.
// Snapshot at the END of the step; walkthrough in ../README.md
public class FizzBuzz
{
    public string Convert(int n)
    {
        if (n < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(n), n, "Convert expects n >= 1");
        }

        var matched = "";
        if (n % 3 == 0)
        {
            matched += "Fizz";
        }
        if (n % 5 == 0)
        {
            matched += "Buzz";
        }
        return matched.Length == 0 ? n.ToString() : matched;
    }
}
