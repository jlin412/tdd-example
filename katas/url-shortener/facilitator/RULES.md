# URL Shortener — facilitator's answer key

**Open this DURING the session, not before.** It is not a spoiler in the way
`solutions/` is — that folder stays closed until the retro, this one you open at
the test-list checkpoint to play product owner.

The stories ([UrlShortenerStory.md](../UrlShortenerStory.md),
[UrlShortenerDatabaseStory.md](../UrlShortenerDatabaseStory.md),
[UrlShortenerHttpStory.md](../UrlShortenerHttpStory.md)) describe the service in
prose and show a walkthrough; they deliberately do **not** hand over a numbered
rule list. Producing that list — as a list of tests — is the mob's first job.
This file is the canonical list to compare theirs against, and it is what the
`R`-numbers in `solutions/` refer to.

## Standard — R1–R9

- **R1.** **shorten** takes a long URL and returns a **short code** — short and
  URL-safe. Exact length and alphabet are the mob's call.
- **R2.** **resolve** takes a code that was issued and returns the **original
  URL**.
- **R3.** Two **different** URLs get two **different** codes. (Needs ≥2
  shortenings to demonstrate; one proves nothing — a constant passes it.)
- **R4.** A short link **keeps working** across a process restart and from
  another server. This is the rule that makes in-process storage wrong, and it
  is the only thing in the standard story that forces a seam. *If a mob never
  feels this, checkpoint #1 has no teeth — see the forcing prompt below.*
- **R5.** Resolving a code that was **never issued** is a defined, deliberate
  behavior. *The mob picks which* — throwing and a Try-pattern are both
  defensible; what matters is that it's chosen, tested, and applied the same way
  every time.
- **R6.** Shortening the **same URL twice** is a defined, deliberate behavior.
  *The mob picks* — same code back, or a fresh one. Both ship in the real world.
- **R7.** **Invalid input is rejected.** Blank and whitespace-only are invalid.
  What else counts (scheme? host?) is the mob's call — but they must say.
- **R8.** **Case sensitivity** of codes is a defined, deliberate behavior. *The
  mob picks* — and it must be expressed in **one place**, not inherited from
  whatever the storage happens to do. (R14 is where a mob finds out whether they
  actually did this.)
- **R9.** A **code collision must not silently lose a link.** Whatever mints
  codes may repeat itself; the store must be *asked* whether a code is taken,
  not assumed to be free. *How* it refuses is the mob's call.

## Database — R10–R14

- **R10.** Storage becomes a **real database**, behind the same interface the
  service already depends on. The service does not change.
- **R11.** **Every repository test runs against every implementation.** One
  shared contract suite, one subclass per store — not two hand-maintained suites.
- **R12 — the punchline.** The real store **rejects** a duplicate code; a naive
  in-memory store **silently overwrites** it. R9 is the rule that catches this,
  and R11 is the mechanism. A mob that skipped either will see two green suites
  and a lost URL.
- **R13.** **Infrastructure exceptions do not escape the service.** Whatever the
  database throws is translated at the boundary into the service's own
  vocabulary. (Not forced until R17.)
- **R14.** The R8 case decision must give the **same answer from both stores**.
  If it doesn't, R8 was never a decision — it was inherited.

## HTTP — R15–R19

- **R15.** `POST /links` with `{ "url": … }` → **201**, a `Location` header
  pointing at the new link, and `{ "code": … }`.
- **R16.** `GET /links/{code}` → **200** and `{ "url": … }`.
- **R17.** An unknown code → **404**. A URL that isn't one → **400**. This is
  what makes R13 pay: a handler cannot map an error it has to reach into a
  database driver to identify.
- **R18.** `POST` of an already-shortened URL → **whatever R6 decided**: 200
  with the existing code if the mob chose "same code", another 201 if they chose
  "fresh code". There is no third right answer.
- **R19.** The endpoint tests drive a **real in-process server** with a real
  `HttpClient`. Routing and model binding are genuinely exercised — which is the
  only reason a misspelled route or a malformed body is catchable at all.

## Playing product owner

Pre-decided answers to the questions a mob reliably asks. Give a one-line ruling
and move on — do not debate:

| They ask | Ruling |
|---|---|
| How long is a code? Which characters? | **Their call.** Short and URL-safe. Don't let them design an alphabet for ten minutes. |
| Same URL twice — same code? | **Their call** (R6). Reference mints a fresh code. Whatever they pick, story 3 must honour it. |
| Resolving a code nobody minted? | **Their call** (R5) — but pick ONE and apply it everywhere. |
| Reference's choice? | It throws. Say so only if they ask *after* deciding. |
| Is `""` a URL? `"   "`? | **Invalid.** Anything beyond that is their call — but they must say which. |
| Should we validate the scheme? | **Their call.** Worth noting `Uri.TryCreate` accepts `javascript:` and `mailto:` quite happily. |
| Is `K3F9Q2` the same as `k3f9q2`? | **Their call** (R8) — then ask *where that decision is written down*. It should be in exactly one place. |
| What if the generator repeats a code? | **The store must be asked, not assumed** (R9). How it refuses is theirs. |
| Can we just use a `Dictionary` and move on? | **For now, yes** — then point at R4 and ask for the test that proves a link survives a restart. |
| Should we use Moq / NSubstitute? | **No** — and it's worth thirty seconds: a mock can only tell you what you told it. Story 2 is about exactly that failure. |
| Do we need async / `Task`? | **No.** Named as a decision, not an oversight. Parking lot. |
| Which column is the primary key? | Implementation detail — don't let them test it. |
| Do we need transactions? Connection pooling? | Out of scope. Parking lot. |

**Out of scope, say no:** link expiry, click analytics, vanity/custom codes,
authentication, rate limiting, deletion, sharding, async repositories,
unit-of-work, middleware, content negotiation beyond JSON.

## The forcing prompt (checkpoint #1)

R4 is the hinge of the whole kata, and it is the one rule a mob will happily
skip, because a `Dictionary` makes every test they've written go green. When
they're green and pleased with themselves, ask for one more test:

> The story says a short link still resolves tomorrow — after a restart, from a
> different server. **Write the test that proves it.**

Let them actually try. They can't, and that failure is the lesson: the design has
no room for the requirement. Do not offer the word "interface" — wait. The
sentence you want from the mob is some version of *"storage has to live
somewhere else."*

## Coverage check

Instead of ticking off R-numbers, ask for four boxes:

- **[positive]** the round trip, and a second link that proves codes differ,
- **[boundary]** the smallest case that proves codes are generated, not constant,
- **[edge]** an unknown code; the same URL twice,
- **[negative]** blank/invalid input; **and a collision that doesn't lose a link.**

Three extras specific to this kata:

- **Did a test they couldn't write drive the refactor?** If storage moved behind
  an interface because someone said "that's the pattern", the kata's main lesson
  did not land. R4 is the pressure; the interface is only the response.
- **Are they running the same test against both stores?** A mob with two separate
  repository suites has two chances to be wrong, and R12 will only fire in one of
  them.
- **Can the caller of the service still see a database exception?** If yes, R13
  is outstanding — and story 3 will make them pay for it.

## What SQLite actually says (so you can name it in the room)

Verified on .NET 8 with `Microsoft.Data.Sqlite` 8.0.8:

| | |
|---|---|
| Message | `SQLite Error 19: 'UNIQUE constraint failed: links.code'.` |
| `SqliteErrorCode` | **19** (`SQLITE_CONSTRAINT`) — the stable one to check |
| `SqliteExtendedErrorCode` | **1555** (`SQLITE_CONSTRAINT_PRIMARYKEY`) — changes if you declare a `UNIQUE` index instead |
| After the throw | the **original row is untouched**; the write was refused whole |
| The naive fake | `dict[code] = url` overwrites, returns normally, **loses a URL** |
| Case | SQLite's default `BINARY` collation finds **0 rows** for `ABC123`; a `Dictionary` built with `StringComparer.OrdinalIgnoreCase` finds it. That's R14. |

## Retro prompt

> Diff the test list you wrote at the start against this one. What did you miss?
> Then: the moment storage moved behind an interface — what actually caused it,
> a test you couldn't write or somebody's instinct? And finally, your in-memory
> store agreed with you all session. Name the thing that eventually disagreed,
> and ask what else you currently believe on nothing but a double's word.
