namespace Kata;

// Stack kata — step by step, STEP 5: [boundary] — last in, first out; size; as many items as you push.
// Snapshot at the END of the step; walkthrough in ../README.md
public class Stack
{
    private const int InitialCapacity = 4;

    private object?[] _items = new object?[InitialCapacity];
    private int _count;

    public void Push(object? item)
    {
        if (_count == _items.Length)
        {
            Grow();
        }

        _items[_count] = item;
        _count++;
    }

    public object? Pop()
    {
        _count--;
        var item = _items[_count];
        _items[_count] = null; // let go: a popped item must not stay reachable
        return item;
    }

    public object? Peek()
    {
        return _items[_count - 1];
    }

    public int Size() => _count;

    public bool IsEmpty() => _count == 0;

    // A C# array has a fixed length, so make a bigger one and copy the items
    // across by hand.
    private void Grow()
    {
        var bigger = new object?[_items.Length * 2];
        for (var i = 0; i < _count; i++)
        {
            bigger[i] = _items[i];
        }

        _items = bigger;
    }
}
