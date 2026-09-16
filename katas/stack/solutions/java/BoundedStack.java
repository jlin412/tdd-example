package kata;

import java.util.Iterator;

// Stack kata — reference solution (extended: R7 + R8, a bounded stack).
// See StackExtendedStory.md for the requirement.
//
// This is pattern-menu item (c) DECORATOR, and the capacity requirement is
// what earns it. Note what did NOT happen: Stack.java was not opened. Every
// test in StackTest still passes, untouched, because the new behavior wraps
// the old class instead of editing it — the Open/Closed Principle with a
// green suite as the receipt.
//
// The alternative — adding a `capacity` field to Stack itself — is a
// legitimate choice too, but it makes capacity a concern of EVERY stack,
// forces a constructor argument on callers who never wanted a limit, and
// puts the old tests at risk. Wrapping keeps "a stack" and "a stack with a
// ceiling" as separate ideas.
//
// FULL CONTRACT (R8): this team throws, matching the empty-stack decision in
// Stack.java — a refused push is exceptional, not an expected outcome the
// caller should have to remember to check. Returning a boolean is the other
// defensible option; evicting the bottom item is a THIRD behavior (a ring
// buffer) that happens to be a different data structure wearing a stack's
// clothes, which is worth saying out loud in the retro.
public class BoundedStack<T> implements Iterable<T> {

    private final Stack<T> stack = new Stack<>();
    private final int capacity;

    public BoundedStack(int capacity) {
        // Only the VALUE needs guarding here: `int capacity` already makes
        // 1.5 and "2" impossible to pass. The Python solution has to test
        // for both, because nothing stops a caller there.
        if (capacity < 0) {
            throw new IllegalArgumentException("capacity must be non-negative, got " + capacity);
        }
        this.capacity = capacity;
    }

    public int capacity() {
        return capacity;
    }

    public boolean isFull() {
        return stack.size() >= capacity;
    }

    public void push(T item) {
        if (isFull()) {
            throw new FullStackException(capacity);
        }
        stack.push(item);
    }

    // Everything else is the wrapped stack's job, unchanged.
    public T pop() {
        return stack.pop();
    }

    public T peek() {
        return stack.peek();
    }

    public int size() {
        return stack.size();
    }

    public boolean isEmpty() {
        return stack.isEmpty();
    }

    @Override
    public Iterator<T> iterator() {
        return stack.iterator();
    }
}
