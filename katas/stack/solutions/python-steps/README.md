# Stack (Python), step by step

**Spoilers.** Like the rest of `solutions/`, keep this closed until the retro.

This folder shows one mob's path through the kata, saved after each step. Each
`step-*` folder holds `stack.py` and `test_stack.py` as they stand at the **end**
of that step. Steps 9 and 10 add `bounded_stack.py` and `test_bounded_stack.py`
next to them. Two of the steps are the reference solutions:
- **Step 8b** ([`step-08b-iterator/`](step-08b-iterator/)) is the finished
  classic stack.
- **Step 10** ([`step-10-extended-full/`](step-10-extended-full/)) is the
  finished bounded stack, a Decorator around step 8b's untouched `Stack`.

**No native stack.** A Python list's `append` and `pop` *are* a stack, so this
one never calls them. The list is used as a **fixed-size array**: it is allocated
as `[None] * n`, then read and written only by index. The stack keeps its own
count, and when the array is full it copies everything into a bigger one by hand.
`len(self._items)` is read only as the array's capacity, never as the stack's size.

## Running a step

Each step runs on its own, with no copying. Install pytest once
(`pip install -r ../../python/requirements.txt`), then:

```bash
cd step-05-boundary && pytest
```

Run one folder at a time, because every step has its own `stack.py`. To see
exactly what one step changed, from this folder:

```bash
diff -u step-04-positive step-05-boundary
```

**`step-01-red` fails on purpose.**

| Step | Folder | `pytest` |
|---|---|---|
| 0 · test list | `step-00-test-list` | 1 passed, 17 skipped |
| 1 · RED | `step-01-red` | **1 failed**, 16 skipped |
| 2 · GREEN | `step-02-green` | 1 passed, 16 skipped |
| 3 · REFACTOR (generalize) | `step-03-refactor` | 2 passed, 15 skipped |
| 4 · other positives | `step-04-positive` | 6 passed, 11 skipped |
| 5 · boundary | `step-05-boundary` | 11 passed, 6 skipped |
| 6 · edge | `step-06-edge` | 16 passed, 1 skipped |
| 7 · negative | `step-07-negative` | 17 passed |
| 8a · generics | `step-08a-generics` | 18 passed |
| 8b · iterator | `step-08b-iterator` | 21 passed |
| 9 · extended R7, capacity | `step-09-extended-capacity` | 31 passed, 7 skipped |
| 10 · extended R8, full | `step-10-extended-full` | 38 passed |

The order follows the skeleton's prompts: positives and the boundary, then
checkpoint #1 (edge, then negative), then checkpoint #2 (the pattern menu), then
the extended story's two requirements, one at a time.

Steps 0–2 are the skeleton files plus only the mob's move. From step 3 on, the
skeleton's coaching comments are gone, and any todos not yet promoted wait at the
bottom of the test file as skipped tests. Each RED below comes from a real run:
step N's tests against step N−1's code.

**What's different in Python:** there is only one net. Two steps make that
concrete. At step 7 the mob tries to make the internals untouchable and finds that
Python can't. At step 8a, generics produce no red anywhere.

---

## Step 0 · the test list

Before any code, the file gets 17 skipped tests: the skeleton's 2 plus 15 more,
named as **sequences** ("popping three times returns the items in reverse
order"), because a stack has state. Python-specific ones:
- an empty pop is catchable as an `IndexError`
- `None` is an item, not "empty"

## Step 1 · RED

```
✗ test_a_brand_new_stack_reports_that_it_is_empty  →  AttributeError: 'Stack' object has no attribute 'is_empty'
```

## Step 2 · GREEN

`def is_empty(self): return True`.

## Step 3 · REFACTOR: generalize

"Is not empty after a push" goes red (`'Stack' object has no attribute 'push'`).
The minimum is a counter: `push` does `self._count += 1` and **stores nothing**,
because no test asks for an item back.

## Step 4 · the other positives

Four reds, all `AttributeError`s for `pop` and `peek`. **Still no list.** One
variable passes all four:

```python
def push(self, item) -> None:
    self._top = item
    self._count += 1

def pop(self):
    self._count -= 1
    return self._top
```

The peek test brings `size()`. The REFACTOR extracts the `stack_of(...)` arrange
helper.

## Step 5 · boundary: LIFO, and as many items as you push

Against step 4's single variable, three of the five new tests pass on arrival:
"has size 0", "grows on push and shrinks on pop", and "pops the most recently
pushed item first". Two pushes and one pop can't tell a stack from a variable.
Two go red:

```
✗ TestLastInFirstOut::test_pops_the_rest_in_reverse_order_of_pushing
✗ TestHoldsAsManyItemsAsYouPush::test_keeps_every_item_in_order_however_many_are_pushed
```

**The array arrives:** `[None] * INITIAL_CAPACITY` (4), with
`self._items[self._count] = item` to push and `self._count -= 1;
item = self._items[self._count]` to pop. That fixes LIFO, but the growth test is
**still red**, because the array has 4 slots:

```
IndexError: list assignment index out of range      (the 5th push)
```

In TypeScript the same array grew by itself. Here the stack has to grow it:

```python
def _grow(self) -> None:
    bigger = [None] * (len(self._items) * 2)
    for i in range(self._count):
        bigger[i] = self._items[i]
    self._items = bigger
```

Each popped slot is reset to `None`, so the stack never keeps an item alive after
handing it back.

## Step 6 · edge (checkpoint #1): the empty contract

The first red stops collection, because the error class doesn't exist yet:

```
ImportError: cannot import name 'EmptyStackError' from 'stack'
```

With the class in place, all four empty-contract tests are red, because nothing
raises at all. Here is what step 5's stack actually does on an empty pop:

```
empty pop()  → None, and size() is now -1
empty peek() → None
```

The count goes to −1, and `self._items[-1]` **wraps around** to the last slot of
the array. It's a quiet, wrong answer instead of an error.

The GREEN is `EmptyStackError(IndexError)` and one guard each in `pop` and `peek`.
It subclasses `IndexError` so that callers who already catch what `list.pop()`
raises keep working. The `None` test **passes on arrival**: the raise contract
already avoids the "`None` or empty?" ambiguity.

## Step 7 · negative (checkpoint #1): how private is private?

The mob's first version of the test tries to wreck the internals from outside:

```python
for value in vars(stack).values():
    if isinstance(value, list):
        value.clear()
assert stack.peek() == "b"
```

```
✗ test_cannot_be_tampered_with_from_outside   →  IndexError: list index out of range
```

It is red, and **Python has nothing to make it green**:
- An underscore is a convention.
- A double underscore is only name-mangled (`_Stack__items` is still right there).
- `__slots__` hides the list from `vars()`, but not from `stack._items`.

So the mob does the honest thing and pins the language's answer, the same way the
reference already pins "type hints aren't enforced":

```python
def test_private_is_only_a_convention_in_python(self):
    stack = stack_of("a", "b")
    stack._items[1] = "x"
    assert stack.peek() == "x"
```

The defence that *is* possible is to never hand the list out. Step 8b tests that.

*Discuss:* TypeScript fixed this with `#` fields, and Java and C# never let the
test compile. What does Python's answer cost you, and where does the
responsibility go instead?

## Step 8 · the pattern menu (checkpoint #2), one pattern at a time

**8a · (a) `Generic[T]`, with no red anywhere.** The reference's
`TestTheNetPythonDoesNotGiveYou` pushes a string onto a `Stack[int]`. It passes
against step 7's non-generic stack, and it still passes after `Stack(Generic[T])`
with full type hints. That is the lesson: the hints are documentation, and Python
has no second net to break. Compare the TypeScript trail, where the same move
gives a typecheck-only red.

**8b · (e) the dunder protocols:** `__iter__`, walking the indexes from `count - 1`
down to `0`, and `__len__`.

```
✗ test_cannot_be_mutated_through_anything_the_public_api_hands_out   →  TypeError: 'Stack' object is not iterable
✗ test_walks_the_items_top_down_without_removing_them                →  TypeError: 'Stack' object is not iterable
✗ test_supports_len                                                  →  TypeError: object of type 'Stack' has no len()
```

`list(stack)` builds a **copy**, so mutating it can't touch the stack. This is
the finished classic solution, with 21 tests.

Declined (reasons in `stack.py`'s docstring):
- **(b) a swappable backing store:** it's already hand-rolled.
- **(d) `try_pop`:** the raise contract has no ambiguity left to fix.
- **(f) an immutable stack:** a different design with a different API.

## Step 9 · extended R7: a capacity fixed at creation

**Predicted damage: none.** `BoundedStack` is a Decorator around a private
`Stack`, and `stack.py` and `test_stack.py` are byte-identical to step 8b.

The first red is `ModuleNotFoundError: No module named 'bounded_stack'`. Then the
Python-only part: an invalid capacity is rejected by **type** as well as value,
because nothing else will reject it:
- `-1` → `ValueError`
- `1.5`, `"2"`, `None` and `True` → `TypeError` (`bool` subclasses `int`)

The limit isn't enforced yet; the 7 todos for that wait at the bottom.

## Step 10 · extended R8: a full stack refuses more

The first red is `ImportError: cannot import name 'FullStackError'`. With the
class in place, the 7 R8 tests are red (`is_full` is missing, and no push is
refused). The GREEN is `is_full()` (`size() >= capacity`) plus a guard in `push`.
The total is 21 classic + 17 bounded = **38** tests.
