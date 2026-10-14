using Xunit;

namespace Kata;

// Stack kata — reference test suite (extended: R7 + R8, a bounded stack).
// See StackExtendedStory.md for the requirement.
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

    [Fact] // [positive] (R7)
    public void StartsEmptyAndNotFull()
    {
        var stack = new BoundedStack<string>(2);
        Assert.True(stack.IsEmpty());
        Assert.False(stack.IsFull());
    }

    [Fact] // [positive] (R7)
    public void BecomesFullExactlyAtCapacity()
    {
        var stack = BoundedStackOf(2, "a");
        Assert.False(stack.IsFull());
        stack.Push("b");
        Assert.True(stack.IsFull());
    }

    [Fact] // [edge] (R8)
    public void ThrowsWhenPushingPastCapacity()
    {
        var stack = BoundedStackOf(2, "a", "b");
        Assert.Throws<FullStackException>(() => stack.Push("c"));
    }

    [Fact] // [edge] (R8)
    public void DoesNotGrowWhenAPushIsRefused()
    {
        var stack = BoundedStackOf(2, "a", "b");
        Assert.Throws<FullStackException>(() => stack.Push("c"));
        Assert.Equal(2, stack.Size());
        Assert.Equal("b", stack.Peek());
    }

    [Fact] // [edge] (R8)
    public void AcceptsAPushAgainOnceSomethingIsPopped()
    {
        var stack = BoundedStackOf(2, "a", "b");
        Assert.Equal("b", stack.Pop());
        Assert.False(stack.IsFull());
        stack.Push("c");
        Assert.Equal("c", stack.Peek());
        Assert.Equal(2, stack.Size());
    }

    [Fact] // [boundary] — capacity 0 is legal, and full from birth
    public void CapacityZeroIsEmptyAndFullAtTheSameTime()
    {
        var stack = new BoundedStack<string>(0);
        Assert.True(stack.IsEmpty());
        Assert.True(stack.IsFull());
    }

    [Fact] // [boundary]
    public void CapacityZeroRefusesTheVeryFirstPush()
    {
        var stack = new BoundedStack<string>(0);
        Assert.Throws<FullStackException>(() => stack.Push("a"));
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
}
