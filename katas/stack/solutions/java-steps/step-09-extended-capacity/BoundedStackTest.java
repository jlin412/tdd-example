package kata;

import java.util.ArrayList;
import java.util.List;

import org.junit.jupiter.api.Disabled;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.ValueSource;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

// Stack kata — step by step, STEP 9: extended R7 — a capacity fixed at creation.
// Snapshot at the END of the step; walkthrough in ../README.md
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

    // ── Test list — still to promote (step 10, R8: a full stack refuses more) ──

    // [R8]
    @Test
    @Disabled("todo")
    @DisplayName("starts empty and not full")
    void startsEmptyAndNotFull() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("becomes full exactly at capacity")
    void becomesFullExactlyAtCapacity() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("throws when pushing past capacity")
    void throwsWhenPushingPastCapacity() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("does not grow when a push is refused")
    void doesNotGrowWhenAPushIsRefused() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("accepts a push again once something is popped")
    void acceptsAPushAgainOnceSomethingIsPopped() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("capacity 0 is empty and full at the same time")
    void capacityZeroIsEmptyAndFull() {
    }

    @Test
    @Disabled("todo")
    @DisplayName("capacity 0 refuses the very first push")
    void capacityZeroRefusesTheFirstPush() {
    }
}
