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
//   At checkpoint #1 this was correctly DECLINED. With a single in-memory store, an
//   abstract base class is ceremony: there is nothing to compare against, and
//   "run the same tests against everything" means "run them against the one
//   thing". The team wrote down when it would pay and moved on. That was the
//   right call, and the note was the important part.
//
//   At checkpoint #3, the real database, it stopped being ceremony. The moment a SECOND implementation
//   exists, every one of these tests becomes a question you are asking of BOTH
//   stores — and DoesNotOverwriteALinkWhenTheCodeIsAlreadyTaken is the one that
//   catches a `dict[code] = url` fake red-handed while SQLite has been quietly
//   refusing that write all along.
//
//   In the extended story it earned its keep a second time. "Newest first"
//   is a promise about ORDER, and order is exactly what an in-memory
//   collection gives you by accident: sort by creation time alone and every
//   test with distinct times passes, while two links created in the same
//   instant come back in whatever order the collection happened to keep.
//   ListsTheLaterSavedFirstWhenTwoLinksShareAnInstant asks both stores the
//   same question.
//
//   A fake will confirm whatever you believed when you wrote it. This file is
//   the only thing in the repository that can disagree with you.
public abstract class UrlRepositoryContractTests
{
    protected abstract IUrlRepository NewRepository();

    private static readonly DateTimeOffset Noon = new(2026, 9, 24, 12, 0, 0, TimeSpan.Zero);

    /// A link created at noon, give or take some minutes. Most tests here do
    /// not care when; the ordering tests below care about nothing else.
    private static Link LinkTo(string url, string code, int minutesAfterNoon = 0) =>
        new(code, url, Noon.AddMinutes(minutesAfterNoon));

    [Fact] // [positive] (R1/R2)
    public void FindsAUrlThatWasSaved()
    {
        var repository = NewRepository();
        repository.Save(LinkTo("https://example.com/first", "abc123"));

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
        repository.Save(LinkTo("https://example.com/first", "abc123"));
        repository.Save(LinkTo("https://example.com/second", "xyz789"));

        Assert.Equal("https://example.com/first", repository.Find("abc123"));
        Assert.Equal("https://example.com/second", repository.Find("xyz789"));
    }

    [Fact] // [negative] (R9/R12) — THE ONE THAT MATTERS
    public void RefusesToSaveACodeThatIsAlreadyTaken()
    {
        var repository = NewRepository();
        repository.Save(LinkTo("https://example.com/first", "abc123"));

        Assert.Throws<CodeAlreadyTakenException>(
            () => repository.Save(LinkTo("https://example.com/second", "abc123")));
    }

    [Fact] // [negative] (R9/R12)
    public void DoesNotLoseTheOriginalLinkWhenACodeIsRefused()
    {
        // The assertion that actually names the damage. A store can "refuse"
        // by throwing AFTER it has already clobbered the row; SQLite refuses
        // the write whole, and the fake must too.
        var repository = NewRepository();
        repository.Save(LinkTo("https://example.com/first", "abc123"));

        Assert.Throws<CodeAlreadyTakenException>(
            () => repository.Save(LinkTo("https://example.com/second", "abc123")));

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
        repository.Save(LinkTo("https://example.com/first", "abc123"));

        Assert.Null(repository.Find("ABC123"));
    }

    [Fact] // [edge] (R21)
    public void ListsNothingWhenNothingWasSaved()
    {
        Assert.Empty(NewRepository().All());
    }

    [Fact] // [positive] (R20/R21) — by creation time, which is not the order they arrived in
    public void ListsEveryLinkNewestFirst()
    {
        var repository = NewRepository();
        var first = LinkTo("https://example.com/first", "abc123", minutesAfterNoon: 0);
        var third = LinkTo("https://example.com/third", "xyz789", minutesAfterNoon: 2);
        var second = LinkTo("https://example.com/second", "def456", minutesAfterNoon: 1);

        // Saved out of time order on purpose — two servers with slightly
        // different clocks do exactly this — so "newest first" cannot pass by
        // echoing the save order back. Whole records are compared, so the
        // creation time must also survive the round trip exactly.
        repository.Save(first);
        repository.Save(third);
        repository.Save(second);

        Assert.Equal(new[] { third, second, first }, repository.All());
    }

    [Fact] // [boundary] (R22) — a clock can repeat itself, just like a generator
    public void ListsTheLaterSavedFirstWhenTwoLinksShareAnInstant()
    {
        var repository = NewRepository();
        var earlier = LinkTo("https://example.com/first", "abc123");
        var later = LinkTo("https://example.com/second", "xyz789");

        repository.Save(earlier);
        repository.Save(later);

        Assert.Equal(new[] { later, earlier }, repository.All());
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
