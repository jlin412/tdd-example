using Xunit;

namespace Kata;

// URL Shortener kata — reference test suite for the HTTP layer (R15–R18).
// See UrlShortenerHttpStory.md.
//
// COUNT THESE TESTS. There are six, and the service suite next door has eight
// while the shared contract suite has six that each run twice. That shape — a
// lot at the bottom, few at the top — is the test pyramid, and the story asks
// the mob to derive it rather than be shown it.
//
// Note what is NOT tested here, on purpose:
//   · that two URLs get different codes        (UrlShortenerTests — R3)
//   · that a blank URL is refused              (UrlShortenerTests — R7)
//   · that a repeated code doesn't lose a link (contract suite — R9/R12)
//
// Every one of those is already proven, closer to the code that decides it.
// Re-proving them through a handler would be a slower, more indirect copy of a
// test that already exists. What is genuinely new at this level is the MAPPING:
// an outcome goes in, a status code comes out.
public class UrlEndpointsTests
{
    private sealed class StubGenerator : IShortCodeGenerator
    {
        private readonly Queue<string> _codes;
        public StubGenerator(params string[] codes) => _codes = new Queue<string>(codes);
        public string Next() => _codes.Dequeue();
    }

    private static UrlShortener ShortenerThatMints(params string[] codes) =>
        new(new InMemoryUrlRepository(), new StubGenerator(codes));

    [Fact] // [positive] (R15)
    public void ShorteningAUrlIs201WithTheCodeAndALocation()
    {
        var outcome = UrlEndpoints.Shorten(
            "https://example.com/articles/tdd-mob-katas", ShortenerThatMints("abc123"));

        Assert.Equal(201, outcome.Status);
        Assert.Equal("abc123", outcome.Body);
        Assert.Equal("/links/abc123", outcome.Location);
    }

    [Fact] // [positive] (R16)
    public void ResolvingAKnownCodeIs200WithTheOriginalUrl()
    {
        var shortener = ShortenerThatMints("abc123");
        UrlEndpoints.Shorten("https://example.com/first", shortener);

        var outcome = UrlEndpoints.Resolve("abc123", shortener);

        Assert.Equal(200, outcome.Status);
        Assert.Equal("https://example.com/first", outcome.Body);
    }

    [Fact] // [edge] (R17) — the unknown-code decision from story 1, three sessions later
    public void ResolvingACodeNobodyMintedIs404()
    {
        var outcome = UrlEndpoints.Resolve("nope99", ShortenerThatMints("abc123"));

        Assert.Equal(404, outcome.Status);
    }

    [Theory] // [negative] (R17)
    [InlineData("")]
    [InlineData("   ")]
    public void ShorteningSomethingThatIsNotAUrlIs400(string notAUrl)
    {
        var outcome = UrlEndpoints.Shorten(notAUrl, ShortenerThatMints("abc123"));

        Assert.Equal(400, outcome.Status);
    }

    [Fact] // [edge] (R18) — the callback to the R6 decision
    public void ShorteningAUrlThatIsAlreadyKnownIs201Again()
    {
        // Because R6 chose "a fresh code each time", this really is a creation.
        // A mob that chose "hand back the same code" should expect 200 here and
        // no Location header — and this test is where that choice finally shows
        // up in something a customer can see.
        var shortener = ShortenerThatMints("abc123", "xyz789");
        var url = "https://example.com/same/every/time";

        var first = UrlEndpoints.Shorten(url, shortener);
        var second = UrlEndpoints.Shorten(url, shortener);

        Assert.Equal(201, first.Status);
        Assert.Equal(201, second.Status);
        Assert.NotEqual(first.Body, second.Body);
    }
}
