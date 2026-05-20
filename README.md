# todo-react-playwright

A sample Todo app built with [React 19](https://react.dev/) and [TypeScript](https://www.typescriptlang.org/), powered by [Vite](https://vitejs.dev/) and styled with [MUI (Material UI)](https://mui.com/). End-to-end tests are written with [Playwright](https://playwright.dev/).

## Features

- Add, edit, delete, and complete todo tasks
- Filter tasks by **All**, **Active**, or **Completed**
- Each task stores the creation time and geolocation (latitude/longitude)
- Load tasks from a remote JSON endpoint
- Deploy to GitHub Pages via `npm run deploy`

## Tech Stack

| Area       | Technology                     |
|------------|--------------------------------|
| Framework  | React 19, TypeScript           |
| Build tool | Vite 6                         |
| UI library | MUI (Material UI) 7            |
| Testing    | Playwright                     |
| Linting    | ESLint 9                       |
| ID gen     | nanoid                         |

## Getting started

Requires [Node.js](https://nodejs.org/en/) (LTS recommended).

```bash
npm install
npm run dev
```

The app opens automatically at [http://localhost:3000](http://localhost:3000).

## Available scripts

| Script             | Description                                      |
|--------------------|--------------------------------------------------|
| `npm run dev`      | Start the Vite dev server on port 3000           |
| `npm run build`    | Build the app for production into `dist/`        |
| `npm run preview`  | Locally preview the production build             |
| `npm run lint`     | Run ESLint (zero warnings policy)                |
| `npm test`         | Run Playwright end-to-end tests                  |
| `npm run test:ui`  | Run Playwright tests in interactive UI mode      |
| `npm run deploy`   | Build and deploy to GitHub Pages                 |

## Learn More

- [Vite documentation](https://vitejs.dev/guide/)
- [React documentation](https://react.dev/)
- [MUI documentation](https://mui.com/material-ui/getting-started/)
- [Playwright documentation](https://playwright.dev/docs/intro)

## Contributing

Our project welcomes contributions from any member of our community.
To get started contributing, please see our [Contributor Guide](CONTRIBUTING.md).

By participating in and contributing to our projects and discussions, you acknowledge that you have read and agree to our [Code of Conduct](CODE_OF_CONDUCT.md).

## License

This project is licensed under the [LICENSE](LICENSE).
