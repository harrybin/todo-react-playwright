# Hands-On Lab: End-to-End Tests mit Playwright

**Ziel:** Du lernst, wie du für eine bestehende React-App vollständige End-to-End-Tests mit [Playwright](https://playwright.dev/) schreibst – von der Einrichtung bis zu fortgeschrittenen Mustern wie API-Mocking und dem Page-Object-Model.

**Repo:** `harrybin/todo-react-playwright`  
**Stack:** React 19 · TypeScript · Vite · MUI · Playwright  
**Dauer:** ca. 3–4 Stunden

> Jede Übung enthält Lösungshinweise für **zwei Sprachen** – wähle die für dich passende:
> - 🟦 **TypeScript / JavaScript** (Node.js + `@playwright/test`)
> - 🟪 **C# / .NET** (`Microsoft.Playwright.NUnit`)

---

## Voraussetzungen

### 🟦 TypeScript / JavaScript

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

### 🟪 C# / .NET

| Werkzeug | Mindestversion |
|----------|----------------|
| .NET SDK | 8              |
| Git      | beliebig       |

```bash
git clone https://github.com/harrybin/todo-react-playwright.git
cd todo-react-playwright
# App starten (bleibt im Hintergrund laufen)
npm install && npm run dev
```

Lege ein separates Testprojekt an (z. B. neben dem Repo-Ordner):

```bash
dotnet new nunit -n TodoTests
cd TodoTests
dotnet add package Microsoft.Playwright.NUnit
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install
```

---

## Überblick über die App

Starte die App und mach dich kurz mit ihr vertraut:

```bash
npm run dev   # http://localhost:3000
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

### 🟦 TypeScript / JavaScript

```bash
npm init playwright@latest
```

Beantworte die Fragen des Wizard wie folgt (Empfehlung):

| Frage | Antwort |
|-------|---------|
| Wo sollen Tests liegen? | `tests` |
| GitHub Actions Workflow? | `yes` |
| Browser installieren? | `yes` |

Passe `playwright.config.ts` an:

```ts
// playwright.config.ts
use: {
  baseURL: 'http://localhost:3000',
},
webServer: {
  command: 'npm run dev',
  url: 'http://localhost:3000',
  reuseExistingServer: !process.env.CI,
},
```

```bash
npm test
```

### 🟪 C# / .NET

Die App läuft bereits auf `http://localhost:3000`. Jeder Test erbt von `PageTest` und setzt die Basis-URL in einer `[SetUp]`-Methode oder über `BrowserNewContextOptions`.

Lege `TodoTests.cs` an:

```csharp
using Microsoft.Playwright.NUnit;

namespace TodoTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class AppTests : PageTest
{
    [SetUp]
    public async Task SetUp()
    {
        await Page.GotoAsync("http://localhost:3000/");
    }
}
```

```bash
dotnet test
```

> ℹ️ Die App muss **manuell gestartet** sein (`npm run dev`), da es kein eingebautes `webServer`-Konzept gibt. Alternativ lässt sich der Start im `[OneTimeSetUp]` per `Process.Start` automatisieren.

---

## Exercise 1 – Erster Test: Seitenaufruf und Titel

### Aufgabe

Schreibe einen Test, der:

1. `http://localhost:3000` aufruft,
2. prüft, dass die Seite den Titel **TodoMatic** (h2) enthält.

### Lösungshinweis 🟦 TypeScript

<details>
<summary>Hinweis anzeigen (TypeScript)</summary>

```ts
// tests/app.spec.ts
import { test, expect } from '@playwright/test';

test('Seite zeigt den Titel TodoMatic', async ({ page }) => {
  await page.goto('/');
  await expect(page.locator('h2')).toContainText('TodoMatic');
});
```

</details>

### Lösungshinweis 🟪 C#

<details>
<summary>Hinweis anzeigen (C#)</summary>

```csharp
[Test]
public async Task SeiteZeigtTitelTodoMatic()
{
    // SetUp hat bereits goto aufgerufen
    await Expect(Page.Locator("h2")).ToContainTextAsync("TodoMatic");
}
```

</details>

---

## Exercise 2 – Todo hinzufügen

### Aufgabe

Schreibe einen Test, der eine neue Aufgabe hinzufügt und prüft, dass sie in der Liste erscheint.

> **Hinweis zur Geolocation:** Die `addTask`-Funktion ruft `navigator.geolocation.getCurrentPosition` auf. Ohne Mock bleibt der „Add"-Button ohne Wirkung, weil der Browser die Position verweigert.

### Teilschritte (sprachunabhängig)

1. Geolocation-Permission erteilen und Position setzen, **bevor** die Seite geladen wird.
2. Eingabefeld (`#new-todo-input`) mit einem Namen befüllen.
3. „Add"-Button (`#myUniqueID`) klicken.
4. Prüfen, dass der Name auf der Seite sichtbar ist.

### Lösungshinweis 🟦 TypeScript

<details>
<summary>Hinweis anzeigen (TypeScript)</summary>

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

### Lösungshinweis 🟪 C#

<details>
<summary>Hinweis anzeigen (C#)</summary>

In C# wird der Browser-Kontext mit Geolocation-Optionen neu erstellt. Überschreibe dazu `NewContext`:

```csharp
// In der Testklasse: Geolocation aktivieren
public override BrowserNewContextOptions ContextOptions()
{
    return new BrowserNewContextOptions
    {
        Permissions = new[] { "geolocation" },
        Geolocation = new Geolocation { Latitude = 49.637f, Longitude = 6.901f },
    };
}

[Test]
public async Task NeueAufgabeHinzufuegen()
{
    await Page.GotoAsync("http://localhost:3000/");

    await Page.Locator("#new-todo-input").FillAsync("Playwright lernen");
    await Page.Locator("#myUniqueID").ClickAsync();

    await Expect(Page.GetByText("Playwright lernen")).ToBeVisibleAsync();
}
```

</details>

---

## Exercise 3 – Aufgabe als erledigt markieren

### Aufgabe

Markiere die beim Start angezeigte Aufgabe **„test"** als erledigt und prüfe:

1. Die Checkbox ist angehakt.
2. Der Counter-Text lautet **„0 tasks remaining"**.

### Lösungshinweis 🟦 TypeScript

<details>
<summary>Hinweis anzeigen (TypeScript)</summary>

```ts
test('Aufgabe als erledigt markieren', async ({ page }) => {
  await page.goto('/');

  const checkbox = page.getByRole('checkbox');
  await checkbox.check();

  await expect(checkbox).toBeChecked();
  await expect(page.locator('#list-heading')).toContainText('0 tasks remaining');
});
```

</details>

### Lösungshinweis 🟪 C#

<details>
<summary>Hinweis anzeigen (C#)</summary>

```csharp
[Test]
public async Task AufgabeAlsErledigtMarkieren()
{
    var checkbox = Page.GetByRole(AriaRole.Checkbox);
    await checkbox.CheckAsync();

    await Expect(checkbox).ToBeCheckedAsync();
    await Expect(Page.Locator("#list-heading")).ToContainTextAsync("0 tasks remaining");
}
```

</details>

---

## Exercise 4 – Filter testen

### Aufgabe

Nutze die Filter-Buttons (data-testid: `testID-All`, `testID-Active`, `testID-Completed`):

1. Standardmäßig ist **All** aktiv (`aria-pressed="true"`).
2. Nach dem Anklicken von **Active** verschwindet eine erledigte Aufgabe.
3. Nach dem Anklicken von **Completed** taucht nur die erledigte Aufgabe auf.

### Lösungshinweis 🟦 TypeScript

<details>
<summary>Hinweis anzeigen (TypeScript)</summary>

```ts
test('Filter funktionieren korrekt', async ({ page }) => {
  await page.goto('/');

  await expect(page.getByTestId('testID-All')).toHaveAttribute('aria-pressed', 'true');

  await page.getByRole('checkbox').check();

  await page.getByTestId('testID-Active').click();
  await expect(page.getByText('test')).not.toBeVisible();

  await page.getByTestId('testID-Completed').click();
  await expect(page.getByText('test')).toBeVisible();
});
```

</details>

### Lösungshinweis 🟪 C#

<details>
<summary>Hinweis anzeigen (C#)</summary>

```csharp
[Test]
public async Task FilterFunktionierenKorrekt()
{
    // All ist standardmäßig aktiv
    await Expect(Page.GetByTestId("testID-All"))
        .ToHaveAttributeAsync("aria-pressed", "true");

    // Aufgabe als erledigt markieren
    await Page.GetByRole(AriaRole.Checkbox).CheckAsync();

    // Active-Filter
    await Page.GetByTestId("testID-Active").ClickAsync();
    await Expect(Page.GetByText("test")).Not.ToBeVisibleAsync();

    // Completed-Filter
    await Page.GetByTestId("testID-Completed").ClickAsync();
    await Expect(Page.GetByText("test")).ToBeVisibleAsync();
}
```

</details>

---

## Exercise 5 – Aufgabe bearbeiten

### Aufgabe

Benenne die Aufgabe **„test"** in **„test (bearbeitet)"** um. Prüfe danach, dass der neue Name angezeigt wird.

### Teilschritte (sprachunabhängig)

1. „Edit"-Button klicken.
2. Edit-Textfeld mit neuem Namen befüllen.
3. „Save"-Button klicken.
4. Neuen Namen auf der Seite prüfen.

### Lösungshinweis 🟦 TypeScript

<details>
<summary>Hinweis anzeigen (TypeScript)</summary>

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

### Lösungshinweis 🟪 C#

<details>
<summary>Hinweis anzeigen (C#)</summary>

```csharp
[Test]
public async Task AufgabeBearbeiten()
{
    await Page.GetByRole(AriaRole.Button, new() { Name = "Edit" }).ClickAsync();

    var editField = Page.GetByRole(AriaRole.Textbox).Last;
    await editField.FillAsync("test (bearbeitet)");
    await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

    await Expect(Page.GetByText("test (bearbeitet)")).ToBeVisibleAsync();
}
```

</details>

---

## Exercise 6 – Aufgabe löschen

### Aufgabe

Lösche die Aufgabe **„test"** und prüfe:

1. Die Aufgabe ist nicht mehr sichtbar.
2. Der Heading-Text lautet **„0 tasks remaining"**.

### Lösungshinweis 🟦 TypeScript

<details>
<summary>Hinweis anzeigen (TypeScript)</summary>

```ts
test('Aufgabe löschen', async ({ page }) => {
  await page.goto('/');

  await page.getByRole('button', { name: 'Delete' }).click();

  await expect(page.getByText('test')).not.toBeVisible();
  await expect(page.locator('#list-heading')).toContainText('0 tasks remaining');
});
```

</details>

### Lösungshinweis 🟪 C#

<details>
<summary>Hinweis anzeigen (C#)</summary>

```csharp
[Test]
public async Task AufgabeLoeschen()
{
    await Page.GetByRole(AriaRole.Button, new() { Name = "Delete" }).ClickAsync();

    await Expect(Page.GetByText("test")).Not.ToBeVisibleAsync();
    await Expect(Page.Locator("#list-heading")).ToContainTextAsync("0 tasks remaining");
}
```

</details>

---

## Exercise 7 – Remote Tasks laden (API-Mock)

### Aufgabe

Der Button **„Load remote tasks"** ruft intern `fetch('/remoteTasks.json')` auf.  
Schreibe einen Test, der:

1. Die Netzwerkanfrage an `**/remoteTasks.json` **abfängt** und durch eigene Testdaten ersetzt.
2. Den Button anklickt.
3. Prüft, dass die gemockten Daten erscheinen und die alten Daten verschwunden sind.

### Konzept

**TypeScript:** `page.route(pattern, handler)`  
**C#:** `Page.RouteAsync(pattern, handler)`

### Lösungshinweis 🟦 TypeScript

<details>
<summary>Hinweis anzeigen (TypeScript)</summary>

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
  await expect(page.getByText('test')).not.toBeVisible();
});
```

</details>

### Lösungshinweis 🟪 C#

<details>
<summary>Hinweis anzeigen (C#)</summary>

```csharp
[Test]
public async Task RemoteTasksLadenMitMock()
{
    var mockJson = """
        [{"id":"todo-mock-1","name":"Mock-Aufgabe 1",
          "time":"2024-01-01T00:00:00Z",
          "location":{"latitude":0,"longitude":0},
          "completed":false}]
        """;

    await Page.RouteAsync("**/remoteTasks.json", async route =>
    {
        await route.FulfillAsync(new RouteFulfillOptions
        {
            ContentType = "application/json",
            Body = mockJson,
        });
    });

    await Page.GotoAsync("http://localhost:3000/");
    await Page.GetByRole(AriaRole.Button, new() { Name = "Load remote tasks" }).ClickAsync();

    await Expect(Page.GetByText("Mock-Aufgabe 1")).ToBeVisibleAsync();
    await Expect(Page.GetByText("test")).Not.ToBeVisibleAsync();
}
```

</details>

---

## Exercise 8 – Page Object Model (POM)

### Aufgabe

Refaktoriere deine Tests so, dass du eine `TodoPage`-Klasse verwendest, die alle Selektoren kapselt.

### Minimalanforderung an die Klasse

**TypeScript:**
```ts
// tests/pages/TodoPage.ts
export class TodoPage {
  constructor(page: Page) { /* ... */ }
  async goto(): Promise<void> { /* ... */ }
  async addTask(name: string): Promise<void> { /* ... */ }
  async deleteFirstTask(): Promise<void> { /* ... */ }
}
```

**C#:**
```csharp
public class TodoPage(IPage page)
{
    public async Task GotoAsync() { /* ... */ }
    public async Task AddTaskAsync(string name) { /* ... */ }
    public async Task DeleteFirstTaskAsync() { /* ... */ }
}
```

Schreibe danach einen Test, der über die Klasse eine Aufgabe hinzufügt und wieder löscht.

### Lösungshinweis 🟦 TypeScript

<details>
<summary>Hinweis anzeigen (TypeScript)</summary>

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

### Lösungshinweis 🟪 C#

<details>
<summary>Hinweis anzeigen (C#)</summary>

**`Pages/TodoPage.cs`**

```csharp
using Microsoft.Playwright;

namespace TodoTests.Pages;

public class TodoPage(IPage page)
{
    private ILocator Input => page.Locator("#new-todo-input");
    private ILocator AddButton => page.Locator("#myUniqueID");
    public ILocator ListHeading => page.Locator("#list-heading");

    public async Task GotoAsync()
        => await page.GotoAsync("http://localhost:3000/");

    public async Task AddTaskAsync(string name)
    {
        await Input.FillAsync(name);
        await AddButton.ClickAsync();
    }

    public async Task DeleteFirstTaskAsync()
        => await page.GetByRole(AriaRole.Button, new() { Name = "Delete" })
                     .First.ClickAsync();
}
```

**`PomTests.cs`**

```csharp
using Microsoft.Playwright.NUnit;
using TodoTests.Pages;

namespace TodoTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class PomTests : PageTest
{
    public override BrowserNewContextOptions ContextOptions() => new()
    {
        Permissions = new[] { "geolocation" },
        Geolocation = new Geolocation { Latitude = 49.637f, Longitude = 6.901f },
    };

    [Test]
    public async Task PomAufgabeHinzufuegenUndLoeschen()
    {
        var todoPage = new TodoPage(Page);
        await todoPage.GotoAsync();
        await todoPage.AddTaskAsync("POM-Aufgabe");

        await Expect(Page.GetByText("POM-Aufgabe")).ToBeVisibleAsync();

        await todoPage.DeleteFirstTaskAsync();

        await Expect(Page.GetByText("POM-Aufgabe")).Not.ToBeVisibleAsync();
    }
}
```

</details>

---

## Exercise 9 – CI: Playwright in GitHub Actions

### Aufgabe

Erstelle eine GitHub-Actions-Workflow-Datei, die:

1. Bei jedem Push / PR auf `main` ausgeführt wird.
2. Abhängigkeiten installiert **und** Playwright-Browser installiert.
3. Die Tests ausführt.
4. Den Playwright-HTML-Report als Artefakt hochlädt.

### Lösungshinweis 🟦 TypeScript

<details>
<summary>Hinweis anzeigen (TypeScript)</summary>

```yaml
# .github/workflows/playwright.yml
name: Playwright Tests (TypeScript)

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
          name: playwright-report-ts
          path: playwright-report/
          retention-days: 7
```

</details>

### Lösungshinweis 🟪 C#

<details>
<summary>Hinweis anzeigen (C#)</summary>

```yaml
# .github/workflows/playwright-dotnet.yml
name: Playwright Tests (.NET)

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

      # App starten
      - uses: actions/setup-node@v4
        with:
          node-version: 20
          cache: npm
      - run: npm ci
      - run: npm run dev &
      - run: npx wait-on http://localhost:3000

      # .NET Tests
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 8.x

      - name: Build & install browsers
        working-directory: TodoTests
        run: |
          dotnet build
          pwsh bin/Debug/net8.0/playwright.ps1 install --with-deps

      - name: Run tests
        working-directory: TodoTests
        run: dotnet test --logger "trx;LogFileName=results.trx"

      - uses: actions/upload-artifact@v4
        if: always()
        with:
          name: playwright-report-dotnet
          path: TodoTests/TestResults/
          retention-days: 7
```

</details>

---

## Bonus: Was könnte noch verbessert werden?

Schau dir den Quellcode an und überlege, welche weiteren Tests sinnvoll wären:

- **Bug in `toggleTaskCompleted`:** Finde den Fehler in `App.tsx` (Zeile ~44). Schreibe einen fehlschlagenden Test, der den Bug beweist, und fixe danach den Code.
- **Accessibility:** Nutze `@axe-core/playwright` (TS) oder `Deque.AxeCore.Playwright` (C#), um Barrierefreiheitsprobleme automatisch zu erkennen.
- **Screenshot-Vergleich:** Füge einen visuellen Regressionstest hinzu – `toHaveScreenshot()` (TS) bzw. `Page.ScreenshotAsync` mit Bildvergleich (C#).
- **Mehrere Browser:** Konfiguriere `playwright.config.ts` (TS) oder `[BrowserType]`-Attribute (C#), damit Tests in Chromium, Firefox und WebKit laufen.

---

## Ressourcen

| Thema | TypeScript / JavaScript | C# / .NET |
|-------|------------------------|-----------|
| Einstieg | [playwright.dev/docs/intro](https://playwright.dev/docs/intro) | [playwright.dev/dotnet/docs/intro](https://playwright.dev/dotnet/docs/intro) |
| API-Referenz | [playwright.dev/docs/api](https://playwright.dev/docs/api/class-playwright) | [playwright.dev/dotnet/docs/api](https://playwright.dev/dotnet/docs/api/class-playwright) |
| Page Object Model | [playwright.dev/docs/pom](https://playwright.dev/docs/pom) | [playwright.dev/dotnet/docs/pom](https://playwright.dev/dotnet/docs/pom) |
| Netzwerk-Mocking | [playwright.dev/docs/mock](https://playwright.dev/docs/mock) | [playwright.dev/dotnet/docs/mock](https://playwright.dev/dotnet/docs/mock) |
| Geolocation | [playwright.dev/docs/emulation#geolocation](https://playwright.dev/docs/emulation#geolocation) | [playwright.dev/dotnet/docs/emulation#geolocation](https://playwright.dev/dotnet/docs/emulation#geolocation) |

---

*Happy Testing! 🎭*
