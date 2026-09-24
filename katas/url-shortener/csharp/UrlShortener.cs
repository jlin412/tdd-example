namespace Kata;

// ════════════════════════════════════════════════════════════════════
// UrlShortener — production skeleton (C#)
// ════════════════════════════════════════════════════════════════════
// The requirement lives in the story, not here:
//   · The product:        katas/url-shortener/UrlShortenerStory.md
//   · Then, the table:    katas/url-shortener/UrlShortenerExtendedStory.md
// This is the BACKEND track. The story says nothing about how to build it —
// a database and a web API are things this track discovers it needs, when a
// test it cannot write says so.
//
// TDD rules for this file:
//   · Add NOTHING here until a RED test in UrlShortenerTests.cs demands it.
//   · Write the MINIMUM that makes the current red test green.
//   · Refactor only on green (prompts for that live in the test file).
//
// One thing that is DIFFERENT about this kata: FizzBuzz and Stack test objects
// that stand alone. This one grows a COLLABORATOR — something it has to ask for
// help. Where that collaborator comes from, and what it looks like, is a design
// decision you have not made yet.
//
// So resist one temptation in particular: do NOT give this class a constructor
// parameter yet. Let a failing test be the thing that forces one.
public class UrlShortener
{
    // Intentionally empty. Your first failing test tells you which members to
    // create — and the mob picks the names (STEP 1 in UrlShortenerTests.cs
    // uses `Shorten` and `Resolve` only as placeholders, though they are the
    // story's own vocabulary and worth keeping).
}
