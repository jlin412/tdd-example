# URL Shortener — the Story

Long links break. They wrap in emails, get mangled in chat, and can't be read
down a phone line. We're building a **link shortener**: paste a long URL, get a
short one back, and anyone who uses the short one ends up where the long one
pointed.

This is the whole product, told the way the product owner tells it: what people
see, and what they're promised. It says nothing about how to build it, and it is
not a numbered requirement list — turning it into a list of tests is your first
job.

## The page

One page, and one row across the top of it:

```
┌───────────────────────────────────────────────────────────────────────────────┐
│ [ https://example.com/a/very… ]  ( Create )  [ https://short.example/k3f9q2 ] │
│   the long URL — paste it here                 the short URL — read only      │
└───────────────────────────────────────────────────────────────────────────────┘
```

On the **left**, a box to paste a long URL into. In the middle, a **Create**
button. On the **right**, a box showing the short URL: you can select it and copy
it, but you can't type into it.

Paste a long URL, press **Create** to shorten it, and the short URL appears on
the right — next to the long URL it stands for.

## The short link

A short URL is the shortener's own address plus a **short code** — half a dozen
characters, like `k3f9q2`. Codes are short and URL-safe; exactly how long, and
out of which characters, is the team's call.

Hand a code back to the shortener and it **resolves** it: you get the original
URL. That's how a short link takes someone to the right place.

One promise is not negotiable, because it's the entire reason anybody would use
this: **a short link keeps working.** Paste one into a document, come back to it
a week later — after the shortener has been restarted, and quite possibly on a
different server from the one that made it — and it still takes you to the right
place.

## A walk through it

| you do this | you should see | and the shortener now knows |
|---|---|---|
| open the page | an empty short-URL box | nothing yet |
| paste `https://example.com/a/very/long/path`, press **Create** | `https://short.example/k3f9q2` on the right | `k3f9q2` → `…/a/very/long/path` |
| paste `https://example.com/somewhere/else`, press **Create** | a **different** short URL, say `…/p8w4zt` | both links |
| resolve `k3f9q2` | — | still `https://example.com/a/very/long/path` |
| resolve `nope99` | — | *nothing — that code was never made* |
| press **Create** with the long box empty | nothing happens | unchanged |
| press **Create** twice, quickly | one short URL, not two | one new link, not two |
| press **Create** while the shortener can't be reached | a message saying it didn't work | unchanged |
| paste the **first** URL again, press **Create** | **?** *(the story doesn't say)* | **?** |

Read that table carefully before writing anything. The third row is the only
reason you can tell a real shortener from one that hands everybody the same
answer — and the last row is a decision nobody has made yet.

## What the story doesn't say

Deliberate gaps. Some matter more to the page and some to the shortener behind
it, but all of them are the product's — decide them as a team, then encode each
decision as a test:

- **The same URL, twice.** The existing short link back, or a fresh one? Both are
  real products that real companies ship. Whichever you pick is a promise — about
  what gets kept, and about what people see. Discuss it before you decide it.
- **A code nobody made.** What should resolving `nope99` do? There's no universal
  answer — but whatever it is should happen the same way every time.
- **A URL that isn't one.** An empty box does nothing — but what about `"   "`?
  `"banana"`? And whose job is it to say so: the page's, the shortener's, or
  both?
- **Upper and lower case.** Is `K3F9Q2` the same link as `k3f9q2`? Wherever you
  answer that — is it written down in one place, or does it just happen to be so?
- **Two codes colliding.** Nothing guarantees that whatever makes codes never
  makes the same one twice. What should happen if it does — and would anybody
  find out?
- **The long URL, after Create.** Does it stay beside its short URL, or clear for
  the next one? And if someone edits it afterwards, what should the short box
  show?
- **When it doesn't work.** What exactly does the message say, and what's left on
  the screen when it does?

There is no single correct contract. What matters is that you **choose, and your
tests express the choice.**

## The loop

1. **RED** — write ONE failing test. Run it. Watch it fail for the right reason.
2. **GREEN** — write the *minimum* production code to pass. Nothing more.
3. **REFACTOR** — improve the code (and tests) while staying green. Then repeat.

Before any of that, though: write the **test list** (STEP 0 in your track's test
file).

## Where things live

The product has two halves, and you can build either one — or both, in either
order:

- **The shortener** (backend track, C#): [csharp/](csharp/), starting from
  [UrlShortenerTests.cs](csharp/UrlShortenerTests.cs).
- **The page** (frontend track, Angular): [angular/](angular/), starting from
  [shortener-page.spec.ts](angular/src/app/shortener-page.spec.ts).

The kata [README](README.md) explains how the tracks differ and how to run each.
Reference solutions for both are in [solutions/](solutions/) — keep them closed
until the retro.

## What next

Shipped it? People want to see what they've made. See
[UrlShortenerExtendedStory.md](UrlShortenerExtendedStory.md).
