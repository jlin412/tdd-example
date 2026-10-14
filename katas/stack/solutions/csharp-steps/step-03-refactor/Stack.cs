namespace Kata;

// Stack kata — step by step, STEP 3: REFACTOR — generalize the hard-coded answer.
// Snapshot at the END of the step; walkthrough in ../README.md
public class Stack
{
    private int _count;

    public void Push(object? item)
    {
        _count++;
    }

    public bool IsEmpty() => _count == 0;
}
