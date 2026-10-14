package kata;

// Stack kata — reference solution (classic R1–R6).
//
// Its own type, not a reused java.util one, so a caller can catch exactly
// this and nothing else. (java.util.EmptyStackException exists, but it is
// tied to the legacy java.util.Stack and carries no message; nothing here
// imports it, so inside package kata this name is the one that resolves.)
public class EmptyStackException extends RuntimeException {

    public EmptyStackException(String operation) {
        super("cannot " + operation + " an empty stack");
    }
}
