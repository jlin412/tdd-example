using System.Collections;

namespace Kata;

// Stack kata — reference solution (classic R1–R6).
//
// One possible end state. From the checkpoint-#2 pattern menu this team took
// (a) GENERIC TYPE PARAMETER and (e) IENUMERABLE<T> — added one at a time,
// per the coach's rule, with all tests green between them.
//
// NO NATIVE STACK: the backing store is a plain T[] used only by index —
// _items[_count] = x to push, _items[--_count] to pop — with the stack
// keeping its own count. No Stack<T>, List<T> or Array.Resize: when the array
// is full the stack copies it into a bigger one by hand. A popped slot is
// reset to default, so the stack never keeps an item alive after handing it
// back.
//
// Deliberately NOT taken (all good retro topics):
//   · (b) SWAPPABLE BACKING STORE — the store is already hand-rolled on a
//     plain array. An interface with a linked-list sibling would be ceremony
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

    // Menu item (e) — callers can walk the stack (foreach, LINQ) top-down
    // without ever getting a reference to the array itself. `yield return`
    // hands out values, never the array.
    public IEnumerator<T> GetEnumerator()
    {
        for (var i = _count - 1; i >= 0; i--)
        {
            yield return _items[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
