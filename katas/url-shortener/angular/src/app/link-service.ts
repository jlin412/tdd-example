// ════════════════════════════════════════════════════════════════════
// GIVEN TO YOU — the frontend track's only collaborator, as a contract.
// ════════════════════════════════════════════════════════════════════
// Not part of the exercise. The page talks to a link service it does not own;
// this file says what that service PROMISES, and deliberately nothing about
// how it keeps them (HTTP, a database, how codes are minted — that is the
// backend track's problem, not yours).
//
// There is NO implementation here, on purpose. In your tests you supply a
// stand-in: a FAKE (a small class that really works, in memory) or a MOCK
// (vi.fn() with canned answers). Choosing between them is your first design
// decision, and the spec file asks you to make it out loud.
//
// It is an abstract class rather than an interface because Angular's DI needs
// something that exists at runtime to use as a token:
//
//   TestBed.configureTestingModule({
//     providers: [{ provide: LinkService, useValue: yourStandIn }],
//   });

export interface ShortLink {
  /** The long URL, exactly as the page sent it. */
  readonly url: string;
  /** The short link to show people, e.g. https://short.example/k3f9q2 */
  readonly shortUrl: string;
  /** When the link was created, as an ISO-8601 timestamp. */
  readonly createdAt: string;
}

export abstract class LinkService {
  /**
   * Creates a short link for `url` and resolves with it. Rejects if the service
   * refuses the URL or cannot be reached.
   */
  abstract create(url: string): Promise<ShortLink>;

  /**
   * Every link the service holds, newest first by `createdAt`. Rejects if the
   * service cannot be reached. (Nothing needs this until the extended story.)
   */
  abstract list(): Promise<ShortLink[]>;
}
