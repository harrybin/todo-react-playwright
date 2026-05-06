# Hands-On Lab: End-to-End Tests mit Playwright

**Ziel:** Du lernst, wie du für eine bestehende React-App vollständige End-to-End-Tests mit [Playwright](https://playwright.dev/) schreibst – von der Einrichtung bis zu fortgeschrittenen Mustern wie API-Mocking und dem Page-Object-Model.

**Repo:** `harrybin/todo-react-playwright`  
**Stack:** React 19 · TypeScript · Vite · MUI · Playwright  
**Dauer:** ca. 4–5 Stunden

> Jede Übung enthält Lösungshinweise für **zwei Sprachen** und **zwei IDEs** – wähle jeweils die für dich passende:
> - 🟦 **TypeScript / JavaScript** (Node.js + `@playwright/test`) → IDE: **Visual Studio Code**
> - 🟪 **C# / .NET** → IDE: **Visual Studio** – wähle dein Test-Framework:
>   - **NUnit** (`Microsoft.Playwright.NUnit`)
>   - **xUnit** (`Microsoft.Playwright.Xunit`)
>   - **MSTest** (`Microsoft.Playwright.MSTest`)

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

Lege ein separates Testprojekt an – wähle dein Framework:

**NUnit**
```bash
dotnet new nunit -n TodoTests
cd TodoTests
dotnet add package Microsoft.Playwright.NUnit
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install
```

**xUnit**
```bash
dotnet new xunit -n TodoTests
cd TodoTests
dotnet add package Microsoft.Playwright.Xunit
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install
```

**MSTest**
```bash
dotnet new mstest -n TodoTests
cd TodoTests
dotnet add package Microsoft.Playwright.MSTest
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install
```

---

## IDE-Setup

### 🟦 Visual Studio Code (TypeScript / JavaScript)

**Empfohlene Erweiterungen** – einmalig installieren:

| Erweiterung | ID | Zweck |
|-------------|----|-------|
| Playwright Test for VS Code | `ms-playwright.playwright` | Tests ausführen, debuggen, Codegen starten |
| ESLint | `dbaeumer.vscode-eslint` | Linting |

**Installation über die Kommandozeile:**

```bash
code --install-extension ms-playwright.playwright
```

**Playwright-Panel öffnen:**  
Klicke in der linken Activity Bar auf das ▶️ **Testing**-Icon (Reagenzglas). Dort siehst du alle Tests, kannst sie einzeln starten und direkt in den Trace Viewer springen.

**Nützliche VS-Code-Befehle** (`Ctrl+Shift+P`):

| Befehl | Wirkung |
|--------|---------|
| `Playwright: Record new` | Codegen im Browser starten |
| `Playwright: Show trace viewer` | Letzten Trace öffnen |
| `Playwright: Pick locator` | Element im Browser anklicken → Locator wird ins Clipboard kopiert |

**Debug-Konfiguration** (bereits durch die Erweiterung enthalten):  
Setze einen Breakpoint in einer Testdatei → Rechtsklick auf den Test → **Debug Test**.  
Der Browser öffnet sich im *Slow-Motion-Modus* und hält am Breakpoint an.

---

### 🟪 Visual Studio (C# / .NET)

**Voraussetzungen:**

- Visual Studio 2022 (Version 17.0+) mit Workload **.NET desktop development**
- Je nach Framework das passende NuGet-Paket im Testprojekt:

| Framework | NuGet-Paket |
|-----------|-------------|
| NUnit | `Microsoft.Playwright.NUnit` |
| xUnit | `Microsoft.Playwright.Xunit` |
| MSTest | `Microsoft.Playwright.MSTest` |

Alle drei funktionieren gleich im **Test Explorer** (`Test` → `Test Explorer`, `Ctrl+E, T`).

**Breakpoints setzen:**  
Klicke links neben eine Zeile im Testcode → roter Punkt erscheint.  
Im Test Explorer auf **Debug Selected Tests** klicken – Visual Studio hält am Breakpoint an, während der Browser im Hintergrund weiterläuft.

**Playwright-Inspektor aus dem Test heraus öffnen:**  
Setze vor dem zu untersuchenden Schritt folgende Umgebungsvariable im Debug-Profil oder per Code:

```csharp
// am Anfang des Tests – öffnet den Playwright Inspector
Environment.SetEnvironmentVariable("PWDEBUG", "1");
```

Alternativ in den **Debug Launch Profiles** (Rechtsklick auf Projekt → Properties → Debug):

| Variable | Wert |
|----------|------|
| `PWDEBUG` | `1` |

Der Playwright Inspector öffnet sich dann automatisch beim nächsten Testlauf über den Debugger.

---

## Überblick über die App und Konfigurationshinweise

### App starten

```bash
npm run dev   # startet Vite Dev-Server auf http://localhost:3000
```

> 💡 Das Repo enthält eine `.nvmrc`-Datei. Falls du `nvm` verwendest, wechsle zuerst zur richtigen Node-Version:
> ```bash
> nvm use   # liest Version aus .nvmrc
> ```

### Verfügbare npm-Skripte

| Skript | Zweck |
|--------|-------|
| `npm run dev` | App lokal starten (Port 3000) |
| `npm test` | Playwright-Tests headless ausführen |
| `npm run test:ui` | Playwright UI-Modus (interaktiv) |
| `npm run build` | Produktions-Build erstellen |
| `npm run lint` | ESLint ausführen |

### App-Konfiguration auf einen Blick

**`vite.config.js`** setzt den Dev-Server-Port explizit auf **3000**:

```js
// vite.config.js
export default defineConfig({
  plugins: [react()],
  base: 'http://localhost:3000/',
});
```

→ Die Playwright-Config muss `baseURL: 'http://localhost:3000'` verwenden, damit relative Pfade (`'/'`) korrekt aufgelöst werden.

**`src/main.tsx`** enthält einen **fest einkompilierten initialen Datensatz**:

```ts
const DATA: Task[] = [
  {
    id: 'todo-iYhueLHTq-6wprHhsXYF6',
    name: 'test',                        // ← diese Aufgabe ist beim App-Start immer vorhanden
    time: '2024-11-18T16:12:44.160Z',
    location: { latitude: 49.6370557, longitude: 6.9014314 },
    completed: false,
  },
];
```

→ Alle Tests, die eine frische Seite laden, sehen genau **eine Aufgabe mit dem Namen „test"**.

**`public/remoteTasks.json`** wird vom „Load remote tasks"-Button per `fetch()` geladen und **ersetzt die komplette Taskliste**. Beim Testen sollte die Anfrage gemockt werden (→ Exercise 7).

### ⚠️ Wichtiger Hinweis: Geolocation-Pflicht

Die `addTask`-Funktion in `App.tsx` ruft **immer** `navigator.geolocation.getCurrentPosition` auf, bevor eine Aufgabe gespeichert wird:

```ts
// App.tsx – addTask
function addTask(name: string) {
  navigator.geolocation.getCurrentPosition((position) => {
    const newTask = { id: ..., name, location: position.coords, ... };
    setTasks([...tasks, newTask]);
  });
}
```

**Ohne Geolocation-Mock wird keine Aufgabe gespeichert** – der Callback wird einfach nie aufgerufen. Das betrifft alle Tests, die eine neue Aufgabe anlegen (Exercises 2, 8).

→ Lösung: Geolocation **vor** `page.goto()` mocken:

```ts
// TypeScript
await context.grantPermissions(['geolocation']);
await context.setGeolocation({ latitude: 49.637, longitude: 6.901 });
```

```csharp
// C# – ContextOptions() überschreiben
public override BrowserNewContextOptions ContextOptions() => new()
{
    Permissions = new[] { "geolocation" },
    Geolocation = new Geolocation { Latitude = 49.637f, Longitude = 6.901f },
};
```

### Selektoren und Test-IDs in der App

| Element | Selektor / Attribut | Wo definiert |
|---------|---------------------|--------------|
| Eingabefeld für neue Aufgabe | `#new-todo-input` | `Form.tsx` |
| „Add"-Button | `#myUniqueID` | `Form.tsx` |
| Filter-Buttons | `data-testid="testID-All"` / `testID-Active` / `testID-Completed` | `FilterButton.tsx` |
| Aufgaben-Counter | `#list-heading` | `App.tsx` |
| Aufgaben-Checkboxen | `role="checkbox"` | `Todo.tsx` |

> 💡 Playwright's `getByTestId()` sucht standardmäßig nach `data-testid`-Attributen – das passt exakt zu den `testID-*`-Attributen in `FilterButton.tsx`.



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

Ersetze die generierte `playwright.config.ts` durch diese vollständige Konfiguration:

```ts
// playwright.config.ts
import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  // Alle .spec.ts-Dateien im tests/-Ordner werden ausgeführt
  testDir: './tests',

  // Jeden Test bis zu 2x wiederholen bei Fehler (nur in CI)
  retries: process.env.CI ? 2 : 0,

  // Parallele Ausführung
  workers: process.env.CI ? 1 : undefined,

  // HTML-Report nach jedem Lauf erzeugen
  reporter: 'html',

  use: {
    // Basis-URL: alle page.goto('/') Aufrufe lösen gegen diese URL auf
    baseURL: 'http://localhost:3000',

    // Trace bei erstem Retry aufzeichnen (für Fehleranalyse im Trace Viewer)
    trace: 'on-first-retry',

    // Screenshot bei Testfehler
    screenshot: 'only-on-failure',
  },

  // Browser-Projekte – mindestens Chromium aktivieren
  projects: [
    { name: 'chromium', use: { ...devices['Desktop Chrome'] } },
    // { name: 'firefox',  use: { ...devices['Desktop Firefox'] } },
    // { name: 'webkit',   use: { ...devices['Desktop Safari'] } },
  ],

  // App automatisch starten; bereits laufende Instanz in der Entwicklung wiederverwenden
  webServer: {
    command: 'npm run dev',
    url: 'http://localhost:3000',
    reuseExistingServer: !process.env.CI,
    timeout: 30_000,
  },
});
```

Tests ausführen:

```bash
npm test              # headless
npm run test:ui       # interaktiver UI-Modus
npx playwright test --headed   # sichtbarer Browser
```

> ℹ️ Die von Playwright generierten Beispiel-Tests (z. B. `tests/example.spec.ts`) können gelöscht werden.

### 🟪 C# / .NET

Die App läuft bereits auf `http://localhost:3000`. Alle drei Frameworks nutzen die gleiche `PageTest`-Basisklasse aus dem jeweiligen Playwright-Paket – nur die Attribute unterscheiden sich.

**Schnellübersicht der Framework-Attribute:**

| | NUnit | xUnit | MSTest |
|---|---|---|---|
| Klassen-Attribut | `[TestFixture]` | *(keines)* | `[TestClass]` |
| Test-Attribut | `[Test]` | `[Fact]` | `[TestMethod]` |
| Setup | `[SetUp]` | Konstruktor / `InitializeAsync` | `[TestInitialize]` |
| Teardown | `[TearDown]` | `DisposeAsync` | `[TestCleanup]` |
| Basisklasse | `Microsoft.Playwright.NUnit.PageTest` | `Microsoft.Playwright.Xunit.PageTest` | `Microsoft.Playwright.MSTest.PageTest` |

**NUnit** – Beispiel-Gerüst:

```csharp
using Microsoft.Playwright.NUnit;

namespace TodoTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class AppTests : PageTest
{
    [SetUp]
    public async Task SetUp() => await Page.GotoAsync("http://localhost:3000/");
}
```

**xUnit** – Beispiel-Gerüst:

```csharp
using Microsoft.Playwright.Xunit;

namespace TodoTests;

public class AppTests : PageTest
{
    public AppTests() { /* xUnit konstruiert die Klasse vor jedem Test */ }

    // Kein [Fact] auf Klassenebene nötig; Page ist per Basisklasse verfügbar
}
```

**MSTest** – Beispiel-Gerüst:

```csharp
using Microsoft.Playwright.MSTest;

namespace TodoTests;

[TestClass]
public class AppTests : PageTest
{
    [TestInitialize]
    public async Task SetUp() => await Page.GotoAsync("http://localhost:3000/");
}
```

```bash
dotnet test
```

> ℹ️ Die App muss **manuell gestartet** sein (`npm run dev`), da es kein eingebautes `webServer`-Konzept gibt.

---

## Methodik: Wie schreibe ich Tests? Code-getrieben vs. Codegen

Playwright-Tests können auf zwei Wegen entstehen. Beide führen zum Ziel – aber mit unterschiedlichen Stärken.

| | Code-getrieben ✍️ | Codegen 🎥 |
|---|---|---|
| **Wie** | Test wird von Hand als Code geschrieben | Interaktionen im Browser aufzeichnen → Code wird generiert |
| **Einstiegshürde** | Höher – erfordert API-Kenntnisse | Niedrig – keine Vorkenntnisse nötig |
| **Code-Qualität** | Direkt wartbar, semantisch, präzise | Oft verbose, fragile Selektoren, braucht Nacharbeit |
| **Lerneffekt** | Hoch – man versteht die API aktiv | Gering – man klickt, liest ab |
| **Empfehlung Trainer** | ⭐ **Bevorzugt** | Als Einstieg oder Locator-Hilfe |

> 🎯 **Trainerempfehlung:** Schreibe Tests grundsätzlich code-getrieben. Codegen ist ein nützliches Werkzeug, um Locators schnell zu ermitteln oder einen ersten Entwurf zu generieren – aber der Output sollte immer überarbeitet werden.

---

### Ansatz 1 – Code-getrieben ✍️

Tests werden direkt als Code geschrieben. Du entscheidest bewusst, welche Locators du verwendest, und nutzt semantische Selektoren (`getByRole`, `getByLabel`, `getByTestId`), die robuster gegen UI-Änderungen sind als CSS-Selektoren oder XPath.

**Vorgehensweise:**
1. App im Browser manuell explorieren
2. Relevante UI-Elemente identifizieren (IDs, Rollen, Test-IDs aus dem Quellcode)
3. Test in der IDE schreiben
4. Ausführen und iterieren

**Beispiel – TodoMatic, Titel prüfen:**

```ts
// TypeScript – direkt geschrieben
test('Titel ist sichtbar', async ({ page }) => {
  await page.goto('/');
  await expect(page.getByRole('heading', { level: 2 })).toContainText('TodoMatic');
});
```

```csharp
// C# – direkt geschrieben
[Test] // oder [Fact] / [TestMethod]
public async Task TitelIstSichtbar()
{
    await Page.GotoAsync("http://localhost:3000/");
    await Expect(Page.GetByRole(AriaRole.Heading, new() { Level = 2 }))
        .ToContainTextAsync("TodoMatic");
}
```

---

### Ansatz 2 – Codegen 🎥

Playwright zeichnet Klicks und Eingaben im Browser auf und generiert daraus automatisch Testcode. Gut geeignet, um:
- den Einstieg zu erleichtern,
- Locators für unbekannte Elemente schnell herauszufinden,
- ein erstes Testgerüst zu erzeugen, das danach bereinigt wird.

**🟦 TypeScript – Codegen starten:**

```bash
# Terminal
npx playwright codegen http://localhost:3000

# VS Code: Ctrl+Shift+P → "Playwright: Record new"
```

**🟪 C# – Codegen mit Framework-Target:**

```bash
# NUnit
pwsh bin/Debug/net8.0/playwright.ps1 codegen http://localhost:3000 --target=csharp-nunit

# MSTest
pwsh bin/Debug/net8.0/playwright.ps1 codegen http://localhost:3000 --target=csharp-mstest

# Generisches C# (für xUnit manuell anpassen)
pwsh bin/Debug/net8.0/playwright.ps1 codegen http://localhost:3000 --target=csharp
```

> **🎯 Pick-Locator-Tipp:** Im Playwright Inspector kannst du mit dem **🎯 Pick locator**-Button gezielt einzelne Elemente anklicken, um den besten Locator zu ermitteln – ohne einen ganzen Test aufzuzeichnen.

---

### Hands-on: Beide Ansätze im Vergleich

**Aufgabe:** Schreibe denselben Test auf beide Arten und vergleiche das Ergebnis.

**Szenario:** Öffne die App und prüfe, dass der Text „1 task remaining" sichtbar ist.

#### Schritt 1 – Codegen verwenden

Starte Codegen, öffne die App und beobachte, was generiert wird – ohne zu klicken. Kopiere den Code.

Typischer Codegen-Output (TypeScript):

```ts
// ⚠️ Automatisch generiert – oft zu fragil für echte Tests
import { test, expect } from '@playwright/test';

test('test', async ({ page }) => {
  await page.goto('http://localhost:3000/');
  await expect(page.locator('#list-heading')).toContainText('1 task remaining');
});
```

#### Schritt 2 – Code-getrieben schreiben

Schreibe denselben Test von Hand – ohne den Codegen-Output zu kopieren.

<details>
<summary>Lösungshinweis 🟦 TypeScript</summary>

```ts
// ✅ Code-getrieben – semantisch, wartbar
import { test, expect } from '@playwright/test';

test('Startzustand zeigt eine offene Aufgabe', async ({ page }) => {
  await page.goto('/');
  await expect(page.locator('#list-heading')).toContainText('1 task remaining');
});
```

</details>

<details>
<summary>Lösungshinweis 🟪 C# (NUnit / xUnit / MSTest)</summary>

```csharp
// ✅ Code-getrieben
// Attribut: [Test] / [Fact] / [TestMethod]
public async Task StartzustandZeigtEineOffeneAufgabe()
{
    await Page.GotoAsync("http://localhost:3000/");
    await Expect(Page.Locator("#list-heading")).ToContainTextAsync("1 task remaining");
}
```

</details>

#### Schritt 3 – Vergleich und Reflexion

Betrachte beide Versionen und beantworte folgende Fragen:

| Frage | Codegen | Code-getrieben |
|-------|---------|----------------|
| Wie heißt der Test? | `'test'` (generisch) | Beschreibend |
| Welche URL wird verwendet? | Absolute URL `http://...` | Relativ `'/'` → nutzt `baseURL` |
| Locator-Strategie | ID-basiert | ID-basiert (hier gleich, aber oft anders) |
| Lesbarkeit | Mittel | Hoch |

> 💡 Codegen nutzt standardmäßig absolute URLs und manchmal fragile Selektoren wie `nth()` oder komplexe CSS-Pfade. Code-getrieben lässt dich `getByRole`, `getByLabel` und andere semantische Locators wählen, die deutlich robuster sind.

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
<summary>Hinweis anzeigen (C# – NUnit / xUnit / MSTest)</summary>

> Die Playwright-API-Aufrufe (`Expect`, `Page.Locator`, …) sind in allen drei Frameworks **identisch**. Nur die Test-Infrastruktur-Attribute unterscheiden sich.

**NUnit**
```csharp
[Test]
public async Task SeiteZeigtTitelTodoMatic()
{
    await Expect(Page.Locator("h2")).ToContainTextAsync("TodoMatic");
}
```

**xUnit**
```csharp
[Fact]
public async Task SeiteZeigtTitelTodoMatic()
{
    await Page.GotoAsync("http://localhost:3000/");
    await Expect(Page.Locator("h2")).ToContainTextAsync("TodoMatic");
}
```

**MSTest**
```csharp
[TestMethod]
public async Task SeiteZeigtTitelTodoMatic()
{
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
<summary>Hinweis anzeigen (C# – NUnit / xUnit / MSTest)</summary>

In allen drei Frameworks wird Geolocation über `ContextOptions()` gesetzt – diese Methode ist in `PageTest` überschreibbar.

**NUnit**
```csharp
public override BrowserNewContextOptions ContextOptions() => new()
{
    Permissions = new[] { "geolocation" },
    Geolocation = new Geolocation { Latitude = 49.637f, Longitude = 6.901f },
};

[Test]
public async Task NeueAufgabeHinzufuegen()
{
    await Page.GotoAsync("http://localhost:3000/");
    await Page.Locator("#new-todo-input").FillAsync("Playwright lernen");
    await Page.Locator("#myUniqueID").ClickAsync();
    await Expect(Page.GetByText("Playwright lernen")).ToBeVisibleAsync();
}
```

**xUnit**
```csharp
public override BrowserNewContextOptions ContextOptions() => new()
{
    Permissions = new[] { "geolocation" },
    Geolocation = new Geolocation { Latitude = 49.637f, Longitude = 6.901f },
};

[Fact]
public async Task NeueAufgabeHinzufuegen()
{
    await Page.GotoAsync("http://localhost:3000/");
    await Page.Locator("#new-todo-input").FillAsync("Playwright lernen");
    await Page.Locator("#myUniqueID").ClickAsync();
    await Expect(Page.GetByText("Playwright lernen")).ToBeVisibleAsync();
}
```

**MSTest**
```csharp
public override BrowserNewContextOptions ContextOptions() => new()
{
    Permissions = new[] { "geolocation" },
    Geolocation = new Geolocation { Latitude = 49.637f, Longitude = 6.901f },
};

[TestMethod]
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
<summary>Hinweis anzeigen (C# – NUnit / xUnit / MSTest)</summary>

> Die Playwright-API-Zeilen sind für alle Frameworks gleich – tausche nur das Attribut aus.

**NUnit:** `[Test]` &nbsp;|&nbsp; **xUnit:** `[Fact]` &nbsp;|&nbsp; **MSTest:** `[TestMethod]`

```csharp
// Attribut je nach Framework: [Test] / [Fact] / [TestMethod]
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
<summary>Hinweis anzeigen (C# – NUnit / xUnit / MSTest)</summary>

> Tausche nur das Attribut aus: `[Test]` / `[Fact]` / `[TestMethod]`

```csharp
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
<summary>Hinweis anzeigen (C# – NUnit / xUnit / MSTest)</summary>

> Tausche nur das Attribut aus: `[Test]` / `[Fact]` / `[TestMethod]`

```csharp
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
<summary>Hinweis anzeigen (C# – NUnit / xUnit / MSTest)</summary>

> Tausche nur das Attribut aus: `[Test]` / `[Fact]` / `[TestMethod]`

```csharp
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
<summary>Hinweis anzeigen (C# – NUnit / xUnit / MSTest)</summary>

> Tausche nur das Attribut aus: `[Test]` / `[Fact]` / `[TestMethod]`

```csharp
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

**C# (framework-unabhängig – die Page-Klasse selbst kennt keine Test-Attribute):**
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
<summary>Hinweis anzeigen (C# – NUnit / xUnit / MSTest)</summary>

**`Pages/TodoPage.cs`** – framework-unabhängig:

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

**`PomTests.cs` – NUnit**

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

**`PomTests.cs` – xUnit**

```csharp
using Microsoft.Playwright.Xunit;
using TodoTests.Pages;

namespace TodoTests;

public class PomTests : PageTest
{
    public override BrowserNewContextOptions ContextOptions() => new()
    {
        Permissions = new[] { "geolocation" },
        Geolocation = new Geolocation { Latitude = 49.637f, Longitude = 6.901f },
    };

    [Fact]
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

**`PomTests.cs` – MSTest**

```csharp
using Microsoft.Playwright.MSTest;
using TodoTests.Pages;

namespace TodoTests;

[TestClass]
public class PomTests : PageTest
{
    public override BrowserNewContextOptions ContextOptions() => new()
    {
        Permissions = new[] { "geolocation" },
        Geolocation = new Geolocation { Latitude = 49.637f, Longitude = 6.901f },
    };

    [TestMethod]
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

## Exercise 9 – Debugging-Tools: Codegen, Trace Viewer und Browser DevTools

### Hintergrund

Playwright liefert drei leistungsstarke Werkzeuge, die beim Schreiben und Debuggen von Tests helfen:

| Werkzeug | Wozu? |
|----------|-------|
| **Codegen** | Interaktionen im Browser aufzeichnen → fertiger Testcode wird generiert |
| **Trace Viewer** | Schritt-für-Schritt-Replay eines fehlgeschlagenen Tests inkl. Screenshots, Netzwerk, Konsole |
| **Browser DevTools** | Klassische Entwicklertools (DOM-Inspektor, Network, Console) direkt im Playwright-Browser |

---

### Teil A – Codegen als Locator-Hilfe und Refactoring-Übung

> 💡 Die grundlegende Gegenüberstellung von Code-getrieben vs. Codegen wurde bereits im Abschnitt **„Methodik"** (vor Exercise 1) behandelt. Hier geht es um den gezielten Einsatz von Codegen als Werkzeug im Testeentwicklungs-Workflow.

#### Aufgabe

1. Zeichne mit Codegen folgenden Ablauf auf:
   - App öffnen
   - Aufgabe „Codegen-Test" eingeben und auf „Add" klicken  
     *(Achtung: Geolocation muss im Browser erlaubt sein!)*
   - Den „Delete"-Button der neuen Aufgabe klicken
2. Kopiere den generierten Code in `tests/codegen-raw.spec.ts`.
3. Führe ihn aus – läuft er durch?
4. **Refaktoriere** ihn anschließend in `tests/codegen-refactored.spec.ts`:
   - Absolute URLs → relative Pfade (`'/'`)
   - Fragile Selektoren → semantische Locators (`getByRole`, `getByLabel`, `getByTestId`)
   - Generischer Testname → beschreibender Name
   - Geolocation-Mock hinzufügen (ohne den läuft der Test nicht zuverlässig)

#### 🟦 TypeScript – Codegen starten

**Variante 1 – Terminal:**
```bash
npx playwright codegen http://localhost:3000
```

**Variante 2 – VS Code:**  
`Ctrl+Shift+P` → `Playwright: Record new` → URL eingeben.

Ein Browserfenster und der **Playwright Inspector** öffnen sich. Alle Klicks und Eingaben werden in Echtzeit als TypeScript-Code angezeigt.  
Klicke auf 📋 **Copy**, um den Code zu übernehmen.

> **🎯 Pick locator:** Klicke im Inspector auf den Fadenkreuz-Button und dann auf ein Element – der optimale Locator wird direkt angezeigt, ohne einen vollständigen Test aufzuzeichnen.

#### 🟪 C# – Codegen starten

```bash
# NUnit
pwsh bin/Debug/net8.0/playwright.ps1 codegen http://localhost:3000 --target=csharp-nunit

# MSTest
pwsh bin/Debug/net8.0/playwright.ps1 codegen http://localhost:3000 --target=csharp-mstest

# xUnit: generischen Output verwenden und Attribute manuell anpassen
pwsh bin/Debug/net8.0/playwright.ps1 codegen http://localhost:3000 --target=csharp
```

#### Lösungshinweis – Typischer Codegen-Output vs. refaktorierte Version

<details>
<summary>Vergleich anzeigen (TypeScript)</summary>

**Codegen-Output (typisch, unbereinigt):**
```ts
// tests/codegen-raw.spec.ts
import { test, expect } from '@playwright/test';

test('test', async ({ page }) => {
  await page.goto('http://localhost:3000/');
  await page.locator('#new-todo-input').click();
  await page.locator('#new-todo-input').fill('Codegen-Test');
  await page.locator('#myUniqueID').click();
  await page.locator('li').filter({ hasText: 'Codegen-Test' })
    .getByRole('button', { name: 'Delete' }).click();
});
```

Probleme: absoluter URL, kein Geolocation-Mock, generischer Testname, kein `expect`.

**Refaktoriert (code-getrieben):**
```ts
// tests/codegen-refactored.spec.ts
import { test, expect } from '@playwright/test';

test('Aufgabe hinzufügen und wieder löschen', async ({ page, context }) => {
  await context.grantPermissions(['geolocation']);
  await context.setGeolocation({ latitude: 49.637, longitude: 6.901 });

  await page.goto('/');
  await page.locator('#new-todo-input').fill('Codegen-Test');
  await page.locator('#myUniqueID').click();

  await expect(page.getByText('Codegen-Test')).toBeVisible();

  await page.getByRole('button', { name: 'Delete' }).click();

  await expect(page.getByText('Codegen-Test')).not.toBeVisible();
});
```

</details>

<details>
<summary>Vergleich anzeigen (C# – NUnit)</summary>

**Codegen-Output (typisch):**
```csharp
[Test]
public async Task Test()
{
    await Page.GotoAsync("http://localhost:3000/");
    await Page.Locator("#new-todo-input").ClickAsync();
    await Page.Locator("#new-todo-input").FillAsync("Codegen-Test");
    await Page.Locator("#myUniqueID").ClickAsync();
    await Page.Locator("li").Filter(new() { HasText = "Codegen-Test" })
        .GetByRole(AriaRole.Button, new() { Name = "Delete" }).ClickAsync();
}
```

**Refaktoriert:**
```csharp
public override BrowserNewContextOptions ContextOptions() => new()
{
    Permissions = new[] { "geolocation" },
    Geolocation = new Geolocation { Latitude = 49.637f, Longitude = 6.901f },
};

[Test]
public async Task AufgabeHinzufuegenUndLoeschen()
{
    await Page.GotoAsync("http://localhost:3000/");
    await Page.Locator("#new-todo-input").FillAsync("Codegen-Test");
    await Page.Locator("#myUniqueID").ClickAsync();

    await Expect(Page.GetByText("Codegen-Test")).ToBeVisibleAsync();

    await Page.GetByRole(AriaRole.Button, new() { Name = "Delete" }).ClickAsync();

    await Expect(Page.GetByText("Codegen-Test")).Not.ToBeVisibleAsync();
}
```

</details>

---

### Teil B – Trace Viewer: fehlgeschlagene Tests analysieren

#### Aufgabe

1. Schreibe absichtlich einen fehlschlagenden Test (z. B. prüfe auf Text, der nicht existiert).
2. Aktiviere die Trace-Aufzeichnung.
3. Öffne den Trace und analysiere, an welchem Schritt der Test gescheitert ist.

#### 🟦 TypeScript – Trace aktivieren

In `playwright.config.ts`:
```ts
use: {
  trace: 'on-first-retry',   // Trace beim ersten Retry aufzeichnen
  // oder: 'on'              // immer aufzeichnen
},
```

Nach einem fehlgeschlagenen Testlauf liegt der Trace unter `test-results/<testname>/trace.zip`.

**Trace öffnen:**
```bash
npx playwright show-trace test-results/<testname>/trace.zip
```

Oder in VS Code: `Ctrl+Shift+P` → `Playwright: Show trace viewer` → Datei auswählen.

**Im Trace Viewer:**
- Linke Spalte: alle Aktionen des Tests (klicke eine an, um den zugehörigen Screenshot zu sehen)
- Reiter **Network**: alle Netzwerkanfragen zum jeweiligen Zeitpunkt
- Reiter **Console**: Konsolenausgaben
- Reiter **Source**: Testquellcode mit markierter Zeile

#### 🟪 C# – Trace aktivieren

```csharp
// In der Testklasse, z. B. in [SetUp]:
await Context.Tracing.StartAsync(new()
{
    Screenshots = true,
    Snapshots = true,
    Sources = true,
});
```

```csharp
// In [TearDown]:
await Context.Tracing.StopAsync(new()
{
    Path = $"trace-{TestContext.CurrentContext.Test.Name}.zip",
});
```

**Trace öffnen:**
```bash
pwsh bin/Debug/net8.0/playwright.ps1 show-trace trace-<testname>.zip
```

---

### Teil C – Browser DevTools

#### Aufgabe

Öffne die Browser-DevTools während eines laufenden Tests und inspiziere den DOM sowie die Netzwerkanfragen.

#### 🟦 TypeScript

Starte den Test im **Debug-Modus** mit `--debug`-Flag:

```bash
npx playwright test --debug
```

Der Playwright Inspector öffnet sich, der Browser läuft im Vordergrund.  
Drücke im Inspector auf **Pause** → öffne dann mit `F12` die Browser DevTools.

Alternativ: Setze in `playwright.config.ts` `slowMo: 1000` (Millisekunden pro Aktion), um genug Zeit zu haben:

```ts
use: {
  headless: false,
  slowMo: 1000,
},
```

#### 🟪 C#

```csharp
// Am Anfang des Tests – startet den Playwright Inspector
Environment.SetEnvironmentVariable("PWDEBUG", "1");
```

Oder starte aus Visual Studio heraus mit **Debug Selected Tests** (Breakpoint setzen → `F5`).  
Der Browser öffnet sich sichtbar; drücke `F12` für DevTools.

> **Wichtig:** `headless: false` (TS) bzw. `LaunchOptions = new() { Headless = false }` (C#) ist Voraussetzung, damit DevTools zugänglich sind.

---

### Lösungshinweis 🟦 TypeScript – vollständiger Trace-Test

<details>
<summary>Hinweis anzeigen (TypeScript)</summary>

```ts
// playwright.config.ts – Trace für alle Tests aktivieren
use: {
  baseURL: 'http://localhost:3000',
  trace: 'on',
  headless: false,
  slowMo: 500,
},
```

```ts
// tests/trace-demo.spec.ts
import { test, expect } from '@playwright/test';

test('absichtlich fehlschlagender Test', async ({ page }) => {
  await page.goto('/');
  // Dieser Text existiert nicht – Test schlägt fehl
  await expect(page.getByText('Diese Aufgabe gibt es nicht')).toBeVisible();
});
```

Nach dem Lauf:
```bash
npx playwright show-trace test-results/trace-demo-absichtlich-fehlschlagender-Test/trace.zip
```

</details>

### Lösungshinweis 🟪 C# – vollständiger Trace-Test

<details>
<summary>Hinweis anzeigen (C#)</summary>

```csharp
[TestFixture]
public class TraceTests : PageTest
{
    [SetUp]
    public async Task StartTracing()
    {
        await Context.Tracing.StartAsync(new()
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true,
        });
    }

    [TearDown]
    public async Task StopTracing()
    {
        await Context.Tracing.StopAsync(new()
        {
            Path = $"trace-{TestContext.CurrentContext.Test.Name}.zip",
        });
    }

    [Test]
    public async Task AbsichtlichFehlschlagenderTest()
    {
        await Page.GotoAsync("http://localhost:3000/");
        // Dieser Text existiert nicht – Test schlägt fehl
        await Expect(Page.GetByText("Diese Aufgabe gibt es nicht")).ToBeVisibleAsync();
    }
}
```

Nach dem Lauf:
```bash
pwsh bin/Debug/net8.0/playwright.ps1 show-trace trace-AbsichtlichFehlschlagenderTest.zip
```

</details>

---

## Exercise 10 – Screenshots und Videoaufzeichnung

Playwright kann Testläufe automatisch per Screenshot und Video dokumentieren – hilfreich beim Debuggen, im CI-Reporting und für visuelle Regressionstests.

### Überblick

| Funktion | Konfigurationsoption | Wann sinnvoll |
|----------|---------------------|---------------|
| Screenshot bei Fehler | `screenshot: 'only-on-failure'` | Standard-Debugging |
| Screenshot immer | `screenshot: 'on'` | Visuelle Dokumentation |
| Screenshot manuell | `page.screenshot()` im Test | Gezielter Beweis |
| Visueller Regressionstest | `expect(page).toHaveScreenshot()` | UI-Änderungen erkennen |
| Video bei Fehler | `video: 'retain-on-failure'` | CI-Fehleranalyse |
| Video immer | `video: 'on'` | Vollständige Aufzeichnung |

---

### Teil A – Automatische Screenshots und Videos per Konfiguration

#### Aufgabe

Konfiguriere Playwright so, dass:
1. Bei jedem Testfehler automatisch ein Screenshot gespeichert wird.
2. Bei jedem Testfehler eine Videoaufzeichnung gespeichert wird.

Schreibe anschließend absichtlich einen fehlschlagenden Test und prüfe, wo die Artefakte landen.

#### Lösungshinweis 🟦 TypeScript

<details>
<summary>Hinweis anzeigen (TypeScript)</summary>

In `playwright.config.ts`:

```ts
use: {
  baseURL: 'http://localhost:3000',

  // Screenshot bei jedem fehlgeschlagenen Test
  screenshot: 'only-on-failure',
  // Alternativen: 'on' (immer) | 'off' (nie)

  // Video bei fehlgeschlagenem Test behalten
  video: 'retain-on-failure',
  // Alternativen: 'on' (immer) | 'off' (nie) | 'on-first-retry'
},
```

Nach einem fehlgeschlagenen Testlauf:

```
test-results/
  <testname>/
    test-failed-1.png     ← Screenshot
    video.webm            ← Video
```

Artefakte öffnen:
```bash
# HTML-Report enthält Screenshot und Video eingebettet
npx playwright show-report
```

</details>

#### Lösungshinweis 🟪 C#

<details>
<summary>Hinweis anzeigen (C# – NUnit / xUnit / MSTest)</summary>

In C# werden Screenshot und Video direkt im Test oder `[TearDown]` gesteuert.  
Es gibt keine zentrale `playwright.config`-Entsprechung – nutze stattdessen `LaunchOptions` und Kontext-Optionen.

**Screenshot bei Fehler (NUnit-Beispiel, analog für xUnit/MSTest):**

```csharp
[TearDown]  // xUnit: DisposeAsync | MSTest: [TestCleanup]
public async Task TakeScreenshotOnFailure()
{
    // NUnit-spezifisch: Status prüfen
    if (TestContext.CurrentContext.Result.Outcome.Status
            == NUnit.Framework.Interfaces.TestStatus.Failed)
    {
        var screenshotPath =
            $"screenshot-{TestContext.CurrentContext.Test.Name}.png";
        await Page.ScreenshotAsync(new() { Path = screenshotPath, FullPage = true });
        TestContext.AddTestAttachment(screenshotPath, "Failure Screenshot");
    }
}
```

**Video-Aufzeichnung:**

```csharp
// In ContextOptions() – Video für alle Tests aktivieren
public override BrowserNewContextOptions ContextOptions() => new()
{
    RecordVideoDir = "videos/",
    RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 },
};

[TearDown]
public async Task SaveVideo()
{
    // Video wird erst beim Schließen der Page finalisiert
    await Page.CloseAsync();
    // Die .webm-Datei liegt jetzt in videos/
}
```

Video-Pfad nach dem Test:
```
videos/
  <guid>.webm
```

</details>

---

### Teil B – Screenshot manuell im Test aufnehmen

#### Aufgabe

Nimm an einem definierten Punkt im Test einen gezielten Screenshot auf – z. B. direkt nachdem eine neue Aufgabe hinzugefügt wurde – und speichere ihn mit einem aussagekräftigen Dateinamen.

#### Lösungshinweis 🟦 TypeScript

<details>
<summary>Hinweis anzeigen (TypeScript)</summary>

```ts
test('Screenshot nach dem Hinzufügen einer Aufgabe', async ({ page, context }) => {
  await context.grantPermissions(['geolocation']);
  await context.setGeolocation({ latitude: 49.637, longitude: 6.901 });

  await page.goto('/');
  await page.locator('#new-todo-input').fill('Screenshot-Aufgabe');
  await page.locator('#myUniqueID').click();

  await expect(page.getByText('Screenshot-Aufgabe')).toBeVisible();

  // Gezielter Screenshot – ganzseitig
  await page.screenshot({
    path: 'test-results/nach-hinzufuegen.png',
    fullPage: true,
  });

  // Alternativ: nur ein Element fotografieren
  await page.locator('#list-heading').screenshot({
    path: 'test-results/list-heading.png',
  });
});
```

</details>

#### Lösungshinweis 🟪 C#

<details>
<summary>Hinweis anzeigen (C# – NUnit / xUnit / MSTest)</summary>

```csharp
// Attribut: [Test] / [Fact] / [TestMethod]
public async Task ScreenshotNachHinzufuegen()
{
    await Page.GotoAsync("http://localhost:3000/");
    await Page.Locator("#new-todo-input").FillAsync("Screenshot-Aufgabe");
    await Page.Locator("#myUniqueID").ClickAsync();

    await Expect(Page.GetByText("Screenshot-Aufgabe")).ToBeVisibleAsync();

    // Ganzseitiger Screenshot
    await Page.ScreenshotAsync(new()
    {
        Path = "nach-hinzufuegen.png",
        FullPage = true,
    });

    // Nur ein Element
    await Page.Locator("#list-heading").ScreenshotAsync(new()
    {
        Path = "list-heading.png",
    });
}
```

</details>

---

### Teil C – Visueller Regressionstest mit `toHaveScreenshot`

Playwright kann Screenshots mit einem **gespeicherten Referenz-Screenshot** vergleichen und schlägt fehl, wenn sich die UI verändert hat.

#### Aufgabe

1. Schreibe einen Test, der einen visuellen Snapshot der Seite anlegt.
2. Führe ihn zweimal aus – beim ersten Lauf wird der Referenz-Screenshot erzeugt, beim zweiten wird verglichen.
3. Verändere dann etwas in der App (z. B. einen Text in `App.tsx`) und prüfe, ob der Test fehlschlägt.
4. Aktualisiere den Referenz-Screenshot mit dem Update-Flag.

#### Lösungshinweis 🟦 TypeScript

<details>
<summary>Hinweis anzeigen (TypeScript)</summary>

```ts
// tests/visual.spec.ts
import { test, expect } from '@playwright/test';

test('Visueller Snapshot der Startseite', async ({ page }) => {
  await page.goto('/');

  // Beim ersten Lauf: Referenz-Screenshot wird erzeugt
  // Ab dem zweiten Lauf: Vergleich mit Referenz
  await expect(page).toHaveScreenshot('startseite.png', {
    maxDiffPixels: 100,   // erlaubte Pixelabweichung
  });
});

test('Visueller Snapshot der Todo-Liste', async ({ page }) => {
  await page.goto('/');

  // Nur einen bestimmten Bereich vergleichen
  await expect(page.locator('ul.todo-list')).toHaveScreenshot('todo-liste.png');
});
```

Referenz-Screenshots liegen in:
```
tests/visual.spec.ts-snapshots/
  startseite-chromium-linux.png
```

Referenz aktualisieren (nach bewusster UI-Änderung):
```bash
npx playwright test --update-snapshots
```

</details>

#### Lösungshinweis 🟪 C#

<details>
<summary>Hinweis anzeigen (C# – NUnit / xUnit / MSTest)</summary>

In C# gibt es kein eingebautes `toHaveScreenshot`-Äquivalent. Der übliche Ansatz ist ein manueller Pixel-Vergleich mit einem gespeicherten Referenzbild:

```csharp
using SixLabors.ImageSharp;         // dotnet add package SixLabors.ImageSharp
using SixLabors.ImageSharp.PixelFormats;

// Attribut: [Test] / [Fact] / [TestMethod]
public async Task VisuellerSnapshotStartseite()
{
    await Page.GotoAsync("http://localhost:3000/");

    var screenshotBytes = await Page.ScreenshotAsync(new() { FullPage = true });
    var referenzPfad = "snapshots/startseite-referenz.png";

    if (!File.Exists(referenzPfad))
    {
        // Erster Lauf: Referenz speichern
        Directory.CreateDirectory("snapshots");
        await File.WriteAllBytesAsync(referenzPfad, screenshotBytes);
        Assert.Pass("Referenz-Screenshot erstellt – Test erneut ausführen zum Vergleich.");
        return;
    }

    // Vergleich
    using var referenz = Image.Load<Rgba32>(referenzPfad);
    using var aktuell  = Image.Load<Rgba32>(screenshotBytes);

    Assert.That(aktuell.Width,  Is.EqualTo(referenz.Width),  "Breite unterschiedlich");
    Assert.That(aktuell.Height, Is.EqualTo(referenz.Height), "Höhe unterschiedlich");

    int abweichung = 0;
    for (int y = 0; y < referenz.Height; y++)
        for (int x = 0; x < referenz.Width; x++)
            if (referenz[x, y] != aktuell[x, y]) abweichung++;

    Assert.That(abweichung, Is.LessThanOrEqualTo(200),
        $"Zu viele unterschiedliche Pixel: {abweichung}");
}
```

> 💡 Für produktiven Einsatz empfiehlt sich eine dedizierte Bibliothek wie **Playwright.Contrib.FluentAssertions** oder **ImageSharp.Compare** für stabilere Bildvergleiche.

</details>

---

## Exercise 11 – CI: Tests in der Pipeline ausführen

Playwright-Tests lassen sich in verschiedenen CI/CD-Umgebungen automatisieren. Diese Übung zeigt **vier Varianten** – wähle die für euren Stack passende.

| Variante | Wann verwenden? |
|----------|----------------|
| **A – GitHub Actions** | Repository liegt auf GitHub |
| **B – Azure Pipelines** | Azure DevOps als CI/CD-Plattform |
| **C – Docker Container** | Reproduzierbare, isolierte Ausführung (lokal & in CI) |
| **D – Azure Playwright Service** | Skalierbare Cloud-Ausführung auf Microsoft-Infrastruktur |

### Aufgabe (alle Varianten)

Richte einen automatisierten Testlauf ein, der:

1. Bei jedem Push / PR auf `main` ausgeführt wird.
2. Abhängigkeiten installiert **und** Playwright-Browser bereitstellt.
3. Die Tests ausführt.
4. Den Test-Report als Artefakt speichert.

---

### Variante A – GitHub Actions

#### Lösungshinweis 🟦 TypeScript

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

#### Lösungshinweis 🟪 C# (NUnit / xUnit / MSTest)

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

      # React-App starten
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

### Variante B – Azure Pipelines

#### Lösungshinweis 🟦 TypeScript

<details>
<summary>Hinweis anzeigen (TypeScript)</summary>

```yaml
# azure-pipelines.yml
trigger:
  branches:
    include: [main]

pr:
  branches:
    include: [main]

pool:
  vmImage: ubuntu-latest

steps:
  - task: NodeTool@0
    inputs:
      versionSpec: '20.x'
    displayName: Node.js einrichten

  - script: npm ci
    displayName: Abhängigkeiten installieren

  - script: npx playwright install --with-deps
    displayName: Playwright-Browser installieren

  - script: npm test
    displayName: Tests ausführen
    env:
      CI: true

  - task: PublishPipelineArtifact@1
    condition: always()
    inputs:
      targetPath: playwright-report
      artifact: playwright-report-ts
      publishLocation: pipeline
```

</details>

#### Lösungshinweis 🟪 C# (NUnit / xUnit / MSTest)

<details>
<summary>Hinweis anzeigen (C#)</summary>

```yaml
# azure-pipelines.yml
trigger:
  branches:
    include: [main]

pr:
  branches:
    include: [main]

pool:
  vmImage: ubuntu-latest

steps:
  # React-App starten
  - task: NodeTool@0
    inputs:
      versionSpec: '20.x'
    displayName: Node.js einrichten

  - script: npm ci
    displayName: npm-Abhängigkeiten installieren

  - script: npm run dev &
    displayName: React-App im Hintergrund starten

  - script: npx wait-on http://localhost:3000
    displayName: Warten bis App bereit ist

  # .NET Tests
  - task: UseDotNet@2
    inputs:
      version: '8.x'
    displayName: .NET SDK einrichten

  - script: dotnet build
    workingDirectory: TodoTests
    displayName: Projekt bauen

  - script: pwsh bin/Debug/net8.0/playwright.ps1 install --with-deps
    workingDirectory: TodoTests
    displayName: Playwright-Browser installieren

  - script: dotnet test --logger "trx;LogFileName=results.trx"
    workingDirectory: TodoTests
    displayName: Tests ausführen

  - task: PublishTestResults@2
    condition: always()
    inputs:
      testResultsFormat: VSTest
      testResultsFiles: 'TodoTests/TestResults/*.trx'
    displayName: TRX-Ergebnisse veröffentlichen

  - task: PublishPipelineArtifact@1
    condition: always()
    inputs:
      targetPath: TodoTests/TestResults
      artifact: playwright-report-dotnet
      publishLocation: pipeline
```

</details>

---

### Variante C – Docker Container

Das offizielle Playwright-Docker-Image `mcr.microsoft.com/playwright` enthält Node.js und alle Browser – kein separates `playwright install` nötig.

> Stelle sicher, dass die verwendete Image-Version (`v1.52.0`) mit der im Projekt genutzten Playwright-Version übereinstimmt.

#### Lokal ausführen

**TypeScript**
```bash
# Tests direkt im Container starten (Repo-Ordner wird eingebunden)
docker run --rm \
  -v "$(pwd)":/work -w /work \
  -e CI=true \
  mcr.microsoft.com/playwright:v1.52.0-jammy \
  /bin/bash -c "npm ci && npm test"
```

**C#** (App und Tests im selben Container)
```bash
docker run --rm \
  -v "$(pwd)":/work -w /work \
  mcr.microsoft.com/playwright/dotnet:v1.52.0-jammy \
  /bin/bash -c "
    # Node für die React-App
    curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && apt-get install -y nodejs
    npm ci && npm run dev &
    npx wait-on http://localhost:3000
    cd TodoTests && dotnet build && dotnet test
  "
```

#### Lösungshinweis 🟦 TypeScript – GitHub Actions mit Docker-Container

<details>
<summary>Hinweis anzeigen (TypeScript)</summary>

```yaml
# .github/workflows/playwright-docker.yml
name: Playwright Tests (Docker)

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  test:
    runs-on: ubuntu-latest
    container:
      image: mcr.microsoft.com/playwright:v1.52.0-jammy
      options: --user 1001   # verhindert Rechte-Probleme beim Schreiben

    steps:
      - uses: actions/checkout@v4

      - run: npm ci

      - run: npm test
        env:
          CI: true
          HOME: /root

      - uses: actions/upload-artifact@v4
        if: always()
        with:
          name: playwright-report-docker-ts
          path: playwright-report/
          retention-days: 7
```

</details>

#### Lösungshinweis 🟪 C# – Azure Pipelines mit Docker-Container

<details>
<summary>Hinweis anzeigen (C#)</summary>

```yaml
# azure-pipelines.yml (Docker-Variante)
trigger:
  branches:
    include: [main]

pool:
  vmImage: ubuntu-latest

container: mcr.microsoft.com/playwright/dotnet:v1.52.0-jammy

steps:
  # Node.js für die React-App nachrüsten
  - script: |
      curl -fsSL https://deb.nodesource.com/setup_20.x | bash -
      apt-get install -y nodejs
    displayName: Node.js im Container installieren

  - script: npm ci && npm run dev &
    displayName: React-App starten

  - script: npx wait-on http://localhost:3000
    displayName: Warten bis App bereit ist

  - script: dotnet build
    workingDirectory: TodoTests
    displayName: Projekt bauen

  - script: dotnet test --logger "trx;LogFileName=results.trx"
    workingDirectory: TodoTests
    displayName: Tests ausführen

  - task: PublishTestResults@2
    condition: always()
    inputs:
      testResultsFormat: VSTest
      testResultsFiles: 'TodoTests/TestResults/*.trx'
```

</details>

---

### Variante D – Azure Playwright Service (Bonus)

[**Azure Playwright Service**](https://azure.microsoft.com/en-us/products/playwright-testing) ist ein verwalteter Cloud-Dienst, der Playwright-Tests auf skalierbarer Microsoft-Infrastruktur ausführt – inklusive paralleler Ausführung auf mehreren Browsern ohne eigene Browser-Installation.

**Wann sinnvoll?**
- Viele Tests sollen parallel laufen (schnellere Gesamtlaufzeit)
- Kein eigenes Browser-Setup in der CI-Umgebung gewünscht
- Zentrale Verwaltung von Playwright-Versionen und Artefakten über das Azure-Portal

**Voraussetzung:** Azure-Abonnement + Playwright Testing Workspace im [Azure-Portal](https://portal.azure.com) erstellen.

#### Setup 🟦 TypeScript

```bash
npm install @azure/microsoft-playwright-testing
```

Erstelle `playwright.service.config.ts` neben der bestehenden Config:

```ts
// playwright.service.config.ts
import { defineConfig } from '@playwright/test';
import { getServiceConfig, ServiceOS } from '@azure/microsoft-playwright-testing';
import baseConfig from './playwright.config';

export default defineConfig(
  getServiceConfig(baseConfig, {
    os: ServiceOS.LINUX,
    runId: process.env.BUILD_BUILDID ?? new Date().toISOString(),
  })
);
```

Tests ausführen (Zugangsdaten als Umgebungsvariablen):

```bash
PLAYWRIGHT_SERVICE_URL=<URL-aus-Azure-Portal> \
PLAYWRIGHT_SERVICE_ACCESS_TOKEN=<Token-aus-Azure-Portal> \
npx playwright test --config=playwright.service.config.ts
```

#### Lösungshinweis 🟦 TypeScript – GitHub Actions

<details>
<summary>Hinweis anzeigen (TypeScript)</summary>

```yaml
# .github/workflows/playwright-azure-service.yml
name: Playwright Tests (Azure Playwright Service)

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

      - name: Tests über Azure Playwright Service ausführen
        run: npx playwright test --config=playwright.service.config.ts
        env:
          PLAYWRIGHT_SERVICE_URL: ${{ secrets.PLAYWRIGHT_SERVICE_URL }}
          PLAYWRIGHT_SERVICE_ACCESS_TOKEN: ${{ secrets.PLAYWRIGHT_SERVICE_ACCESS_TOKEN }}
          CI: true

      - uses: actions/upload-artifact@v4
        if: always()
        with:
          name: playwright-report-azure-service
          path: playwright-report/
          retention-days: 7
```

> 🔑 `PLAYWRIGHT_SERVICE_URL` und `PLAYWRIGHT_SERVICE_ACCESS_TOKEN` als **Repository Secrets** in den GitHub-Einstellungen hinterlegen (`Settings` → `Secrets and variables` → `Actions`).

</details>

#### Setup 🟪 C# (NUnit / MSTest)

```bash
# NUnit
dotnet add package Azure.Developer.MicrosoftPlaywrightTesting.NUnit

# MSTest
dotnet add package Azure.Developer.MicrosoftPlaywrightTesting.MSTest
```

Erstelle `.runsettings` im Testprojekt:

```xml
<!-- .runsettings -->
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <PlaywrightService>
    <Os>linux</Os>
    <RunId>$(BUILD_BUILDID)</RunId>
  </PlaywrightService>
</RunSettings>
```

Tests ausführen:

```bash
PLAYWRIGHT_SERVICE_URL=<URL-aus-Azure-Portal> \
PLAYWRIGHT_SERVICE_ACCESS_TOKEN=<Token-aus-Azure-Portal> \
dotnet test --settings .runsettings
```

#### Lösungshinweis 🟪 C# – Azure Pipelines

<details>
<summary>Hinweis anzeigen (C#)</summary>

```yaml
# azure-pipelines.yml (Azure Playwright Service)
trigger:
  branches:
    include: [main]

pool:
  vmImage: ubuntu-latest

steps:
  - task: NodeTool@0
    inputs:
      versionSpec: '20.x'

  - script: npm ci && npm run dev &
    displayName: React-App starten

  - script: npx wait-on http://localhost:3000
    displayName: Warten bis App bereit ist

  - task: UseDotNet@2
    inputs:
      version: '8.x'

  - script: dotnet build
    workingDirectory: TodoTests

  - script: dotnet test --settings .runsettings
    workingDirectory: TodoTests
    displayName: Tests über Azure Playwright Service ausführen
    env:
      PLAYWRIGHT_SERVICE_URL: $(PLAYWRIGHT_SERVICE_URL)
      PLAYWRIGHT_SERVICE_ACCESS_TOKEN: $(PLAYWRIGHT_SERVICE_ACCESS_TOKEN)
      BUILD_BUILDID: $(Build.BuildId)

  - task: PublishTestResults@2
    condition: always()
    inputs:
      testResultsFormat: VSTest
      testResultsFiles: 'TodoTests/TestResults/*.trx'
```

> 🔑 `PLAYWRIGHT_SERVICE_URL` und `PLAYWRIGHT_SERVICE_ACCESS_TOKEN` als **Pipeline-Variablen** in Azure DevOps hinterlegen (`Pipeline` → `Edit` → `Variables`) und als **Secret** markieren.

</details>

#### xUnit + Azure Playwright Service

xUnit wird über das generische Logging-Paket eingebunden:

```bash
dotnet add package Azure.Developer.MicrosoftPlaywrightTesting.TestLogger
```

Testausführung mit dem Service-Logger:

```bash
PLAYWRIGHT_SERVICE_URL=<URL> \
PLAYWRIGHT_SERVICE_ACCESS_TOKEN=<Token> \
dotnet test --logger "microsoft-playwright-testing"
```

---

## Bonus: Was könnte noch verbessert werden?

Schau dir den Quellcode an und überlege, welche weiteren Tests sinnvoll wären:

- **Bug in `toggleTaskCompleted`:** Finde den Fehler in `App.tsx` (Zeile ~44). Schreibe einen fehlschlagenden Test, der den Bug beweist, und fixe danach den Code.
- **Accessibility:** Nutze `@axe-core/playwright` (TS) oder `Deque.AxeCore.Playwright` (C#), um Barrierefreiheitsprobleme automatisch zu erkennen.
- **Screenshot-Vergleich:** Erweitere Exercise 10 um weitere Seiten-Snapshots und integriere den visuellen Regressionstest in die CI-Pipeline (Exercise 11).
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
| Codegen | [playwright.dev/docs/codegen](https://playwright.dev/docs/codegen) | [playwright.dev/dotnet/docs/codegen](https://playwright.dev/dotnet/docs/codegen) |
| Trace Viewer | [playwright.dev/docs/trace-viewer](https://playwright.dev/docs/trace-viewer) | [playwright.dev/dotnet/docs/trace-viewer](https://playwright.dev/dotnet/docs/trace-viewer) |
| VS Code Extension | [marketplace.visualstudio.com](https://marketplace.visualstudio.com/items?itemName=ms-playwright.playwright) | – |
| Debugging | [playwright.dev/docs/debug](https://playwright.dev/docs/debug) | [playwright.dev/dotnet/docs/debug](https://playwright.dev/dotnet/docs/debug) |
| Docker Image | [mcr.microsoft.com/playwright](https://mcr.microsoft.com/en-us/product/playwright/about) | [mcr.microsoft.com/playwright/dotnet](https://mcr.microsoft.com/en-us/product/playwright/dotnet/about) |
| Azure Playwright Service | [learn.microsoft.com/azure/playwright-testing](https://learn.microsoft.com/azure/playwright-testing/quickstart-run-end-to-end-tests) | [learn.microsoft.com/azure/playwright-testing](https://learn.microsoft.com/azure/playwright-testing/quickstart-run-end-to-end-tests?tabs=nunit) |
| Azure Pipelines | [learn.microsoft.com/azure/devops/pipelines](https://learn.microsoft.com/azure/devops/pipelines/get-started/what-is-azure-pipelines) | [learn.microsoft.com/azure/devops/pipelines](https://learn.microsoft.com/azure/devops/pipelines/get-started/what-is-azure-pipelines) |

---

*Happy Testing! 🎭*
