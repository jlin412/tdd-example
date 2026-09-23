using System.Net;
using System.Net.Http.Json;
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
// an HTTP suite. Twenty-four test methods there; fourteen here.
//
// WHAT IS DELIBERATELY ABSENT: any fake store. There is nothing in this
// solution that can lie to you about how storage behaves, because there is
// nothing standing in for storage. The failure mode solution 1 spends a whole
// story on — a fake that silently overwrites a taken code while the database
// refuses it — is not mitigated here. It is unreachable.
//
// WHAT IS PRESENT, AND WHY: a stub generator. Exactly one double, introduced
// for exactly one reason — "a code collision must not lose a link" is a state
// you cannot reach by asking a real random generator nicely. That is the test
// that justifies a seam, and it is worth being able to say so in one sentence.
// Everything else runs against the real thing.
public class UrlShortenerApiTests : IAsyncLifetime
{
    private sealed class FixedCodes : IShortCodeGenerator
    {
        private readonly Queue<string> _codes;
        public FixedCodes(params string[] codes) => _codes = new Queue<string>(codes);
        public string Next() => _codes.Dequeue();
    }

    // The real database. SqliteTestDatabase holds one open connection and
    // creates the schema; xUnit builds a fresh instance of this class per test
    // method, so every test below gets a pristine, empty database for free.
    private readonly SqliteTestDatabase _database = new();

    private WebApplication _app = null!;
    private HttpClient _client = null!;

    // Most tests don't care what the codes are, so the default is the REAL
    // generator. Tests that need to control minting call StartWith(...) instead.
    public Task InitializeAsync() => StartWith(new RandomShortCodeGenerator());

    private async Task StartWith(IShortCodeGenerator generator)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddSingleton(_database.Connection);
        builder.Services.AddSingleton(generator);
        builder.Services.AddSingleton<UrlShortener>();

        _app = builder.Build();
        _app.MapUrlEndpoints();
        await _app.StartAsync();
        _client = _app.GetTestClient();
    }

    private async Task RestartWith(IShortCodeGenerator generator)
    {
        await _app.DisposeAsync();
        await StartWith(generator);
    }

    public async Task DisposeAsync()
    {
        await _app.DisposeAsync();
        _database.Dispose();
    }

    private Task<HttpResponseMessage> Post(string url) =>
        _client.PostAsJsonAsync("/links", new ShortenRequest(url));

    private static async Task<string> CodeOf(HttpResponseMessage response) =>
        (await response.Content.ReadFromJsonAsync<ShortenResponse>())!.Code;

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

        var resolved = await _client.GetAsync($"/links/{code}");
        Assert.Equal(HttpStatusCode.OK, resolved.StatusCode);
        Assert.Equal(
            "https://example.com/articles/tdd-mob-katas",
            (await resolved.Content.ReadFromJsonAsync<ResolveResponse>())!.Url);
    }

    // ── 2. codes are actually generated ───────────────────────────────
    // Note the assertion: codes DIFFER. Never what they are. Reading the code
    // out of the response instead of pinning a literal is what lets the real
    // generator run — no stub needed for this or any test below except the
    // collision one.
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

        Assert.Equal("https://example.com/first",
            (await (await _client.GetAsync($"/links/{first}")).Content
                .ReadFromJsonAsync<ResolveResponse>())!.Url);
        Assert.Equal("https://example.com/second",
            (await (await _client.GetAsync($"/links/{second}")).Content
                .ReadFromJsonAsync<ResolveResponse>())!.Url);
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

    // ── 4. the one place a double is justified ────────────────────────
    [Fact] // [negative] (R9) — a collision must not lose a link
    public async Task ARepeatedCodeMintsAnotherRatherThanOverwriting()
    {
        // The real generator will not collide on demand, and waiting for it to
        // is not a test strategy. This is the state that earns a seam — and it
        // is the ONLY one in this solution.
        await RestartWith(new FixedCodes("abc123", "abc123", "xyz789"));

        var first = await CodeOf(await Post("https://example.com/first"));
        var second = await CodeOf(await Post("https://example.com/second"));

        Assert.Equal("abc123", first);
        Assert.Equal("xyz789", second);

        // The proof that nothing was quietly overwritten. Note that this is
        // asserting against the REAL database's PRIMARY KEY, not against a
        // fake's promise to behave like one.
        Assert.Equal("https://example.com/first",
            (await (await _client.GetAsync("/links/abc123")).Content
                .ReadFromJsonAsync<ResolveResponse>())!.Url);
        Assert.Equal("https://example.com/second",
            (await (await _client.GetAsync("/links/xyz789")).Content
                .ReadFromJsonAsync<ResolveResponse>())!.Url);
    }

    [Fact] // [edge] (R9) — a generator that will never cooperate
    public async Task GivingUpBeatsLoopingForeverWhenEveryCodeIsTaken()
    {
        await RestartWith(new FixedCodes("abc123", "abc123", "abc123", "abc123", "abc123", "abc123"));
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

    // ── 5. what only the transport can catch ──────────────────────────
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

    // ── 6. what only the real database can catch ──────────────────────
    [Fact] // [positive] (R4) — persistence, proven rather than promised
    public async Task LinksSurviveTheProcessThatCreatedThem()
    {
        var code = await CodeOf(await Post("https://example.com/first"));

        // Tear the whole web application down and build a brand new one over
        // the same database — a restart, as far as the app is concerned.
        await RestartWith(new RandomShortCodeGenerator());

        var resolved = await _client.GetAsync($"/links/{code}");

        Assert.Equal(HttpStatusCode.OK, resolved.StatusCode);
        Assert.Equal("https://example.com/first",
            (await resolved.Content.ReadFromJsonAsync<ResolveResponse>())!.Url);
    }
}
