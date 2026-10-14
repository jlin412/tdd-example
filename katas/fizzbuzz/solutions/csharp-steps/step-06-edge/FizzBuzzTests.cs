using Xunit;

namespace Kata;

// FizzBuzz kata — step by step, STEP 6: [edge] — 0 and negatives are out of the game.
// Snapshot at the END of the step; walkthrough in ../README.md
public class FizzBuzzTests
{
    private readonly FizzBuzz _fizzBuzz = new();

    [Theory]
    [InlineData(1, "1")]
    [InlineData(2, "2")]
    [InlineData(4, "4")]
    public void NonMultiplesReturnTheNumber(int n, string expected)  // [positive]
        => Assert.Equal(expected, _fizzBuzz.Convert(n));

    [Theory]
    [InlineData(3, "Fizz")]
    [InlineData(9, "Fizz")]
    [InlineData(5, "Buzz")]
    [InlineData(20, "Buzz")]
    [InlineData(15, "FizzBuzz")]
    [InlineData(45, "FizzBuzz")]
    public void WordRules(int n, string expected)  // [positive]/[boundary]
        => Assert.Equal(expected, _fizzBuzz.Convert(n));

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-15)]
    public void ValuesBelowOneAreRejected(int n)  // [edge]
        => Assert.Throws<ArgumentOutOfRangeException>(() => _fizzBuzz.Convert(n));

    // ── Test list — still to promote ──

    // [negative]
    [Fact(Skip = "todo")]
    public void NonIntegerInputIsRejected()  // "3", 3.5, null — or does the compiler do it for us?
    {
    }
}
