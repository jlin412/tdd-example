using Xunit;

namespace Kata;

// FizzBuzz kata — step by step, STEP 4: the other [positive] cases — each word rule on its own.
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
    public void WordRules(int n, string expected)  // [positive]
        => Assert.Equal(expected, _fizzBuzz.Convert(n));

    // ── Test list — still to promote ──

    // [boundary]
    [Fact(Skip = "todo")]
    public void FifteenIsTheFirstMultipleOfBothSoItPrintsFizzBuzz()
    {
    }

    [Fact(Skip = "todo")]
    public void FortyFiveIsAnotherMultipleOfBothSoItPrintsFizzBuzz()
    {
    }

    // [edge]
    [Fact(Skip = "todo")]
    public void ZeroIsOutOfTheGameEvenThoughZeroModThreeIsZero()
    {
    }

    [Fact(Skip = "todo")]
    public void MinusOneIsNegativeSoItIsRejected()
    {
    }

    [Fact(Skip = "todo")]
    public void MinusFifteenIsRejectedEvenThoughItIsAMultipleOfBoth()
    {
    }

    // [negative]
    [Fact(Skip = "todo")]
    public void NonIntegerInputIsRejected()  // "3", 3.5, null — or does the compiler do it for us?
    {
    }
}
