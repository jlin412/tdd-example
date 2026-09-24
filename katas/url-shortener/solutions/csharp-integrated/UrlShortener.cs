using System.Security.Cryptography;
using Microsoft.Data.Sqlite;

namespace Kata;

// URL Shortener kata — reference solution 2 of 2: INTEGRATED.
// See ../Solution2-Integrated.md for the test order and the rationale.
//
// Read this next to solutions/csharp-isolated/UrlShortener.cs. Same product
// story, same requirements, same rules — and a visibly different design,
// because it was driven by a different kind of test.
//
// THE THING TO NOTICE FIRST: there is no IUrlRepository in this file, and no
// fake anywhere in this solution. Not because a repository would be wrong, but
// because nothing ever *forced* one. Every test in this solution runs against
// a real SQLite database, so no test was ever blocked by the lack of a seam —
// and adding an interface with exactly one implementation, on the strength of
// nothing but a hunch that it might help later, is the definition of
// speculative design. YAGNI won on the evidence available.
//
// What that buys, and what it costs, is the whole argument in
// ../Solution2-Integrated.md. The short version:
//
//   BOUGHT — no double means no double can lie. The bug that solution 1's
//   contract suite exists to catch (an in-memory store that silently
//   overwrites a taken code) is UNREACHABLE here. This class cannot be run
//   against anything but the real thing.
//
//   COST — this class now knows what SQLite is. Look at the catch clause in
//   Shorten(): a service that is supposedly about minting links is
//   pattern-matching on a database driver's integer error code. Swap the store
//   and you edit the service. Solution 1's service would not change by a line.
//
// TWO SEAMS SURVIVE, each because a test demanded it, not on principle:
//   · IShortCodeGenerator — "two links that collide must not overwrite each
//     other" is a state you cannot reach by asking a real random generator
//     nicely.
//   · TimeProvider (the extended story) — "the table is ordered by when each
//     link was created, not by when the request arrived" can only be shown
//     with a clock that disagrees with the order of arrival, and the real one
//     never will on demand. .NET 8's own abstraction; the fake is a five-line
//     subclass in the test file.
// Two seams, two reasons, written down.

/// A link as the table needs it.
public sealed record Link(string Code, string Url, DateTimeOffset CreatedAt);

public class UnknownCodeException : Exception
{
    public UnknownCodeException(string code)
        : base($"no link was ever issued for code '{code}'")
    {
    }
}

public interface IShortCodeGenerator
{
    string Next();
}

public sealed class RandomShortCodeGenerator : IShortCodeGenerator
{
    private const string Alphabet = "abcdefghijkmnpqrstuvwxyz23456789";
    private const int CodeLength = 6;

    public string Next()
    {
        var code = new char[CodeLength];
        for (var i = 0; i < CodeLength; i++)
        {
            code[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
        }

        return new string(code);
    }
}

public class UrlShortener
{
    // SQLITE_CONSTRAINT. In solution 1 this constant lives in a repository, far
    // away from any domain logic. Here it sits in the service, which is the
    // coupling made visible.
    private const int SqliteConstraintViolation = 19;

    private const int MaxMintingAttempts = 5;

    private readonly string _connectionString;
    private readonly IShortCodeGenerator _generator;
    private readonly TimeProvider _clock;

    // A connection STRING, not a connection — and that is not a detail.
    //
    // The first version of this class took a shared SqliteConnection, which is
    // how almost everyone writes it. It passed every test in this file except
    // one: ConcurrentRequestsNeverIssueTheSameCodeTwice, which failed
    // intermittently with a NullReferenceException from inside the driver,
    // because a SqliteConnection is not thread-safe and a web application is
    // the most concurrent thing there is.
    //
    // No isolated test could have found that. It is not reachable through a
    // fake repository, it is not visible when you call the service directly
    // from one thread, and it does not depend on any rule in the story. It is
    // a property of the wiring — which is exactly what an integrated test is
    // for, and the strongest single argument in this solution's favour.
    public UrlShortener(string connectionString, IShortCodeGenerator generator, TimeProvider clock)
    {
        _connectionString = connectionString;
        _generator = generator;
        _clock = clock;
    }

    // Opening per operation is cheap: Microsoft.Data.Sqlite pools connections,
    // so this is a pool checkout rather than a new database handle.
    private SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    // Hands back the whole link it made — the HTTP layer answers a create with
    // it (R24), and only this method knows the moment it was created.
    public Link Shorten(string longUrl)
    {
        if (string.IsNullOrWhiteSpace(longUrl))
        {
            throw new ArgumentException("a url is required", nameof(longUrl));
        }

        for (var attempt = 0; attempt < MaxMintingAttempts; attempt++)
        {
            var link = new Link(_generator.Next(), longUrl, _clock.GetUtcNow());

            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText =
                "INSERT INTO links (code, url, created_at) VALUES ($code, $url, $createdAt)";
            command.Parameters.AddWithValue("$code", link.Code);
            command.Parameters.AddWithValue("$url", link.Url);
            // UTC ticks sort exactly as the instants they stand for.
            command.Parameters.AddWithValue("$createdAt", link.CreatedAt.UtcTicks);

            try
            {
                command.ExecuteNonQuery();
                return link;
            }
            catch (SqliteException e) when (e.SqliteErrorCode == SqliteConstraintViolation)
            {
                // The code was taken. The database refused the write whole, so
                // nothing was lost — mint another.
                //
                // Worth pausing here: solution 1 needed a CodeAlreadyTakenException
                // to express this, because its service could not see SQLite. This
                // one needs no such type — and pays for the saving by importing
                // Microsoft.Data.Sqlite into its domain logic. Neither is free.
            }
        }

        throw new InvalidOperationException(
            $"could not mint an unused code after {MaxMintingAttempts} attempts");
    }

    public string Resolve(string shortCode)
    {
        // No COLLATE clause, so SQLite's default BINARY collation applies:
        // codes are case-sensitive. In solution 1 that decision had to be
        // stated twice — once per store — and a contract test existed to prove
        // the two agreed. Here there is only one store, so there is only one
        // place it can be, and nothing to disagree with.
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT url FROM links WHERE code = $code";
        command.Parameters.AddWithValue("$code", shortCode);

        return command.ExecuteScalar() as string ?? throw new UnknownCodeException(shortCode);
    }

    public IReadOnlyList<Link> List()
    {
        // Newest first by creation time; the later-saved of two links created
        // in the same instant first. rowid only ever grows for this table, so
        // it is the "saved later" tie-break. In solution 1 this ORDER BY had a
        // twin in the in-memory store and a contract test to keep the two in
        // step; here it is the only copy there is.
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT code, url, created_at FROM links ORDER BY created_at DESC, rowid DESC";

        using var reader = command.ExecuteReader();
        var links = new List<Link>();
        while (reader.Read())
        {
            links.Add(new Link(
                reader.GetString(0),
                reader.GetString(1),
                new DateTimeOffset(reader.GetInt64(2), TimeSpan.Zero)));
        }

        return links;
    }
}
