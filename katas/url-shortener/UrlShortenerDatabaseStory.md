# URL Shortener — the Database Story

You've shipped [the shortener](UrlShortenerStory.md). Every test is green. You
have a storage class you wrote yourself, and you know exactly how it behaves
when a code is already taken, because you decided that and then made it so.

So here's the question this story is built on:

> The compiler proved your in-memory store *implements* the interface.
> What proved it *behaves like the real thing*?

Nothing has. That's what today is about.

## What changes

The links have to survive. Not "survive in principle" — survive a process
restart, on real storage, the way the first story promised on day one and your
design has been quietly not delivering ever since.

Storage becomes **SQLite**. A real database, a real table, real SQL.

Everything else about the shortener is exactly as it was, and everything the
first story left unsettled is still settled however you settled it.

## What you've been given

[`csharp/SqliteTestDatabase.cs`](csharp/SqliteTestDatabase.cs) — a live
connection and this table, already created for you:

```sql
CREATE TABLE links (
    code TEXT PRIMARY KEY NOT NULL,
    url  TEXT NOT NULL
)
```

Read that schema. One line of it is about to matter a great deal.

It's given to you because SQLite has a sharp edge that teaches nothing about
TDD: an in-memory database belongs to **one connection object**, and vanishes
the moment that connection closes. The file explains it. Let it handle that, and
spend your time on the part that *is* the lesson.

The SQL you'll need is four lines of ADO.NET, and it looks like this:

```csharp
using var command = connection.CreateCommand();
command.CommandText = "INSERT INTO links (code, url) VALUES ($code, $url)";
command.Parameters.AddWithValue("$code", code);
command.Parameters.AddWithValue("$url", url);
command.ExecuteNonQuery();              // or ExecuteScalar() to read one value
```

That's the idiom, not the design. What the class is called, what it implements,
and how it reports what it finds are all still yours.

## Your job, in order

1. **Predict, out loud, before you write anything.** You're about to run your
   existing tests against a different implementation of the same interface. Which
   of them do you expect to pass? Write the list down. This prediction is the
   most valuable thing you'll produce today, and it only counts if it's on the
   whiteboard *before* the bar goes red.

2. **Make the suite runnable against either store.** Right now your repository
   tests name your in-memory class directly. Pull them up into an abstract test
   class with one abstract method that hands back *a* repository, and let a
   subclass say which one. xUnit inherits `[Fact]`s from base classes, so this is
   a **move**, not a rewrite — your in-memory subclass should go green
   immediately, having changed nothing but where the tests live.

   That's pattern-menu item **(c)**, the one that looked like ceremony in
   checkpoint #1 because you only had one implementation. You're about to have
   two.

3. **Write the SQLite repository.** Same interface. Then add the three-line
   subclass that points the shared suite at it.

4. **Run it.** Compare against your prediction.

5. **Now deal with what you find** — test-first, one red at a time.

## What you're going to find

Something that has been green all session is going to go red, and it will be
the test about a code that's already taken.

Your in-memory store and SQLite genuinely disagree about what that means, and
neither of them is being unreasonable. One of them is wrong for your product —
and until this moment, you had no way of knowing which, because the only thing
you ever asked was the store you wrote yourself.

That's the whole lesson, and it's worth saying plainly: **a fake will confirm
whatever you believed when you wrote it.** Shared contract tests are the only
thing that keeps one honest.

Then two follow-ups, both worth a red bar of their own:

- **Whatever SQLite throws when it objects — should the caller of your shortener
  ever see it?** A caller asked to shorten a URL. Handing them back an exception
  type from a database driver tells them about a decision they didn't make and a
  technology they shouldn't have to know about. That's pattern-menu item **(e)**,
  and this is the moment it earns its place.

- **Go back to your case-sensitivity decision.** Try resolving a code in the
  wrong case against *both* stores. If they disagree, ask why — and notice that
  the answer was never written down in your code. It was inherited from whatever
  the storage happened to do, which means it was never really a decision at all.

## Where things live

- Reference solution:
  [solutions/csharp/UrlRepositories.cs](solutions/csharp/UrlRepositories.cs) and
  [solutions/csharp/UrlRepositoryContractTests.cs](solutions/csharp/UrlRepositoryContractTests.cs)

## What next

Two stores, one contract, and a service that keeps its own vocabulary. Now put a
front door on it: [UrlShortenerHttpStory.md](UrlShortenerHttpStory.md).
