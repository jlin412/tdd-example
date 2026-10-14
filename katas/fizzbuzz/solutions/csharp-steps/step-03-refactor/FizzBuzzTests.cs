using Xunit;

namespace Kata;

// FizzBuzz kata — step by step, STEP 3: REFACTOR — generalize the hard-coded answer.
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

    // ── Test list — still to promote ──

    // [positive]
    [Fact(Skip = "todo")]
    public void ThreeIsAMultipleOfThreeSoItPrintsFizz()
    {
    }

    [Fact(Skip = "todo")]
    public void NineIsABiggerMultipleOfThreeSoItStillPrintsFizz()
    {
    }

    [Fact(Skip = "todo")]
    public void FiveIsAMultipleOfFiveSoItPrintsBuzz()
    {
    }

    [Fact(Skip = "todo")]
    public void TwentyIsABiggerMultipleOfFiveSoItStillPrintsBuzz()
    {
    }

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
