namespace Kata;

// URL Shortener kata — reference solution (standard R1–R9, plus R13 once
// story 2 forces it).
//
// One possible end state. From the checkpoint-#1 pattern menu this team took
// (a) PORT + HAND-WRITTEN FAKE and (b) INJECTED CODE GENERATOR — added one at a
// time, per the coach's rule, with all tests green between them.
//
// Deliberately NOT taken in story 1:
//   · (c) SHARED CONTRACT TESTS — honest at the time: one implementation means
//     an abstract suite has nothing to compare against. The team wrote down
//     WHEN it would pay ("the day a second store exists") and declined it.
//     Story 2 is that day, and it is where the decision gets revisited —
//     see UrlRepositoryContractTests.cs.
//   · (e) DOMAIN vs INFRASTRUCTURE ERRORS — nothing forced it while the only
//     store was one this team wrote. SqliteUrlRepository is where it earns its
//     place, and story 3 is where it gets paid.
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

// ── The port ─────────────────────────────────────────────────────────
// Menu item (a). Note whose vocabulary this is in: "save a link", "find a
// link". Nothing here knows what SQL is, which is exactly why the service
// can't accidentally learn.
public interface IUrlRepository
{
    /// <summary>Stores a new link. Throws if the code is already taken (R9).</summary>
    void Save(string code, string url);

    /// <summary>The URL for this code, or null if no such code was ever issued.</summary>
    string? Find(string code);
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
// which is the whole point of (e), and what makes story 3's status-code
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

    // Note what is absent: a parameterless constructor. An in-memory default
    // would be convenient and would quietly let a caller build a shortener that
    // violates R4 — links that don't survive a restart. Making storage a
    // required argument means that can't happen by accident.
    public UrlShortener(IUrlRepository repository, IShortCodeGenerator generator)
    {
        _repository = repository;
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
            try
            {
                _repository.Save(code, longUrl);
                return code;
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
}
