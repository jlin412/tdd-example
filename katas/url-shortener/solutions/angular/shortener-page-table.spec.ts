// URL Shortener kata — frontend reference test suite, extended story (F7–F10).
// The bar's own rules (F1–F6) are proven in shortener-page.spec.ts; these tests
// are about the table, and use the same hand-written fake.

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InMemoryLinkService } from './in-memory-link-service';
import { LinkService, ShortLink } from './link-service';
import { ShortenerPageWithTable } from './shortener-page-table';

type Fixture = ComponentFixture<ShortenerPageWithTable>;

async function render(service: LinkService): Promise<Fixture> {
  TestBed.configureTestingModule({ providers: [{ provide: LinkService, useValue: service }] });
  const fixture = TestBed.createComponent(ShortenerPageWithTable);
  await settle(fixture);
  return fixture;
}

// See shortener-page.spec.ts: whenStable() cannot see the fake's promises.
async function settle(f: Fixture): Promise<void> {
  await new Promise((resolve) => setTimeout(resolve));
  await f.whenStable();
}

const page = (f: Fixture) => f.nativeElement as HTMLElement;
const text = (f: Fixture) => page(f).textContent ?? '';
const alertText = (f: Fixture) =>
  page(f).querySelector('[role="alert"]')?.textContent?.trim() ?? '';

/** Each body row as [long URL, short URL], top to bottom. */
const rows = (f: Fixture) =>
  [...page(f).querySelectorAll('tbody tr')].map((row) =>
    [...row.querySelectorAll('td')].map((cell) => cell.textContent?.trim()),
  );

async function create(f: Fixture, url: string): Promise<void> {
  const longBox = page(f).querySelector('input[aria-label="Long URL"]') as HTMLInputElement;
  longBox.value = url;
  longBox.dispatchEvent(new Event('input'));
  await settle(f);
  (page(f).querySelector('button') as HTMLButtonElement).click();
  await settle(f);
}

const link = (code: string, url: string, createdAt: string): ShortLink => ({
  url,
  shortUrl: `https://short.example/${code}`,
  createdAt,
});

describe('ShortenerPageWithTable (through the DOM, backend faked)', () => {
  it('shows every link the service holds, newest first [positive] (F7)', async () => {
    const f = await render(
      new InMemoryLinkService({
        links: [
          link('p8w4zt', 'https://example.com/somewhere/else', '2026-09-24T12:01:00.000Z'),
          link('k3f9q2', 'https://example.com/a/very/long/path', '2026-09-24T12:00:00.000Z'),
        ],
      }),
    );

    expect(rows(f)).toEqual([
      ['https://example.com/somewhere/else', 'https://short.example/p8w4zt'],
      ['https://example.com/a/very/long/path', 'https://short.example/k3f9q2'],
    ]);
  });

  it('says so when there are no links yet [edge] (F8)', async () => {
    const f = await render(new InMemoryLinkService());

    expect(text(f)).toContain('No links yet');
    expect(rows(f)).toEqual([]);
  });

  it('does not claim there are no links before the service has answered [edge] (F8)', async () => {
    const service = new InMemoryLinkService();
    service.hold();
    const f = await render(service);
    expect(text(f)).not.toContain('No links yet');

    service.release();
    await settle(f);
    expect(text(f)).toContain('No links yet');
  });

  it('puts a link you create on top of the table [positive] (F9)', async () => {
    const f = await render(
      new InMemoryLinkService({
        codes: ['p8w4zt'],
        links: [link('k3f9q2', 'https://example.com/older', '2026-09-24T12:00:00.000Z')],
      }),
    );

    await create(f, 'https://example.com/newer');

    expect(rows(f)).toEqual([
      ['https://example.com/newer', 'https://short.example/p8w4zt'],
      ['https://example.com/older', 'https://short.example/k3f9q2'],
    ]);
  });

  it('shows the same URL twice when the service mints two links for it [edge] (discussion)', async () => {
    // A consequence of the SERVICE's same-URL decision, not a rule of the page.
    // A service that returned the existing link would need this test changed —
    // and the page to reload rather than prepend.
    const f = await render(new InMemoryLinkService({ codes: ['k3f9q2', 'p8w4zt'] }));

    await create(f, 'https://example.com/again');
    await create(f, 'https://example.com/again');

    expect(rows(f)).toEqual([
      ['https://example.com/again', 'https://short.example/p8w4zt'],
      ['https://example.com/again', 'https://short.example/k3f9q2'],
    ]);
  });

  it('says so when the links cannot be loaded [negative] (F10)', async () => {
    const service = new InMemoryLinkService();
    service.failure = new Error('service unavailable');
    const f = await render(service);

    expect(alertText(f)).toBe("Couldn't load your links. Try again later.");
    expect(text(f)).not.toContain('No links yet');
  });

  it('does not pass off one new link as the whole table when loading failed [edge] (F10)', async () => {
    const service = new InMemoryLinkService();
    service.failure = new Error('service unavailable');
    const f = await render(service);

    service.failure = null;
    await create(f, 'https://example.com/made-anyway');

    expect(rows(f)).toEqual([]);
    expect(text(f)).not.toContain('No links yet');
  });
});
