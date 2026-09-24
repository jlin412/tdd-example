import { defineConfig, devices } from '@playwright/test';

const port = 5179;
const baseURL = `http://127.0.0.1:${port}`;

export default defineConfig({
  testDir: '.',
  testMatch: 'smoke.spec.ts',
  reporter: 'list',
  use: { baseURL },
  projects: [{ name: 'chromium', use: { ...devices['Desktop Chrome'] } }],
  webServer: {
    // The same script `npm start` runs: build the reference page, then start
    // the reference backend serving it — one origin, so no proxy or CORS.
    command: 'node serve.mjs',
    url: baseURL,
    env: { PORT: String(port) },
    // A fresh server every run: its database lives in memory, so the test can
    // rely on starting with no links at all.
    reuseExistingServer: false,
    // The first run restores NuGet packages and builds both halves.
    timeout: 180_000,
  },
});
