# URL Shortener — facilitator's answer key

**Open this DURING the session, not before.** It is not a spoiler in the way
`solutions/` is — that folder stays closed until the retro, this one you open at
the test-list checkpoint to play product owner.

The stories ([UrlShortenerStory.md](../UrlShortenerStory.md),
[UrlShortenerExtendedStory.md](../UrlShortenerExtendedStory.md)) describe the
product in prose and show a walkthrough; they deliberately do **not** hand over a
numbered rule list, and they say nothing about how either half is built.
Producing the list — as a list of tests — is the mob's first job. This file is
the canonical list to compare theirs against, and it is what the numbers in
`solutions/` refer to:

- **R-numbers** are the **backend track** (C#) — the shortener.
- **F-numbers** are the **frontend track** (Angular) — the page.

Both tracks read the same story. A mob does one per session; the retro is richer
when a second session does the other.

## Backend track

### The shortener — R1–R9

- **R1.** **shorten** takes a long URL and returns a **short code** — short and
  URL-safe. Exact length and alphabet are the mob's call.
- **R2.** **resolve** takes a code that was issued and returns the **original
  URL**.
- **R3.** Two **different** URLs get two **different** codes. (Needs ≥2
  shortenings to demonstrate; one proves nothing — a constant passes it.)
- **R4.** A short link **keeps working** across a process restart and from
  another server. This is the rule that makes in-process storage wrong, and it
  is the only thing in the story that forces a seam. *If a mob never feels this,
  checkpoint #1 has no teeth — see the forcing prompt below.*
- **R5.** Resolving a code that was **never issued** is a defined, deliberate
  behavior. *The mob picks which* — throwing and a Try-pattern are both
  defensible; what matters is that it's chosen, tested, and applied the same way
  every time.
- **R6.** Shortening the **same URL twice** is a defined, deliberate behavior —
  and a **discussion**, not a ruling from you. *The mob picks* — same code back,
  or a fresh one. Both ship in the real world.
- **R7.** **Invalid input is rejected.** Blank and whitespace-only are invalid.
  What else counts (scheme? host?) is the mob's call — but they must say.
- **R8.** **Case sensitivity** of codes is a defined, deliberate behavior. *The
  mob picks* — and it must be expressed in **one place**, not inherited from
  whatever the storage happens to do. (R14 is where a mob finds out whether they
  actually did this.)
- **R9.** A **code collision must not silently lose a link.** Whatever mints
  codes may repeat itself; the store must be *asked* whether a code is taken,
  not assumed to be free. *How* it refuses is the mob's call.

### The real database — R10–R14 (discovered at checkpoint #3)

Nothing in the story says "database". R4 does: a link that must outlive the
process and be readable from another server cannot live in a `Dictionary`.

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

### HTTP — R15–R19 (discovered at checkpoint #4)

Nothing in the story says "HTTP" either. The page does: people use the product
from a browser, and a browser cannot construct a C# object. The route shapes
below are the reference's; the **status-code semantics** are the rules.

- **R15.** Creating a link → **201**, a `Location` header pointing at the new
  link, and the code. (Reference: `POST /links` with `{ "url": … }` →
  `{ "code": … }`.)
- **R16.** Resolving a known code → **200** and the URL. (Reference:
  `GET /links/{code}` → `{ "url": … }`.)
- **R17.** An unknown code → **404**. A URL that isn't one → **400**. This is
  what makes R13 pay: a handler cannot map an error it has to reach into a
  database driver to identify.
- **R18.** Creating a link for an already-shortened URL → **whatever R6
  decided**: 200 with the existing code if the mob chose "same code", another
  201 if they chose "fresh code". There is no third right answer.
- **R19.** The endpoint tests drive a **real in-process server** with a real
  `HttpClient`. Routing and model binding are genuinely exercised — which is the
  only reason a misspelled route or a malformed body is catchable at all.

### The table (extended story) — R20–R24

- **R20.** Every link records **when it was created**, at the moment it is
  created. The clock is a collaborator, exactly like the code generator: "newest
  first" cannot be asserted against a clock nobody controls.
- **R21.** The shortener can **list every link, newest first** by creation time.
  Nothing yet → an empty list. By *creation time* — not by the order links
  happened to be saved, which differs whenever two servers' clocks disagree.
- **R22.** Two links created in the **same instant** list in a defined order.
  *The mob picks*; the reference lists the later-saved first. Both stores must
  give the same answer (R11 again) — this is where a sort that is merely
  *stable* passes every test with distinct times and then disagrees.
- **R23.** Over HTTP the table is **200 with a list** — empty means `[]`, not
  404. (Reference: `GET /links` → `[{ "code", "url", "createdAt" }]`.)
- **R24.** Creating a link over HTTP answers with **the whole link** — code, url
  and createdAt, the same shape as a table row — not the code alone. Nothing in
  either track forces this: it was found by the end-to-end smoke test
  (`smoke/`), when the page's contract met this API for the first time. Worth
  telling a backend mob *after* they've finished, as a question: "who is going
  to call this, and what will they need back?"

## Frontend track

The shortener is **given as a contract** (`angular/src/app/link-service.ts`):
`create(url)` and `list()`, both Promises, no implementation. The mob never
builds it; their tests stand in for it.

### The page — F1–F6

- **F1.** One row: a box for the long URL on the left, a **Create** button in
  the middle, a **read-only** box for the short URL on the right. The short box
  starts empty.
- **F2.** Pressing Create asks the service to shorten **exactly the long URL**,
  trimmed, and shows the short URL it hands back.
- **F3.** **Blank or whitespace-only** input asks the service **nothing**.
- **F4.** **One request at a time.** While the service is working, Create is
  disabled — and Enter in the long box, which a disabled button knows nothing
  about, asks nothing more either.
- **F5.** When the service **fails**, the page says so and shows no short URL. A
  later attempt that works clears the message.
- **F6.** A short URL is only ever shown **beside the long URL it was made for**.
  Edit the long URL and the short box stops showing it.

### The table (extended story) — F7–F10

- **F7.** On opening, the page shows **every link the service holds, newest
  first** — long URL, then short URL, the same order as the bar.
- **F8.** No links → a message saying so. But **not loaded yet is not empty**:
  the message appears only once the service has actually answered.
- **F9.** A link you create appears **at the top** of the table.
- **F10.** If the links **can't be loaded**, the page says so — and does not then
  pass off a newly created link as the whole table.

## Playing product owner

Pre-decided answers to the questions a mob reliably asks. Give a one-line ruling
and move on — do not debate. The exception is marked: that one is *supposed* to
be debated.

| Track | They ask | Ruling |
|---|---|---|
| both | Same URL twice — same link back, or a new one? | **Discuss it — that's the point** (R6). Give them five minutes, then make them decide. Both references mint a fresh one. |
| both | How long is a code? Which characters? | **Their call.** Short and URL-safe. Don't let them design an alphabet for ten minutes. |
| both | Is `""` a URL? `"   "`? | **No.** Anything beyond that is their call — but they must say which. |
| both | Is `"banana"` a URL? Whose job is checking? | **Their call.** Worth noting the backend reference checks only blankness ("scheme checking is the caller's job") and the frontend reference only blankness too — so in the references, *nobody* checks. Ask whether that's a decision or an accident. |
| both | Per-user links? Accounts? | **No** — every link, everyone. Out of scope. |
| backend | Resolving a code nobody minted? | **Their call** (R5) — but pick ONE and apply it everywhere. |
| backend | Reference's choice? | It throws. Say so only if they ask *after* deciding. |
| backend | Should we validate the scheme? | **Their call.** Worth noting `Uri.TryCreate` accepts `javascript:` and `mailto:` quite happily. |
| backend | Is `K3F9Q2` the same as `k3f9q2`? | **Their call** (R8) — then ask *where that decision is written down*. It should be in exactly one place. |
| backend | What if the generator repeats a code? | **The store must be asked, not assumed** (R9). How it refuses is theirs. |
| backend | Can we just use a `Dictionary` and move on? | **For now, yes** — then point at R4 and ask for the test that proves a link survives a restart. |
| backend | Should we use Moq / NSubstitute? | **No** — and it's worth thirty seconds: a mock can only tell you what you told it. Checkpoint #3 is about exactly that failure. |
| backend | Do we need async / `Task`? | **No.** Named as a decision, not an oversight. Parking lot. |
| backend | Which column is the primary key? | Implementation detail — don't let them test it. |
| backend | Two links in the same instant — which first? | **Their call** (R22). Reference: the later-saved one. |
| backend | Can the clock just be `DateTime.UtcNow`? | **Not if a test has to say what time it is.** .NET 8's `TimeProvider` is the seam; the fake is a five-line subclass. |
| frontend | Fake or mock for the service? | **Their call — out loud, before the first Create test.** Then the retro compares. |
| frontend | Can we call the API with `HttpClient`? | **No.** The page talks to the contract in `link-service.ts`; how the real service is reached is the other track's problem. |
| frontend | Can a test check the layout? | **Document order, not pixels** — jsdom has no layout engine. Long box, Create, short box is also the order Tab and a screen reader follow. |
| frontend | Does the long URL clear after Create? | **Their call.** Reference keeps it, so the pair reads left to right — and then F6 matters. |
| frontend | Exact wording of labels and messages? | **Their call.** Reference: "Short URL", "Long URL", "Couldn't create a short link. Try again." |
| frontend | Table: sort by `createdAt` on the page, or trust the order? | **Their call.** Reference trusts the contract — re-sorting would hide a backend that broke its promise. |
| frontend | Table: ask for the whole list again after Create, or add the row? | **Their call.** Reference adds the row, which only works because the service mints a fresh link every time. |

**Out of scope, say no:** accounts, link expiry, click analytics, vanity/custom
codes, deletion, editing, search, paging, copy-to-clipboard, rate limiting,
sharding, async repositories, unit-of-work, middleware, content negotiation
beyond JSON, and — for the frontend track — building the real service.

## The forcing prompts

### Backend, checkpoint #1 — R4

R4 is the hinge of the backend track, and it is the one rule a mob will happily
skip, because a `Dictionary` makes every test they've written go green. When
they're green and pleased with themselves, ask for one more test:

> The story says a short link still resolves tomorrow — after a restart, from a
> different server. **Write the test that proves it.**

Let them actually try. They can't, and that failure is the lesson: the design has
no room for the requirement. Do not offer the word "interface" — wait. The
sentence you want from the mob is some version of *"storage has to live
somewhere else."*

### Backend, checkpoint #3 — the prediction

Before they point their tests at SQLite, make them write down which tests they
expect to pass. On the whiteboard, where everyone can see it. The prediction is
worth more than the result, and it only counts if it was written first.

### Backend, checkpoint #4 — the page

If they're stuck on "why HTTP?", don't answer it. Read them the story's first
line about the page, and ask how a browser would call `UrlShortener.Shorten`.

### Frontend, the first Create test

The first test that presses Create **cannot pass** without a service, and
there isn't one. Watch for two escapes: writing an `HttpClient` service (point at
`link-service.ts` — the contract is given), and hard-coding a short URL in the
component (legal as a GREEN step — then ask which test makes it wrong). The
sentence you want is *"we need a stand-in, and we have to choose what kind."*

## Coverage check

Instead of ticking off numbers, ask for four boxes.

**Backend:**

- **[positive]** the round trip, and a second link that proves codes differ,
- **[boundary]** the smallest case that proves codes are generated, not constant,
- **[edge]** an unknown code; the same URL twice,
- **[negative]** blank/invalid input; **and a collision that doesn't lose a link.**

Three extras specific to this track:

- **Did a test they couldn't write drive the refactor?** If storage moved behind
  an interface because someone said "that's the pattern", the kata's main lesson
  did not land. R4 is the pressure; the interface is only the response.
- **Are they running the same test against both stores?** A mob with two separate
  repository suites has two chances to be wrong, and R12 will only fire in one of
  them.
- **Can the caller of the service still see a database exception?** If yes, R13
  is outstanding — and checkpoint #4 will make them pay for it.

**Frontend:**

- **[positive]** Create shows the short URL; the service was asked for exactly
  what was typed,
- **[boundary]** the moment Create stops working, and the moment it works again,
- **[edge]** the empty short box; editing the long URL after Create; Enter while
  waiting,
- **[negative]** blank input asks nothing; a failing service shows a message.

And one frontend-specific check: **are the tests driven through the DOM?** If a
test reaches into the component's fields, the checkpoint-#1 refactor will break
it — which is the whole lesson.

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
| Ties | `ORDER BY created_at DESC` alone makes no promise about two rows with the same time. Drop either reference store's tie-break and exactly that store's run of the same-instant test fails. That's R22. |

## Retro prompt

> Diff the test list you wrote at the start against this one. What did you miss?
> Then, backend: the moment storage moved behind an interface — what actually
> caused it, a test you couldn't write or somebody's instinct? Your in-memory
> store agreed with you all session; name the thing that eventually disagreed.
> Frontend: your stand-in agreed with you too — and *nothing* in your track ever
> checked it against a real shortener. Where would that check live, and who
> owns it?

This repo's answer is [`smoke/`](../smoke/): one Playwright test, reference page
and reference backend together. It found R24 — a gap every suite in both tracks
had been green through. Show it last.
