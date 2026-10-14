package kata;

// ════════════════════════════════════════════════════════════════════
// Stack — production skeleton (Java)
// ════════════════════════════════════════════════════════════════════
// The requirement lives in the story, not here:
//   · Classic:  katas/stack/StackStory.md
//   · Extended: katas/stack/StackExtendedStory.md
//
// TDD rules for this file:
//   · Add NOTHING here until a RED test in StackTest demands it.
//   · Write the MINIMUM that makes the current red test green.
//   · Refactor only on green (prompts for that live in the test file).
//
// You have TWO safety nets in Java, and one of them runs before the other:
// javac refuses to build code whose types don't line up, so a whole class
// of mistake never reaches your tests. `mvn test` compiles first, then
// runs — which is why your first RED here is a COMPILE error.
public class Stack {
    public boolean isEmpty() {
        return true;
    }
}
