package kata;

// Stack kata — step by step, STEP 5: [boundary] — last in, first out; size; as many items as you push.
// Snapshot at the END of the step; walkthrough in ../README.md
public class Stack {

    private static final int INITIAL_CAPACITY = 4;

    private Object[] items = new Object[INITIAL_CAPACITY];
    private int count = 0;

    public void push(Object item) {
        if (count == items.length) {
            grow();
        }
        items[count] = item;
        count++;
    }

    public Object pop() {
        count--;
        Object item = items[count];
        items[count] = null; // let go: a popped item must not stay reachable
        return item;
    }

    public Object peek() {
        return items[count - 1];
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
