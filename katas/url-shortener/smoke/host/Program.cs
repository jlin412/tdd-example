using Kata;

// The smoke test's backend: the integrated reference solution's routes, over a
// real SQLite database, serving the built reference page from the same origin.
//
// The database is SqliteTestDatabase's in-memory one — alive exactly as long as
// this process, which is what the smoke test wants: every run starts empty.

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = Environment.GetEnvironmentVariable("SMOKE_WEB_ROOT")
        ?? throw new InvalidOperationException("set SMOKE_WEB_ROOT to the built page (see prepare.mjs)"),
});

var database = new SqliteTestDatabase();
builder.Services.AddSingleton(
    new UrlShortener(database.ConnectionString, new RandomShortCodeGenerator(), TimeProvider.System));

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapUrlEndpoints();
app.MapFallbackToFile("index.html");
app.Lifetime.ApplicationStopped.Register(database.Dispose);
app.Run();
