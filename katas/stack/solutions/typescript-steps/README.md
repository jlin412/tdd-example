# Stack (TypeScript), step by step

**Spoilers.** Like the rest of `solutions/`, keep this closed until the retro.

This folder shows one mob's path through the kata, saved after each step. Each
`step-*` folder holds `stack.ts` and `stack.spec.ts` as they stand at the
**end** of that step. Steps 9 and 10 add `boundedStack.ts` and
`boundedStack.spec.ts` next to them. Two of the steps are the reference
solutions:
- **Step 8b** ([`step-08b-iterator/`](step-08b-iterator/)) is the finished
  classic stack.
- **Step 10** ([`step-10-extended-full/`](step-10-extended-full/)) is the
  finished bounded stack, a Decorator around step 8b's untouched `Stack`.

**No native stack.** The stack is built on a plain array used **only by index**
(`items[count] = x` to push, `items[--count]` to pop), with its own count. It
uses no `push`, `pop`, `splice` or `shift`, and doesn't even read `.length`. An
array's `push` and `pop` *are* a stack, so using them would hand the interesting
part of the kata to the runtime.

## Running a step

Tests, from `katas/stack/typescript/` (after `npm install`):

```bash
npx vitest run --root ../solutions/typescript-steps/step-05-boundary
```

**Both nets.** `tsc` needs the skeleton's `tsconfig.json` and `node_modules`, so to
type-check a step, copy it over the skeleton **in a scratch copy of the repo**:

```bash
# from katas/stack/
cp solutions/typescript-steps/step-05-boundary/*.ts typescript/ && (cd typescript && npm test && npm run typecheck)
```

To see exactly what one step changed, from this folder:

```bash
diff -u step-04-positive step-05-boundary
```

**`step-01-red` fails on purpose**, under both nets.

| Step | Folder | `npm test` | `typecheck` |
|---|---|---|---|
| 0 · test list | `step-00-test-list` | 1 passed, 16 todo | ✓ |
| 1 · RED | `step-01-red` | **1 failed**, 15 todo | **✗** |
| 2 · GREEN | `step-02-green` | 1 passed, 15 todo | ✓ |
| 3 · REFACTOR (generalize) | `step-03-refactor` | 2 passed, 14 todo | ✓ |
| 4 · other positives | `step-04-positive` | 6 passed, 10 todo | ✓ |
| 5 · boundary | `step-05-boundary` | 11 passed, 5 todo | ✓ |
| 6 · edge | `step-06-edge` | 15 passed, 1 todo | ✓ |
| 7 · negative | `step-07-negative` | 16 passed | ✓ |
| 8a · generics | `step-08a-generics` | 17 passed | ✓ |
| 8b · iterator | `step-08b-iterator` | 19 passed | ✓ |
| 9 · extended R7, capacity | `step-09-extended-capacity` | 27 passed, 7 todo | ✓ |
| 10 · extended R8, full | `step-10-extended-full` | 34 passed | ✓ |

The order follows the skeleton's prompts: positives and the boundary, then
checkpoint #1 (edge, then negative), then checkpoint #2 (the pattern menu), then
the extended story's two requirements, one at a time.

Steps 0–2 are the skeleton files plus only the mob's move. From step 3 on, the
skeleton's coaching comments are gone, and any todos not yet promoted wait at the
bottom of the spec. Until step 8a the stack holds `any` (generics are a menu item,
reached by refactor), and until step 7 its fields use TypeScript's `private`.
Each RED below comes from a real run: step N's specs against step N−1's code,
under **both** `npm test` and `npm run typecheck`. Where they disagree, that
disagreement is the lesson.

---

## Step 0 · the test list

Before any code, the spec gets 16 `it.todo`s: the skeleton's 2 plus 14 more,
grouped as [positive], [boundary], [edge] and [negative]. Each is named as a
**sequence**, not a single call ("popping three times returns the items in
reverse order"), because a stack has state. The generics and iterator tests are
not on the list; they come from design decisions in step 8.

## Step 1 · RED: two reds, one per net

Uncomment the STEP 1 assertion and delete the todo it promotes.

```
npm test           ✗ a brand-new stack is empty  →  TypeError: stack.isEmpty is not a function
npm run typecheck  error TS2339: Property 'isEmpty' does not exist on type 'Stack'.
```

Same mistake, two different reports: one at runtime, one at compile time. This
pair is the reason the kata is in TypeScript.

## Step 2 · GREEN

```ts
isEmpty(): boolean {
  return true;
}
```

## Step 3 · REFACTOR: generalize

The skeleton says the next test is what makes a hard-coded `true` wrong:

```
✗ is not empty after a push  →  TypeError: stack.push is not a function
  error TS2339: Property 'push' does not exist on type 'Stack'.
```

The minimum is a counter, `private count = 0`, with `push` doing `count++`.
**`push` stores nothing yet.** No test asks for an item back, so nothing needs
the item.

## Step 4 · the other positives

```
✗ hands back the item that was pushed                    →  stackOf(...).pop is not a function
✗ is empty again once the only item is popped            →  stack.pop is not a function
✗ returns the top item                                   →  stackOf(...).peek is not a function
✗ does NOT remove it — size is unchanged and peeking …   →  stack.peek is not a function
```

**Still no array.** One variable passes all four:

```ts
private top: any;

push(item: any): void { this.top = item; this.count++; }
pop(): any            { this.count--; return this.top; }
peek(): any           { return this.top; }
```

The peek test brings `size()`. The REFACTOR extracts the `stackOf(...)` arrange
helper, because the pushes have started repeating.

*Discuss:* is this cheating? It is the minimum, and the next step shows exactly
which test it fails.

## Step 5 · boundary: what it takes to prove LIFO

Three of the five new tests **pass on arrival** against step 4's single
variable: "has size 0", "grows on push and shrinks on pop", and this one:

```
✓ pops the most recently pushed item first   (push a, push b, pop → 'b')
```

Two pushes and one pop can't tell a stack from a single variable. Only the second
pop proves the order:

```
✗ pops the rest in reverse order of pushing                →  expected [ 'c', 'c', 'c' ] to deeply equal [ 'c', 'b', 'a' ]
✗ keeps every item, in order, however many are pushed      →  expected 99 to be 98
```

The second red is the story's "how big can it get?" gap: push 100, pop 100.
**The array arrives now**, because a test finally needs more than one item. It is
used by index only, and it fixes **both** reds:

```ts
private items: any[] = [];
private count = 0;

push(item: any): void {
  this.items[this.count] = item;
  this.count++;
}

pop(): any {
  this.count--;
  const item = this.items[this.count];
  this.items[this.count] = undefined; // let go: a popped item must not stay reachable
  return item;
}

peek(): any {
  return this.items[this.count - 1];
}
```

No growth code is needed: a JavaScript array grows by itself when you write
past its end. In the Python, Java and C# trails the same growth test goes red a
second time, against a fixed-size array, and forces the stack to copy into a
bigger one by hand.

Clearing the popped slot is invisible to every test. It is hygiene, the
"obsolete reference" bug from *Effective Java*. Without it the stack keeps every
popped object alive.

*Discuss:* the story says one push proves nothing. Here even two pushes proved
nothing, because it took two **pops**. How small is the real smallest LIFO test?

## Step 6 · edge (checkpoint #1): the empty contract

```
✗ throws when popping an empty stack                 →  expected [Function] to throw an error
✗ throws when peeking an empty stack                 →  expected [Function] to throw an error
✗ throws again after a stack is emptied by popping   →  expected [Function] to throw an error
  error TS2305: Module '"./stack"' has no exported member 'EmptyStackError'.
```

Step 5's code read `items[-1]` and quietly handed back `undefined`, just as
`Array.prototype.pop()` would. The mob decides to **throw**, so the GREEN is an
`EmptyStackError` and one guard each in `pop` and `peek`.

The nothing-shaped test **passes on arrival**:

```
✓ can hold undefined without that meaning "empty"
```

That's because the contract throws. Under the "return `undefined` when empty"
contract, this test is where a caller can no longer tell an empty stack from a
stack holding `undefined`. That ambiguity is what decided the contract.

## Step 7 · negative (checkpoint #1): `private` is not private

The story asks whether a caller can change the stack behind its back. The public
API hands nothing out, so the test reaches **past** it, the way any JavaScript
caller can:

```ts
for (const field of Object.values(stack)) {
  if (Array.isArray(field)) field.length = 0;
}
expect(stack.peek()).toBe('b');
```

```
✗ cannot be tampered with from outside, even with a cast  →  expected undefined to be 'b'
```

`npm run typecheck` is **green**. TypeScript's `private` exists only at compile
time; at runtime `items` is an ordinary property that `Object.values` finds and
wipes. (`size()` still says 2, because the count lives in a separate field. The
damage only shows when you look at the items.)

The GREEN is JavaScript's own private fields, `#items` and `#count`. These are
private at runtime, so `Object.values` finds nothing.

*Discuss:* the type checker was green the whole time. Which net could ever have
caught this?

## Step 8 · the pattern menu (checkpoint #2), one pattern at a time

**8a · (a) generic type parameter, driven by a typecheck-only red.** The test
that pins TypeScript's answer comes first:

```ts
const stack = new Stack();
// `npm run typecheck` fails if the next line ever STOPS being a type error.
// @ts-expect-error — a string is not a number
stack.push('a');
expect(stack.pop()).toBe('a');
```

Against step 7's stack of `any`, the two nets now disagree in the opposite
direction:

```
npm test           Tests  16 passed (16)
npm run typecheck  error TS2578: Unused '@ts-expect-error' directive.
```

The red exists **only in the second net**. The GREEN makes it `Stack<T>`, with
`#items: (T | undefined)[]` and a generic `stackOf<T>`, and the test becomes a
`Stack<number>`. Pushing a string is now a type error, so the directive is used
and typecheck is green, while `npm test` still happily runs the push. That one
test shows exactly what each tool checks.

**8b · (e) iterator.** Callers want to walk the stack, and the leak test wants
something the API hands out:

```
✗ walks the items top-down without removing them               →  TypeError: stack is not iterable
✗ cannot be mutated through anything the public API hands out  →  TypeError: stack is not iterable
  error TS2488: Type 'Stack<string>' must have a '[Symbol.iterator]()' method that returns an iterator.
```

The GREEN is a generator walking the indexes `count - 1` down to `0`, still with
no Array method. Spreading it gives a **copy**, so mutating the copy can't touch
the stack. This is the finished classic solution.

Declined (reasons in `stack.ts`'s header):
- **(b) a swappable backing store:** it's already hand-rolled on a plain array.
- **(d) Result/Option:** the throw contract has no ambiguity left to fix.
- **(f) an immutable stack:** a different design with a different public API.

## Step 9 · extended R7: a capacity fixed at creation

**Predict the damage: none.** The bounded stack is a **Decorator**: a new
`BoundedStack<T>` that holds a private `Stack<T>` and forwards to it. `stack.ts`
and `stack.spec.ts` are not opened in steps 9 or 10; they are byte-identical to
step 8b, and staying green untouched is the proof.

```
✗ [boundedStack.spec.ts]  Cannot find module './boundedStack'
  error TS2307: Cannot find module './boundedStack' or its corresponding type declarations.
```

The steps:
1. Create the class.
2. "Reports the capacity it was created with" goes red, then green.
3. -1, 1.5 and `NaN` are rejected with a `RangeError` at construction.
4. Delegation: LIFO, peek, the empty-pop throw and iteration all forward to the
   wrapped stack.

The limit is **not enforced yet**. That is step 10, and the 7 todos for it wait
at the bottom of the spec.

*Discuss:* the alternative was adding `capacity` to `Stack` itself. Then every
old `new Stack()` would pass `npm test` and fail `npm run typecheck`. Was
wrapping worth a second class?

## Step 10 · extended R8: a full stack refuses more

```
✗ starts empty and not full                       →  TypeError: stack.isFull is not a function
✗ becomes full exactly at capacity                →  TypeError: stack.isFull is not a function
✗ throws when pushing past capacity               →  expected [Function] to throw an error
✗ does not grow when a push is refused            →  expected [Function] to throw an error
✗ accepts a push again once something is popped   →  TypeError: stack.isFull is not a function
✗ is empty and full at the same time  (capacity 0) →  TypeError: stack.isFull is not a function
✗ refuses the very first push         (capacity 0) →  expected [Function] to throw an error
  error TS2305: Module '"./boundedStack"' has no exported member 'FullStackError'.
```

The GREEN is `isFull()` (`size() >= capacity`) and a `FullStackError` guard in
`push`. Throwing matches the empty-stack decision: a refused push is exceptional,
not a result the caller has to remember to check. The full suite is 19 classic +
15 bounded = **34** tests, green under both nets.
