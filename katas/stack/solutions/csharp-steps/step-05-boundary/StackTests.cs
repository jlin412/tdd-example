using Xunit;

namespace Kata;

// Stack kata — step by step, STEP 5: [boundary] — last in, first out; size; as many items as you push.
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

    [Fact] // [edge]
    public void ANewStackHasSize0()
    {
        Assert.Equal(0, new Stack().Size());
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

    // Two items is the SMALLEST case that can tell a stack from a queue —
    // but only a second pop proves the order (see the walkthrough, step 5).
    [Fact] // [boundary]
    public void PopsTheMostRecentlyPushedItemFirst()
    {
        Assert.Equal("b", StackOf("a", "b").Pop());
    }

    [Fact] // [boundary]
    public void PopsTheRestInReverseOrderOfPushing()
    {
        var stack = StackOf("a", "b", "c");
        Assert.Equal(new object?[] { "c", "b", "a" }, new[] { stack.Pop(), stack.Pop(), stack.Pop() });
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

    [Fact] // [boundary]
    public void SizeTracksTheWholeSequenceNotJustThePushes()
    {
        var stack = new Stack();
        Assert.Equal(0, stack.Size());
        stack.Push("a");
        Assert.Equal(1, stack.Size());
        stack.Push("b");
        Assert.Equal(2, stack.Size());
        stack.Pop();
        Assert.Equal(1, stack.Size());
    }

    // The story never sets a limit, and a C# array has a fixed length.
    // This is the test that makes the stack grow its own array.
    [Fact] // [boundary]
    public void HoldsAsManyItemsAsYouPushAndGivesThemBackInReverse()
    {
        var stack = new Stack();
        for (var i = 0; i < 100; i++)
        {
            stack.Push(i);
        }

        Assert.Equal(100, stack.Size());
        for (var i = 99; i >= 0; i--)
        {
            Assert.Equal(i, stack.Pop());
        }

        Assert.True(stack.IsEmpty());
    }

    // ── Test list — still to promote ──

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
