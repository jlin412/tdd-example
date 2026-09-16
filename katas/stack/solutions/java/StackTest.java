package kata;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertNull;
import static org.junit.jupiter.api.Assertions.assertSame;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

// Stack kata — reference test suite (classic R1–R6, one possible end state).
// This is what a mob's suite might look like after the whole session.
class StackTest {

    // The arrange helper the mob extracts once the setup starts repeating.
    @SafeVarargs
    private static <T> Stack<T> stackOf(T... items) {
        Stack<T> stack = new Stack<>();
        for (T item : items) {
            stack.push(item);
        }
        return stack;
    }

    private static <T> List<T> toList(Iterable<T> iterable) {
        List<T> seen = new ArrayList<>();
        iterable.forEach(seen::add);
        return seen;
    }

    @Test
    @DisplayName("[edge] a new stack is empty and has size 0")
    void aNewStackIsEmpty() {
        Stack<String> stack = new Stack<>();
        assertTrue(stack.isEmpty());
        assertEquals(0, stack.size());
    }

    @Test
    @DisplayName("[positive] push then pop hands back the item that was pushed")
    void pushThenPopHandsBackTheItem() {
        assertEquals("a", stackOf("a").pop());
    }

    @Test
    @DisplayName("[positive] the stack is empty again once the only item is popped")
    void emptyAgainOnceTheOnlyItemIsPopped() {
        Stack<String> stack = stackOf("a");
        stack.pop();
        assertTrue(stack.isEmpty());
    }

    // Two items is the SMALLEST case that can tell a stack from a queue.
    @Test
    @DisplayName("[boundary] pops the most recently pushed item first")
    void popsTheMostRecentlyPushedItemFirst() {
        assertEquals("b", stackOf("a", "b").pop());
    }

    @Test
    @DisplayName("[boundary] pops the rest in reverse order of pushing")
    void popsTheRestInReverseOrder() {
        Stack<String> stack = stackOf("a", "b", "c");
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
        Stack<String> stack = stackOf("a", "b");
        assertEquals("b", stack.peek());
        assertEquals("b", stack.peek());
        assertEquals(2, stack.size());
    }

    @Test
    @DisplayName("[boundary] size tracks the whole sequence, not just the pushes")
    void sizeTracksTheWholeSequence() {
        Stack<String> stack = new Stack<>();
        assertEquals(0, stack.size());
        stack.push("a");
        assertEquals(1, stack.size());
        stack.push("b");
        assertEquals(2, stack.size());
        stack.pop();
        assertEquals(1, stack.size());
    }

    @Test
    @DisplayName("[edge] the empty contract: pop and peek both throw")
    void popAndPeekOnEmptyBothThrow() {
        assertThrows(EmptyStackException.class, () -> new Stack<String>().pop());
        assertThrows(EmptyStackException.class, () -> new Stack<String>().peek());
    }

    @Test
    @DisplayName("[edge] and it throws again after a stack is emptied by popping")
    void throwsAgainAfterBeingEmptied() {
        Stack<String> stack = stackOf("a");
        stack.pop();
        assertThrows(EmptyStackException.class, stack::pop);
    }

    // Precisely the case that would be ambiguous if pop() returned null to
    // signal "empty". Because this stack throws instead, it is not.
    @Test
    @DisplayName("[edge] can hold null without that meaning \"empty\"")
    void canHoldNullWithoutThatMeaningEmpty() {
        Stack<String> stack = stackOf((String) null);
        assertFalse(stack.isEmpty());
        assertNull(stack.pop());
        assertTrue(stack.isEmpty());
    }

    @Test
    @DisplayName("[negative] the internals do not leak — the iterator hands out a copy")
    void theInternalsDoNotLeak() {
        Stack<String> stack = stackOf("a", "b");
        List<String> seen = toList(stack);
        seen.add("c");
        seen.clear();
        assertEquals(2, stack.size());
        assertEquals("b", stack.peek());
    }

    @Test
    @DisplayName("[negative] and the iterator cannot remove through it either")
    void theIteratorCannotRemove() {
        Iterator<String> iterator = stackOf("a", "b").iterator();
        iterator.next();
        assertThrows(UnsupportedOperationException.class, iterator::remove);
    }

    @Test
    @DisplayName("[positive] iterable, top-down, without removing anything")
    void iterableTopDown() {
        Stack<String> stack = stackOf("a", "b", "c");
        assertEquals(List.of("c", "b", "a"), toList(stack));
        assertEquals(3, stack.size());
    }

    // [negative] Half of this category is already written for you, by javac:
    // stackOf("a").push(1) and `int x = stackOf("a").pop();` simply do not
    // compile. There is nothing to assert — the build is the assertion.
    // What the compiler canNOT tell you is the rest of this file: that pop
    // on empty throws, that peek leaves the stack alone, that the iterator
    // hands out a copy. Both nets, different catches.

    @Test
    @DisplayName("[edge] generics are erased — the runtime does not know what T was")
    void genericsAreErasedAtRuntime() {
        // javac enforces T and then throws it away. This is the one place the
        // Java and C# solutions genuinely differ: in C# these two would be
        // DIFFERENT runtime types. Worth knowing before you try to write a
        // test that asks a stack what it holds — you cannot.
        assertSame(new Stack<String>().getClass(), new Stack<Integer>().getClass());
    }
}
