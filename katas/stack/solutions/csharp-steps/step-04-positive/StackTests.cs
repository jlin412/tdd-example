using Xunit;

namespace Kata;

// Stack kata — step by step, STEP 4: the other [positive] cases — push/pop, peek, size.
// Snapshot at the END of the step; walkthrough in ../README.md
public class StackTests
{
    // The arrange helper the mob extracts once the setup starts repeating.
    private static Stack StackOf(params object?[] items)
    {
        var stack = new Stack();
        foreach (var item in items)
        {
            stack.Push(item);
        }

        return stack;
    }

    [Fact] // [edge]
    public void ANewStackIsEmpty()
    {
        Assert.True(new Stack().IsEmpty());
    }

    [Fact] // [positive]
    public void IsNotEmptyAfterAPush()
    {
        Assert.False(StackOf("a").IsEmpty());
    }

    [Fact] // [positive]
    public void PushThenPopHandsBackTheItemThatWasPushed()
    {
        Assert.Equal("a", StackOf("a").Pop());
    }

    [Fact] // [positive]
    public void IsEmptyAgainOnceTheOnlyItemIsPopped()
    {
        var stack = StackOf("a");
        stack.Pop();
        Assert.True(stack.IsEmpty());
    }

    [Fact] // [positive]
    public void PeekReturnsTheTopItem()
    {
        Assert.Equal("b", StackOf("a", "b").Peek());
    }

    [Fact] // [positive]
    public void PeekDoesNotRemoveItSizeUnchangedAndPeekingTwiceAgrees()
    {
        var stack = StackOf("a", "b");
        Assert.Equal("b", stack.Peek());
        Assert.Equal("b", stack.Peek());
        Assert.Equal(2, stack.Size());
    }

    // ── Test list — still to promote ──

    // [boundary]
    [Fact(Skip = "todo")]
    public void ABrandNewStackHasSize0()
    {
    }

    [Fact(Skip = "todo")]
    public void AfterPushingAThenBPopReturnsB()
    {
    }

    [Fact(Skip = "todo")]
    public void PoppingThreeTimesReturnsTheItemsInReverse()
    {
    }

    [Fact(Skip = "todo")]
    public void SizeGrowsOnPushAndShrinksOnPop()
    {
    }

    [Fact(Skip = "todo")]
    public void AStackHoldsAsManyItemsAsYouPush()
    {
    }

    // [edge]
    [Fact(Skip = "todo")]
    public void PoppingOrPeekingAnEmptyStackThrows()
    {
    }

    [Fact(Skip = "todo")]
    public void PoppingAgainAfterTheStackIsEmptiedThrows()
    {
    }

    [Fact(Skip = "todo")]
    public void AnEmptyPopIsCatchableAsAnInvalidOperationException()
    {
    }

    [Fact(Skip = "todo")]
    public void NullCanBePushedAndPoppedWithoutMeaningEmpty()
    {
    }

    [Fact(Skip = "todo")]
    public void ZeroCanBePushedAndPoppedWithoutMeaningEmpty()
    {
    }

    // [negative]
    [Fact(Skip = "todo")]
    public void TheBackingArrayCannotBeTamperedWithFromOutside()
    {
    }
}
