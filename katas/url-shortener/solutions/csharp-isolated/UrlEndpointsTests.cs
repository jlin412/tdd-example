using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kata;

// URL Shortener kata — reference test suite for the HTTP layer (R15–R19, R23–R24).
// Checkpoint #4 in the skeleton is where a mob discovers it needs one.
//
// These drive a REAL in-process server with a REAL HttpClient. Routing, model
// binding, JSON serialization, status codes and headers are all genuinely
// exercised — no port is opened and nothing is installed, but nothing is
// simulated either. A typo in a route template fails a test here.
//
// COUNT THESE TESTS. There are ten, against eleven in the service suite and
// nine in the contract suite that each run twice. That shape — a lot at the
// bottom, few at the top — is the test pyramid, and checkpoint #4 asks the mob
// to derive it by counting rather than be shown a diagram.
//
// Note what is NOT tested here, on purpose:
//   · that two URLs get different codes        (UrlShortenerTests — R3)
//   · that a blank URL is refused              (UrlShortenerTests — R7)
//   · that a repeated code doesn't lose a link (contract suite — R9/R12)
//   · how two links created at once are ordered (contract suite — R22)
//
// Every one of those is already proven, closer to the code that decides it,
// and roughly two orders of magnitude faster. Re-proving them through HTTP
// would be a slower, more indirect copy of a test that already exists.
// What is genuinely new at this level is the WIRING: that a request actually
// reaches the right handler and comes back as the right status code.
public class UrlEndpointsTests : IAsyncLifetime
{
    // Deterministic codes, and storage that vanishes with the test. The
    // service and contract suites already prove the real store works; these
    // tests are about HTTP, so they use the fast double — the same judgement
    // the rest of the kata has been making about which suite proves what.
    private sealed class StubGenerator : IShortCodeGenerator
    {
        private readonly Queue<string> _codes;
        public StubGenerator(params string[] codes) => _codes = new Queue<string>(codes);
        public string Next() => _codes.Dequeue();
    }

    private WebApplication _app = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddSingleton<IUrlRepository, InMemoryUrlRepository>();
        builder.Services.AddSingleton<IShortCodeGenerator>(
            _ => new StubGenerator("abc123", "xyz789"));
        // The real clock. Ordering by time is proven in the contract suite;
        // here only the wiring of the list route is under test.
        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddSingleton<UrlShortener>();

        _app = builder.Build();
        _app.MapUrlEndpoints();          // the production routes, mounted as-is
        await _app.StartAsync();
        _client = _app.GetTestClient();
    }

    public async Task DisposeAsync() => await _app.DisposeAsync();

    private Task<HttpResponseMessage> PostLink(string url) =>
        _client.PostAsJsonAsync("/links", new ShortenRequest(url));

    [Fact] // [positive] (R15/R24)
    public async Task ShorteningAUrlIs201WithTheWholeLinkAndALocation()
    {
        var response = await PostLink("https://example.com/articles/tdd-mob-katas");

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/links/abc123", response.Headers.Location?.ToString());
        var created = (await response.Content.ReadFromJsonAsync<LinkResponse>())!;
        Assert.Equal("abc123", created.Code);
        Assert.Equal("https://example.com/articles/tdd-mob-katas", created.Url);
        // R24 — the same link, in the same shape, as the table will list it.
        // The clock here is the real one, so the table is the only honest
        // thing to compare the creation time against.
        var listed = (await _client.GetFromJsonAsync<LinkResponse[]>("/links"))!;
        Assert.Equal(listed.Single(), created);
    }

    [Fact] // [positive] (R16)
    public async Task ResolvingAKnownCodeIs200WithTheOriginalUrl()
    {
        await PostLink("https://example.com/first");

        var response = await _client.GetAsync("/links/abc123");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            "https://example.com/first",
            (await response.Content.ReadFromJsonAsync<ResolveResponse>())!.Url);
    }

    [Fact] // [edge] (R17) — the unknown-code decision from checkpoint #1, much later
    public async Task ResolvingACodeNobodyMintedIs404()
    {
        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync("/links/nope99")).StatusCode);
    }

    [Theory] // [negative] (R17)
    [InlineData("")]
    [InlineData("   ")]
    public async Task ShorteningSomethingThatIsNotAUrlIs400(string notAUrl)
    {
        Assert.Equal(HttpStatusCode.BadRequest, (await PostLink(notAUrl)).StatusCode);
    }

    [Fact] // [edge] (R18) — the callback to the R6 decision
    public async Task ShorteningAUrlThatIsAlreadyKnownIs201Again()
    {
        // Because R6 chose "a fresh code each time", this really is a creation.
        // A mob that chose "hand back the same code" should expect 200 here and
        // no Location header.
        var url = "https://example.com/same/every/time";

        var first = await PostLink(url);
        var second = await PostLink(url);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Created, second.StatusCode);
        Assert.NotEqual(
            (await first.Content.ReadFromJsonAsync<LinkResponse>())!.Code,
            (await second.Content.ReadFromJsonAsync<LinkResponse>())!.Code);
    }

    [Fact] // [positive] (R23) — the table's route
    public async Task ListingIs200WithEveryLinkNewestFirst()
    {
        await PostLink("https://example.com/first");
        await PostLink("https://example.com/second");

        var response = await _client.GetAsync("/links");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var listed = (await response.Content.ReadFromJsonAsync<LinkResponse[]>())!;
        Assert.Equal(new[] { "xyz789", "abc123" }, listed.Select(link => link.Code));
        Assert.Equal("https://example.com/second", listed[0].Url);
    }

    [Fact] // [edge] (R23) — an empty table is a list, not a 404
    public async Task ListingBeforeAnythingIsShortenedIs200WithNothingInIt()
    {
        var response = await _client.GetAsync("/links");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty((await response.Content.ReadFromJsonAsync<LinkResponse[]>())!);
    }

    // ── The two tests that only exist because this server is real ──────
    // Neither of these could be written against handler functions called
    // directly. They are the answer to "what does a real transport buy you",
    // and between them they are most of what goes wrong when a service is
    // wired up for the first time.

    [Fact] // [negative] — ROUTING is real
    public async Task AMisspelledRouteIs404()
    {
        // Call the production routes at the wrong path. Nothing in the domain
        // is involved; this only passes because a real router looked at a real
        // request and found nothing. Change the "/links" group to "/lnks" in
        // UrlEndpoints.cs and watch this suite — and only this suite — go red.
        Assert.Equal(HttpStatusCode.NotFound, (await _client.PostAsync("/lnks", null)).StatusCode);
    }

    [Fact] // [negative] — MODEL BINDING is real
    public async Task AMalformedJsonBodyIs400WithoutReachingTheService()
    {
        var response = await _client.PostAsync(
            "/links", new StringContent("{ not json", System.Text.Encoding.UTF8, "application/json"));

        // ASP.NET Core rejects this before any of our code runs. Worth sitting
        // with: an entire class of failure is handled by the framework, and you
        // would never know whether it was handled WELL without a test that
        // speaks HTTP.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
