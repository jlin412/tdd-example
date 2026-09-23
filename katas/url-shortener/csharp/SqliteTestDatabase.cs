using Microsoft.Data.Sqlite;

namespace Kata;

// ════════════════════════════════════════════════════════════════════
// GIVEN TO YOU — test infrastructure, not part of the exercise.
// ════════════════════════════════════════════════════════════════════
// You do not need this until STORY 2 (UrlShortenerDatabaseStory.md). Until
// then it just sits here, unused. Nothing in story 1 touches SQL.
//
// It exists because SQLite has one sharp edge that teaches nothing about TDD,
// and it would eat ten minutes of your session:
//
//   A "Data Source=:memory:" database belongs to ONE SqliteConnection object.
//   Dispose that connection and the schema and every row are gone. Open a
//   SECOND connection with the very same connection string and you get a
//   DIFFERENT, EMPTY database — it is not a shared name.
//
// So this class opens exactly one connection, creates the schema on it, and
// hands that live connection out. Use it like this:
//
//   public class MyRepositoryTests : IDisposable
//   {
//       private readonly SqliteTestDatabase _db = new();
//       public void Dispose() => _db.Dispose();
//
//       [Fact]
//       public void SomeTest()
//       {
//           var repository = new SqliteUrlRepository(_db.Connection);
//           ...
//       }
//   }
//
// xUnit builds a FRESH instance of a test class for every single test method,
// so that gives you a pristine, isolated database per test for free — no
// cleanup SQL, no test-ordering hazards, nothing shared between tests.
//
// The schema is given too. It is a premise of the kata, not a lesson in it —
// but do read it, because one line of it is going to matter a great deal.
public sealed class SqliteTestDatabase : IDisposable
{
    public SqliteConnection Connection { get; }

    public SqliteTestDatabase()
    {
        Connection = new SqliteConnection("Data Source=:memory:");
        Connection.Open();

        using var schema = Connection.CreateCommand();
        // NOT NULL is deliberate: SQLite has a legacy quirk where a
        // non-INTEGER PRIMARY KEY column still accepts NULLs without it.
        schema.CommandText = """
            CREATE TABLE links (
                code TEXT PRIMARY KEY NOT NULL,
                url  TEXT NOT NULL
            )
            """;
        schema.ExecuteNonQuery();
    }

    public void Dispose() => Connection.Dispose();
}
