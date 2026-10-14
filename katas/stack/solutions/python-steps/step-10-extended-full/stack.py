"""Stack kata — reference solution (classic R1–R6).

One possible end state. From the checkpoint-#2 pattern menu this team took
(a) GENERIC TYPE PARAMETER and (e) DUNDER PROTOCOLS — added one at a time,
per the coach's rule, with all tests green between them.

NO NATIVE STACK: the backing store is a list used as a FIXED-SIZE ARRAY —
allocated as [None] * n, read and written only by index, and never
append()ed to or pop()ped from. The stack keeps its own count, grows by
copying into a bigger list by hand, and clears each popped slot so it never
keeps an item alive after handing it back. A Python list's append and pop
ARE a stack; leaning on them would hand the interesting part of the kata to
the runtime.

Deliberately NOT taken (all good retro topics):
  · (b) SWAPPABLE BACKING STORE — the store is already hand-rolled on a
    fixed-size list. An ABC with a linked-list sibling would be ceremony
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

from typing import Generic, Iterator, List, Optional, TypeVar, cast

T = TypeVar("T")

INITIAL_CAPACITY = 4


class EmptyStackError(IndexError):
    """pop/peek was called on an empty stack.

    Subclasses IndexError on purpose: that is what list.pop() raises, so a
    caller who already handles "took from an empty container" keeps working.
    """

    def __init__(self, operation: str):
        super().__init__(f"cannot {operation} an empty stack")


class Stack(Generic[T]):
    # Menu item (a) — the type parameter (documentation only: see the module
    # docstring). The list is private by convention only — Python has no
    # access control — so the real defence is that it is never handed out.
    def __init__(self) -> None:
        self._items: List[Optional[T]] = [None] * INITIAL_CAPACITY
        self._count = 0

    def push(self, item: T) -> None:
        if self._count == len(self._items):
            self._grow()
        self._items[self._count] = item
        self._count += 1

    def pop(self) -> T:
        if self.is_empty():
            raise EmptyStackError("pop")
        self._count -= 1
        item = self._items[self._count]
        self._items[self._count] = None  # let go: a popped item must not stay reachable
        return cast(T, item)

    def peek(self) -> T:
        if self.is_empty():
            raise EmptyStackError("peek")
        return cast(T, self._items[self._count - 1])

    def size(self) -> int:
        return self._count

    def is_empty(self) -> bool:
        return self._count == 0

    def _grow(self) -> None:
        # The list is used as a fixed-size array: never append()ed to. So
        # make a bigger one and copy the items across by hand.
        bigger = [None] * (len(self._items) * 2)
        for i in range(self._count):
            bigger[i] = self._items[i]
        self._items = bigger

    # Menu item (e) — the dunder protocols. Callers walk the stack top-down
    # (for…in, list(), unpacking) without ever getting a reference to the
    # list itself, and len(stack) reads the way Python programmers expect.
    # Whether size() should survive alongside __len__ is a fine retro
    # argument: kept here so the four language solutions read alike.
    def __iter__(self) -> Iterator[T]:
        for i in range(self._count - 1, -1, -1):
            yield cast(T, self._items[i])

    def __len__(self) -> int:
        return self.size()
