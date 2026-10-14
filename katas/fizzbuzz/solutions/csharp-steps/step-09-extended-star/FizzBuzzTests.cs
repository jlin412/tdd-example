using Xunit;

namespace Kata;

// FizzBuzz kata — step by step, STEP 9: extended R7 — even numbers with no word print "*".
// Snapshot at the END of the step; walkthrough in ../README.md
public class FizzBuzzTests
{
    private readonly FizzBuzz _fizzBuzz = new();

    [Theory]
    [InlineData(1, "1")]
    [InlineData(7, "7")]
    [InlineData(11, "11")]
    public void NumbersWithNoWord(int n, string expected)  // [positive]
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
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(14)]
    [InlineData(16)]
    public void EvenNumbersWithNoWordReturnStar(int n)  // [positive] R7
        => Assert.Equal("*", _fizzBuzz.Convert(n));

    [Fact]
    public void AWordBeatsStar()  // [edge]
    {
        Assert.Equal("Fizz", _fizzBuzz.Convert(6));       // even, but ÷3
        Assert.Equal("Buzz", _fizzBuzz.Convert(10));      // even, but ÷5
        Assert.Equal("FizzBuzz", _fizzBuzz.Convert(30));  // even, but a word fires
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-15)]
    public void ValuesBelowOneAreRejected(int n)  // [edge]
        => Assert.Throws<ArgumentOutOfRangeException>(() => _fizzBuzz.Convert(n));

    // [negative] runtime type tests are unnecessary in C# — the compiler
    // rejects Convert("3") outright. See the kata README.

    [Fact]
    public void UsesInjectedRulesInsteadOfTheDefaults()  // rules engine
    {
        var evens = new FizzBuzz(new[] { new Rule(2, "Even") });
        Assert.Equal("Even", evens.Convert(4));
        Assert.Equal("3", evens.Convert(3)); // default rules not in play
    }

    // ── Test list — still to promote (step 10, R8: Fizz = the digits contain a 3) ──

    [Fact(Skip = "todo")]
    public void ThirteenContainsA3SoItPrintsFizz()
    {
    }

    [Fact(Skip = "todo")]
    public void TwentyThreeContainsA3SoItPrintsFizz()
    {
    }

    [Fact(Skip = "todo")]
    public void ThirtyThreeContainsA3SoItPrintsFizz()
    {
    }

    [Fact(Skip = "todo")]
    public void ThirtyFiveContainsA3AndIsAMultipleOf5SoItPrintsFizzBuzz()
    {
    }

    [Fact(Skip = "todo")]
    public void OneThirtyContainsA3AndIsAMultipleOf5SoItPrintsFizzBuzz()
    {
    }

    [Fact(Skip = "todo")]
    public void ThirtyTwoIsEvenButContainsA3SoItPrintsFizzNotStar()
    {
    }

    [Fact(Skip = "todo")]
    public void FiftyIsAMultipleOf5WithNo3SoItPrintsBuzz()
    {
    }
}
