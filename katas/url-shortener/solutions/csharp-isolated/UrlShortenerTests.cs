using Xunit;

namespace Kata;

// URL Shortener kata — reference test suite for the SERVICE (R1–R9, R20).
// This is what a mob's suite might look like after checkpoint #2, plus the one
// service-level test the extended story added.
//
// Every test here runs against the in-memory store, and that is a deliberate
// choice, not laziness: these tests are about the SHORTENER's rules — minting,
// resolving, validating, retrying — and none of those rules care where links
// are kept. Pointing them at SQLite would make them slower and no more true.
//
// The tests that DO care live in UrlRepositoryContractTests.cs, and they run
// against both stores. Knowing which suite a new test belongs in is the skill
// this kata is really teaching.
public class UrlShortenerTests
{
    // Menu item (b) paying for itself. The production generator is random; this
    // one is not, so a test can name the exact code it expects instead of
    // asserting something vague about its shape.
    private sealed class StubGenerator : IShortCodeGenerator
    {
        private readonly Queue<string> _codes;

        public StubGenerator(params string[] codes) => _codes = new Queue<string>(codes);

        public string Next() =>
            _codes.Count > 0 ? _codes.Dequeue() : throw new InvalidOperationException("stub ran out of codes");
    }

    // The extended story's seam. .NET 8 ships TimeProvider as the abstraction
    // for "now"; overriding one method is the whole fake, so — as with the
    // repository — nothing is imported to get a double.
    private sealed class FakeClock : TimeProvider
    {
        private DateTimeOffset _now;
        public FakeClock(DateTimeOffset start) => _now = start;
        public override DateTimeOffset GetUtcNow() => _now;
        public void Advance(TimeSpan by) => _now += by;
    }

    private static readonly DateTimeOffset Noon = new(2026, 9, 24, 12, 0, 0, TimeSpan.Zero);

    private static UrlShortener ShortenerThatMints(params string[] codes) =>
        new(new InMemoryUrlRepository(), new StubGenerator(codes), new FakeClock(Noon));

    [Fact] // [positive] (R1/R2) — the round trip, the same test STEP 1 works through
    public void ShorteningAUrlGivesBackACodeThatResolvesToIt()
    {
        var shortener = ShortenerThatMints("abc123");

        var code = shortener.Shorten("https://example.com/articles/tdd-mob-katas").Code;

        Assert.Equal("abc123", code);
        Assert.Equal("https://example.com/articles/tdd-mob-katas", shortener.Resolve(code));
    }

    [Fact] // [boundary] (R3) — one link proves nothing; two is the smallest case that does
    public void TwoDifferentUrlsGetTwoDifferentCodes()
    {
        var shortener = ShortenerThatMints("abc123", "xyz789");

        var first = shortener.Shorten("https://example.com/first").Code;
        var second = shortener.Shorten("https://example.com/second").Code;

        Assert.NotEqual(first, second);
        Assert.Equal("https://example.com/first", shortener.Resolve(first));
        Assert.Equal("https://example.com/second", shortener.Resolve(second));
    }

    [Fact] // [edge] (R5)
    public void ResolvingACodeThatWasNeverIssuedThrows()
    {
        var shortener = ShortenerThatMints("abc123");

        Assert.Throws<UnknownCodeException>(() => shortener.Resolve("nope99"));
    }

    [Fact] // [edge] (R6) — a contract DECISION, not an accident
    public void ShorteningTheSameUrlTwiceMintsAFreshCode()
    {
        var shortener = ShortenerThatMints("abc123", "xyz789");
        var url = "https://example.com/same/every/time";

        var first = shortener.Shorten(url).Code;
        var second = shortener.Shorten(url).Code;

        Assert.NotEqual(first, second);
        // Both codes work. One URL, two front doors — said out loud, in a test,
        // because the other answer was equally defensible.
        Assert.Equal(url, shortener.Resolve(first));
        Assert.Equal(url, shortener.Resolve(second));
    }

    [Theory] // [negative] (R7)
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public void RefusesAUrlThatIsBlank(string notAUrl)
    {
        var shortener = ShortenerThatMints("abc123");

        Assert.Throws<ArgumentException>(() => shortener.Shorten(notAUrl));
    }

    [Fact] // [negative] (R9) — the collision, handled rather than trusted
    public void MintsAnotherCodeWhenTheGeneratorRepeatsItself()
    {
        // The generator hands out "abc123" twice in a row. A service that
        // trusted it would lose the first link; this one asks the store, is
        // refused, and tries again.
        var shortener = ShortenerThatMints("abc123", "abc123", "xyz789");

        var first = shortener.Shorten("https://example.com/first").Code;
        var second = shortener.Shorten("https://example.com/second").Code;

        Assert.Equal("abc123", first);
        Assert.Equal("xyz789", second);
        // The proof that nothing was quietly overwritten:
        Assert.Equal("https://example.com/first", shortener.Resolve("abc123"));
        Assert.Equal("https://example.com/second", shortener.Resolve("xyz789"));
    }

    [Fact] // [edge] (R9) — a generator that will never cooperate
    public void GivesUpRatherThanLoopingForeverWhenEveryCodeIsTaken()
    {
        var shortener = ShortenerThatMints("abc123", "abc123", "abc123", "abc123", "abc123");
        shortener.Shorten("https://example.com/first");

        Assert.Throws<InvalidOperationException>(
            () => shortener.Shorten("https://example.com/second"));
    }

    [Fact] // [edge] (R8)
    public void TreatsCodesAsCaseSensitive()
    {
        var shortener = ShortenerThatMints("abc123");
        shortener.Shorten("https://example.com/first");

        Assert.Throws<UnknownCodeException>(() => shortener.Resolve("ABC123"));
    }

    [Fact] // [positive] (R20/R21) — the table, as far as the SERVICE is concerned
    public void ListsEveryLinkNewestFirstWithTheMomentItWasCreated()
    {
        // Only the stamping is the service's job; HOW the list is ordered is
        // a storage promise, proven against both stores in the contract suite.
        var clock = new FakeClock(Noon);
        var shortener = new UrlShortener(
            new InMemoryUrlRepository(), new StubGenerator("abc123", "xyz789"), clock);

        var first = shortener.Shorten("https://example.com/first");
        clock.Advance(TimeSpan.FromMinutes(1));
        var second = shortener.Shorten("https://example.com/second");

        // Each Shorten hands back the link it made, stamped with the clock's
        // time — and the list agrees with it exactly.
        Assert.Equal(new Link("abc123", "https://example.com/first", Noon), first);
        Assert.Equal(new Link("xyz789", "https://example.com/second", Noon.AddMinutes(1)), second);
        Assert.Equal(new[] { second, first }, shortener.List());
    }
}
