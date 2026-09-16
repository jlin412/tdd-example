package kata;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Iterator;
import java.util.List;

// Stack kata — reference solution (classic R1–R6).
//
// One possible end state. From the checkpoint-#1 pattern menu this team took
// (a) GENERIC TYPE PARAMETER and (e) ITERABLE — added one at a time, per the
// coach's rule, with all tests green between them.
//
// Deliberately NOT taken (all good retro topics):
//   · (b) SWAPPABLE BACKING STORE — an ArrayList already adds and removes at
//     the end in O(1). A List/linked-list pair behind an interface would be
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

    // Menu item (a) — the type parameter. The list is private and final, and
    // never handed out, so a caller cannot mutate the stack behind its back.
    private final List<T> items = new ArrayList<>();

    public void push(T item) {
        items.add(item);
    }

    public T pop() {
        if (isEmpty()) {
            throw new EmptyStackException("pop");
        }
        return items.remove(items.size() - 1);
    }

    public T peek() {
        if (isEmpty()) {
            throw new EmptyStackException("peek");
        }
        return items.get(items.size() - 1);
    }

    public int size() {
        return items.size();
    }

    public boolean isEmpty() {
        return items.isEmpty();
    }

    // Menu item (e) — callers can walk the stack (for-each, streams) top-down
    // without ever getting a reference to the list itself. The copy is
    // deliberate, and so is the unmodifiable wrapper: an iterator that
    // supported remove() would be a second, quieter way into the internals.
    @Override
    public Iterator<T> iterator() {
        List<T> topDown = new ArrayList<>(items);
        Collections.reverse(topDown);
        return Collections.unmodifiableList(topDown).iterator();
    }
}
