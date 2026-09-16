namespace Kata;

// ════════════════════════════════════════════════════════════════════
// Stack — production skeleton (C#)
// ════════════════════════════════════════════════════════════════════
// The requirement lives in the story, not here:
//   · Classic:  katas/stack/StackStory.md
//   · Extended: katas/stack/StackExtendedStory.md
//
// TDD rules for this file:
//   · Add NOTHING here until a RED test in StackTests.cs demands it.
//   · Write the MINIMUM that makes the current red test green.
//   · Refactor only on green (prompts for that live in the test file).
//
// You have TWO safety nets in C#, and one of them runs before the other:
// the compiler refuses to build code whose types don't line up, so a whole
// class of mistake never reaches your tests. `dotnet test` builds first,
// then runs — which is why your first RED here is a BUILD error.
//
// (Naming note: .NET already ships System.Collections.Generic.Stack<T>.
// Inside this namespace yours wins. Writing your own is the exercise —
// the real one is what you'd reach for at work.)
public class Stack
{
    // Intentionally empty. Your first failing test tells you which method to
    // create — and the mob picks the names (STEP 1 in StackTests.cs uses
    // `IsEmpty` only as a placeholder).
}
