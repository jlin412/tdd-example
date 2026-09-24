import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { ShortenerPage } from './app/shortener-page';

bootstrapApplication(ShortenerPage, appConfig).catch((err) => console.error(err));
