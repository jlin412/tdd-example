import { ChangeDetectionStrategy, Component, input } from '@angular/core';

import { ShortLink } from './link-service';

// URL Shortener kata — frontend reference, extended story (F7/F8).
//
// Menu item (a), SMART / DUMB SPLIT, declined at checkpoint #1 and earned here.
// The page now shows two things — the bar and the table — that share one piece
// of state (a link you create must turn up in both). This component shows rows
// and nothing else: it cannot load, create or fail, so it has no state of its
// own to get wrong. The container owns the list and decides when there IS one
// to show.
@Component({
  selector: 'app-link-table',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    @if (links().length === 0) {
      <p class="text-body-secondary">No links yet</p>
    } @else {
      <table class="table table-bordered align-middle">
        <thead class="table-secondary">
          <tr>
            <th>Long URL</th>
            <th>Short URL</th>
          </tr>
        </thead>
        <tbody>
          @for (link of links(); track link.shortUrl) {
            <tr>
              <td>{{ link.url }}</td>
              <td>{{ link.shortUrl }}</td>
            </tr>
          }
        </tbody>
      </table>
    }
  `,
})
export class LinkTable {
  // Columns read long, then short — the same left-to-right order as the bar.
  // The header row is shaded (Bootstrap's table-secondary) and every cell
  // bordered when the page is served with Bootstrap; see smoke/.
  readonly links = input.required<readonly ShortLink[]>();
}
