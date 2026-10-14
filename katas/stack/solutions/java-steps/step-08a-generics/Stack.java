package kata;

// Stack kata — step by step, STEP 8a: pattern (a) GENERIC TYPE PARAMETER — Stack<T>.
// Snapshot at the END of the step; walkthrough in ../README.md
public class Stack<T> {

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
}
