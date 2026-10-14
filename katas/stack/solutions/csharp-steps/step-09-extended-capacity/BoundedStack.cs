using System.Collections;

namespace Kata;

// Stack kata — step by step, STEP 9: extended R7 — a capacity fixed at creation.
// Snapshot at the END of the step; walkthrough in ../README.md
//
// Menu item (c) DECORATOR: a BoundedStack wraps a plain Stack instead of
// editing it — Stack.cs is not opened in steps 9 or 10. The limit is not
// enforced yet; step 10 (R8) does that.
public class BoundedStack<T> : IEnumerable<T>
{
    private readonly Stack<T> _stack = new();

    public BoundedStack(int capacity)
    {
        // Only the VALUE needs guarding here: `int capacity` already makes
        // 1.5 and "2" impossible to pass. The Python solution has to test for
        // both, because nothing stops a caller there.
        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(capacity), capacity, "capacity must be non-negative");
        }

        Capacity = capacity;
    }

    public int Capacity { get; }

    // Everything else is the wrapped stack's job, unchanged.
    public void Push(T item) => _stack.Push(item);

    public T Pop() => _stack.Pop();

    public T Peek() => _stack.Peek();

    public int Size() => _stack.Size();

    public bool IsEmpty() => _stack.IsEmpty();

    public IEnumerator<T> GetEnumerator() => _stack.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
