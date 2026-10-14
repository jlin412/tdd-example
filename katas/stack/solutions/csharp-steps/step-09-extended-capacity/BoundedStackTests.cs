using Xunit;

namespace Kata;

// Stack kata — step by step, STEP 9: extended R7 — a capacity fixed at creation.
// Snapshot at the END of the step; walkthrough in ../README.md
public class BoundedStackTests
{
    private static BoundedStack<T> BoundedStackOf<T>(int capacity, params T[] items)
    {
        var stack = new BoundedStack<T>(capacity);
        foreach (var item in items)
        {
            stack.Push(item);
        }

        return stack;
    }

    [Fact] // [positive] (R7)
    public void ReportsTheCapacityItWasCreatedWith()
    {
        Assert.Equal(3, new BoundedStack<string>(3).Capacity);
    }

    // Only the VALUE is testable here. `int capacity` already rules out 1.5
    // and "2" at compile time — the Python suite needs explicit tests for
    // those, this one does not.
    [Theory] // [negative]
    [InlineData(-1)]
    [InlineData(-42)]
    public void ANegativeCapacityIsRejectedAtConstruction(int capacity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new BoundedStack<string>(capacity));
    }

    [Fact] // [positive]
    public void IsStillLastInFirstOut()
    {
        var stack = BoundedStackOf(3, "a", "b", "c");
        Assert.Equal(new[] { "c", "b", "a" }, new[] { stack.Pop(), stack.Pop(), stack.Pop() });
    }

    [Fact] // [positive]
    public void StillPeeksWithoutRemoving()
    {
        var stack = BoundedStackOf(3, "a", "b");
        Assert.Equal("b", stack.Peek());
        Assert.Equal(2, stack.Size());
    }

    [Fact] // [edge] — the wrapped contract is unchanged
    public void StillThrowsOnAnEmptyPop()
    {
        var stack = new BoundedStack<string>(2);
        Assert.Throws<EmptyStackException>(() => stack.Pop());
    }

    [Fact] // [positive]
    public void IsStillEnumerableTopDown()
    {
        Assert.Equal(new[] { "c", "b", "a" }, BoundedStackOf(3, "a", "b", "c").ToArray());
    }

    // ── Test list — still to promote (step 10, R8: a full stack refuses more) ──

    // [R8]
    [Fact(Skip = "todo")]
    public void StartsEmptyAndNotFull()
    {
    }

    [Fact(Skip = "todo")]
    public void BecomesFullExactlyAtCapacity()
    {
    }

    [Fact(Skip = "todo")]
    public void ThrowsWhenPushingPastCapacity()
    {
    }

    [Fact(Skip = "todo")]
    public void DoesNotGrowWhenAPushIsRefused()
    {
    }

    [Fact(Skip = "todo")]
    public void AcceptsAPushAgainOnceSomethingIsPopped()
    {
    }

    [Fact(Skip = "todo")]
    public void CapacityZeroIsEmptyAndFullAtTheSameTime()
    {
    }

    [Fact(Skip = "todo")]
    public void CapacityZeroRefusesTheVeryFirstPush()
    {
    }
}
