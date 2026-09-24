# URL Shortener — the Extended Story (the table)

You've shipped [the shortener](UrlShortenerStory.md). People like it, and they
keep asking the same question: *"what was that link I made on Tuesday?"*

## What changes

Under the row at the top, the page now shows a **table of every link** — each
short URL beside the long URL it stands for — **newest first**, by when each link
was created. A link you create turns up at the **top** of the table straight
away.

Every link means every link: there are no accounts, so everyone sees the same
table.

Everything else is exactly as it was, and everything the first story left
unsettled is still unsettled however you settled it.

## What it looks like

```
┌───────────────────────────────────────────────────────────────────────────┐
│ [ https://example.com/new ]  ( Create )  [ https://short.example/m2x7cd ] │
├───────────────────────────────────────────────────────────────────────────┤
│  Long URL                                Short URL                        │
│  https://example.com/new                 https://short.example/m2x7cd     │
│  https://example.com/somewhere/else      https://short.example/p8w4zt     │
│  https://example.com/a/very/long/path    https://short.example/k3f9q2     │
└───────────────────────────────────────────────────────────────────────────┘
```

## A walk through it

| you do this | you should see |
|---|---|
| open the page before anyone has made a link | a message saying there are no links yet |
| open it after `k3f9q2` was made at 12:00 and `p8w4zt` at 12:01 | two rows, `p8w4zt` on top |
| create a link for `https://example.com/new` | its row at the top, above both |
| open the page while the shortener can't be reached | a message saying the links couldn't be loaded |
| two links made in the very same instant | **?** *(the story doesn't say which comes first)* |

## What the story doesn't say

- **Two links, one instant.** Clocks repeat themselves, just like code makers do.
  Which of two simultaneous links is "newer"?
- **Whose clock?** A link can be made on one server and listed from another, and
  no two clocks agree exactly. "When it was created" according to what?
- **The same URL, twice — again.** One row or two? Your answer from the first
  story decides. Now that people can *see* it, did you mean it?
- **Still loading, or empty?** For a moment after the page opens, nobody knows
  yet. What should the table say during that moment?

There is no single correct contract. What matters is that you **choose, and your
tests express the choice.**

## Your job, in order

1. **Update your test list first** — the new tests, and any existing ones this
   makes wrong. Write the list before you touch the code.
2. **Run the suite.** Which existing tests still pass? Anything that breaks here
   is telling you something about how tightly your tests were coupled.
3. **Now drive the change test-first**, one red at a time.
