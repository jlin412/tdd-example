package kata;

// Stack kata — reference solution (extended: R7 + R8, a bounded stack).
// See StackExtendedStory.md for the requirement.
public class FullStackException extends RuntimeException {

    public FullStackException(int capacity) {
        super("cannot push onto a full stack (capacity " + capacity + ")");
    }
}
