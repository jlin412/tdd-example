// Builds the page the smoke test drives, without touching the kata's skeleton.
//
// Solutions in this repo carry no build files; the documented way to run one is
// to copy it over a scratch copy of the skeleton. This script is that, written
// down: it copies the Angular workspace into build/web, drops the frontend
// reference solution and the smoke test's HTTP adapter into it, points the
// entry point at the table page, dresses it in Bootstrap, and runs `ng build`.

import { execFileSync } from 'node:child_process';
import {
  cpSync,
  existsSync,
  readFileSync,
  readdirSync,
  rmSync,
  symlinkSync,
  writeFileSync,
} from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const here = path.dirname(fileURLToPath(import.meta.url));
const kata = path.resolve(here, '..');
const workspace = path.join(kata, 'angular');
// Not a dot-folder: ASP.NET's static file provider skips anything under one,
// and the host serves the page straight out of here.
const web = path.join(here, 'build', 'web');

const skipped = new Set(['node_modules', '.angular', 'dist']);

rmSync(path.join(here, 'build'), { recursive: true, force: true });
cpSync(workspace, web, {
  recursive: true,
  filter: (source) => !skipped.has(path.basename(source)) || path.dirname(source) !== workspace,
});

// The skeleton's installed packages, shared rather than copied (hundreds of MB).
const modules = path.join(workspace, 'node_modules');
if (!existsSync(modules)) {
  throw new Error(`run \`npm install\` in ${workspace} first`);
}
symlinkSync(modules, path.join(web, 'node_modules'), process.platform === 'win32' ? 'junction' : 'dir');

// The frontend reference solution, minus its specs, next to the given contract.
const app = path.join(web, 'src', 'app');
const solution = path.join(kata, 'solutions', 'angular');
for (const file of readdirSync(solution)) {
  if (file.endsWith('.ts') && !file.endsWith('.spec.ts')) {
    cpSync(path.join(solution, file), path.join(app, file));
  }
}

// The one real LinkService, and an entry point that uses it.
cpSync(path.join(here, 'web', 'http-link-service.ts'), path.join(app, 'http-link-service.ts'));
cpSync(path.join(here, 'web', 'main.ts'), path.join(web, 'src', 'main.ts'));

// Bootstrap, for people looking at the page by hand. Only this build gets it —
// the kata's own workspace stays dependency-free. The reference templates
// already carry Bootstrap class names (inert in their unit tests).
cpSync(
  path.join(here, 'node_modules', 'bootstrap', 'dist', 'css', 'bootstrap.min.css'),
  path.join(web, 'src', 'bootstrap.min.css'),
);
const angularJson = path.join(web, 'angular.json');
const config = JSON.parse(readFileSync(angularJson, 'utf8'));
const build = Object.values(config.projects)[0].architect.build.options;
build.styles = ['src/bootstrap.min.css', ...build.styles];
writeFileSync(angularJson, JSON.stringify(config, null, 2));

// A page-width container around the component, so it isn't pinned to the edges.
const indexHtml = path.join(web, 'src', 'index.html');
writeFileSync(
  indexHtml,
  readFileSync(indexHtml, 'utf8')
    .replace('<app-shortener-page></app-shortener-page>',
      '<main class="container py-5"><app-shortener-page></app-shortener-page></main>'),
);

execFileSync(process.execPath, [path.join(modules, '@angular', 'cli', 'bin', 'ng.js'), 'build'], {
  cwd: web,
  stdio: 'inherit',
});
