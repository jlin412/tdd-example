package kata;

import java.util.Iterator;

// Stack kata — step by step, STEP 9: extended R7 — a capacity fixed at creation.
// Snapshot at the END of the step; walkthrough in ../README.md
//
// Menu item (c) DECORATOR: a BoundedStack wraps a plain Stack instead of
// editing it — Stack.java is not opened in steps 9 or 10. The limit is not
// enforced yet; step 10 (R8) does that.
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

    // Everything else is the wrapped stack's job, unchanged.
    public void push(T item) {
        stack.push(item);
    }

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
