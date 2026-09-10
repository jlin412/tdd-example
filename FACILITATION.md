# Facilitating the Mob

This is a guide for whoever is running the session. The goal is not to *finish*
the kata — it's to practice the **discipline**: small steps, tests first, honest
red/green, refactoring only on green.

## Roles

- **Driver** — has the keyboard. Types what the mob decides. Does **not** make
  design decisions solo. "The driver is a smart input device."
- **Navigator(s)** — everyone else. They think out loud and tell the driver
  *what* to do at the level of intent ("now write a test that 3 returns Fizz"),
  not keystrokes.
- **Facilitator** (you) — keeps the loop honest, watches the timer, makes sure
  everyone gets the keyboard, and parks tangents.

## Rotation

- Rotate the driver every **4–5 minutes** on a visible timer. Rotate even
  mid-thought — that's the point; ideas must survive the handoff.
- Rotation order: pass the keyboard clockwise. The current navigator who was
  driving last becomes a navigator again.

## Strong-style pairing

> "For an idea to go from your head into the computer, it must go through
> someone else's hands."

The person with the idea should be navigating, not driving. This forces
communication and keeps the whole mob engaged.

## The discipline to enforce

- **One failing test at a time.** No writing test #2 before test #1 is green.
- **Watch it fail.** A test that has never been red proves nothing. If it passes
  immediately, something is wrong — investigate.
- **Minimum code to green.** Resist implementing the "obvious" full solution.
  Let the tests drive the design out. (Yes, `return "1"` is a legitimate first
  step.)
- **Refactor only on green.** Never refactor with a red bar. Refactoring must not
  change behavior, so the tests must not change while you refactor.
- **One pattern at a time.** At the pattern-menu checkpoint the mob picks ONE
  pattern, implements it, gets green, and weighs the payoff before picking
  another. Never two patterns mid-flight.
- **Tests are code too.** Refactor them (extract helpers, parametrize) once green.

## Timeboxing a 60-minute session

| Time | Activity |
|------|----------|
| 0:00–0:10 | Read the kata brief together. Agree on the *contract* (esp. the edge cases). |
| 0:10–0:40 | Work the kata — invent tests, hit the two checkpoints — rotating drivers. |
| 0:40–0:50 | Attempt the stretch step (FizzBuzz Plus). |
| 0:50–1:00 | Retro: reveal the reference solution, compare, discuss trade-offs. |

## Anti-patterns to watch for

- **Silent driving** — the driver solving it alone. Redirect: "what's the mob
  telling you to type?"
- **Skipping red** — writing code then a test that passes first try.
- **Big-bang implementation** — jumping straight to the final algorithm. Ask for
  the *next failing test* instead.
- **Refactoring on red** — stop, get back to green first.
- **Bikeshedding the contract** — timebox the "what should 0 do?" discussion, make
  a decision, write it as a test, move on.

## Retro prompts

- Where did a test *drive* a design decision we wouldn't have made otherwise?
- Which refactor felt safest? Why? (Answer: the one with the most tests behind it.)
- How did the type-checking / validation differ between the dynamically-typed
  languages (Python, JS) and the statically-typed ones (Java, C#)?
