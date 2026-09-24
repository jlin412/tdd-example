import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';

import { LinkService, ShortLink } from './link-service';
import { LinkTable } from './link-table';

// URL Shortener kata — frontend reference, extended story (F1–F10).
//
// A NEW class on the same selector, so the story-1 reference and its tests stay
// readable on their own (the todo kata's edit stretch did the same). The bar is
// unchanged; what is new is that the page now OWNS A LIST, and three things
// touch it: opening the page loads it, creating a link adds to it, and the
// table shows it.
//
// Contract choices the extended story left open:
//   · ORDER — the page shows links in the order the service returns them. The
//     contract promises newest first; re-sorting here would quietly hide a
//     backend that broke that promise from the one team able to fix it.
//   · A NEW LINK goes on top without asking for the whole list again (F9).
//     That leans on the service minting a fresh link every time. A service
//     that handed back an EXISTING link for a URL it has seen would leave a
//     duplicate row here — and reloading the list after each create is the
//     simpler fix on the day that changes.
//   · NOT LOADED YET is not the same as EMPTY. "No links yet" appears only once
//     the service has actually said so (F8); a failed load says something
//     different (F10).
@Component({
  selector: 'app-shortener-page',
  imports: [LinkTable],
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
    @if (links(); as loaded) {
      <app-link-table [links]="loaded" />
    }
  `,
})
export class ShortenerPageWithTable {
  private readonly service = inject(LinkService);

  protected readonly draft = signal('');
  protected readonly waiting = signal(false);
  protected readonly problem = signal('');
  private readonly created = signal<ShortLink | null>(null);

  /** null until the service has answered — which is not the same as empty. */
  protected readonly links = signal<readonly ShortLink[] | null>(null);

  protected readonly shortUrl = computed(() => {
    const link = this.created();
    return link && link.url === this.draft().trim() ? link.shortUrl : '';
  });

  constructor() {
    void this.load(); // F7 — the table is there when the page opens
  }

  private async load(): Promise<void> {
    try {
      this.links.set(await this.service.list());
    } catch {
      this.problem.set("Couldn't load your links. Try again later."); // F10
    }
  }

  protected async create(): Promise<void> {
    const url = this.draft().trim();
    if (!url || this.waiting()) return;

    this.waiting.set(true);
    this.problem.set('');
    try {
      const link = await this.service.create(url);
      this.created.set(link);
      // F9 — on top, and a NEW array: the table is OnPush and only ever sees
      // a list that is replaced, never one mutated underneath it. No list yet
      // (still loading, or failed) stays no list: one row on its own would
      // read as "these are all your links", which nobody knows to be true.
      this.links.update((list) => list && [link, ...list]);
    } catch {
      this.created.set(null);
      this.problem.set("Couldn't create a short link. Try again.");
    } finally {
      this.waiting.set(false);
    }
  }
}
