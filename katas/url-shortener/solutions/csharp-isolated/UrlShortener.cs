namespace Kata;

// URL Shortener kata — reference solution (backend R1–R9, R13 once the real
// database forces it, and R20–R23 from the extended story).
//
// One possible end state. From the checkpoint-#1 pattern menu this team took
// (a) PORT + HAND-WRITTEN FAKE and (b) INJECTED CODE GENERATOR — added one at a
// time, per the coach's rule, with all tests green between them.
//
// Deliberately NOT taken at checkpoint #1:
//   · (c) SHARED CONTRACT TESTS — honest at the time: one implementation means
//     an abstract suite has nothing to compare against. The team wrote down
//     WHEN it would pay ("the day a second store exists") and declined it.
//     Checkpoint #3, the real database, is that day — see
//     UrlRepositoryContractTests.cs.
//   · (e) DOMAIN vs INFRASTRUCTURE ERRORS — nothing forced it while the only
//     store was one this team wrote. SqliteUrlRepository is where it earns its
//     place, and the HTTP layer (checkpoint #4) is where it gets paid.
//
// Declined outright (both good retro topics):
//   · (d) TRY-PATTERN — this team throws from Resolve, and throwing is
//     unambiguous. TryResolve earns its place the day callers are writing
//     try/catch around something that isn't exceptional. (The Stack kata's
//     solution declined its own menu item (d) for the same reason.)
//   · (f) SHORT CODE AS A VALUE OBJECT — a fine design, but at one call site a
//     string carries its weight. The moment a second rule about codes appears
//     (a checksum, a reserved-words list) this flips.
//
// THE EXTENDED STORY (the table) added one more seam, and for the same reason
// as (b): "newest first by creation time" cannot be asserted against a clock
// nobody controls. TimeProvider is .NET 8's own abstraction for "now"; the tests
// subclass it by hand in five lines, so no package was added for it.
//
// CONTRACT CHOICES — every one of these is a decision the story left open, and
// a different mob may defensibly have gone the other way:
//   · R5  unknown code    -> throws UnknownCodeException.
//   · R6  same URL twice  -> a FRESH code each time. Simpler, and it makes
//                            "one URL, two codes" a deliberate tested statement
//                            rather than an accident.
//   · R7  invalid input   -> blank/whitespace throws. Scheme checking is left
//                            to the caller; this service is not a URL validator.
//   · R8  case            -> codes are CASE-SENSITIVE, and that decision lives
//                            in exactly one place per store (see the comments
//                            in UrlRepositories.cs).
//   · R9  collision       -> the store refuses; the service mints another.
//   · R22 same instant    -> two links created at the same moment list the
//                            later-saved one first. Nothing in the story says
//                            so; a clock simply CAN repeat, so it had to be
//                            decided, and the contract suite holds both stores
//                            to it.

// ── A link, as the extended story needs to see it ────────────────────
public sealed record Link(string Code, string Url, DateTimeOffset CreatedAt);

// ── The port ─────────────────────────────────────────────────────────
// Menu item (a). Note whose vocabulary this is in: "save a link", "find a
// link", "every link". Nothing here knows what SQL is, which is exactly why
// the service can't accidentally learn.
public interface IUrlRepository
{
    /// <summary>Stores a new link. Throws if the code is already taken (R9).</summary>
    void Save(Link link);

    /// <summary>The URL for this code, or null if no such code was ever issued.</summary>
    string? Find(string code);

    /// <summary>
    /// Every link, newest first by creation time; links created at the same
    /// instant come latest-saved first (R21/R22). ORDER is part of the
    /// contract — which is exactly the kind of promise a fake gets wrong.
    /// </summary>
    IReadOnlyList<Link> All();
}

// Menu item (b). The only unpredictable thing in the system, behind a seam so
// a test can pin it exactly.
public interface IShortCodeGenerator
{
    string Next();
}

// ── Domain errors ────────────────────────────────────────────────────
// These are the service's own vocabulary. A caller can catch these without
// knowing whether the links live in a dictionary, in SQLite, or on the moon —
// which is the whole point of (e), and what makes the HTTP layer's status-code
// mapping possible.
public class UnknownCodeException : Exception
{
    public UnknownCodeException(string code)
        : base($"no link was ever issued for code '{code}'")
    {
    }
}

public class CodeAlreadyTakenException : Exception
{
    public CodeAlreadyTakenException(string code)
        : base($"the code '{code}' is already in use")
    {
    }
}

// ── The service ──────────────────────────────────────────────────────
public class UrlShortener
{
    // R9: a generator may repeat itself. Rather than trust it, mint again.
    // Five is arbitrary and generous — with a real code space, needing a second
    // attempt is already remarkable.
    private const int MaxMintingAttempts = 5;

    private readonly IUrlRepository _repository;
    private readonly IShortCodeGenerator _generator;
    private readonly TimeProvider _clock;

    // Note what is absent: a parameterless constructor. An in-memory default
    // would be convenient and would quietly let a caller build a shortener that
    // violates R4 — links that don't survive a restart. Making storage a
    // required argument means that can't happen by accident.
    //
    // The clock is required for the same reason as the generator: it is the
    // other thing whose answer changes on its own. Production passes
    // TimeProvider.System; tests pass a clock they can set.
    public UrlShortener(IUrlRepository repository, IShortCodeGenerator generator, TimeProvider clock)
    {
        _repository = repository;
        _generator = generator;
        _clock = clock;
    }

    // Hands back the whole link it made, not just the code: the caller that
    // most needs it is the HTTP layer, which answers a create with the link
    // itself (R24) — and only this method knows the moment it was created.
    public Link Shorten(string longUrl)
    {
        if (string.IsNullOrWhiteSpace(longUrl))
        {
            throw new ArgumentException("a url is required", nameof(longUrl));
        }

        for (var attempt = 0; attempt < MaxMintingAttempts; attempt++)
        {
            // R20 — the moment of creation is recorded when the link is made,
            // not guessed later from where it happens to sit.
            var link = new Link(_generator.Next(), longUrl, _clock.GetUtcNow());
            try
            {
                _repository.Save(link);
                return link;
            }
            catch (CodeAlreadyTakenException)
            {
                // R9 — the store refused, so nothing was lost. Try another.
            }
        }

        throw new InvalidOperationException(
            $"could not mint an unused code after {MaxMintingAttempts} attempts");
    }

    public string Resolve(string shortCode)
    {
        // R5 — the port reports "not found" as null; turning that into the
        // service's own error is this method's job, not the store's.
        return _repository.Find(shortCode) ?? throw new UnknownCodeException(shortCode);
    }

    // R21 — the table. The ordering promise lives in the port's contract, so
    // this is a pass-through; the contract suite is where "newest first" is
    // actually proven, against both stores.
    public IReadOnlyList<Link> List() => _repository.All();
}
