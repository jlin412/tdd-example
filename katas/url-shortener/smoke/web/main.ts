// Entry point for the smoke test's build (copied over src/main.ts by
// prepare.mjs): the extended story's table page, talking to the real backend.

import { provideHttpClient } from '@angular/common/http';
import { provideBrowserGlobalErrorListeners } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';

import { HttpLinkService } from './app/http-link-service';
import { LinkService } from './app/link-service';
import { ShortenerPageWithTable } from './app/shortener-page-table';

bootstrapApplication(ShortenerPageWithTable, {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(),
    { provide: LinkService, useClass: HttpLinkService },
  ],
}).catch((err) => console.error(err));
