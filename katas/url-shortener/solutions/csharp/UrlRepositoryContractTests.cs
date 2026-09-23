using Xunit;

namespace Kata;

// URL Shortener kata — reference solution: THE SHARED CONTRACT SUITE (R11).
//
// This is menu item (c), and it is the point of the whole kata.
//
// Every test below is written ONCE and runs against EVERY implementation of
// IUrlRepository. Not two suites that happen to look similar and drift apart —
// one suite, two subclasses, and a compiler that will not let a new store into
// the codebase without answering all of it.
//
// Read what this buys you in the order the mob experienced it:
//
//   In story 1 this was correctly DECLINED. With a single in-memory store, an
//   abstract base class is ceremony: there is nothing to compare against, and
//   "run the same tests against everything" means "run them against the one
//   thing". The team wrote down when it would pay and moved on. That was the
//   right call, and the note was the important part.
//
//   In story 2 it stopped being ceremony. The moment a SECOND implementation
//   exists, every one of these tests becomes a question you are asking of BOTH
//   stores — and DoesNotOverwriteALinkWhenTheCodeIsAlreadyTaken is the one that
//   catches a `dict[code] = url` fake red-handed while SQLite has been quietly
//   refusing that write all along.
//
//   A fake will confirm whatever you believed when you wrote it. This file is
//   the only thing in the repository that can disagree with you.
public abstract class UrlRepositoryContractTests
{
    protected abstract IUrlRepository NewRepository();

    [Fact] // [positive] (R1/R2)
    public void FindsAUrlThatWasSaved()
    {
        var repository = NewRepository();
        repository.Save("abc123", "https://example.com/first");

        Assert.Equal("https://example.com/first", repository.Find("abc123"));
    }

    [Fact] // [edge] (R5)
    public void FindsNothingForACodeThatWasNeverIssued()
    {
        Assert.Null(NewRepository().Find("nope99"));
    }

    [Fact] // [positive] (R3)
    public void KeepsSeparateLinksApart()
    {
        var repository = NewRepository();
        repository.Save("abc123", "https://example.com/first");
        repository.Save("xyz789", "https://example.com/second");

        Assert.Equal("https://example.com/first", repository.Find("abc123"));
        Assert.Equal("https://example.com/second", repository.Find("xyz789"));
    }

    [Fact] // [negative] (R9/R12) — THE ONE THAT MATTERS
    public void RefusesToSaveACodeThatIsAlreadyTaken()
    {
        var repository = NewRepository();
        repository.Save("abc123", "https://example.com/first");

        Assert.Throws<CodeAlreadyTakenException>(
            () => repository.Save("abc123", "https://example.com/second"));
    }

    [Fact] // [negative] (R9/R12)
    public void DoesNotLoseTheOriginalLinkWhenACodeIsRefused()
    {
        // The assertion that actually names the damage. A store can "refuse"
        // by throwing AFTER it has already clobbered the row; SQLite refuses
        // the write whole, and the fake must too.
        var repository = NewRepository();
        repository.Save("abc123", "https://example.com/first");

        Assert.Throws<CodeAlreadyTakenException>(
            () => repository.Save("abc123", "https://example.com/second"));

        Assert.Equal("https://example.com/first", repository.Find("abc123"));
    }

    [Fact] // [edge] (R8/R14)
    public void TreatsCodesAsCaseSensitive()
    {
        // The second, quieter fidelity gap. A Dictionary built with
        // StringComparer.OrdinalIgnoreCase is a perfectly reasonable-looking
        // fake — and it disagrees with SQLite's default BINARY collation.
        // Neither store is wrong in isolation; they simply cannot both be right
        // about the same product, and only this test says so.
        var repository = NewRepository();
        repository.Save("abc123", "https://example.com/first");

        Assert.Null(repository.Find("ABC123"));
    }
}

// Three lines each. That is the whole cost of holding a store to the contract —
// and the reason a team with two hand-maintained suites has two chances to be
// wrong.

public class InMemoryUrlRepositoryContractTests : UrlRepositoryContractTests
{
    protected override IUrlRepository NewRepository() => new InMemoryUrlRepository();
}

public class SqliteUrlRepositoryContractTests : UrlRepositoryContractTests, IDisposable
{
    // SqliteTestDatabase is GIVEN — it ships in the skeleton folder, not here,
    // and it holds one open connection for the lifetime of this test instance.
    // xUnit builds a fresh instance per test method, so every test above gets a
    // pristine, empty database with no cleanup code anywhere.
    private readonly SqliteTestDatabase _database = new();

    protected override IUrlRepository NewRepository() =>
        new SqliteUrlRepository(_database.Connection);

    public void Dispose() => _database.Dispose();
}
