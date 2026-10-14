namespace Kata;

// Stack kata — step by step, STEP 4: the other [positive] cases — push/pop, peek, size.
// Snapshot at the END of the step; walkthrough in ../README.md
public class Stack
{
    private int _count;
    private object? _top;

    public void Push(object? item)
    {
        _top = item;
        _count++;
    }

    public object? Pop()
    {
        _count--;
        return _top;
    }

    public object? Peek() => _top;

    public int Size() => _count;

    public bool IsEmpty() => _count == 0;
}
