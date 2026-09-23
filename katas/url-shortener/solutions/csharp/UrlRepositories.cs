using System.Security.Cryptography;
using Microsoft.Data.Sqlite;

namespace Kata;

// URL Shortener kata — reference solution: the two stores, and the generator.
//
// Both implement IUrlRepository, and BOTH are held to the same suite in
// UrlRepositoryContractTests.cs. That shared suite is menu item (c), and it is
// the only thing standing between this file and the bug story 2 is about.

// ── The fake (menu item (a)) ─────────────────────────────────────────
// Written by hand, in about a dozen lines, by the team that needed it. It is
// not a mock: it is a real, working implementation of the port that happens to
// keep its links in memory. That distinction is what lets it be held to the
// same contract as the real thing — a configured mock could only ever repeat
// what it was told.
public sealed class InMemoryUrlRepository : IUrlRepository
{
    // R8/R14 — StringComparer.Ordinal is where "codes are case-sensitive"
    // lives for this store. It is deliberate, not a default: the out-of-the-box
    // Dictionary<string, string> comparer is ordinal too, but writing it out
    // means the decision is READ as a decision, and it is what makes this store
    // agree with the SQLite column's BINARY collation.
    private readonly Dictionary<string, string> _links = new(StringComparer.Ordinal);

    public void Save(string code, string url)
    {
        // R9/R12 — THE line the whole kata turns on.
        //
        // The obvious first draft is `_links[code] = url;`. It compiles, it
        // reads fine, every test written before checkpoint #2 stays green — and
        // it SILENTLY DESTROYS a link when a code repeats. No exception, no
        // failure, just a URL that used to work and now points somewhere else.
        //
        // TryAdd refuses instead. That is not a detail: it is this store
        // agreeing to the same contract the database enforces in hardware.
        if (!_links.TryAdd(code, url))
        {
            throw new CodeAlreadyTakenException(code);
        }
    }

    public string? Find(string code) => _links.TryGetValue(code, out var url) ? url : null;
}

// ── The real store ───────────────────────────────────────────────────
public sealed class SqliteUrlRepository : IUrlRepository
{
    // SQLITE_CONSTRAINT. The stable code to check: the EXTENDED code is 1555
    // (SQLITE_CONSTRAINT_PRIMARYKEY) here, but it would be 2067
    // (SQLITE_CONSTRAINT_UNIQUE) if the schema declared a UNIQUE index instead
    // of a PRIMARY KEY. Checking the broad code keeps this working either way.
    private const int SqliteConstraintViolation = 19;

    private readonly SqliteConnection _connection;

    public SqliteUrlRepository(SqliteConnection connection) => _connection = connection;

    public void Save(string code, string url)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = "INSERT INTO links (code, url) VALUES ($code, $url)";
        command.Parameters.AddWithValue("$code", code);
        command.Parameters.AddWithValue("$url", url);

        try
        {
            command.ExecuteNonQuery();
        }
        catch (SqliteException e) when (e.SqliteErrorCode == SqliteConstraintViolation)
        {
            // MENU ITEM (e), and this is the line that earns it.
            //
            // Without this, a SqliteException escapes — and every caller above
            // has to know that links are kept in SQLite in order to understand
            // what went wrong. Story 3's handler would have to reach into a
            // database driver's error codes just to decide between 409 and 500,
            // and swapping SQLite for anything else would break all of them.
            //
            // The message SQLite gives here, for the record:
            //   SQLite Error 19: 'UNIQUE constraint failed: links.code'.
            throw new CodeAlreadyTakenException(code);
        }
    }

    public string? Find(string code)
    {
        // R8/R14 — no COLLATE clause, so this uses SQLite's default BINARY
        // collation: case-SENSITIVE, matching StringComparer.Ordinal above.
        // Had the fake used OrdinalIgnoreCase, the two stores would quietly
        // disagree and only the shared contract suite would ever say so.
        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT url FROM links WHERE code = $code";
        command.Parameters.AddWithValue("$code", code);
        return command.ExecuteScalar() as string;
    }
}

// ── The generator (menu item (b)) ────────────────────────────────────
// The production implementation. Tests never use it — they inject their own,
// which is the entire payoff of having made this a seam.
public sealed class RandomShortCodeGenerator : IShortCodeGenerator
{
    // Deliberately no vowels-and-lookalikes policy, no checksum, no base-62
    // arithmetic. The story said "short and URL-safe" and stopped there; the
    // moment it says more, menu item (f) stops being over-engineering.
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
