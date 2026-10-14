package kata;

// Stack kata — step by step, STEP 4: the other [positive] cases — push/pop, peek, size.
// Snapshot at the END of the step; walkthrough in ../README.md
public class Stack {
    private int count = 0;
    private Object top;

    public void push(Object item) {
        top = item;
        count++;
    }

    public Object pop() {
        count--;
        return top;
    }

    public Object peek() {
        return top;
    }

    public int size() {
        return count;
    }

    public boolean isEmpty() {
        return count == 0;
    }
}
