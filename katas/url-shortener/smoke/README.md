# URL Shortener — the whole product, running

**For facilitators and the retro, not for the mob.** Neither track needs this
folder, and nothing in it is part of either kata. It does two things with the
reference solutions — the Angular page and the integrated C# backend, together:

- **Try it by hand:** `npm start`, then open <http://127.0.0.1:5179>.
- **Prove it end to end:** `npm test` — one Playwright smoke test.

## Try it by hand

```bash
(cd ../angular && npm install)   # once — the page is built with its packages
npm install                       # once — Playwright and Bootstrap
npm start                         # builds the page, starts the server
```

Open **<http://127.0.0.1:5179>** once the log says `Now listening`. Paste a long
URL, press **Create** (or Enter), and the short URL appears on the right; the
table below lists every link, newest first. Open a short URL itself and the
shortener answers with the long one, as JSON — this backend resolves codes
rather than redirecting. Press **Ctrl+C** to stop.

The database lives in memory, so every start is empty and stopping forgets
everything. Set `PORT` to use a different port (`PORT=8080 npm start`). The page
is styled with Bootstrap: a bordered table with a shaded header row.

## The smoke test

The two tracks never meet. The frontend's tests stand in for the service with a
fake; the backend's tests drive their own in-process server. Each suite is green
on its own terms — and neither can tell you whether the two halves actually fit.
This is the **one** test that can: the reference page and the reference backend,
together, in a real browser, over real HTTP, against a real database.

## What it proves

[`smoke.spec.ts`](smoke.spec.ts) — *a link made on the page is kept by the
shortener, resolves, and is listed after a reload*:

1. The page opens and asks the backend for its links: **"No links yet"**.
2. Paste a long URL, press **Create**: a short URL appears.
3. That short URL, requested from the shortener, **resolves to the long one**.
4. Reload: the link is **at the top of the table** — only the shortener could have
   remembered it.

One test, on purpose. Every rule is already proven faster, and closer to the code
that decides it, in the tracks' own suites. This level exists for the one
question they can't ask.

## What it found

Putting the page's contract (`angular/src/app/link-service.ts`) beside the
backend's API for the first time found a real gap: `POST /links` answered with
`{ "code": … }` alone, while the page promises callers the **url** and
**createdAt** of the link it made. Every suite in both tracks was green. The fix
is **R24** in [`facilitator/RULES.md`](../facilitator/RULES.md): a create now
answers with the whole link, the same shape as a table row.

Put the old answer back and this test fails — the page's `create()` loses the
URL, so its own F6 rule refuses to show a short link beside it:

```
Error: expect(locator).toHaveValue(expected) failed
Locator: getByLabel('Short URL')
Expected pattern: /\/links\/[a-z2-9]{6}$/
Received string:  ""
```

That is the retro's closing question, answered: *your stand-in agreed with you,
and nothing in your track checked it against a real shortener — so where does
that check live?* Here. And it is also why there is only one of them.

## What runs

| Piece | What it is |
|---|---|
| [`host/`](host/) | the **integrated** backend reference, served for real — a separate `Microsoft.NET.Sdk.Web` project that compiles `solutions/csharp-integrated/` and the given `SqliteTestDatabase.cs`. In-memory SQLite, so every run starts empty. (Not the isolated reference: its SQLite store shares one connection, which is not thread-safe behind a real server.) |
| [`web/http-link-service.ts`](web/http-link-service.ts) | the only real `LinkService` in the repo: `POST /links`, `GET /links`, and the one translation left between the tracks — the backend deals in **codes**, the page shows **short URLs** (`<origin>/links/<code>`, where this shortener resolves one) |
| [`web/main.ts`](web/main.ts) | an entry point for the extended story's table page, wired to that service |
| [`prepare.mjs`](prepare.mjs) | assembles the page in `build/` — a copy of the Angular workspace with the frontend reference dropped in (the repo's usual "copy the solution over a scratch skeleton", scripted) — adds Bootstrap's CSS and a page container, and builds it |
| [`serve.mjs`](serve.mjs) | runs `prepare.mjs`, then starts the host on port 5179 — what `npm start` runs, and what the smoke test's `webServer` runs, so what you click is exactly what the test checks |

The host serves the built page and the API from **one origin**, so there is no
proxy and no CORS. Bootstrap is loaded only into this build: the reference
templates carry its class names (`table table-bordered`, a `table-secondary`
header row, an `input-group` bar), which do nothing in the tracks' unit tests
and nothing in the kata's own workspace, which gains no dependency.

## Running the test

Needs .NET SDK 8+, Node 20.19+/22.12+/24+, and the Angular workspace installed
(see *Try it by hand* above).

```bash
npx playwright install chromium     # once — downloads the browser
npm test                            # builds both halves, starts the host, runs the test
npx playwright test --headed        # the same, in a visible browser window
```

The test starts its own server, so stop any `npm start` first — both use port
5179.

The first run restores NuGet packages and takes longer; after that it is a few
seconds. Verified with Playwright 1.63, Angular 21.2 and .NET 8: **1 passed**.
