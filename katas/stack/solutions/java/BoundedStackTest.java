package kata;

import java.util.ArrayList;
import java.util.List;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.ValueSource;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

// Stack kata — reference test suite (extended: R7 + R8, a bounded stack).
// See StackExtendedStory.md for the requirement.
class BoundedStackTest {

    @SafeVarargs
    private static <T> BoundedStack<T> boundedStackOf(int capacity, T... items) {
        BoundedStack<T> stack = new BoundedStack<>(capacity);
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
    @DisplayName("[positive] (R7) reports the capacity it was created with")
    void reportsItsCapacity() {
        assertEquals(3, new BoundedStack<String>(3).capacity());
    }

    @Test
    @DisplayName("[positive] (R7) starts empty and not full")
    void startsEmptyAndNotFull() {
        BoundedStack<String> stack = new BoundedStack<>(2);
        assertTrue(stack.isEmpty());
        assertFalse(stack.isFull());
    }

    @Test
    @DisplayName("[positive] (R7) becomes full exactly at capacity")
    void becomesFullExactlyAtCapacity() {
        BoundedStack<String> stack = boundedStackOf(2, "a");
        assertFalse(stack.isFull());
        stack.push("b");
        assertTrue(stack.isFull());
    }

    @Test
    @DisplayName("[edge] (R8) throws when pushing past capacity")
    void throwsWhenPushingPastCapacity() {
        BoundedStack<String> stack = boundedStackOf(2, "a", "b");
        assertThrows(FullStackException.class, () -> stack.push("c"));
    }

    @Test
    @DisplayName("[edge] (R8) does not grow when a push is refused")
    void doesNotGrowWhenAPushIsRefused() {
        BoundedStack<String> stack = boundedStackOf(2, "a", "b");
        assertThrows(FullStackException.class, () -> stack.push("c"));
        assertEquals(2, stack.size());
        assertEquals("b", stack.peek());
    }

    @Test
    @DisplayName("[edge] (R8) accepts a push again once something is popped")
    void acceptsAPushAgainOnceSomethingIsPopped() {
        BoundedStack<String> stack = boundedStackOf(2, "a", "b");
        assertEquals("b", stack.pop());
        assertFalse(stack.isFull());
        stack.push("c");
        assertEquals("c", stack.peek());
        assertEquals(2, stack.size());
    }

    @Test
    @DisplayName("[boundary] capacity 0 is legal — empty and full at the same time")
    void capacityZeroIsEmptyAndFull() {
        BoundedStack<String> stack = new BoundedStack<>(0);
        assertTrue(stack.isEmpty());
        assertTrue(stack.isFull());
    }

    @Test
    @DisplayName("[boundary] capacity 0 refuses the very first push")
    void capacityZeroRefusesTheFirstPush() {
        BoundedStack<String> stack = new BoundedStack<>(0);
        assertThrows(FullStackException.class, () -> stack.push("a"));
    }

    // Only the VALUE is testable here. `int capacity` already rules out 1.5
    // and "2" at compile time — the Python suite needs explicit tests for
    // those, this one does not.
    @ParameterizedTest
    @ValueSource(ints = {-1, -42})
    @DisplayName("[negative] a negative capacity is rejected at construction")
    void aNegativeCapacityIsRejected(int capacity) {
        assertThrows(IllegalArgumentException.class, () -> new BoundedStack<String>(capacity));
    }

    @Test
    @DisplayName("[positive] still last-in-first-out")
    void stillLastInFirstOut() {
        BoundedStack<String> stack = boundedStackOf(3, "a", "b", "c");
        assertEquals(List.of("c", "b", "a"), List.of(stack.pop(), stack.pop(), stack.pop()));
    }

    @Test
    @DisplayName("[positive] still peeks without removing")
    void stillPeeksWithoutRemoving() {
        BoundedStack<String> stack = boundedStackOf(3, "a", "b");
        assertEquals("b", stack.peek());
        assertEquals(2, stack.size());
    }

    @Test
    @DisplayName("[edge] still throws on an empty pop — the wrapped contract is unchanged")
    void stillThrowsOnAnEmptyPop() {
        BoundedStack<String> stack = new BoundedStack<>(2);
        assertThrows(EmptyStackException.class, stack::pop);
    }

    @Test
    @DisplayName("[positive] still iterable, top-down")
    void stillIterableTopDown() {
        assertEquals(List.of("c", "b", "a"), toList(boundedStackOf(3, "a", "b", "c")));
    }
}
