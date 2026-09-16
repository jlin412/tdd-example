using System.Collections;

namespace Kata;

// Stack kata — reference solution (classic R1–R6).
//
// One possible end state. From the checkpoint-#1 pattern menu this team took
// (a) GENERIC TYPE PARAMETER and (e) IENUMERABLE<T> — added one at a time,
// per the coach's rule, with all tests green between them.
//
// Deliberately NOT taken (all good retro topics):
//   · (b) SWAPPABLE BACKING STORE — a List<T> already adds and removes at the
//     end in O(1). An interface with a linked-list sibling would be ceremony
//     with no payoff at this size; the interface is worth introducing the day
//     a SECOND implementation actually exists.
//   · (d) THE TRY-PATTERN — this team throws on empty (see below), and
//     throwing is unambiguous. TryPop(out T) earns its place only if you
//     picked the "return default" contract and then hit the clash below.
//   · (f) IMMUTABLE STACK — a fine design, but a different one; it changes
//     the public API from `void Push` to `Stack<T> Push` and every caller
//     with it. (.NET ships ImmutableStack<T> if you want to read one.)
//
// THE EMPTY CONTRACT (R6): this team throws — the same thing .NET's own
// Stack<T> does. The alternative, returning default(T), is defensible right
// up until you notice that default(int) IS 0 and default(string?) IS null:
// the caller then cannot tell an empty stack from a stack whose top item is
// a perfectly good zero. Throwing has no such blind spot, so the ambiguity
// decided it.
//
// ON GENERICS: <T> is enforced by the compiler AND kept at runtime — C#
// generics are reified, so Stack<string> and Stack<int> really are different
// types with different code. The Java solution's are erased; that contrast
// is worth a minute in the retro.
public class EmptyStackException : InvalidOperationException
{
    public EmptyStackException(string operation)
        : base($"cannot {operation} an empty stack")
    {
    }
}

public class Stack<T> : IEnumerable<T>
{
    // Menu item (a) — the type parameter. The list is private and readonly,
    // and never handed out, so a caller cannot mutate the stack behind its
    // back.
    private readonly List<T> _items = new();

    public void Push(T item) => _items.Add(item);

    public T Pop()
    {
        if (IsEmpty())
        {
            throw new EmptyStackException("pop");
        }

        var top = _items[^1];
        _items.RemoveAt(_items.Count - 1);
        return top;
    }

    public T Peek()
    {
        if (IsEmpty())
        {
            throw new EmptyStackException("peek");
        }

        return _items[^1];
    }

    public int Size() => _items.Count;

    public bool IsEmpty() => _items.Count == 0;

    // Menu item (e) — callers can walk the stack (foreach, LINQ) top-down
    // without ever getting a reference to the list itself. `yield return`
    // hands out values, never the collection.
    public IEnumerator<T> GetEnumerator()
    {
        for (var i = _items.Count - 1; i >= 0; i--)
        {
            yield return _items[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
