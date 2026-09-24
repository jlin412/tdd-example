import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';

import { LinkService, ShortLink } from './link-service';

// URL Shortener kata — frontend reference (F1–F6, one possible end state).
//
// From the checkpoint-#1 menu this team took, one at a time:
//   · (c) COMPUTED DERIVED STATE — what the short-URL box shows is not stored
//     anywhere; it is derived (see shortUrl below). That is what makes F6 a
//     one-liner instead of a rule every event handler has to remember.
//   · (e) A REUSABLE FAKE — the stand-in written inside the first spec moved
//     into its own file (in-memory-link-service.ts) once a second test file
//     needed it. It is also the only kind of stand-in that could power
//     `npm start` for a demo; a vi.fn() mock has no memory to show.
//
// Declined, with reasons:
//   · (a) SMART / DUMB SPLIT — one bar with three controls is one component's
//     worth of template. The table in the extended story is what earns it.
//   · (b) SIGNAL STORE — four signals and one method; a store would be a
//     pass-through with a longer name.
//   · (d) OBSERVABLES — the contract hands back Promises, and async/await reads
//     top to bottom. A single-flight RxJS operator (exhaustMap) would replace
//     one boolean guard with a concept the rest of the mob has to learn.
//
// Contract choices the story left open (a different mob may have gone the
// other way):
//   · The long URL STAYS in its box after Create, so the pair reads left to
//     right. Editing it clears the short URL (F6) rather than leaving a short
//     link beside a URL it was not made for.
//   · Only blank input is refused on the page. Whether "banana" is a URL is the
//     service's call — it rejects, and the page shows F5's message.
//   · The same URL twice asks the service twice and shows whatever comes back.
//     Fresh link or the existing one is the service's decision, not the page's.
@Component({
  selector: 'app-shortener-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  // Bootstrap class names throughout: inert in the unit tests (jsdom applies
  // no CSS), styled when the page is served by smoke/ with Bootstrap loaded.
  template: `
    <h1 class="h3 mb-4">URL Shortener</h1>
    <div class="input-group mb-3">
      <input
        class="form-control"
        aria-label="Long URL"
        type="text"
        placeholder="Paste a long URL"
        [value]="draft()"
        (input)="draft.set($any($event.target).value)"
        (keyup.enter)="create()"
      />
      <button class="btn btn-primary" type="button" (click)="create()" [disabled]="waiting()">
        Create
      </button>
      <input
        class="form-control bg-body-tertiary"
        aria-label="Short URL"
        placeholder="Short URL"
        readonly
        [value]="shortUrl()"
      />
    </div>
    @if (problem()) {
      <p class="alert alert-danger" role="alert">{{ problem() }}</p>
    }
  `,
})
export class ShortenerPage {
  private readonly links = inject(LinkService);

  protected readonly draft = signal('');
  protected readonly waiting = signal(false);
  protected readonly problem = signal('');
  private readonly created = signal<ShortLink | null>(null);

  // F6 — (c) paying for itself. Nothing ever has to remember to CLEAR the
  // short box: it simply stops matching the moment the long URL changes.
  protected readonly shortUrl = computed(() => {
    const link = this.created();
    return link && link.url === this.draft().trim() ? link.shortUrl : '';
  });

  protected async create(): Promise<void> {
    const url = this.draft().trim();
    // F3 — blank asks nothing. F4 — nor does a second press while waiting.
    // The [disabled] button covers the click; this guard is what stops Enter
    // in the text box, which a disabled button knows nothing about.
    if (!url || this.waiting()) return;

    this.waiting.set(true);
    this.problem.set('');
    try {
      this.created.set(await this.links.create(url)); // F2
    } catch {
      this.created.set(null);
      this.problem.set("Couldn't create a short link. Try again."); // F5
    } finally {
      this.waiting.set(false);
    }
  }
}
