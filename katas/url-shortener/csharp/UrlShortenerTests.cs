using Xunit;

namespace Kata;

// ════════════════════════════════════════════════════════════════════
// URL Shortener kata — YOU write the tests.
// ════════════════════════════════════════════════════════════════════
// The story is in katas/url-shortener/UrlShortenerStory.md — read it first,
// walkthrough table and all. This is the BACKEND track: the service behind the
// page the story describes. (The page itself is the Angular track, in
// ../angular/. The two tracks share the story and nothing else.)
//
// STEP 0 is the test LIST: before any code, name as many tests as the mob
// can think of. STEP 1 then works one of them through the loop:
//   RED      — write ONE failing test. Run it. Watch it fail for the
//              right reason before writing any production code.
//   GREEN    — write the MINIMUM code in UrlShortener.cs that passes.
//   REFACTOR — on green only. Then loop.
//
// STEP 1's three steps are labelled inside the test. Every test after it is
// yours to invent — the prompts suggest WHAT to think about, not which exact
// cases to write.
//
// Test-type legend:  [positive] [boundary] [edge] [negative]
//
// WHAT IS NEW HERE — FizzBuzz and Stack test objects that stand alone. A
// FizzBuzz, a Stack: you build it, you poke it, done.
// This one is going to grow a COLLABORATOR — something it depends on, that it
// cannot do its job without. Nobody is going to tell you when. A test you
// cannot write is what tells you.
//
// Two habits that matter more than usual because of that:
//   · A test here is a SEQUENCE. You cannot resolve a code you did not
//     shorten first, so every test has arrange steps.
//   · Name what a CALLER sees, not how it is stored. Storage is about to
//     move, and test names that mention it will read as lies afterwards.
public class UrlShortenerTests
{
    // ═════════════════════════════════════════════════════════════════
    // STEP 0 · THE TEST LIST — do this BEFORE you write any code
    // ═════════════════════════════════════════════════════════════════
    // The first move in TDD isn't a test, it's a LIST of the tests you
    // want. As a mob, out loud, name as many as you can — go for QUANTITY
    // now and prune later. You are translating the story into test
    // language: every behavior you can state in English should become a
    // name on this list.
    //
    // A skipped Fact is a todo that `dotnet test` prints back to you.
    // Two to start you off:

    [Fact(Skip = "todo")]
    public void ShorteningAUrlThenResolvingTheCodeGivesBackTheOriginalUrl()
    {
    }

    [Fact(Skip = "todo")]
    public void TwoDifferentUrlsGetTwoDifferentCodes()
    {
    }

    // Now your turn — keep adding. Walk the story's table row by row, then
    // push PAST it into what the table doesn't show. Aim for a dozen or
    // more before anyone touches production code, and cover all four
    // types:
    //   [positive] the round trip; a second link; codes that differ
    //   [boundary] the FIRST case that proves codes are actually generated
    //              rather than hard-coded (how many links does that take?)
    //   [edge]     resolving a code nobody ever minted; shortening the
    //              same URL twice
    //   [negative] a URL that isn't one — and you get to say what that means
    //
    // Then run `dotnet test`, read your list back, pick ONE, and turn it
    // into a real test at STEP 1. Drop the Skip as you promote it.
    //
    // (Keep the list alive: every time you think "what about…?" mid-session,
    //  add a skipped Fact instead of chasing it and losing the red you're on.)

    // ── STEP 1 · [positive] · the round trip · the RED → GREEN → REFACTOR loop, worked ──
    // The assertion ships COMMENTED OUT — uncommenting it is your RED step.
    [Fact]
    public void ShorteningAUrlGivesBackACodeThatResolvesToIt()
    {
        var shortener = new UrlShortener();

        // 1. RED — agree the two method names as a mob (`Shorten` and
        //    `Resolve` are the story's own vocabulary, so they are worth
        //    keeping), uncomment the two lines below, and run `dotnet test`.
        //    It won't even BUILD — the class has no such members:
        //        error CS1061: 'UrlShortener' does not contain a definition
        //        for 'Shorten'
        //    In a statically typed language that IS your red, and you get it
        //    without running a single test.
        //    (Heads up: the reference solution in solutions/ uses Shorten()
        //     and Resolve(), so other names stop it being a literal drop-in.)
        // var code = shortener.Shorten("https://example.com/articles/tdd-mob-katas");
        // Assert.Equal("https://example.com/articles/tdd-mob-katas", shortener.Resolve(code));

        // 2. GREEN — add those members with the MINIMUM to pass. Returning a
        //    constant code and a constant URL is entirely legitimate; let the
        //    next example force more.

        // 3. REFACTOR — generalize: the next test (a SECOND, different URL)
        //    is what makes a hard-coded pair wrong. Let it.
    }

    // ─────────────────────────────────────────────────────────────────
    // From here on, the tests are yours. ONE red at a time.
    // Every test needs an ARRANGE — the shortening that sets up the code you
    // are about to resolve. Watch that setup grow; when it starts repeating,
    // remember that tests are code too.
    // ─────────────────────────────────────────────────────────────────

    // [positive] — a second, different URL. Promote it from your STEP 0 list.
    //   What does this prove that the round trip alone did not? (Look at what
    //   your GREEN step actually wrote before you answer.)

    // [boundary] — do the two codes differ?
    //   Assert it. Then ask the harder question: how is your code being
    //   GENERATED right now? If the honest answer is "a counter", that is
    //   fine for today — but write down what a counter would do in a system
    //   with two servers. You will want that note later.

    // [edge] — resolving a code nobody ever minted.
    //   The story does NOT say what happens. Throw? Hand back nothing?
    //   Something else? DECIDE as a mob, then encode the decision as a test
    //   (Assert.Throws<T> is one tool; there are others). Whatever you pick,
    //   apply it consistently — you will meet this decision again at
    //   checkpoint #4, where it turns into an HTTP status code.

    // [edge] — shortening the SAME URL twice.
    //   The same code back, or a fresh one? The story leaves this open on
    //   purpose and BOTH answers are defensible. Pick one, and notice that
    //   you have just made a promise about your storage that you will have to
    //   keep later.

    // ═════════════════════════════════════════════════════════════════
    // CHECKPOINT #1 · REFACTOR — THE PATTERN MENU  (only when green)
    // ═════════════════════════════════════════════════════════════════
    // Before the menu, the prompt that makes it necessary.
    //
    //   The story says a short link still resolves TOMORROW — after the
    //   service has been restarted, and from a different server.
    //
    //   WRITE THE TEST THAT PROVES YOUR CODE DOES THAT.
    //
    // Take a real run at it. You will not manage it, and the reason you
    // cannot is the entire point of this checkpoint: everything you have
    // stored lives inside one object, in one process, for as long as that
    // process happens to be alive. The story asked for something your design
    // has no room for.
    //
    // That is test pressure. It is the best reason there is to change a
    // design — not taste, not fashion, but a test you cannot write.
    //
    // Then the menu. As a mob, DECIDE which ONE pattern to implement:
    //
    //   (a) PORT + HAND-WRITTEN FAKE — push storage out behind an interface
    //       that the DOMAIN owns (it says "save a link", not "execute SQL"),
    //       and write an in-memory class implementing it. Notice what just
    //       happened: the Dictionary you already had IS that class. You wrote
    //       a test double without importing anything.
    //       Payoff check: every test you already have stays green while the
    //       storage moves out from underneath it. That green bar is the
    //       receipt. (These two are ONE item on purpose — you cannot extract
    //       an interface and stay green with nothing behind it.)
    //   (b) INJECTED CODE GENERATOR — the code you hand out is the only
    //       unpredictable thing in this system, and right now your tests are
    //       working around it. Make it a constructor argument.
    //       Payoff check: a test can now assert the EXACT code — no
    //       StartsWith, no regex, no "just check it's not empty".
    //   (c) SHARED CONTRACT TESTS — one abstract test class that every
    //       implementation of the port must pass.
    //       Be honest today: you have exactly ONE implementation, so this is
    //       ceremony with nothing to weigh against it. Weigh it, write down
    //       WHEN it would pay, and most likely decline it. (Checkpoint #3 is
    //       when.)
    //   (d) TRY-PATTERN INSTEAD OF THROWING — bool TryResolve(string code,
    //       out string url), the idiom .NET uses on its own collections.
    //       Worth it if your unknown-code decision above is making callers
    //       write try/catch for something that isn't exceptional.
    //   (e) DOMAIN ERRORS vs INFRASTRUCTURE ERRORS — the service promises
    //       things in ITS OWN vocabulary, and whatever the storage throws is
    //       translated at the boundary rather than leaking through.
    //       Nothing forces this today. Something will.
    //   (f) SHORT CODE AS A VALUE OBJECT — a type instead of a bare string,
    //       giving the alphabet, the length and the case-sensitivity question
    //       exactly one home instead of being spread across call sites.
    //
    // COACH'S RULE — ONE PATTERN AT A TIME:
    //   implement the chosen pattern → all green → ask "did it pay for
    //   itself? do we want another?" → only then pick the next item.
    //   Never two patterns mid-flight. Structural changes ((a), (f)) are
    //   still driven test-first, and the tests you already have must stay
    //   green throughout — that's your proof the refactor is safe.
    //
    // DISCUSS: which patterns are RIGHT here — and which are over-engineering?
    // Options, not obligations.
    // (Weigh and likely DECLINE: a MOCKING LIBRARY — you just wrote a working
    //  fake in about twelve lines. What would Moq or NSubstitute have added,
    //  and what would they have hidden? A configured mock can only ever tell
    //  you what you told it; a fake you wrote is a real implementation you can
    //  actually run things against. Hold that thought until checkpoint #3.
    //  A GENERIC IRepository<T> — it cannot express "is this code already
    //  taken?", which is the one question this domain genuinely needs to ask.
    //  ASYNC EVERYWHERE — Task<string> on every method is what you'd ship at
    //  work; it is declined here to keep the loop tight. Name it as a
    //  decision, not an oversight.
    //  UNIT OF WORK / transactions — one table, one write. Parking lot.)
    //
    // Tests are code too: by now the arrange steps repeat. Extract a helper
    // (a ShortenerWith(...)?) — and notice that a good helper makes the NEXT
    // test easier to write, which is the real payoff.

    // ═════════════════════════════════════════════════════════════════
    // CHECKPOINT #2 · NEGATIVE & EDGE — pin the contract down
    // ═════════════════════════════════════════════════════════════════
    // Back to RED work: the corners the story deliberately left open. ONE
    // failing test at a time.

    // [negative] — a URL that isn't one.
    //   Empty string? Whitespace? "banana"? DECIDE what this service accepts,
    //   then test it. Careful before you reach for the framework:
    //   Uri.TryCreate(s, UriKind.Absolute, out _) cheerfully accepts
    //   "javascript:alert(1)" and "mailto:bob@example.com". It will not make
    //   this decision for you — it only tells you the shape is legal.
    //   And ask the question underneath: is validating URLs even THIS
    //   object's job, or does it belong to whoever calls it?

    // [edge] — are codes case-sensitive?
    //   Is "K3F9Q2" the same link as "k3f9q2"? DECIDE and test it. Then ask
    //   the question that actually matters: WHERE is that decision written
    //   down? If the honest answer is "in whatever my storage happens to do",
    //   you have not made a decision — you have inherited one.

    // [negative] — THE ONE THAT MATTERS. Make your generator collide.
    //   Whatever mints your codes, arrange for it to hand out the SAME code
    //   twice, and shorten two different URLs with it.
    //   Now look at your in-memory store. If it does what almost every
    //   first draft does — dict[code] = url — then it just SILENTLY
    //   OVERWROTE a link. A URL is gone. Nothing threw. Nothing turned red.
    //   Every test you have written today still passes.
    //   That is a data-loss bug, in code you wrote, that your whole suite
    //   sailed past. Write the test that catches it. Then decide what a
    //   store SHOULD do when a code is already taken — reject it? retry?
    //   and make your fake do that.
    //
    //   Sit with one question before you move on, because checkpoint #3 is
    //   built entirely on it:
    //       You just decided how storage behaves, and then you made your own
    //       fake behave that way. What, exactly, would tell you if a REAL
    //       database disagreed with you?

    // REFACTOR (on green):
    //   · Does UrlShortener read as domain vocabulary — minting and resolving
    //     links — or does it still read as bookkeeping over a dictionary?
    //   · Is each contract decision above expressed in ONE place, or repeated
    //     at every entry point?

    // ═════════════════════════════════════════════════════════════════
    // CHECKPOINT #3 · IS IT TRUE? — the links have to survive
    // ═════════════════════════════════════════════════════════════════
    // The story promised on day one that a short link keeps working after a
    // restart, and from a different server. Your links live in a store YOU
    // wrote, in memory, in one process. So:
    //
    //   The compiler proved your in-memory store IMPLEMENTS the interface.
    //   What proved it BEHAVES LIKE THE REAL THING?
    //
    // Nothing has. Storage becomes a real database: SQLite, with nothing to
    // install. SqliteTestDatabase.cs is GIVEN — a live connection and a table.
    // Read its header, and read its schema: one line of it is going to matter.
    // The SQL is plain ADO.NET, and this is the whole idiom:
    //
    //     using var command = connection.CreateCommand();
    //     command.CommandText = "INSERT INTO links (code, url) VALUES ($code, $url)";
    //     command.Parameters.AddWithValue("$code", code);
    //     command.Parameters.AddWithValue("$url", url);
    //     command.ExecuteNonQuery();          // or ExecuteScalar() to read one value
    //
    // That's the idiom, not the design. In order:
    //   1. PREDICT, out loud, before writing anything. You are about to run
    //      your storage tests against a second implementation of the same
    //      interface. Which of them will pass? Put the list on the whiteboard
    //      BEFORE the bar goes red — it only counts if it was written first.
    //   2. Make those tests runnable against EITHER store. What is the least
    //      you can change so one test class asks its questions of any
    //      repository? (Look at menu item (c) again. And: does xUnit run
    //      [Fact]s it inherits from a base class?)
    //   3. Write the SQLite repository behind the same interface.
    //   4. Run it, compare against the prediction, and deal with what you find
    //      — one red at a time.
    //
    // Two follow-ups, each worth a red bar of its own:
    //   · Whatever SQLite throws when it objects — should a caller of your
    //     shortener ever see it? What would that caller have to know about?
    //   · Resolve a code in the wrong case against BOTH stores. If they
    //     disagree, where was your case decision actually written down?

    // ═════════════════════════════════════════════════════════════════
    // CHECKPOINT #4 · THE FRONT DOOR — how does the page reach you?
    // ═════════════════════════════════════════════════════════════════
    // Reread the story: people shorten links on a web PAGE, in a browser. Your
    // shortener works — for anyone who can construct a C# object in your
    // process. A browser can't. What does the page need to be able to ask,
    // and what should it hear back — for a new link, for a code nobody
    // minted, for a URL that isn't one? Decide that conversation as a mob:
    // which requests, which status codes, which JSON.
    //
    // One structural hint, because it is what makes this pleasant: put the
    // routes in an EXTENSION METHOD, not a Program.cs —
    //
    //     public static void MapUrlEndpoints(this IEndpointRouteBuilder app) { ... }
    //
    // Production owns the routes, and a real host and a TEST host can both
    // mount them. The test host is a real server that never opens a port:
    //
    //     var builder = WebApplication.CreateBuilder();
    //     builder.WebHost.UseTestServer();
    //     // ...register what UrlShortener needs...
    //     var app = builder.Build();
    //     app.MapUrlEndpoints();
    //     await app.StartAsync();
    //     var client = app.GetTestClient();   // a real HttpClient
    //
    // (Everything that needs is already referenced in the .csproj.)
    //
    // BEFORE you write a test here, decide what belongs here. You already
    // prove that two URLs get different codes, that a blank URL is refused,
    // that a repeated code never loses a link. How many of those should be
    // proven AGAIN through HTTP? Be able to say why before you answer.
    //
    // Two tests only a real server can pass — write your own versions:
    //   [negative] a misspelled route. What comes back, and what decided it?
    //   [negative] a body that isn't JSON at all. Whose code rejects it?
    //
    // Then two experiments, each settling an argument that otherwise runs on
    // taste:
    //   · Misspell your route prefix by ONE character and run everything.
    //     Which suites notice — and which stay green while the service is
    //     perfect and completely unreachable?
    //   · Count your tests per suite — HTTP, service, storage — and time each
    //     tier. That shape has a name. What does the clock say about how many
    //     of the slow ones you can afford?

    // ── WHAT NEXT · the extended story, the table ─────────────────────
    //   Shipped the backend? The product owner wants a table of every link,
    //   newest first: katas/url-shortener/UrlShortenerExtendedStory.md.
    //   Before you open it, predict which of your suites will need a new
    //   test — and which layer will need to know what time it is.
}
