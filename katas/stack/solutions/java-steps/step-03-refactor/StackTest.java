package kata;

import org.junit.jupiter.api.Disabled;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

// Stack kata — step by step, STEP 3: REFACTOR — generalize the hard-coded answer.
// Snapshot at the END of the step; walkthrough in ../README.md
class StackTest {

    @Test
    @DisplayName("[edge] a new stack is empty")
    void aNewStackIsEmpty() {
        Stack stack = new Stack();
        assertTrue(stack.isEmpty());
    }

    @Test
    @DisplayName("[positive] a stack is not empty after a push")
    void aStackIsNotEmptyAfterAPush() {
        Stack stack = new Stack();
        stack.push("a");
        assertFalse(stack.isEmpty());
    }

    // ── Test list — still to promote ──

    // [positive]
    @Test
    @Disabled("todo")
    @DisplayName("pushing one item then popping returns that item")
    void pushingOneItemThenPoppingReturnsThatItem() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("popping the only item empties the stack again")
    void poppingTheOnlyItemEmptiesTheStackAgain() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("peek returns the top item")
    void peekReturnsTheTopItem() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("peek does not remove the item — peeking twice agrees, and the size is unchanged")
    void peekDoesNotRemoveTheItem() {
    }

    // [boundary]
    @Test
    @Disabled("todo")
    @DisplayName("a brand-new stack has size 0")
    void aBrandNewStackHasSize0() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("after pushing a then b, pop returns b")
    void afterPushingAThenBPopReturnsB() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("popping three times returns the items in reverse order")
    void poppingThreeTimesReturnsTheItemsInReverse() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("size grows on push and shrinks on pop")
    void sizeGrowsOnPushAndShrinksOnPop() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("a stack holds as many items as you push — 100 in, 100 back out in reverse")
    void aStackHoldsAsManyItemsAsYouPush() {
    }

    // [edge]
    @Test
    @Disabled("todo")
    @DisplayName("popping or peeking an empty stack throws")
    void poppingOrPeekingAnEmptyStackThrows() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("popping again after the stack has been emptied throws")
    void poppingAgainAfterTheStackIsEmptiedThrows() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("null can be pushed and popped — it does not mean \"empty\"")
    void nullCanBePushedAndPoppedWithoutMeaningEmpty() {
    }

    // [negative]
    @Test
    @Disabled("todo")
    @DisplayName("the backing array cannot be tampered with from outside")
    void theBackingArrayCannotBeTamperedWith() {
    }
}
