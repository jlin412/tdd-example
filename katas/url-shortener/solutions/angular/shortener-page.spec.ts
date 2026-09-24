// URL Shortener kata — frontend reference test suite (F1–F6, one possible end
// state). Drives the page through the DOM; the backend is the hand-written fake
// in in-memory-link-service.ts, handed to the page through Angular's DI.

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InMemoryLinkService } from './in-memory-link-service';
import { LinkService } from './link-service';
import { ShortenerPage } from './shortener-page';

type Fixture = ComponentFixture<ShortenerPage>;

async function render(service: LinkService = new InMemoryLinkService()): Promise<Fixture> {
  TestBed.configureTestingModule({ providers: [{ provide: LinkService, useValue: service }] });
  const fixture = TestBed.createComponent(ShortenerPage);
  await fixture.whenStable();
  return fixture;
}

const page = (f: Fixture) => f.nativeElement as HTMLElement;
const shortBox = (f: Fixture) =>
  page(f).querySelector('input[aria-label="Short URL"]') as HTMLInputElement;
const longBox = (f: Fixture) =>
  page(f).querySelector('input[aria-label="Long URL"]') as HTMLInputElement;
const createButton = (f: Fixture) =>
  [...page(f).querySelectorAll('button')].find(
    (b) => b.textContent?.trim() === 'Create',
  ) as HTMLButtonElement;
const alertText = (f: Fixture) =>
  page(f).querySelector('[role="alert"]')?.textContent?.trim() ?? '';

// whenStable() only waits for work ANGULAR knows about. A promise the fake
// resolves — especially one a test releases by hand — is invisible to zoneless
// change detection, so on its own whenStable() can return before the page has
// seen the answer. A zero-length timeout queues behind every pending promise;
// after it, whenStable() has something true to wait for.
async function settle(f: Fixture): Promise<void> {
  await new Promise((resolve) => setTimeout(resolve));
  await f.whenStable();
}

async function type(f: Fixture, text: string): Promise<void> {
  longBox(f).value = text;
  longBox(f).dispatchEvent(new Event('input'));
  await settle(f);
}

async function pressCreate(f: Fixture): Promise<void> {
  createButton(f).click();
  await settle(f);
}

async function pressEnter(f: Fixture): Promise<void> {
  longBox(f).dispatchEvent(new KeyboardEvent('keyup', { key: 'Enter' }));
  await settle(f);
}

describe('ShortenerPage (through the DOM, backend faked)', () => {
  it('shows the short URL in a box nobody can type into, empty to begin with [edge] (F1)', async () => {
    const f = await render();

    expect(shortBox(f).readOnly).toBe(true);
    expect(shortBox(f).value).toBe('');
  });

  it('reads long URL, Create, short URL — left to right [positive] (F1)', async () => {
    // jsdom has no layout engine, so "on the right" cannot be measured here.
    // What CAN be pinned is document order, which is also the order a screen
    // reader and the Tab key walk through.
    const f = await render();

    const controls = [...page(f).querySelectorAll('input, button')];

    expect(controls).toEqual([longBox(f), createButton(f), shortBox(f)]);
  });

  it('asks the service for exactly the URL that was typed [positive] (F2)', async () => {
    const service = new InMemoryLinkService();
    const f = await render(service);

    await type(f, 'https://example.com/a/very/long/path');
    await pressCreate(f);

    expect(service.asked).toEqual(['https://example.com/a/very/long/path']);
  });

  it('shows the short URL the service hands back [positive] (F2)', async () => {
    const f = await render(new InMemoryLinkService({ codes: ['k3f9q2'] }));

    await type(f, 'https://example.com/a/very/long/path');
    await pressCreate(f);

    expect(shortBox(f).value).toBe('https://short.example/k3f9q2');
    expect(longBox(f).value).toBe('https://example.com/a/very/long/path');
  });

  it('trims the URL before asking [edge] (F2)', async () => {
    const service = new InMemoryLinkService();
    const f = await render(service);

    await type(f, '  https://example.com/padded  ');
    await pressCreate(f);

    expect(service.asked).toEqual(['https://example.com/padded']);
  });

  it.each(['', '   '])('asks nothing for blank input %j [negative] (F3)', async (blank) => {
    const service = new InMemoryLinkService();
    const f = await render(service);

    await type(f, blank);
    await pressCreate(f);

    expect(service.asked).toEqual([]);
    expect(shortBox(f).value).toBe('');
  });

  it('disables Create while the service is working [edge] (F4)', async () => {
    const service = new InMemoryLinkService();
    service.hold();
    const f = await render(service);

    await type(f, 'https://example.com/slow');
    await pressCreate(f);
    expect(createButton(f).disabled).toBe(true);

    service.release();
    await settle(f);
    expect(createButton(f).disabled).toBe(false);
  });

  it('asks only once when Enter is pressed again while waiting [edge] (F4)', async () => {
    // The disabled button stops a second CLICK. Nothing about the button stops
    // Enter in the text box — this is the test that keeps the guard honest.
    const service = new InMemoryLinkService();
    service.hold();
    const f = await render(service);

    await type(f, 'https://example.com/slow');
    await pressEnter(f);
    await pressEnter(f);
    service.release();
    await settle(f);

    expect(service.asked).toEqual(['https://example.com/slow']);
  });

  it('says so when the service fails, and shows no short URL [negative] (F5)', async () => {
    const service = new InMemoryLinkService();
    service.failure = new Error('service unavailable');
    const f = await render(service);

    await type(f, 'https://example.com/unlucky');
    await pressCreate(f);

    expect(alertText(f)).toBe("Couldn't create a short link. Try again.");
    expect(shortBox(f).value).toBe('');
  });

  it('clears the message once a later attempt succeeds [edge] (F5)', async () => {
    const service = new InMemoryLinkService();
    service.failure = new Error('service unavailable');
    const f = await render(service);
    await type(f, 'https://example.com/unlucky');
    await pressCreate(f);

    service.failure = null;
    await pressCreate(f);

    expect(alertText(f)).toBe('');
    expect(shortBox(f).value).toBe('https://short.example/k3f9q2');
  });

  it('never leaves a short URL beside a long URL it was not made for [edge] (F6)', async () => {
    const f = await render();
    await type(f, 'https://example.com/first');
    await pressCreate(f);

    await type(f, 'https://example.com/first-but-edited');

    expect(shortBox(f).value).toBe('');
  });

  it('asks again for the same URL and shows whatever comes back [edge] (discussion)', async () => {
    // The page does not decide whether the same URL gets the same link. The
    // service does — and this fake, like the backend reference, mints a fresh
    // one every time.
    const service = new InMemoryLinkService({ codes: ['k3f9q2', 'p8w4zt'] });
    const f = await render(service);
    await type(f, 'https://example.com/again');

    await pressCreate(f);
    await pressCreate(f);

    expect(service.asked).toEqual(['https://example.com/again', 'https://example.com/again']);
    expect(shortBox(f).value).toBe('https://short.example/p8w4zt');
  });
});
