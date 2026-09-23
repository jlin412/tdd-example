using System.Security.Cryptography;
using Microsoft.Data.Sqlite;

namespace Kata;

// URL Shortener kata — reference solution 2 of 2: INTEGRATED.
// See ../Solution2-Integrated.md for the test order and the rationale.
//
// Read this next to solutions/csharp-isolated/UrlShortener.cs. Same three
// stories, same requirements, same rules — and a visibly different design,
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
// ONE SEAM SURVIVES: IShortCodeGenerator. Not on principle — because a test
// demanded it. "Two links that collide must not overwrite each other" is a
// state you cannot reach by asking a real random generator nicely, and that is
// exactly the kind of test that justifies a double. One seam, one reason,
// written down.

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

    private readonly SqliteConnection _connection;
    private readonly IShortCodeGenerator _generator;

    public UrlShortener(SqliteConnection connection, IShortCodeGenerator generator)
    {
        _connection = connection;
        _generator = generator;
    }

    public string Shorten(string longUrl)
    {
        if (string.IsNullOrWhiteSpace(longUrl))
        {
            throw new ArgumentException("a url is required", nameof(longUrl));
        }

        for (var attempt = 0; attempt < MaxMintingAttempts; attempt++)
        {
            var code = _generator.Next();

            using var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO links (code, url) VALUES ($code, $url)";
            command.Parameters.AddWithValue("$code", code);
            command.Parameters.AddWithValue("$url", longUrl);

            try
            {
                command.ExecuteNonQuery();
                return code;
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
        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT url FROM links WHERE code = $code";
        command.Parameters.AddWithValue("$code", shortCode);

        return command.ExecuteScalar() as string ?? throw new UnknownCodeException(shortCode);
    }
}
