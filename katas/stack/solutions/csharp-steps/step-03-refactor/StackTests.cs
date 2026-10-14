using Xunit;

namespace Kata;

// Stack kata — step by step, STEP 3: REFACTOR — generalize the hard-coded answer.
// Snapshot at the END of the step; walkthrough in ../README.md
public class StackTests
{
    [Fact] // [edge]
    public void ANewStackIsEmpty()
    {
        Assert.True(new Stack().IsEmpty());
    }

    [Fact] // [positive]
    public void IsNotEmptyAfterAPush()
    {
        var stack = new Stack();
        stack.Push("a");
        Assert.False(stack.IsEmpty());
    }

    // ── Test list — still to promote ──

    // [positive]
    [Fact(Skip = "todo")]
    public void PushingOneItemThenPoppingReturnsThatItem()
    {
    }

    [Fact(Skip = "todo")]
    public void PoppingTheOnlyItemEmptiesTheStackAgain()
    {
    }

    [Fact(Skip = "todo")]
    public void PeekReturnsTheTopItem()
    {
    }

    [Fact(Skip = "todo")]
    public void PeekDoesNotRemoveTheItem()
    {
    }

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
