package kata;

import java.util.List;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertNull;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

// Stack kata — step by step, STEP 7: [negative] — the compiler keeps the array private.
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
    @DisplayName("[edge] a new stack has size 0")
    void aNewStackHasSize0() {
        Stack stack = new Stack();
        assertEquals(0, stack.size());
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

    // Two items is the SMALLEST case that can tell a stack from a queue —
    // but only a second pop proves the order (see the walkthrough, step 5).
    @Test
    @DisplayName("[boundary] pops the most recently pushed item first")
    void popsTheMostRecentlyPushedItemFirst() {
        assertEquals("b", stackOf("a", "b").pop());
    }

    @Test
    @DisplayName("[boundary] pops the rest in reverse order of pushing")
    void popsTheRestInReverseOrder() {
        Stack stack = stackOf("a", "b", "c");
        assertEquals(List.of("c", "b", "a"), List.of(stack.pop(), stack.pop(), stack.pop()));
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

    @Test
    @DisplayName("[boundary] size tracks the whole sequence, not just the pushes")
    void sizeTracksTheWholeSequence() {
        Stack stack = new Stack();
        assertEquals(0, stack.size());
        stack.push("a");
        assertEquals(1, stack.size());
        stack.push("b");
        assertEquals(2, stack.size());
        stack.pop();
        assertEquals(1, stack.size());
    }

    // The story never sets a limit, and a Java array has a fixed length.
    // This is the test that makes the stack grow its own array.
    @Test
    @DisplayName("[boundary] holds as many items as you push — and gives them back in reverse")
    void holdsAsManyItemsAsYouPush() {
        Stack stack = new Stack();
        for (int i = 0; i < 100; i++) {
            stack.push(i);
        }
        assertEquals(100, stack.size());
        for (int i = 99; i >= 0; i--) {
            assertEquals(i, stack.pop());
        }
        assertTrue(stack.isEmpty());
    }

    @Test
    @DisplayName("[edge] the empty contract: pop and peek both throw")
    void popAndPeekOnEmptyBothThrow() {
        assertThrows(EmptyStackException.class, () -> new Stack().pop());
        assertThrows(EmptyStackException.class, () -> new Stack().peek());
    }

    @Test
    @DisplayName("[edge] and it throws again after a stack is emptied by popping")
    void throwsAgainAfterBeingEmptied() {
        Stack stack = stackOf("a");
        stack.pop();
        assertThrows(EmptyStackException.class, stack::pop);
    }

    // Precisely the case that would be ambiguous if pop() returned null to
    // signal "empty". Because this stack throws instead, it is not.
    @Test
    @DisplayName("[edge] can hold null without that meaning \"empty\"")
    void canHoldNullWithoutThatMeaningEmpty() {
        Stack stack = stackOf((Object) null);
        assertFalse(stack.isEmpty());
        assertNull(stack.pop());
        assertTrue(stack.isEmpty());
    }

    // [negative] the backing array cannot be tampered with from outside: a
    // test that tries — `stack.items[1] = "x";` — does not even compile
    // ("items has private access in kata.Stack"). The compiler guards every
    // honest caller. (Reflection with setAccessible(true) can still get in;
    // that is a deliberate break-in, not an accident.) What a caller CAN
    // reach is whatever the public API hands out — see the iterator tests.
}
