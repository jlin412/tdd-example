using Xunit;

namespace Kata;

// FizzBuzz kata — reference test suite (classic R1–R6, one possible end state).
// This is what a mob's suite might look like after the whole session.
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

    [Fact]
    public void UsesInjectedRulesInsteadOfTheDefaults()  // rules engine
    {
        var evens = new FizzBuzz(new[] { new Rule(2, "Even") });
        Assert.Equal("Even", evens.Convert(4));
        Assert.Equal("3", evens.Convert(3)); // default rules not in play
    }
}
