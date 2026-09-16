using System.Collections;

namespace Kata;

// Stack kata — reference solution (extended: R7 + R8, a bounded stack).
// See StackExtendedStory.md for the requirement.
//
// This is pattern-menu item (c) DECORATOR, and the capacity requirement is
// what earns it. Note what did NOT happen: Stack.cs was not opened. Every
// test in StackTests.cs still passes, untouched, because the new behavior
// wraps the old class instead of editing it — the Open/Closed Principle with
// a green suite as the receipt.
//
// The alternative — adding a `capacity` field to Stack itself — is a
// legitimate choice too, but it makes capacity a concern of EVERY stack,
// forces a constructor argument on callers who never wanted a limit, and
// puts the old tests at risk. Wrapping keeps "a stack" and "a stack with a
// ceiling" as separate ideas.
//
// FULL CONTRACT (R8): this team throws, matching the empty-stack decision in
// Stack.cs — a refused push is exceptional, not an expected outcome the
// caller should have to remember to check. Returning a bool is the other
// defensible option; evicting the bottom item is a THIRD behavior (a ring
// buffer) that happens to be a different data structure wearing a stack's
// clothes, which is worth saying out loud in the retro.
public class FullStackException : InvalidOperationException
{
    public FullStackException(int capacity)
        : base($"cannot push onto a full stack (capacity {capacity})")
    {
    }
}

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

    public bool IsFull() => _stack.Size() >= Capacity;

    public void Push(T item)
    {
        if (IsFull())
        {
            throw new FullStackException(Capacity);
        }

        _stack.Push(item);
    }

    // Everything else is the wrapped stack's job, unchanged.
    public T Pop() => _stack.Pop();

    public T Peek() => _stack.Peek();

    public int Size() => _stack.Size();

    public bool IsEmpty() => _stack.IsEmpty();

    public IEnumerator<T> GetEnumerator() => _stack.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
