import { LinkService, ShortLink } from './link-service';

// URL Shortener kata — frontend reference: the stand-in for the backend.
//
// The spec's first design decision was FAKE or MOCK. This team wrote a fake: a
// real, working LinkService that keeps its links in memory. Three reasons,
// each one a test that got easier:
//
//   · It REMEMBERS. `asked` records every URL the page sent, so "the service
//     was asked for exactly what was typed" and "blank input asks nothing" are
//     plain array assertions — no spy API to learn.
//   · It can WAIT. hold()/release() keep every answer back until the test says
//     so, which is the only way to look at the page mid-request (F4).
//   · It is ONE object with a memory, so the extended story's "create, then the
//     table shows it on top" needs no extra wiring: list() already knows.
//
// A mock (vi.fn() returning canned promises) could do each of those, one
// configuration at a time. What it cannot do is behave like a service nobody
// told it about — and what a fake cannot do is disagree with the real backend.
// Nothing in this track checks that the real service keeps these promises;
// that gap is the backend track's contract suite, one kata away. Say so at the
// retro.
//
// It also mints a FRESH short link every time, even for a URL it has seen —
// the same answer the backend reference chose. A mob whose product owner
// wanted "the same link back" would change this, and the page would not need
// to know.
export class InMemoryLinkService extends LinkService {
  /** Every URL the page asked to shorten, in the order it asked. */
  readonly asked: string[] = [];

  /** When set, every call fails the way a refusing or unreachable service would. */
  failure: Error | null = null;

  private links: ShortLink[];
  private readonly codes: string[];
  private readonly now: () => Date;
  private gate: Promise<void> = Promise.resolve();
  private openGate: () => void = () => undefined;

  constructor(
    options: { links?: ShortLink[]; codes?: string[]; now?: () => Date } = {},
  ) {
    super();
    this.links = [...(options.links ?? [])];
    this.codes = [...(options.codes ?? ['k3f9q2', 'p8w4zt', 'm2x7cd'])];
    this.now = options.now ?? (() => new Date());
  }

  /** Hold every answer back until release() — to look at the page while it waits. */
  hold(): void {
    this.gate = new Promise((open) => (this.openGate = open));
  }

  release(): void {
    this.openGate();
  }

  override async create(url: string): Promise<ShortLink> {
    this.asked.push(url);
    await this.gate;
    if (this.failure) throw this.failure;

    const code = this.codes.shift();
    if (!code) throw new Error('the fake ran out of codes — give it more');

    const link: ShortLink = {
      url,
      shortUrl: `https://short.example/${code}`,
      createdAt: this.now().toISOString(),
    };
    this.links = [link, ...this.links]; // newest first, as the contract promises
    return link;
  }

  override async list(): Promise<ShortLink[]> {
    await this.gate;
    if (this.failure) throw this.failure;
    return [...this.links];
  }
}
