using Xunit;

namespace Kata;

// FizzBuzz kata — step by step, STEP 8a: pattern (a) RULES ENGINE — the rules become data.
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

    // [negative] runtime type tests are unnecessary in C# — the compiler
    // rejects Convert("3") outright. See the kata README.
}
