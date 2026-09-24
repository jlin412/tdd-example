using Microsoft.Data.Sqlite;

namespace Kata;

// ════════════════════════════════════════════════════════════════════
// GIVEN TO YOU — test infrastructure, not part of the exercise.
// ════════════════════════════════════════════════════════════════════
// You do not need this until your tests demand that a link outlive the process
// that created it (checkpoint #3 in UrlShortenerTests.cs). Until then it just
// sits here, unused.
//
// It exists because SQLite has two sharp edges that teach nothing about TDD,
// and between them they would eat ten minutes of your session:
//
//   1. A "Data Source=:memory:" database belongs to ONE SqliteConnection
//      object. Dispose it and the schema and every row are gone, and a SECOND
//      connection with the same connection string gets a DIFFERENT, EMPTY
//      database. So this class holds one connection open for its whole life.
//
//   2. A SqliteConnection is NOT thread-safe. Two requests using the same one
//      at the same time is undefined behaviour — in practice a
//      NullReferenceException from somewhere inside the driver. If anything
//      you build here will be used concurrently, it needs a connection of its
//      own, which is why ConnectionString is exposed alongside Connection.
//
// Hence a uniquely-named shared-cache database rather than plain ":memory:":
// the keep-alive connection below keeps it alive, and anyone who needs their
// own connection can open one against the same data.
//
// Use it like this:
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
//
// created_at is there from day one for the same reason the csproj references
// its packages from day one: the reference solutions carry no copy of this
// file, so the column the extended story needs has to exist already. It is
// nullable because nothing writes it until then.
public sealed class SqliteTestDatabase : IDisposable
{
    /// A connection you may use directly — single-threaded only.
    public SqliteConnection Connection { get; }

    /// Use this if anything you are testing might touch the database from more
    /// than one thread at a time: open a connection per operation instead of
    /// sharing one. Opening is cheap — Microsoft.Data.Sqlite pools connections.
    public string ConnectionString { get; }

    public SqliteTestDatabase()
    {
        // A unique name per instance, so tests running in parallel never see
        // each other's rows. Shared cache is what lets a second connection
        // reach the same in-memory database at all.
        ConnectionString = $"Data Source=kata-{Guid.NewGuid():N};Mode=Memory;Cache=Shared";

        // This one stays open for the lifetime of the fixture. The moment the
        // last connection closes, the database ceases to exist.
        Connection = new SqliteConnection(ConnectionString);
        Connection.Open();

        using var schema = Connection.CreateCommand();
        // NOT NULL is deliberate: SQLite has a legacy quirk where a
        // non-INTEGER PRIMARY KEY column still accepts NULLs without it.
        schema.CommandText = """
            CREATE TABLE links (
                code       TEXT PRIMARY KEY NOT NULL,
                url        TEXT NOT NULL,
                created_at INTEGER
            )
            """;
        schema.ExecuteNonQuery();
    }

    public void Dispose() => Connection.Dispose();
}
