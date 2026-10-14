package kata;

import java.util.Iterator;
import java.util.NoSuchElementException;

// Stack kata — reference solution (classic R1–R6).
//
// One possible end state. From the checkpoint-#2 pattern menu this team took
// (a) GENERIC TYPE PARAMETER and (e) ITERABLE — added one at a time, per the
// coach's rule, with all tests green between them.
//
// NO NATIVE STACK: the backing store is a plain Object[] used only by index
// — items[count] = x to push, items[--count] to pop — with the stack keeping
// its own count. No java.util.Stack, Deque or ArrayList, and no
// Arrays.copyOf: when the array is full the stack copies it into a bigger
// one by hand. A popped slot is nulled, so the stack never keeps an item
// alive after handing it back (the "obsolete reference" bug, Effective Java
// item 7 — the very bug that book found in a hand-rolled stack).
//
// Deliberately NOT taken (all good retro topics):
//   · (b) SWAPPABLE BACKING STORE — the store is already hand-rolled on a
//     plain array. An array/linked-list pair behind an interface would be
//     ceremony with no payoff at this size; the interface is worth
//     introducing the day a SECOND implementation actually exists.
//   · (d) OPTIONAL — this team throws on empty (see below), and throwing is
//     unambiguous. An Optional<T> tryPop() earns its place only if you picked
//     the "return null" contract and then hit the null-vs-empty clash — and
//     even then, mind the trap: Optional cannot hold null, so it cannot
//     express "I popped a null item" either.
//   · (f) IMMUTABLE STACK — a fine design, but a different one; it changes
//     the public API from `void push` to `Stack<T> push` and every caller
//     with it.
//
// THE EMPTY CONTRACT (R6): this team throws. The alternative — returning null
// — is defensible right up until you push null into the stack, at which point
// the caller cannot tell an empty stack from a stack whose top item is null.
// Throwing has no such blind spot, so the ambiguity decided it.
//
// ON GENERICS: <T> is enforced by javac and then ERASED. At runtime a
// Stack<String> and a Stack<Integer> are the same class and neither knows
// what T was — see the erasure test in StackTest. The C# solution's
// generics are reified; that contrast is worth a minute in the retro.
public class Stack<T> implements Iterable<T> {

    private static final int INITIAL_CAPACITY = 4;

    // Menu item (a) — the type parameter. Generics are erased, so Java cannot
    // create a T[]: the array is an Object[] and every read casts back to T.
    // It is private and never handed out, so a caller cannot reach it.
    private Object[] items = new Object[INITIAL_CAPACITY];
    private int count = 0;

    public void push(T item) {
        if (count == items.length) {
            grow();
        }
        items[count] = item;
        count++;
    }

    @SuppressWarnings("unchecked")
    public T pop() {
        if (isEmpty()) {
            throw new EmptyStackException("pop");
        }
        count--;
        T item = (T) items[count];
        items[count] = null; // let go: a popped item must not stay reachable
        return item;
    }

    @SuppressWarnings("unchecked")
    public T peek() {
        if (isEmpty()) {
            throw new EmptyStackException("peek");
        }
        return (T) items[count - 1];
    }

    public int size() {
        return count;
    }

    public boolean isEmpty() {
        return count == 0;
    }

    // A Java array has a fixed length, so make a bigger one and copy the
    // items across by hand.
    private void grow() {
        Object[] bigger = new Object[items.length * 2];
        for (int i = 0; i < count; i++) {
            bigger[i] = items[i];
        }
        items = bigger;
    }

    // Menu item (e) — callers can walk the stack (for-each, streams) top-down
    // without ever getting a reference to the array itself. The iterator hands
    // out values; its remove() is the interface default, which throws — an
    // iterator that could remove would be a second, quieter way in.
    @Override
    public Iterator<T> iterator() {
        return new Iterator<>() {
            private int cursor = count - 1;

            @Override
            public boolean hasNext() {
                return cursor >= 0;
            }

            @Override
            @SuppressWarnings("unchecked")
            public T next() {
                if (!hasNext()) {
                    throw new NoSuchElementException();
                }
                return (T) items[cursor--];
            }
        };
    }
}
