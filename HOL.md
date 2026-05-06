# Hands-On Lab: End-to-End Tests mit Playwright

**Ziel:** Du lernst, wie du für eine bestehende React-App vollständige End-to-End-Tests mit [Playwright](https://playwright.dev/) schreibst – von der Einrichtung bis zu fortgeschrittenen Mustern wie API-Mocking und dem Page-Object-Model.

**Repo:** `harrybin/todo-react-playwright`  
**Stack:** React 19 · TypeScript · Vite · MUI · Playwright  
**Dauer:** ca. 3–4 Stunden

---

## Voraussetzungen

| Werkzeug | Mindestversion |
|----------|----------------|
| Node.js  | 18 LTS         |
| npm      | 9              |
| Git      | beliebig       |

```bash
git clone https://github.com/harrybin/todo-react-playwright.git
cd todo-react-playwright
npm install
```

---

## Überblick über die App

Starte die App und mach dich kurz mit ihr vertraut:

```bash
npm run dev
```

Die **TodoMatic**-App bietet:

| Feature | UI-Element |
|---------|-----------|
| Aufgabe hinzufügen | Textfeld + „Add"-Button |
| Aufgabe erledigen | Checkbox je Todo |
| Aufgabe bearbeiten | „Edit"-Button → Textfeld → „Save" |
| Aufgabe löschen | „Delete"-Button |
| Filter | „All" / „Active" / „Completed" |
| Remote-Tasks laden | „Load remote tasks"-Button → Fetch von `remoteTasks.json` |

Relevante Quelldateien:

```
src/
  App.tsx           # Haupt-Komponente, Zustand, Filter
  Task.ts           # TypeScript-Interface Task
  components/
    Form.tsx         # Eingabeformular
    FilterButton.tsx # Filter-Buttons (data-testid="testID-<Name>")
    Todo.tsx         # Einzelnes Todo-Item
public/
  remoteTasks.json  # Wird via fetch() geladen
```

---

## Setup: Playwright installieren und konfigurieren

```bash
npm init playwright@latest
```

Beantworte die Fragen des Wizard wie folgt (Empfehlung):

| Frage | Antwort |
|-------|---------|
| Wo sollen Tests liegen? | `tests` |
| GitHub Actions Workflow? | `yes` |
| Browser installieren? | `yes` |

Danach existiert `playwright.config.ts`. Passe die `baseURL` an:

```ts
// playwright.config.ts
use: {
  baseURL: 'http://localhost:3000',
},
```

Und konfiguriere den `webServer`-Block, damit Playwright die App automatisch startet:

```ts
webServer: {
  command: 'npm run dev',
  url: 'http://localhost:3000',
  reuseExistingServer: !process.env.CI,
},
```

Teste die Grundkonfiguration:

```bash
npm test
```

> ℹ️ Die von Playwright generierten Beispiel-Tests kannst du löschen oder behalten – sie stören nicht.

---

## Exercise 1 – Erster Test: Seitenaufruf und Titel

### Aufgabe

Erstelle die Datei `tests/app.spec.ts`.  
Schreibe einen Test, der:

1. `http://localhost:3000` aufruft,
2. prüft, dass die Seite den Titel **TodoMatic** (h2) enthält.

### Anforderungen

- Verwende `page.goto('/')`.
- Verwende einen Playwright-Locator, der auf `h2` zeigt, und prüfe mit `toContainText`.

### Lösungshinweis

<details>
<summary>Hinweis anzeigen</summary>

```ts
import { test, expect } from '@playwright/test';

test('Seite zeigt den Titel TodoMatic', async ({ page }) => {
  await page.goto('/');
  await expect(page.locator('h2')).toContainText('TodoMatic');
});
```

</details>

---

## Exercise 2 – Todo hinzufügen

### Aufgabe

Schreibe einen Test, der eine neue Aufgabe hinzufügt und prüft, dass sie in der Liste erscheint.

> **Hinweis zur Geolocation:** Die `addTask`-Funktion ruft `navigator.geolocation.getCurrentPosition` auf. Ohne Mock bleibt der „Add"-Button ohne Wirkung, weil der Browser die Position verweigert.

### Teilschritte

1. Mocke die Geolocation **vor** `page.goto('/')`:
   ```ts
   await page.context().grantPermissions(['geolocation']);
   await page.context().setGeolocation({ latitude: 49.637, longitude: 6.901 });
   ```
2. Fülle das Eingabefeld (`#new-todo-input`) mit einem Testnamen.
3. Klicke auf den „Add"-Button (`#myUniqueID`).
4. Prüfe, dass der Testname irgendwo auf der Seite sichtbar ist.

### Lösungshinweis

<details>
<summary>Hinweis anzeigen</summary>

```ts
test('Neue Aufgabe hinzufügen', async ({ page, context }) => {
  await context.grantPermissions(['geolocation']);
  await context.setGeolocation({ latitude: 49.637, longitude: 6.901 });

  await page.goto('/');

  await page.locator('#new-todo-input').fill('Playwright lernen');
  await page.locator('#myUniqueID').click();

  await expect(page.getByText('Playwright lernen')).toBeVisible();
});
```

</details>

---

## Exercise 3 – Aufgabe als erledigt markieren

### Aufgabe

Markiere die bereits beim Start angezeigte Aufgabe **„test"** als erledigt und prüfe:

1. Die Checkbox ist angehakt.
2. Der Counter-Text oben wechselt (z. B. „0 tasks remaining").

### Lösungshinweis

<details>
<summary>Hinweis anzeigen</summary>

```ts
test('Aufgabe als erledigt markieren', async ({ page }) => {
  await page.goto('/');

  // Checkbox anhand des zugehörigen Textes finden
  const checkbox = page.getByRole('checkbox');
  await checkbox.check();

  await expect(checkbox).toBeChecked();
  await expect(page.locator('#list-heading')).toContainText('0 tasks remaining');
});
```

</details>

---

## Exercise 4 – Filter testen

### Aufgabe

Nutze die Filter-Buttons (data-testid: `testID-All`, `testID-Active`, `testID-Completed`), um folgendes Verhalten zu testen:

1. Standardmäßig ist **All** aktiv (`aria-pressed="true"`).
2. Nach dem Anklicken von **Active** verschwindet eine erledigte Aufgabe.
3. Nach dem Anklicken von **Completed** taucht nur die erledigte Aufgabe auf.

### Lösungshinweis

<details>
<summary>Hinweis anzeigen</summary>

```ts
test('Filter funktionieren korrekt', async ({ page }) => {
  await page.goto('/');

  // All ist aktiv
  await expect(page.getByTestId('testID-All')).toHaveAttribute('aria-pressed', 'true');

  // Aufgabe als erledigt markieren
  await page.getByRole('checkbox').check();

  // Active-Filter: erledigte Aufgabe nicht sichtbar
  await page.getByTestId('testID-Active').click();
  await expect(page.getByText('test')).not.toBeVisible();

  // Completed-Filter: erledigte Aufgabe sichtbar
  await page.getByTestId('testID-Completed').click();
  await expect(page.getByText('test')).toBeVisible();
});
```

</details>

---

## Exercise 5 – Aufgabe bearbeiten

### Aufgabe

Bearbeite die vorhandene Aufgabe **„test"** und benenne sie in **„test (bearbeitet)"** um. Prüfe danach, dass der neue Name angezeigt wird.

### Teilschritte

1. Klicke auf den „Edit"-Button.
2. Warte, bis das Edit-Formular sichtbar ist.
3. Lösche den vorhandenen Wert und gib den neuen Namen ein.
4. Klicke auf „Save".
5. Prüfe, dass der neue Name auf der Seite steht.

### Lösungshinweis

<details>
<summary>Hinweis anzeigen</summary>

```ts
test('Aufgabe bearbeiten', async ({ page }) => {
  await page.goto('/');

  await page.getByRole('button', { name: 'Edit' }).click();

  const editField = page.getByRole('textbox').last();
  await editField.fill('test (bearbeitet)');
  await page.getByRole('button', { name: 'Save' }).click();

  await expect(page.getByText('test (bearbeitet)')).toBeVisible();
});
```

</details>

---

## Exercise 6 – Aufgabe löschen

### Aufgabe

Lösche die vorhandene Aufgabe **„test"** und prüfe:

1. Die Aufgabe ist nicht mehr sichtbar.
2. Der Heading-Text lautet **„0 tasks remaining"**.

### Lösungshinweis

<details>
<summary>Hinweis anzeigen</summary>

```ts
test('Aufgabe löschen', async ({ page }) => {
  await page.goto('/');

  await page.getByRole('button', { name: 'Delete' }).click();

  await expect(page.getByText('test')).not.toBeVisible();
  await expect(page.locator('#list-heading')).toContainText('0 tasks remaining');
});
```

</details>

---

## Exercise 7 – Remote Tasks laden (API-Mock)

### Aufgabe

Der Button **„Load remote tasks"** ruft intern `fetch('/remoteTasks.json')` auf und ersetzt die aktuelle Taskliste.  
Schreibe einen Test, der:

1. Die Netzwerkanfrage an `**/remoteTasks.json` abfängt und durch eigene Testdaten ersetzt.
2. Den Button anklickt.
3. Prüft, dass die gemockten Daten erscheinen.

### Konzept: `page.route`

```ts
await page.route('**/remoteTasks.json', async (route) => {
  await route.fulfill({
    contentType: 'application/json',
    body: JSON.stringify([ /* deine Testdaten */ ]),
  });
});
```

### Lösungshinweis

<details>
<summary>Hinweis anzeigen</summary>

```ts
test('Remote Tasks laden mit Mock', async ({ page }) => {
  const mockTasks = [
    {
      id: 'todo-mock-1',
      name: 'Mock-Aufgabe 1',
      time: new Date().toISOString(),
      location: { latitude: 0, longitude: 0 },
      completed: false,
    },
  ];

  await page.route('**/remoteTasks.json', async (route) => {
    await route.fulfill({
      contentType: 'application/json',
      body: JSON.stringify(mockTasks),
    });
  });

  await page.goto('/');
  await page.getByRole('button', { name: 'Load remote tasks' }).click();

  await expect(page.getByText('Mock-Aufgabe 1')).toBeVisible();
  // Ursprüngliche Aufgabe "test" sollte verschwunden sein
  await expect(page.getByText('test')).not.toBeVisible();
});
```

</details>

---

## Exercise 8 – Page Object Model (POM)

### Aufgabe

Refaktoriere deine Tests so, dass du eine `TodoPage`-Klasse verwendest, die die Selektoren kapselt.

### Minimalanforderung an die Klasse

```ts
// tests/pages/TodoPage.ts
import { type Page, type Locator } from '@playwright/test';

export class TodoPage {
  readonly page: Page;
  readonly input: Locator;
  readonly addButton: Locator;
  readonly listHeading: Locator;

  constructor(page: Page) { /* ... */ }

  async goto() { /* ... */ }
  async addTask(name: string) { /* ... */ }
  async deleteFirstTask() { /* ... */ }
}
```

Schreibe danach einen Test, der über `TodoPage` eine Aufgabe hinzufügt und wieder löscht.

### Lösungshinweis

<details>
<summary>Hinweis anzeigen</summary>

**`tests/pages/TodoPage.ts`**

```ts
import { type Page, type Locator } from '@playwright/test';

export class TodoPage {
  readonly page: Page;
  readonly input: Locator;
  readonly addButton: Locator;
  readonly listHeading: Locator;

  constructor(page: Page) {
    this.page = page;
    this.input = page.locator('#new-todo-input');
    this.addButton = page.locator('#myUniqueID');
    this.listHeading = page.locator('#list-heading');
  }

  async goto() {
    await this.page.goto('/');
  }

  async addTask(name: string) {
    await this.input.fill(name);
    await this.addButton.click();
  }

  async deleteFirstTask() {
    await this.page.getByRole('button', { name: 'Delete' }).first().click();
  }
}
```

**`tests/todo-pom.spec.ts`**

```ts
import { test, expect } from '@playwright/test';
import { TodoPage } from './pages/TodoPage';

test('POM: Aufgabe hinzufügen und löschen', async ({ page, context }) => {
  await context.grantPermissions(['geolocation']);
  await context.setGeolocation({ latitude: 49.637, longitude: 6.901 });

  const todoPage = new TodoPage(page);
  await todoPage.goto();
  await todoPage.addTask('POM-Aufgabe');

  await expect(page.getByText('POM-Aufgabe')).toBeVisible();

  await todoPage.deleteFirstTask();

  await expect(page.getByText('POM-Aufgabe')).not.toBeVisible();
});
```

</details>

---

## Exercise 9 – CI: Playwright in GitHub Actions

### Aufgabe

Erstelle (oder vervollständige) eine GitHub-Actions-Workflow-Datei, die:

1. Bei jedem Push / PR auf `main` ausgeführt wird.
2. Node.js 20 verwendet.
3. Abhängigkeiten installiert **und** Playwright-Browser installiert.
4. `npm test` ausführt.
5. Den Playwright-HTML-Report als Artefakt hochlädt.

### Lösungshinweis

<details>
<summary>Hinweis anzeigen</summary>

```yaml
# .github/workflows/playwright.yml
name: Playwright Tests

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - uses: actions/setup-node@v4
        with:
          node-version: 20
          cache: npm

      - run: npm ci
      - run: npx playwright install --with-deps

      - run: npm test
        env:
          CI: true

      - uses: actions/upload-artifact@v4
        if: always()
        with:
          name: playwright-report
          path: playwright-report/
          retention-days: 7
```

</details>

---

## Bonus: Was könnte noch verbessert werden?

Schau dir den Quellcode an und überlege, welche weiteren Tests oder Verbesserungen sinnvoll wären:

- **Bug in `toggleTaskCompleted`:** Finde den Fehler in `App.tsx` (Zeile ~44). Kannst du einen fehlschlagenden Test schreiben, der den Bug beweist, und danach den Fix vornehmen?
- **Accessibility:** Nutze `@axe-core/playwright`, um Barrierefreiheitsprobleme automatisch zu erkennen.
- **Screenshot-Vergleich:** Füge einen visuellen Regressionstest mit `toHaveScreenshot()` hinzu.
- **Mehrere Browser:** Konfiguriere `playwright.config.ts` so, dass Tests in Chromium, Firefox und WebKit laufen.

---

## Ressourcen

| Thema | Link |
|-------|------|
| Playwright Docs | https://playwright.dev/docs/intro |
| Playwright API-Referenz | https://playwright.dev/docs/api/class-playwright |
| Page Object Models | https://playwright.dev/docs/pom |
| Netzwerk-Mocking | https://playwright.dev/docs/mock |
| React Testing-Strategie | https://reactjs.org/docs/testing.html |

---

*Happy Testing! 🎭*
