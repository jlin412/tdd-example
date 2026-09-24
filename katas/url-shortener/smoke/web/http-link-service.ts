import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';

import { LinkService, ShortLink } from './link-service';

// The only real LinkService in the repo, and it exists for the smoke test.
//
// The frontend track never builds one — its tests stand in for the service —
// so the page's contract (link-service.ts) and the backend's API were written
// by different tracks that never met. This adapter is where they meet.
//
// Putting them side by side found one real gap, fixed on the backend (R24):
// POST /links used to answer with nothing but {code}, while the page promises
// callers the url and createdAt of the link it made. One difference remains,
// on purpose, and this class translates it: the backend deals in CODES, the
// page shows SHORT URLS. The story defines a short URL as the shortener's own
// address plus the code; /links/{code} is where this shortener resolves one.

/** A link as the backend's JSON spells it. */
interface LinkJson {
  readonly code: string;
  readonly url: string;
  readonly createdAt: string;
}

@Injectable()
export class HttpLinkService extends LinkService {
  private readonly http = inject(HttpClient);

  override async create(url: string): Promise<ShortLink> {
    return toShortLink(await firstValueFrom(this.http.post<LinkJson>('/links', { url })));
  }

  override async list(): Promise<ShortLink[]> {
    return (await firstValueFrom(this.http.get<LinkJson[]>('/links'))).map(toShortLink);
  }
}

function toShortLink(link: LinkJson): ShortLink {
  return {
    url: link.url,
    shortUrl: new URL(`/links/${link.code}`, location.origin).href,
    createdAt: link.createdAt,
  };
}
