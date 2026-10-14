package kata;

import org.junit.jupiter.api.Disabled;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

// Stack kata — step by step, STEP 4: the other [positive] cases — push/pop, peek, size.
// Snapshot at the END of the step; walkthrough in ../README.md
class StackTest {

    // The arrange helper the mob extracts once the setup starts repeating.
    private static Stack stackOf(Object... items) {
        Stack stack = new Stack();
        for (Object item : items) {
            stack.push(item);
        }
        return stack;
    }

    @Test
    @DisplayName("[edge] a new stack is empty")
    void aNewStackIsEmpty() {
        Stack stack = new Stack();
        assertTrue(stack.isEmpty());
    }

    @Test
    @DisplayName("[positive] a stack is not empty after a push")
    void aStackIsNotEmptyAfterAPush() {
        assertFalse(stackOf("a").isEmpty());
    }

    @Test
    @DisplayName("[positive] push then pop hands back the item that was pushed")
    void pushThenPopHandsBackTheItem() {
        assertEquals("a", stackOf("a").pop());
    }

    @Test
    @DisplayName("[positive] the stack is empty again once the only item is popped")
    void emptyAgainOnceTheOnlyItemIsPopped() {
        Stack stack = stackOf("a");
        stack.pop();
        assertTrue(stack.isEmpty());
    }

    @Test
    @DisplayName("[positive] peek returns the top item")
    void peekReturnsTheTopItem() {
        assertEquals("b", stackOf("a", "b").peek());
    }

    @Test
    @DisplayName("[positive] peek does NOT remove — size unchanged, twice agrees")
    void peekDoesNotRemove() {
        Stack stack = stackOf("a", "b");
        assertEquals("b", stack.peek());
        assertEquals("b", stack.peek());
        assertEquals(2, stack.size());
    }

    // ── Test list — still to promote ──

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
