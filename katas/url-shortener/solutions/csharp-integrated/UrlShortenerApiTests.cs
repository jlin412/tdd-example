using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kata;

// URL Shortener kata — reference solution 2 of 2: INTEGRATED.
// The ENTIRE test suite. See ../Solution2-Integrated.md for the order and why.
//
// One file. Every test drives a real in-process server, over real HTTP, against
// a real SQLite database. Routing, model binding, JSON, the service, the SQL
// and the constraints are all genuinely exercised on every single line below.
//
// Compare with solution 1, which needs four test classes to cover the same
// requirements: a service suite against a fake, a contract suite run twice, and
// an HTTP suite.
//
// WHAT IS DELIBERATELY ABSENT: any fake store. There is nothing in this
// solution that can lie to you about how storage behaves, because there is
// nothing standing in for storage. The failure mode solution 1 spends a whole
// checkpoint on — a fake that silently overwrites a taken code while the
// database refuses it — is not mitigated here. It is unreachable.
//
// WHAT IS PRESENT, AND WHY: two doubles, each introduced for one reason. A
// stub generator, because a code collision is a state you cannot reach by
// asking a real random generator nicely. A fake clock (the extended story),
// because "ordered by creation time, not by arrival" can only be shown with a
// clock that disagrees with the order requests arrived in. Each is the test
// that justifies a seam, and it is worth being able to say so in one sentence.
// Everything else runs against the real thing.
public class UrlShortenerApiTests : IAsyncLifetime
{
    // ── Test doubles, and the rule for having them ────────────────────
    // A double needs a test that cannot be written without it. These three
    // qualify; nothing else in the solution does.

    /// Hands out exactly these codes, in order. For pinning a collision.
    private sealed class FixedCodes : IShortCodeGenerator
    {
        private readonly Queue<string> _codes;
        public FixedCodes(params string[] codes) => _codes = new Queue<string>(codes);
        public string Next() => _codes.Dequeue();
    }

    /// A clock that says whatever the test tells it to. For the table, whose
    /// order is by creation time — something the real clock will never get
    /// "wrong" on demand.
    private sealed class FakeClock : TimeProvider
    {
        private DateTimeOffset _now;
        public FakeClock(DateTimeOffset now) => _now = now;
        public override DateTimeOffset GetUtcNow() => _now;
        public void Advance(TimeSpan by) => _now += by;
    }

    private static readonly DateTimeOffset Noon = new(2026, 9, 24, 12, 0, 0, TimeSpan.Zero);

    /// A deliberately tiny code space, so concurrent requests genuinely
    /// contend instead of scattering across a billion possibilities.
    private sealed class SmallCodeSpace : IShortCodeGenerator
    {
        private readonly int _size;
        public SmallCodeSpace(int size) => _size = size;
        public string Next() => $"c{RandomNumberGenerator.GetInt32(_size)}";
    }

    // The real database. SqliteTestDatabase holds one open connection and
    // creates the schema; xUnit builds a fresh instance of this class per test
    // method, so every test below gets a pristine, empty database — which is
    // also what makes it safe for xUnit to run test classes in parallel.
    private readonly SqliteTestDatabase _database = new();

    private WebApplication _app = null!;
    private HttpClient _client = null!;

    // Most tests don't care what the codes are, so the default is the REAL
    // generator. Tests that need to control minting say so, loudly, in their
    // own body — see RestartTheApplication below.
    public Task InitializeAsync() => StartTheApplication(new RandomShortCodeGenerator());

    private async Task StartTheApplication(IShortCodeGenerator generator, TimeProvider? clock = null)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddSingleton(generator);
        // The connection STRING, so the service opens its own connection per
        // operation. Sharing one SqliteConnection across requests is not
        // thread-safe — see the note in UrlShortener's constructor, and the
        // concurrency test that found it.
        builder.Services.AddSingleton(
            new UrlShortener(_database.ConnectionString, generator, clock ?? TimeProvider.System));

        _app = builder.Build();
        _app.MapUrlEndpoints();
        await _app.StartAsync();
        _client = _app.GetTestClient();
    }

    /// Tears the whole web application down and builds a new one over the SAME
    /// database. Named for what it does, because in two tests below the restart
    /// is not setup — it is the thing being tested.
    private async Task RestartTheApplication(
        IShortCodeGenerator mintingCodesFrom, TimeProvider? tellingTimeBy = null)
    {
        await _app.DisposeAsync();
        await StartTheApplication(mintingCodesFrom, tellingTimeBy);
    }

    public async Task DisposeAsync()
    {
        await _app.DisposeAsync();
        _database.Dispose();
    }

    // ── Talking to the API ────────────────────────────────────────────
    // These deliberately do NOT use ShortenRequest / LinkResponse /
    // ResolveResponse — the production records. A test that deserializes into
    // the very type it is meant to be pinning cannot detect a change to it:
    // rename LinkResponse.Code tomorrow and every assertion would still
    // compile and still pass while the published JSON silently changed shape.
    //
    // So the wire contract is spelled out here, by hand, in the only place it
    // should live in a test: an anonymous object going out, and JSON property
    // names coming back. Note what that immediately makes visible and the
    // round-trip through production types hid — the wire is camelCase.

    private Task<HttpResponseMessage> Post(string url) =>
        _client.PostAsJsonAsync("/links", new { url });

    private static async Task<string> FieldOf(HttpResponseMessage response, string field) =>
        (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty(field).GetString()!;

    private static Task<string> CodeOf(HttpResponseMessage response) => FieldOf(response, "code");

    private async Task<string> ResolveUrlAt(string code) =>
        await FieldOf(await _client.GetAsync($"/links/{code}"), "url");

    /// The table, read off the wire by property name — code, url, createdAt.
    private async Task<(string Code, string Url, DateTimeOffset CreatedAt)[]> TheTable()
    {
        var response = await _client.GetAsync("/links");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<JsonElement>())
            .EnumerateArray()
            .Select(row => (
                row.GetProperty("code").GetString()!,
                row.GetProperty("url").GetString()!,
                row.GetProperty("createdAt").GetDateTimeOffset()))
            .ToArray();
    }

    // ── 1. the round trip ─────────────────────────────────────────────
    // The first test written, and it covers more ground than solution 1's
    // first six combined: a real request is routed, bound, handled, written to
    // SQL, read back, serialized and returned.
    [Fact] // [positive] (R1/R2/R15/R16)
    public async Task AShortenedUrlCanBeResolvedAgain()
    {
        var created = await Post("https://example.com/articles/tdd-mob-katas");
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var code = await CodeOf(created);
        Assert.Equal($"/links/{code}", created.Headers.Location?.ToString());

        Assert.Equal("https://example.com/articles/tdd-mob-katas", await ResolveUrlAt(code));
    }

    // ── 2. codes are actually generated ───────────────────────────────
    // Note the assertion: codes DIFFER. Never what they are. Reading the code
    // out of the response instead of pinning a literal is what lets the real
    // generator run — no stub is needed for this or any test below except the
    // two that are explicitly about minting.
    [Fact] // [boundary] (R3)
    public async Task TwoDifferentUrlsGetTwoDifferentCodes()
    {
        var first = await CodeOf(await Post("https://example.com/first"));
        var second = await CodeOf(await Post("https://example.com/second"));

        Assert.NotEqual(first, second);
    }

    [Fact] // [positive] (R3) — and both still resolve to the right place
    public async Task EachCodeResolvesToItsOwnUrl()
    {
        var first = await CodeOf(await Post("https://example.com/first"));
        var second = await CodeOf(await Post("https://example.com/second"));

        Assert.Equal("https://example.com/first", await ResolveUrlAt(first));
        Assert.Equal("https://example.com/second", await ResolveUrlAt(second));
    }

    // ── 3. the contract decisions ─────────────────────────────────────
    [Fact] // [edge] (R5/R17)
    public async Task ACodeNobodyMintedIs404()
    {
        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync("/links/nope99")).StatusCode);
    }

    [Theory] // [negative] (R7/R17)
    [InlineData("")]
    [InlineData("   ")]
    public async Task AUrlThatIsBlankIs400(string notAUrl)
    {
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(notAUrl)).StatusCode);
    }

    [Fact] // [edge] (R6/R18) — the idempotency decision: a FRESH code each time
    public async Task TheSameUrlTwiceMintsAFreshCode()
    {
        var url = "https://example.com/same/every/time";

        var first = await Post(url);
        var second = await Post(url);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Created, second.StatusCode);
        Assert.NotEqual(await CodeOf(first), await CodeOf(second));
    }

    [Fact] // [edge] (R8) — case sensitivity, decided once because there is one store
    public async Task CodesAreCaseSensitive()
    {
        var code = await CodeOf(await Post("https://example.com/first"));

        var shouted = await _client.GetAsync($"/links/{code.ToUpperInvariant()}");

        Assert.Equal(HttpStatusCode.NotFound, shouted.StatusCode);
    }

    // ── 4. minting, where the one double earns its place ──────────────
    [Fact] // [negative] (R9) — a collision must not lose a link
    public async Task ARepeatedCodeMintsAnotherRatherThanOverwriting()
    {
        // The real generator will not collide on demand, and waiting for it to
        // is not a test strategy. This is the state that earns a seam.
        await RestartTheApplication(mintingCodesFrom: new FixedCodes("abc123", "abc123", "xyz789"));

        var first = await CodeOf(await Post("https://example.com/first"));
        var second = await CodeOf(await Post("https://example.com/second"));

        Assert.Equal("abc123", first);
        Assert.Equal("xyz789", second);

        // The proof that nothing was quietly overwritten. Note that this is
        // asserting against the REAL database's PRIMARY KEY, not against a
        // fake's promise to behave like one.
        Assert.Equal("https://example.com/first", await ResolveUrlAt("abc123"));
        Assert.Equal("https://example.com/second", await ResolveUrlAt("xyz789"));
    }

    [Fact] // [edge] (R9) — a generator that will never cooperate
    public async Task GivingUpBeatsLoopingForeverWhenEveryCodeIsTaken()
    {
        await RestartTheApplication(mintingCodesFrom:
            new FixedCodes("abc123", "abc123", "abc123", "abc123", "abc123", "abc123"));
        await Post("https://example.com/first");

        // The service gives up — and NOTHING maps InvalidOperationException to
        // a status code, so it escapes the endpoint entirely.
        //
        // This assertion is worth reading twice, because writing it was how the
        // gap was found. Solution 1's isolated suite asserts the same give-up
        // behaviour and stops there, perfectly green, never asking what a
        // caller would see. Only a test that speaks HTTP can ask that question,
        // and the answer is: nobody decided. In production ASP.NET Core turns
        // this into a bare 500; TestServer rethrows it to the caller instead,
        // which is why the test reads like this rather than asserting a status.
        //
        // That divergence between the test host and production is itself the
        // sort of thing this suite exists to surface — and the right follow-up
        // is a red test demanding a deliberate status code, not a nicer assert.
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => Post("https://example.com/second"));
    }

    // ── 5. concurrency, which only an integrated test can ask about ───
    [Fact] // [negative] (R9) — the race, for real this time
    public async Task ConcurrentRequestsNeverIssueTheSameCodeTwice()
    {
        // The collision test above simulates contention with a scripted
        // generator. This one causes it: fifty requests in flight at once,
        // against a code space small enough that they genuinely fight over it.
        //
        // What makes this answerable at all is that the whole stack is real —
        // a fake repository can be made to behave however you imagined under
        // concurrency, which is exactly the assurance you do not want here.
        await RestartTheApplication(mintingCodesFrom: new SmallCodeSpace(512));

        var urls = Enumerable.Range(0, 50).Select(i => $"https://example.com/{i}").ToArray();

        var responses = await Task.WhenAll(urls.Select(Post));

        Assert.All(responses, r => Assert.Equal(HttpStatusCode.Created, r.StatusCode));

        var codes = await Task.WhenAll(responses.Select(CodeOf));

        // No code was handed to two callers. A service that trusted its
        // generator, or a store that overwrote on conflict, fails here.
        Assert.Equal(codes.Length, codes.Distinct().Count());

        // And every link points where its own request said it should.
        for (var i = 0; i < urls.Length; i++)
        {
            Assert.Equal(urls[i], await ResolveUrlAt(codes[i]));
        }
    }

    // ── 6. what only the transport can catch ──────────────────────────
    [Fact] // [negative] — routing is real
    public async Task AMisspelledRouteIs404()
    {
        Assert.Equal(HttpStatusCode.NotFound, (await _client.PostAsync("/lnks", null)).StatusCode);
    }

    [Fact] // [negative] — model binding is real
    public async Task AMalformedJsonBodyIs400WithoutReachingTheService()
    {
        var response = await _client.PostAsync(
            "/links", new StringContent("{ not json", System.Text.Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── 7. what only the real database can catch ──────────────────────
    [Fact] // [positive] (R4) — persistence, proven rather than promised
    public async Task LinksSurviveTheProcessThatCreatedThem()
    {
        var code = await CodeOf(await Post("https://example.com/first"));

        // The restart IS the assertion here: a brand new application, over the
        // same database, resolving a code the previous one minted.
        await RestartTheApplication(mintingCodesFrom: new RandomShortCodeGenerator());

        Assert.Equal("https://example.com/first", await ResolveUrlAt(code));
    }

    // ── 8. the extended story: the table ──────────────────────────────
    [Fact] // [positive] (R15/R24) — a create answers with the whole link
    public async Task CreatingALinkAnswersWithTheWholeLink()
    {
        // Added after the end-to-end smoke test put the page's contract beside
        // this API: the page promises callers the url and createdAt of what it
        // made, and a bare {code} could not keep that promise.
        await RestartTheApplication(new FixedCodes("abc123"), tellingTimeBy: new FakeClock(Noon));

        var created = await Post("https://example.com/first");

        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var link = await created.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("abc123", link.GetProperty("code").GetString());
        Assert.Equal("https://example.com/first", link.GetProperty("url").GetString());
        Assert.Equal(Noon, link.GetProperty("createdAt").GetDateTimeOffset());
    }

    [Fact] // [edge] (R21/R23) — an empty table is a list, not a 404
    public async Task TheTableIsEmptyBeforeAnythingIsShortened()
    {
        Assert.Empty(await TheTable());
    }

    [Fact] // [positive] (R20/R21/R23) — newest first, stamped with when it was made
    public async Task TheTableListsEveryLinkNewestFirst()
    {
        var clock = new FakeClock(Noon);
        await RestartTheApplication(new RandomShortCodeGenerator(), tellingTimeBy: clock);

        var first = await CodeOf(await Post("https://example.com/first"));
        clock.Advance(TimeSpan.FromMinutes(1));
        var second = await CodeOf(await Post("https://example.com/second"));

        Assert.Equal(
            new[]
            {
                (second, "https://example.com/second", Noon.AddMinutes(1)),
                (first, "https://example.com/first", Noon),
            },
            await TheTable());
    }

    [Fact] // [boundary] (R22) — a clock can repeat itself, just like a generator
    public async Task TwoLinksCreatedInTheSameInstantListTheLaterFirst()
    {
        await RestartTheApplication(new RandomShortCodeGenerator(), tellingTimeBy: new FakeClock(Noon));

        var first = await CodeOf(await Post("https://example.com/first"));
        var second = await CodeOf(await Post("https://example.com/second"));

        Assert.Equal(new[] { second, first }, (await TheTable()).Select(row => row.Code));
    }

    [Fact] // [positive] (R4/R21) — ordered by creation time, not by arrival
    public async Task TheTableFollowsCreationTimeEvenWhenServersClocksDisagree()
    {
        // The test that earns the clock seam. A link made on a server whose
        // clock runs ahead, then one made after a restart on a server whose
        // clock runs behind: the second ARRIVED later, but was stamped earlier,
        // and the table follows the stamp. No real clock does this on demand.
        await RestartTheApplication(new RandomShortCodeGenerator(),
            tellingTimeBy: new FakeClock(Noon.AddMinutes(5)));
        var madeAhead = await CodeOf(await Post("https://example.com/ahead"));

        await RestartTheApplication(new RandomShortCodeGenerator(), tellingTimeBy: new FakeClock(Noon));
        var madeBehind = await CodeOf(await Post("https://example.com/behind"));

        Assert.Equal(new[] { madeAhead, madeBehind }, (await TheTable()).Select(row => row.Code));
    }
}
