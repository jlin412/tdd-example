import { expect, test } from '@playwright/test';

// URL Shortener kata — THE smoke test: the reference page and the reference
// backend, together, in a real browser, over real HTTP, against a real
// database. One test on purpose. Every rule is already proven, faster and
// closer to the code that decides it, in each track's own suite. The only
// question left for this level is the one no single track can ask: do the two
// halves actually fit?

test('a link made on the page is kept by the shortener, resolves, and is listed after a reload', async ({
  page,
  request,
}) => {
  const longUrl = 'https://example.com/a/very/long/path';

  // The page asks the backend for its links, and a fresh backend has none.
  await page.goto('/');
  await expect(page.getByText('No links yet')).toBeVisible();

  // Create: the page posts the URL, and the code that comes back is shown as
  // a short URL.
  await page.getByLabel('Long URL').fill(longUrl);
  await page.getByRole('button', { name: 'Create' }).click();
  const shortBox = page.getByLabel('Short URL');
  await expect(shortBox).toHaveValue(/\/links\/[a-z2-9]{6}$/);
  const shortUrl = await shortBox.inputValue();

  // The short URL on the screen really resolves, at the shortener, to the long one.
  const resolved = await request.get(shortUrl);
  expect(resolved.ok()).toBe(true);
  expect(await resolved.json()).toEqual({ url: longUrl });

  // A fresh page remembers nothing — so a row after a reload was kept by the
  // shortener, and listed back through GET /links.
  await page.reload();
  await expect(page.getByRole('row').nth(1).getByRole('cell')).toHaveText([longUrl, shortUrl]);
});
