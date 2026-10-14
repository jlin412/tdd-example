package kata;

// Stack kata — step by step, STEP 3: REFACTOR — generalize the hard-coded answer.
// Snapshot at the END of the step; walkthrough in ../README.md
public class Stack {
    private int count = 0;

    public void push(Object item) {
        count++;
    }

    public boolean isEmpty() {
        return count == 0;
    }
}
