using Xunit;

namespace Kata;

// FizzBuzz kata — reference test suite (extended: R1–R6 with R7 + R8).
// See FizzBuzzExtendedStory.md for the requirement.
public class FizzBuzzExtendedTests
{
    private readonly FizzBuzzExtended _fizzBuzz = new();

    [Theory]
    [InlineData(1, "1")]
    [InlineData(7, "7")]
    [InlineData(9, "9")]
    [InlineData(11, "11")]
    public void NumbersWithNoWord(int n, string expected)  // [positive]
        => Assert.Equal(expected, _fizzBuzz.Convert(n));

    [Theory]
    [InlineData(3)]
    [InlineData(13)]
    [InlineData(23)]
    [InlineData(33)]
    public void FizzWhenDigitsContainA3(int n)  // [positive] R8
        => Assert.Equal("Fizz", _fizzBuzz.Convert(n));

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    public void BuzzMultiplesOfFive(int n)  // [positive]
        => Assert.Equal("Buzz", _fizzBuzz.Convert(n));

    [Theory]
    [InlineData(30)]
    [InlineData(35)]
    [InlineData(130)]
    public void FizzBuzzContains3AndMultipleOf5(int n)  // [boundary]
        => Assert.Equal("FizzBuzz", _fizzBuzz.Convert(n));

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
        Assert.Equal("Fizz", _fizzBuzz.Convert(32));      // even, but contains a 3
        Assert.Equal("Buzz", _fizzBuzz.Convert(10));      // even, but ÷5
        Assert.Equal("FizzBuzz", _fizzBuzz.Convert(30));  // even, but a word fires
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-15)]
    public void ValuesBelowOneAreRejected(int n)  // [edge]
        => Assert.Throws<ArgumentOutOfRangeException>(() => _fizzBuzz.Convert(n));

    [Fact]
    public void UsesInjectedRulesInsteadOfTheDefaults()  // rules engine
    {
        var evens = new FizzBuzzExtended(new[] { new FizzBuzzExtended.Rule(n => n % 2 == 0, "Even") });
        Assert.Equal("Even", evens.Convert(4));
        Assert.Equal("3", evens.Convert(3)); // default rules not in play; 3 is odd → itself
    }
}
