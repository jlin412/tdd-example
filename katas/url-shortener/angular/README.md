# URL Shortener — frontend track

> **This is the workspace for the URL Shortener kata's frontend track — the brief lives one level up.**
>
> - **Start here:** [../UrlShortenerStory.md](../UrlShortenerStory.md) — the product
> - **Then:** [../UrlShortenerExtendedStory.md](../UrlShortenerExtendedStory.md) — the table
> - **Running the session:** [../README.md](../README.md) — the two tracks, session
>   flow, pattern menus, contract decisions
>
> Fill in [`src/app/shortener-page.ts`](src/app/shortener-page.ts) and
> [`src/app/shortener-page.spec.ts`](src/app/shortener-page.spec.ts). The service
> behind the page is given as a contract, [`src/app/link-service.ts`](src/app/link-service.ts),
> with no implementation — your tests stand in for it. Write the test list
> (STEP 0) first, then uncomment STEP 1's assertion for your first red:
>
> ```bash
> npm install && npm test
> ```
>
> A fresh skeleton is all green — STEP 1 is commented out and the STEP 0 todos
> report as pending. Uncommenting STEP 1 is the move.

---

Everything below is the stock Angular CLI readme.

This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 21.2.23.

## Development server

To start a local development server, run:

```bash
ng serve
```

Once the server is running, open your browser and navigate to `http://localhost:4200/`. The application will automatically reload whenever you modify any of the source files.

## Code scaffolding

Angular CLI includes powerful code scaffolding tools. To generate a new component, run:

```bash
ng generate component component-name
```

For a complete list of available schematics (such as `components`, `directives`, or `pipes`), run:

```bash
ng generate --help
```

## Building

To build the project run:

```bash
ng build
```

This will compile your project and store the build artifacts in the `dist/` directory. By default, the production build optimizes your application for performance and speed.

## Running unit tests

To execute unit tests with the [Vitest](https://vitest.dev/) test runner, use the following command:

```bash
ng test
```

## Running end-to-end tests

For end-to-end (e2e) testing, run:

```bash
ng e2e
```

Angular CLI does not come with an end-to-end testing framework by default. You can choose one that suits your needs.

## Additional Resources

For more information on using the Angular CLI, including detailed command references, visit the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.
