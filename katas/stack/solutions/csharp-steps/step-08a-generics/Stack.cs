namespace Kata;

// Stack kata — step by step, STEP 8a: pattern (a) GENERIC TYPE PARAMETER — Stack<T>.
// Snapshot at the END of the step; walkthrough in ../README.md
public class EmptyStackException : InvalidOperationException
{
    public EmptyStackException(string operation)
        : base($"cannot {operation} an empty stack")
    {
    }
}

public class Stack<T>
{
    private const int InitialCapacity = 4;

    // Menu item (a) — the type parameter. C# generics are reified, so this
    // really is a T[]. It is private and never handed out, so a caller cannot
    // reach it.
    private T[] _items = new T[InitialCapacity];
    private int _count;

    public void Push(T item)
    {
        if (_count == _items.Length)
        {
            Grow();
        }

        _items[_count] = item;
        _count++;
    }

    public T Pop()
    {
        if (IsEmpty())
        {
            throw new EmptyStackException("pop");
        }

        _count--;
        var item = _items[_count];
        _items[_count] = default!; // let go: a popped item must not stay reachable
        return item;
    }

    public T Peek()
    {
        if (IsEmpty())
        {
            throw new EmptyStackException("peek");
        }

        return _items[_count - 1];
    }

    public int Size() => _count;

    public bool IsEmpty() => _count == 0;

    // A C# array has a fixed length, so make a bigger one and copy the items
    // across by hand.
    private void Grow()
    {
        var bigger = new T[_items.Length * 2];
        for (var i = 0; i < _count; i++)
        {
            bigger[i] = _items[i];
        }

        _items = bigger;
    }
}
