// Builds the reference page and starts the reference backend serving it, on
// one origin: http://127.0.0.1:5179 (set PORT to change it). `npm start` runs
// this so you can try the product by hand; the smoke test's webServer runs the
// very same script, so what you click is exactly what the test checks.
//
// The database is in memory: every start begins with no links, and stopping
// the server (Ctrl+C) forgets them.

import { spawn } from 'node:child_process';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const here = path.dirname(fileURLToPath(import.meta.url));
const port = process.env.PORT ?? '5179';
const url = `http://127.0.0.1:${port}`;

await import('./prepare.mjs'); // assembles and builds the page into build/

console.log(`\nStarting the shortener — open ${url} once it says "Now listening".\n`);
const server = spawn(
  'dotnet',
  ['run', '--project', path.join(here, 'host'), '--', '--urls', url],
  {
    stdio: 'inherit',
    env: {
      ...process.env,
      SMOKE_WEB_ROOT: path.join(here, 'build', 'web', 'dist', 'urlShortenerKata', 'browser'),
    },
  },
);

for (const signal of ['SIGINT', 'SIGTERM']) {
  process.on(signal, () => server.kill(signal));
}
server.on('exit', (code) => process.exit(code ?? 0));
