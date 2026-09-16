"""Stack kata — reference solution (classic R1–R6).

One possible end state. From the checkpoint-#1 pattern menu this team took
(a) GENERIC TYPE PARAMETER and (e) DUNDER PROTOCOLS — added one at a time,
per the coach's rule, with all tests green between them.

Deliberately NOT taken (all good retro topics):
  · (b) SWAPPABLE BACKING STORE — a Python list already appends and pops at
    the end in O(1). An ABC with a linked-list sibling would be ceremony
    with no payoff at this size; the interface is worth introducing the day
    a SECOND implementation actually exists.
  · (d) RESULT / OPTION — this team raises on empty (see below), and raising
    is unambiguous. try_pop() earns its place only if you picked the
    "return None" contract and then hit the None-vs-empty clash.
  · (f) IMMUTABLE STACK — a fine design, but a different one; it changes
    push() from a mutation to a factory and every caller with it.

THE EMPTY CONTRACT (R6): this team raises. The alternative — returning None
— is defensible right up until you push None into the stack, at which point
the caller cannot tell an empty stack from a stack whose top item is None.
Raising has no such blind spot, so the ambiguity decided it.

ABOUT THE TYPE HINTS: they are documentation. Nothing in this file is
enforced at runtime — Stack[int]().push("nope") runs perfectly happily. The
Java, C# and TypeScript solutions get that check for free from a compiler;
Python does not, which is why test_stack.py pins the behavior explicitly
instead of pretending otherwise.
"""
from __future__ import annotations

from typing import Generic, Iterator, List, TypeVar

T = TypeVar("T")


class EmptyStackError(IndexError):
    """pop/peek was called on an empty stack.

    Subclasses IndexError on purpose: that is what list.pop() raises, so a
    caller who already handles "took from an empty container" keeps working.
    """

    def __init__(self, operation: str):
        super().__init__(f"cannot {operation} an empty stack")


class Stack(Generic[T]):
    # Menu item (a) — the type parameter. The list is private by convention
    # and never handed out, so a caller cannot mutate the stack behind its
    # back.
    def __init__(self) -> None:
        self._items: List[T] = []

    def push(self, item: T) -> None:
        self._items.append(item)

    def pop(self) -> T:
        if self.is_empty():
            raise EmptyStackError("pop")
        return self._items.pop()

    def peek(self) -> T:
        if self.is_empty():
            raise EmptyStackError("peek")
        return self._items[-1]

    def size(self) -> int:
        return len(self._items)

    def is_empty(self) -> bool:
        return len(self._items) == 0

    # Menu item (e) — the dunder protocols. Callers walk the stack top-down
    # (for…in, list(), unpacking) without ever getting a reference to the
    # list itself, and len(stack) reads the way Python programmers expect.
    # Whether size() should survive alongside __len__ is a fine retro
    # argument: kept here so the four language solutions read alike.
    def __iter__(self) -> Iterator[T]:
        for item in reversed(self._items):
            yield item

    def __len__(self) -> int:
        return self.size()
