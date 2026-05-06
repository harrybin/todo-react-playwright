# Hands-on Lab: Playwright End-to-End Testing mit der TodoMatic-App

> **Niveau:** Fortgeschrittene Bootcamp-Übung  
> **Dauer:** ca. 4–5 Stunden  
> **App-Repo:** [harrybin/todo-react-playwright](https://github.com/harrybin/todo-react-playwright)  
> **Referenz-Demos:** [norschel/PlaywrightDemos](https://github.com/norschel/PlaywrightDemos)

---

## Lernziele

Nach dieser HOL kannst du:

- Playwright-Tests in TypeScript/JavaScript **und** C# (.NET) aufsetzen
- Lokatorstrategien (ARIA-Roles, `data-testid`, CSS, Text) gezielt einsetzen
- Browser-APIs wie Geolocation mocken
- Netzwerk-Requests abfangen und manipulieren
- Screenshots, Videos und Traces aufzeichnen
- Tests cross-browser und mit Mobile-Emulation ausführen
- JavaScript direkt ins DOM injizieren via `page.evaluate()`
- **Codegen, Trace Viewer und Browser DevTools** als Debugging-Werkzeuge einsetzen
- Playwright in einer GitHub Actions CI/CD-Pipeline betreiben

---

## Voraussetzungen

| Voraussetzung | TypeScript / JavaScript | C# / .NET |
|---|---|---|
| Laufzeitumgebung | Node.js 18+ | .NET 8 SDK+ |
| IDE | Visual Studio Code | Visual Studio 2022 **oder** Visual Studio Code |
| Grundkenntnisse | TypeScript / JS | C# |
| Gemeinsam | Git-Grundkenntnisse, React-Grundkenntnisse (hilfreich) | |

---

## Teil 0: IDE-Setup

### Visual Studio Code (TypeScript & C#)

1. Installiere die Extension **[Playwright Test for VS Code](https://marketplace.visualstudio.com/items?itemName=ms-playwright.playwright)**
   - Ermöglicht Tests direkt aus dem Editor starten
   - Integrierter **Codegen** (Test-Recorder) per Klick
   - Trace-Viewer direkt in VS Code
2. Für C#: Installiere zusätzlich die **[C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)**-Extension

Nach der Installation erscheint in der Seitenleiste das **Beaker-Symbol** (Testing). Dort kannst du:
- Einzelne Tests starten/debuggen
- Den **Record new** Button nutzen (= Codegen)
- Traces direkt öffnen

### Visual Studio 2022 (C# / .NET)

1. Stelle sicher, dass das **.NET 8 SDK** installiert ist
2. Öffne den **Test Explorer** (`Test → Test Explorer`)
3. Playwright-Tests erscheinen dort automatisch nach dem Build
4. Für den integrierten Debugger: Breakpoints setzen → Rechtsklick im Test Explorer → **Debug**

> **Tipp:** Visual Studio bietet keinen Playwright Codegen direkt, aber du kannst ihn per PowerShell-Skript aufrufen (siehe Übung 0B).

---

## Teil 0B: Projekt-Setup

### Setup: TypeScript / JavaScript (VS Code)

```bash
# App-Repo klonen und starten
git clone https://github.com/harrybin/todo-react-playwright.git
cd todo-react-playwright
npm install
npm run dev
# App läuft auf http://localhost:3000
```

Playwright installieren:

```bash
npm install --save-dev @playwright/test
npx playwright install
```

`playwright.config.ts` im Projektstamm erstellen:

```typescript
import { defineConfig, devices } from "@playwright/test";

export default defineConfig({
  testDir: "./tests",
  fullyParallel: true,
  retries: 1,
  reporter: [["html"], ["list"]],
  use: {
    baseURL: "http://localhost:3000",
    trace: "on-first-retry",
    screenshot: "only-on-failure",
  },
  projects: [
    { name: "chromium", use: { ...devices["Desktop Chrome"] } },
  ],
  webServer: {
    command: "npm run dev",
    url: "http://localhost:3000",
    reuseExistingServer: true,
  },
});
```

```bash
mkdir tests
npx playwright test --list  # Setup prüfen
```

---

### Setup: C# / .NET (Visual Studio oder VS Code)

```bash
# In einem separaten Verzeichnis neben dem App-Repo
dotnet new mstest -n TodoPlaywrightTests
cd TodoPlaywrightTests
dotnet add package Microsoft.Playwright.MSTest

# Projekt bauen
dotnet build

# Playwright-Browser installieren (PowerShell-Skript wird beim Build generiert)
pwsh bin/Debug/net8.0/playwright.ps1 install
```

Erstelle `playwright.config.json` (optional, für baseURL):

```json
{
  "use": {
    "baseURL": "http://localhost:3000"
  }
}
```

Basisklasse für alle Tests (`TestBase.cs`):

```csharp
using Microsoft.Playwright.MSTest;

[TestClass]
public class TestBase : PageTest
{
    // PageTest stellt this.Page, this.Browser, this.Context bereit
    // baseURL aus Umgebungsvariable oder Default
    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            BaseURL = Environment.GetEnvironmentVariable("PLAYWRIGHT_BASE_URL")
                      ?? "http://localhost:3000",
        };
    }
}
```

> **Hinweis:** Starte die TodoMatic-App (`npm run dev`) bevor du C#-Tests ausführst.  
> In der CI-Pipeline übernimmt ein `webServer`-Equivalent die App-Start-Logik.

---

## Teil 1: Debugging-Tools

Bevor du mit den Übungen beginnst, lerne die wichtigsten Playwright-Debugging-Tools kennen – sie ersparen dir Stunden bei der Fehlersuche.

### 🎬 Tool 1: Playwright Codegen (Test-Recorder)

Codegen zeichnet deine Browser-Interaktionen auf und generiert automatisch Playwright-Testcode.

**TypeScript (VS Code):**

```bash
# Codegen direkt starten
npx playwright codegen http://localhost:3000

# Oder in VS Code: Seitenleiste → Testing (Beaker) → "Record new" Button
```

**C# (.NET):**

```powershell
# PowerShell
pwsh bin/Debug/net8.0/playwright.ps1 codegen http://localhost:3000
```

**Was passiert:**
- Ein Browser öffnet sich mit der App
- Ein separates Fenster zeigt den generierten Code in Echtzeit
- Klicke, tippe, navigiere – Codegen übersetzt alles in Test-Code
- Kopiere den generierten Code als Startpunkt für deine Tests

> **Übung:** Starte Codegen, füge eine neue Aufgabe hinzu, markiere sie als abgeschlossen, und lösche sie. Betrachte den generierten Code. Welche Locator-Strategien wählt Codegen automatisch?

---

### 🔍 Tool 2: Playwright Inspector (PWDEBUG)

Der Inspector erlaubt Step-by-Step-Debugging direkt im Browser.

**TypeScript:**

```bash
# Vor dem Test-Aufruf setzen
PWDEBUG=1 npx playwright test smoke.spec.ts

# Windows PowerShell:
$env:PWDEBUG=1; npx playwright test smoke.spec.ts
```

**C#:**

```bash
# Umgebungsvariable setzen, dann normal testen
$env:PWDEBUG=1; dotnet test --filter "SmokeTest"
```

**Alternativ im Code (hält den Test an):**

```typescript
// TypeScript
await page.pause(); // öffnet den Inspector an dieser Stelle
```

```csharp
// C#
await Page.PauseAsync(); // öffnet den Inspector an dieser Stelle
```

**Features des Inspectors:**
- **Step over**: Test Schritt für Schritt ausführen
- **Locator Explorer**: Locatoren direkt auf der Seite ausprobieren
- **Pick locator**: Element anklicken → Inspector zeigt den besten Locator

---

### 📊 Tool 3: Playwright Trace Viewer

Der Trace Viewer ist ein vollständiger Zeitstrahl des Tests – mit DOM-Snapshots, Netzwerk-Requests und Screenshots zu jedem Schritt.

**TypeScript – Trace nach Test öffnen:**

```bash
npx playwright show-trace test-results/pfad-zum-test/trace.zip
```

**C# – Trace nach Test öffnen:**

```powershell
pwsh bin/Debug/net8.0/playwright.ps1 show-trace test-results/trace.zip
```

**In VS Code:** Nach einem fehlgeschlagenen Test erscheint in der Test-Ergebnis-Ansicht ein **"Show Trace"**-Link, der den Trace direkt in VS Code öffnet.

**Trace in der Config aktivieren:**

```typescript
// playwright.config.ts
use: {
  trace: "on",               // immer
  // trace: "on-first-retry" // nur beim Retry (empfohlen für CI)
  // trace: "retain-on-failure"
}
```

```csharp
// In C# per Umgebungsvariable vor dem Test
Environment.SetEnvironmentVariable("PLAYWRIGHT_TRACE", "on");
// Oder manuell im Test (siehe Exercise 7)
```

---

### 🌐 Tool 4: Browser DevTools

Playwright kann die Browser DevTools für Debugging-Sessions aktivieren.

**TypeScript:**

```typescript
// Browser im sichtbaren Modus + DevTools öffnen
test.use({ headless: false, launchOptions: { devtools: true } });
```

**C#:**

```csharp
// In playwright.config.json oder per LaunchOptions
public override BrowserTypeLaunchOptions LaunchOptions =>
    new() { Headless = false, Devtools = true };
```

**Nützlich für:**
- JavaScript-Fehler in der Konsole prüfen (`page.on('console', ...)`)
- Netzwerk-Traffic live beobachten
- CSS-Selektoren in der DevTools-Konsole ausprobieren: `$$('[data-testid]')`

---

## Übersicht der App-Struktur (TodoMatic)

| Element | Locator-Hinweis | Besonderheit |
|---|---|---|
| Seitentitel | `<h2>` mit Text "TodoMatic" | |
| Eingabefeld neue Aufgabe | `id="new-todo-input"` | |
| Hinzufügen-Button | `id="myUniqueID"` (Text: "Add") | |
| Filter-Buttons | `data-testid="testID-All/Active/Completed"` | `aria-pressed` zeigt aktiven Filter |
| Aufgaben-Liste | `role="list"` mit `aria-labelledby="list-heading"` | |
| Aufgaben-Anzahl | `id="list-heading"` | Text: "N tasks remaining" |
| Logo-Bild | `alt="Site Logo"` | HTTP-Anfrage: `getsitelogo.png` |
| Remote-Tasks-Button | Text "Load remote tasks" | Lädt `remoteTasks.json` per fetch |
| Bearbeiten-Button | Text "Edit" | |
| Löschen-Button | Text "Delete" | |
| Speichern-Button (Edit) | Text "Save" | |

> **⚠️ Geolocation:** Das Hinzufügen einer Aufgabe ruft `navigator.geolocation.getCurrentPosition` auf. Ohne explizites Mocken schlägt `addTask` lautlos fehl – die Aufgabe wird nie gespeichert.

---

## Exercise 1: Smoke Test – Seiteninhalt prüfen

**Ziel:** Grundstruktur eines Playwright-Tests verstehen. Prüfe, ob die Seite korrekt lädt und alle wichtigen UI-Elemente sichtbar sind.

**Aufgabe:**

Schreibe einen Test, der:
1. Die App öffnet
2. Den Browser-Tab-Titel prüft
3. Die Überschrift "TodoMatic" prüft
4. Das Eingabefeld und den "Add"-Button prüft
5. Alle drei Filter-Buttons prüft

> **🎬 Tipp:** Nutze zuerst **Codegen** (`npx playwright codegen http://localhost:3000` bzw. `pwsh playwright.ps1 codegen`), um dir automatisch einen Startpunkt generieren zu lassen. Beobachte, welche Locatoren Codegen für die Filter-Buttons wählt.

<details>
<summary>💡 Lösungshinweis TypeScript</summary>

Erstelle `tests/smoke.spec.ts`:

```typescript
import { test, expect } from "@playwright/test";

test("smoke test - app loads correctly", async ({ page }) => {
  await page.goto("/");

  // Browser-Titel
  await expect(page).toHaveTitle(/TodoMatic/);

  // Haupt-Überschrift
  await expect(page.getByRole("heading", { name: "TodoMatic" })).toBeVisible();

  // Formular-Elemente
  await expect(page.locator("#new-todo-input")).toBeVisible();
  await expect(page.locator("#myUniqueID")).toBeVisible();

  // Filter-Buttons via data-testid
  await expect(page.getByTestId("testID-All")).toBeVisible();
  await expect(page.getByTestId("testID-Active")).toBeVisible();
  await expect(page.getByTestId("testID-Completed")).toBeVisible();
});
```

```bash
npx playwright test smoke.spec.ts
```

</details>

<details>
<summary>💡 Lösungshinweis C#</summary>

Erstelle `SmokeTests.cs`:

```csharp
using Microsoft.Playwright.MSTest;

[TestClass]
public class SmokeTests : PageTest
{
    [TestMethod]
    public async Task AppLoadsCorrectly()
    {
        await Page.GotoAsync("http://localhost:3000");

        // Browser-Titel
        await Expect(Page).ToHaveTitleAsync(new Regex("TodoMatic"));

        // Haupt-Überschrift
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "TodoMatic" }))
            .ToBeVisibleAsync();

        // Formular-Elemente
        await Expect(Page.Locator("#new-todo-input")).ToBeVisibleAsync();
        await Expect(Page.Locator("#myUniqueID")).ToBeVisibleAsync();

        // Filter-Buttons via data-testid
        await Expect(Page.GetByTestId("testID-All")).ToBeVisibleAsync();
        await Expect(Page.GetByTestId("testID-Active")).ToBeVisibleAsync();
        await Expect(Page.GetByTestId("testID-Completed")).ToBeVisibleAsync();
    }
}
```

**Visual Studio:** Test Explorer → Rebuild → Test ausführen  
**VS Code:** Testing-Seitenleiste → Einzelnen Test starten

```bash
# Kommandozeile
dotnet test --filter "AppLoadsCorrectly"
```

</details>

---

## Exercise 2: Geolocation mocken und Aufgabe hinzufügen

**Ziel:** Browser-APIs mocken. Die App ruft `navigator.geolocation.getCurrentPosition` beim Hinzufügen auf – ohne Mock passiert nichts.

**Aufgabe:**

Schreibe einen Test, der:
1. Geolocation auf München (Lat 48.1372, Lon 11.5755) mockt
2. Eine Aufgabe "Playwright lernen" hinzufügt
3. Prüft, dass die Aufgabe in der Liste erscheint
4. Prüft, dass der Zähler um 1 gestiegen ist

> **🔍 Debug-Tipp:** Nutze `await page.pause()` / `await Page.PauseAsync()` direkt nach dem Klick auf "Add", um im Inspector zu beobachten, ob die Aufgabe erscheint oder ob die Geolocation blockiert.

<details>
<summary>💡 Lösungshinweis TypeScript</summary>

```typescript
import { test, expect } from "@playwright/test";

test.use({
  geolocation: { latitude: 48.1372, longitude: 11.5755 },
  permissions: ["geolocation"],
});

test("add a new task", async ({ page }) => {
  await page.goto("/");

  const headingText = await page.locator("#list-heading").textContent();
  const initialCount = parseInt(headingText?.match(/\d+/)?.[0] ?? "0");

  await page.locator("#new-todo-input").fill("Playwright lernen");
  await page.locator("#myUniqueID").click();

  await expect(
    page.getByRole("list").getByText("Playwright lernen")
  ).toBeVisible();

  await expect(page.locator("#list-heading")).toContainText(
    `${initialCount + 1}`
  );
});
```

</details>

<details>
<summary>💡 Lösungshinweis C#</summary>

```csharp
[TestClass]
public class AddTaskTests : PageTest
{
    public override BrowserNewContextOptions ContextOptions() =>
        new()
        {
            BaseURL = "http://localhost:3000",
            Geolocation = new Geolocation { Latitude = 48.1372f, Longitude = 11.5755f },
            Permissions = new[] { "geolocation" },
        };

    [TestMethod]
    public async Task AddNewTask()
    {
        await Page.GotoAsync("/");

        var headingText = await Page.Locator("#list-heading").TextContentAsync();
        var initialCount = int.Parse(Regex.Match(headingText ?? "0", @"\d+").Value);

        await Page.Locator("#new-todo-input").FillAsync("Playwright lernen");
        await Page.Locator("#myUniqueID").ClickAsync();

        await Expect(
            Page.GetByRole(AriaRole.List).GetByText("Playwright lernen")
        ).ToBeVisibleAsync();

        await Expect(Page.Locator("#list-heading"))
            .ToContainTextAsync($"{initialCount + 1}");
    }
}
```

**Warum Geolocation mocken?**  
Ohne Grant hängt der Callback und die Aufgabe wird nie gespeichert. Dieses Pattern ist in allen echten Apps relevant, die Location-APIs nutzen – z. B. Store-Finder oder Delivery-Tracking.

</details>

---

## Exercise 3: Aufgabe bearbeiten und löschen

**Ziel:** Interaktion mit dynamisch gerenderten, voneinander unabhängigen Listenelementen. Der Schlüssel ist präzises Locator-Chaining.

**Aufgabe:**

**Test A – Bearbeiten:**
1. Aufgabe "Alte Bezeichnung" hinzufügen
2. Auf deren "Edit"-Button klicken
3. Neuen Namen "Neue Bezeichnung" eingeben und speichern
4. Prüfen: "Neue Bezeichnung" sichtbar, "Alte Bezeichnung" weg

**Test B – Löschen:**
1. Aufgabe "Zu löschende Aufgabe" hinzufügen
2. "Delete" klicken
3. Prüfen: Aufgabe nicht mehr vorhanden

> **🎬 Codegen-Tipp:** Zeichne die Edit-Sequenz mit Codegen auf. Schau dir an, welchen Locator Codegen für den "Edit"-Button erzeugt – Playwright erkennt den Kontext automatisch.

<details>
<summary>💡 Lösungshinweis TypeScript</summary>

```typescript
import { test, expect } from "@playwright/test";

test.use({
  geolocation: { latitude: 48.1372, longitude: 11.5755 },
  permissions: ["geolocation"],
});

async function addTask(page: import("@playwright/test").Page, name: string) {
  await page.locator("#new-todo-input").fill(name);
  await page.locator("#myUniqueID").click();
  await expect(page.getByRole("list").getByText(name)).toBeVisible();
}

test("edit a task", async ({ page }) => {
  await page.goto("/");
  await addTask(page, "Alte Bezeichnung");

  // Gezielt den Edit-Button im richtigen ListItem finden
  const taskItem = page
    .getByRole("listitem")
    .filter({ hasText: "Alte Bezeichnung" });
  await taskItem.getByRole("button", { name: "Edit" }).click();

  await taskItem.getByRole("textbox").fill("Neue Bezeichnung");
  await taskItem.getByRole("button", { name: "Save" }).click();

  await expect(page.getByRole("list").getByText("Neue Bezeichnung")).toBeVisible();
  await expect(page.getByRole("list").getByText("Alte Bezeichnung")).not.toBeVisible();
});

test("delete a task", async ({ page }) => {
  await page.goto("/");
  await addTask(page, "Zu löschende Aufgabe");

  const taskItem = page
    .getByRole("listitem")
    .filter({ hasText: "Zu löschende Aufgabe" });
  await taskItem.getByRole("button", { name: "Delete" }).click();

  await expect(
    page.getByRole("list").getByText("Zu löschende Aufgabe")
  ).not.toBeVisible();
});
```

</details>

<details>
<summary>💡 Lösungshinweis C#</summary>

```csharp
[TestClass]
public class TaskManagementTests : PageTest
{
    public override BrowserNewContextOptions ContextOptions() =>
        new()
        {
            BaseURL = "http://localhost:3000",
            Geolocation = new Geolocation { Latitude = 48.1372f, Longitude = 11.5755f },
            Permissions = new[] { "geolocation" },
        };

    private async Task AddTaskAsync(string name)
    {
        await Page.Locator("#new-todo-input").FillAsync(name);
        await Page.Locator("#myUniqueID").ClickAsync();
        await Expect(Page.GetByRole(AriaRole.List).GetByText(name)).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task EditTask()
    {
        await Page.GotoAsync("/");
        await AddTaskAsync("Alte Bezeichnung");

        // Locator-Chaining: ListItem filtern, dann Edit-Button darin finden
        var taskItem = Page.GetByRole(AriaRole.Listitem)
            .Filter(new() { HasText = "Alte Bezeichnung" });
        await taskItem.GetByRole(AriaRole.Button, new() { Name = "Edit" }).ClickAsync();

        await taskItem.GetByRole(AriaRole.Textbox).FillAsync("Neue Bezeichnung");
        await taskItem.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.List).GetByText("Neue Bezeichnung"))
            .ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.List).GetByText("Alte Bezeichnung"))
            .Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task DeleteTask()
    {
        await Page.GotoAsync("/");
        await AddTaskAsync("Zu löschende Aufgabe");

        var taskItem = Page.GetByRole(AriaRole.Listitem)
            .Filter(new() { HasText = "Zu löschende Aufgabe" });
        await taskItem.GetByRole(AriaRole.Button, new() { Name = "Delete" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.List).GetByText("Zu löschende Aufgabe"))
            .Not.ToBeVisibleAsync();
    }
}
```

**Schlüsselkonzept – `.Filter()` / `.filter()`:**  
Schränkt einen breiten Locator auf Elemente ein, die bestimmten Text enthalten. Das C#-Äquivalent ist `.Filter(new() { HasText = "..." })` – exakt das Muster aus den [PlaywrightDemos DDC 2024](https://github.com/norschel/PlaywrightDemos/blob/main/PlaywrightDemos/PlaywrightE2ETests_DDC2024.cs).

</details>

---

## Exercise 4: Filter-Funktionalität testen

**Ziel:** Zustandsabhängige UI-Tests – prüfe, dass All/Active/Completed die Liste korrekt filtern.

**Aufgabe:**

1. Drei Aufgaben hinzufügen: "Task A", "Task B", "Task C"
2. "Task A" als abgeschlossen markieren
3. **Completed**-Filter: Nur "Task A" sichtbar
4. **Active**-Filter: Nur "Task B" und "Task C" sichtbar
5. **All**: Alle drei sichtbar
6. `aria-pressed`-Attribut des aktiven Filter-Buttons prüfen

> **🔍 Inspector-Tipp:** Setze `await page.pause()` nach dem Aktivieren des Completed-Filters. Prüfe im Locator Explorer (Pick Locator), ob du `getByTestId("testID-Completed")` im DOM findest. Versuche auch `$$('[data-testid]')` in der Browser-Konsole (DevTools).

<details>
<summary>💡 Lösungshinweis TypeScript</summary>

```typescript
import { test, expect } from "@playwright/test";

test.use({
  geolocation: { latitude: 48.1372, longitude: 11.5755 },
  permissions: ["geolocation"],
});

async function addTask(page: import("@playwright/test").Page, name: string) {
  await page.locator("#new-todo-input").fill(name);
  await page.locator("#myUniqueID").click();
  await expect(page.getByRole("list").getByText(name)).toBeVisible();
}

test("filter buttons work correctly", async ({ page }) => {
  await page.goto("/");

  await addTask(page, "Task A");
  await addTask(page, "Task B");
  await addTask(page, "Task C");

  // Task A abschließen
  await page
    .getByRole("listitem")
    .filter({ hasText: "Task A" })
    .getByRole("checkbox")
    .check();

  // Completed-Filter
  await page.getByTestId("testID-Completed").click();
  await expect(page.getByTestId("testID-Completed")).toHaveAttribute(
    "aria-pressed", "true"
  );
  await expect(page.getByRole("list").getByText("Task A")).toBeVisible();
  await expect(page.getByRole("list").getByText("Task B")).not.toBeVisible();
  await expect(page.getByRole("list").getByText("Task C")).not.toBeVisible();

  // Active-Filter
  await page.getByTestId("testID-Active").click();
  await expect(page.getByRole("list").getByText("Task B")).toBeVisible();
  await expect(page.getByRole("list").getByText("Task C")).toBeVisible();
  await expect(page.getByRole("list").getByText("Task A")).not.toBeVisible();

  // Alle
  await page.getByTestId("testID-All").click();
  for (const name of ["Task A", "Task B", "Task C"]) {
    await expect(page.getByRole("list").getByText(name)).toBeVisible();
  }
});
```

</details>

<details>
<summary>💡 Lösungshinweis C#</summary>

```csharp
[TestClass]
public class FilterTests : PageTest
{
    public override BrowserNewContextOptions ContextOptions() =>
        new()
        {
            BaseURL = "http://localhost:3000",
            Geolocation = new Geolocation { Latitude = 48.1372f, Longitude = 11.5755f },
            Permissions = new[] { "geolocation" },
        };

    private async Task AddTaskAsync(string name)
    {
        await Page.Locator("#new-todo-input").FillAsync(name);
        await Page.Locator("#myUniqueID").ClickAsync();
        await Expect(Page.GetByRole(AriaRole.List).GetByText(name)).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task FilterButtonsWorkCorrectly()
    {
        await Page.GotoAsync("/");

        await AddTaskAsync("Task A");
        await AddTaskAsync("Task B");
        await AddTaskAsync("Task C");

        // Task A abschließen
        await Page.GetByRole(AriaRole.Listitem)
            .Filter(new() { HasText = "Task A" })
            .GetByRole(AriaRole.Checkbox)
            .CheckAsync();

        // Completed-Filter
        await Page.GetByTestId("testID-Completed").ClickAsync();
        await Expect(Page.GetByTestId("testID-Completed"))
            .ToHaveAttributeAsync("aria-pressed", "true");
        await Expect(Page.GetByRole(AriaRole.List).GetByText("Task A")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.List).GetByText("Task B")).Not.ToBeVisibleAsync();

        // Active-Filter
        await Page.GetByTestId("testID-Active").ClickAsync();
        await Expect(Page.GetByRole(AriaRole.List).GetByText("Task B")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.List).GetByText("Task A")).Not.ToBeVisibleAsync();

        // Alle
        await Page.GetByTestId("testID-All").ClickAsync();
        foreach (var name in new[] { "Task A", "Task B", "Task C" })
        {
            await Expect(Page.GetByRole(AriaRole.List).GetByText(name)).ToBeVisibleAsync();
        }
    }
}
```

</details>

---

## Exercise 5: Netzwerk-Mocking – Remote-Tasks abfangen

**Ziel:** `page.route()` / `Page.RouteAsync()` einsetzen, um HTTP-Anfragen abzufangen und durch Testdaten zu ersetzen.

**Hintergrund:** Der "Load remote tasks"-Button ruft `fetch("remoteTasks.json")` auf.

**Aufgabe:**

**Test A:** Mock mit zwei definierten Testaufgaben – prüfe, dass nur diese erscheinen.  
**Test B:** HTTP 500 simulieren – prüfe, dass die App korrekt reagiert.  
**Test C:** 2-Sekunden-Verzögerung simulieren – prüfe, dass Aufgaben nach dem Warten erscheinen.

> **📊 Trace-Tipp:** Aktiviere `trace: "on"` und öffne den Trace nach dem Test. Im **Network**-Tab siehst du genau, welche Route abgefangen wurde und welchen Response du zurückgeliefert hast.

<details>
<summary>💡 Lösungshinweis TypeScript</summary>

```typescript
import { test, expect } from "@playwright/test";

const mockTasks = [
  {
    id: "todo-mock-1",
    name: "Gemockte Aufgabe 1",
    time: "2024-01-01T10:00:00.000Z",
    location: { latitude: 48.1372, longitude: 11.5755 },
    completed: false,
  },
  {
    id: "todo-mock-2",
    name: "Gemockte Aufgabe 2",
    time: "2024-01-01T11:00:00.000Z",
    location: { latitude: 52.52, longitude: 13.405 },
    completed: true,
  },
];

test("load remote tasks - success", async ({ page }) => {
  // Route VOR goto registrieren!
  await page.route("**/remoteTasks.json", (route) =>
    route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify(mockTasks),
    })
  );

  await page.goto("/");
  await page.getByRole("button", { name: "Load remote tasks" }).click();

  await expect(page.getByRole("list").getByText("Gemockte Aufgabe 1")).toBeVisible();
  await expect(page.getByRole("list").getByText("Gemockte Aufgabe 2")).toBeVisible();
});

test("load remote tasks - server error", async ({ page }) => {
  await page.route("**/remoteTasks.json", (route) =>
    route.fulfill({ status: 500, body: "Internal Server Error" })
  );

  await page.goto("/");
  await page.getByRole("button", { name: "Load remote tasks" }).click();

  await expect(page.getByRole("list").getByText("Gemockte")).not.toBeVisible();
});

test("load remote tasks - slow network", async ({ page }) => {
  await page.route("**/remoteTasks.json", async (route) => {
    await new Promise((r) => setTimeout(r, 2000));
    await route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify(mockTasks),
    });
  });

  await page.goto("/");
  await page.getByRole("button", { name: "Load remote tasks" }).click();
  await expect(
    page.getByRole("list").getByText("Gemockte Aufgabe 1")
  ).toBeVisible({ timeout: 5000 });
});
```

</details>

<details>
<summary>💡 Lösungshinweis C#</summary>

```csharp
[TestClass]
public class NetworkMockTests : PageTest
{
    private static readonly string MockTasksJson = JsonSerializer.Serialize(new[]
    {
        new { id = "todo-mock-1", name = "Gemockte Aufgabe 1",
              time = "2024-01-01T10:00:00.000Z",
              location = new { latitude = 48.1372, longitude = 11.5755 },
              completed = false },
        new { id = "todo-mock-2", name = "Gemockte Aufgabe 2",
              time = "2024-01-01T11:00:00.000Z",
              location = new { latitude = 52.52, longitude = 13.405 },
              completed = true },
    });

    [TestMethod]
    public async Task LoadRemoteTasks_Success()
    {
        await Page.RouteAsync("**/remoteTasks.json", route => route.FulfillAsync(
            new RouteFulfillOptions
            {
                Status = 200,
                ContentType = "application/json",
                Body = MockTasksJson,
            }));

        await Page.GotoAsync("http://localhost:3000");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Load remote tasks" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.List).GetByText("Gemockte Aufgabe 1"))
            .ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.List).GetByText("Gemockte Aufgabe 2"))
            .ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task LoadRemoteTasks_ServerError()
    {
        await Page.RouteAsync("**/remoteTasks.json", route => route.FulfillAsync(
            new RouteFulfillOptions { Status = 500, Body = "Internal Server Error" }));

        await Page.GotoAsync("http://localhost:3000");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Load remote tasks" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.List).GetByText("Gemockte"))
            .Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task LoadRemoteTasks_SlowNetwork()
    {
        await Page.RouteAsync("**/remoteTasks.json", async route =>
        {
            await Task.Delay(2000);
            await route.FulfillAsync(new RouteFulfillOptions
            {
                Status = 200,
                ContentType = "application/json",
                Body = MockTasksJson,
            });
        });

        await Page.GotoAsync("http://localhost:3000");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Load remote tasks" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.List).GetByText("Gemockte Aufgabe 1"))
            .ToBeVisibleAsync(new() { Timeout = 5000 });
    }
}
```

</details>

---

## Exercise 6: Response-Manipulation – Logo durch Testbild ersetzen

**Ziel:** Das fortgeschrittene Route-Pattern: Echten Request abschicken, dann nur den Body ersetzen. Inspiriert vom "Holiday Theme"-Demo der [norschel/PlaywrightDemos](https://github.com/norschel/PlaywrightDemos/blob/main/PlaywrightDemos/PlaywrightE2ETests_IT_Tage_2025.cs).

**Aufgabe:**

1. Erstelle ein einfaches Test-PNG (z. B. 150×50 px) unter `tests/fixtures/test-logo.png`
2. Schreibe einen Test, der `**/getsitelogo.png` abfängt und durch dein Test-Logo ersetzt
3. Prüfe, dass das `<img alt="Site Logo">` noch sichtbar ist
4. **Bonusaufgabe:** Nutze `route.fetch()` / `route.FetchAsync()`, um den echten Request durchzulassen, aber nur den Body zu ersetzen

> **📊 Trace-Tipp:** Im Trace Viewer siehst du im Network-Tab, welche Route für `getsitelogo.png` ausgelöst wurde und welcher Response zurückkam. Vergleiche Original vs. Mock.

<details>
<summary>💡 Lösungshinweis TypeScript</summary>

```typescript
import { test, expect } from "@playwright/test";
import { readFileSync } from "fs";
import { join } from "path";

test("replace logo with test image", async ({ page }) => {
  const logoBytes = readFileSync(
    join(__dirname, "fixtures", "test-logo.png")
  );

  await page.route("**/getsitelogo.png", (route) =>
    route.fulfill({
      status: 200,
      contentType: "image/png",
      body: logoBytes,
    })
  );

  await page.goto("/");
  await expect(page.getByRole("img", { name: "Site Logo" })).toBeVisible();
});

// Fortgeschrittene Variante: Echter Request + Body-Ersatz
test("replace logo body after real fetch", async ({ page }) => {
  const logoBytes = readFileSync(
    join(__dirname, "fixtures", "test-logo.png")
  );

  await page.route("**/getsitelogo.png", async (route) => {
    const response = await route.fetch(); // echten Request abschicken
    await route.fulfill({
      response,      // originale Headers übernehmen
      body: logoBytes, // nur Body ersetzen
    });
  });

  await page.goto("/");
  await expect(page.getByRole("img", { name: "Site Logo" })).toBeVisible();
});
```

</details>

<details>
<summary>💡 Lösungshinweis C#</summary>

```csharp
[TestClass]
public class ImageMockTests : PageTest
{
    [TestMethod]
    public async Task ReplaceLogo()
    {
        var logoBytes = await File.ReadAllBytesAsync("tests/fixtures/test-logo.png");

        await Page.RouteAsync("**/getsitelogo.png", route => route.FulfillAsync(
            new RouteFulfillOptions
            {
                Status = 200,
                ContentType = "image/png",
                BodyBytes = logoBytes,
            }));

        await Page.GotoAsync("http://localhost:3000");
        await Expect(Page.GetByRole(AriaRole.Img, new() { Name = "Site Logo" }))
            .ToBeVisibleAsync();
    }

    // Fortgeschrittene Variante: Echter Request + Body-Ersatz
    [TestMethod]
    public async Task ReplaceLogoBodyAfterRealFetch()
    {
        var logoBytes = await File.ReadAllBytesAsync("tests/fixtures/test-logo.png");

        await Page.RouteAsync("**/getsitelogo.png", async route =>
        {
            var response = await route.FetchAsync(); // echten Request abschicken
            await route.FulfillAsync(new RouteFulfillOptions
            {
                Response = response,     // originale Headers übernehmen
                BodyBytes = logoBytes,   // nur Body ersetzen
                Headers = new Dictionary<string, string>(response.Headers)
                {
                    ["Content-Type"] = "image/png",
                },
            });
        });

        await Page.GotoAsync("http://localhost:3000");
        await Expect(Page.GetByRole(AriaRole.Img, new() { Name = "Site Logo" }))
            .ToBeVisibleAsync();
    }
}
```

**Verbindung zu PlaywrightDemos:**  
Dieses `FetchAsync()`-Pattern ist die Kernidee des "Santa Hat"-Demos aus dem [IT-Tage 2025](https://github.com/norschel/PlaywrightDemos/blob/main/PlaywrightDemos/PlaywrightE2ETests_IT_Tage_2025.cs): Speaker-Fotos werden durch Weihnachtsmützen-Bilder ersetzt, ohne den echten HTTP-Aufruf zu verhindern.

</details>

---

## Exercise 7: Screenshots, Video und Traces

**Ziel:** Die drei wichtigsten Diagnosewerkzeuge von Playwright aktiv einsetzen.

### Teil A: Screenshot

**Aufgabe:** Konfiguriere die Config für automatische Screenshots bei Fehlern. Schreibe außerdem einen Test, der manuell einen Full-Page-Screenshot aufnimmt.

<details>
<summary>💡 Lösungshinweis TypeScript</summary>

`playwright.config.ts`:
```typescript
use: { screenshot: "only-on-failure" }
```

Manuell im Test:
```typescript
test("screenshot on demand", async ({ page }) => {
  await page.goto("/");
  await page.screenshot({ path: "tests/screenshots/app-state.png", fullPage: true });
});
```

</details>

<details>
<summary>💡 Lösungshinweis C#</summary>

```csharp
[TestMethod]
public async Task TakeScreenshot()
{
    await Page.GotoAsync("http://localhost:3000");
    await Page.ScreenshotAsync(new PageScreenshotOptions
    {
        Path = "screenshots/app-state.png",
        FullPage = true,
    });
}
```

Automatisch bei Fehler (in `playwright.config.json`):
```json
{ "use": { "screenshot": "only-on-failure" } }
```

</details>

### Teil B: Video-Aufnahme

**Aufgabe:** Aktiviere Video-Aufnahme für einen Test und finde die `.webm`-Datei in `test-results/`.

<details>
<summary>💡 Lösungshinweis TypeScript</summary>

```typescript
test.use({ video: "on" });

test("record task workflow", async ({ page }) => {
  await page.goto("/");
  // ... Interaktionen
});
```

Oder global in `playwright.config.ts`:
```typescript
use: { video: "retain-on-failure" }
```

</details>

<details>
<summary>💡 Lösungshinweis C#</summary>

Per `playwright.config.json`:
```json
{ "use": { "video": "retain-on-failure" } }
```

Oder per Umgebungsvariable vor dem Test:
```powershell
$env:PLAYWRIGHT_VIDEO = "on"; dotnet test
```

</details>

### Teil C: Trace Viewer

**Aufgabe:** Aktiviere Traces, führe einen Test aus und öffne den Trace Viewer. Navigiere durch DOM-Snapshots und Network-Tab.

<details>
<summary>💡 Lösungshinweis TypeScript</summary>

Manuell im Test steuern:
```typescript
test("manual trace", async ({ page, context }) => {
  await context.tracing.start({ screenshots: true, snapshots: true, sources: true });
  await page.goto("/");
  // ... Testschritte ...
  await context.tracing.stop({ path: "tests/traces/my-trace.zip" });
});
```

Trace öffnen:
```bash
npx playwright show-trace tests/traces/my-trace.zip
```

In **VS Code**: Nach fehlgeschlagenem Test → "Show Trace"-Link in der Testing-Seitenleiste.

</details>

<details>
<summary>💡 Lösungshinweis C#</summary>

```csharp
[TestMethod]
public async Task TraceTest()
{
    await Context.Tracing.StartAsync(new TracingStartOptions
    {
        Screenshots = true,
        Snapshots = true,
        Sources = true,
    });

    await Page.GotoAsync("http://localhost:3000");
    // ... Testschritte ...

    await Context.Tracing.StopAsync(new TracingStopOptions
    {
        Path = "traces/my-trace.zip",
    });
}
```

Trace öffnen:
```powershell
pwsh bin/Debug/net8.0/playwright.ps1 show-trace traces/my-trace.zip
```

In **Visual Studio**: Trace-Datei im Test-Ergebnis-Fenster als Attachment anhängen:
```csharp
TestContext.AddResultFile("traces/my-trace.zip");
```

**Aus PlaywrightDemos:** Tracing wurde ab [BASTA! 2024](https://github.com/norschel/PlaywrightDemos/blob/main/PlaywrightDemos/PlaywrightE2ETests_Basta2024.cs) zum Standard-Debugging-Werkzeug – unverzichtbar in CI/CD, wo kein lokaler Browser verfügbar ist.

</details>

---

## Exercise 8: Mobile Device Emulation

**Ziel:** Die App auf Mobilgeräten testen – Viewport, User-Agent, Touch-Events und Pixel-Ratio werden automatisch gesetzt.

**Aufgabe:**

1. Schreibe einen Test mit iPhone 15 Pro Emulation
2. Prüfe alle Hauptelemente auf dem mobilen Viewport
3. Mache einen Screenshot als Nachweis
4. **Bonus:** Vergleiche iPhone (Portrait) mit iPad (Landscape)

<details>
<summary>💡 Lösungshinweis TypeScript</summary>

```typescript
import { test, expect, devices } from "@playwright/test";

test.use({
  ...devices["iPhone 15 Pro"],
  geolocation: { latitude: 48.1372, longitude: 11.5755 },
  permissions: ["geolocation"],
});

test("app works on mobile", async ({ page }) => {
  await page.goto("/");

  await expect(page.getByRole("heading", { name: "TodoMatic" })).toBeVisible();
  await expect(page.locator("#new-todo-input")).toBeVisible();

  await page.locator("#new-todo-input").fill("Mobile Task");
  await page.locator("#myUniqueID").click();
  await expect(page.getByRole("list").getByText("Mobile Task")).toBeVisible();

  await page.screenshot({ path: "tests/screenshots/mobile-iphone15.png", fullPage: true });
});
```

Alle Gerätedefinitionen anzeigen:
```typescript
import { devices } from "@playwright/test";
console.log(Object.keys(devices));
```

</details>

<details>
<summary>💡 Lösungshinweis C#</summary>

```csharp
[TestClass]
public class MobileTests : PageTest
{
    public override BrowserNewContextOptions ContextOptions()
    {
        // iPhone 15 Pro Geräteprofil
        var iPhone = Playwright.Devices["iPhone 15 Pro"];
        return new BrowserNewContextOptions(iPhone)
        {
            BaseURL = "http://localhost:3000",
            Geolocation = new Geolocation { Latitude = 48.1372f, Longitude = 11.5755f },
            Permissions = new[] { "geolocation" },
        };
    }

    [TestMethod]
    public async Task AppWorksOnMobile()
    {
        await Page.GotoAsync("/");

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "TodoMatic" }))
            .ToBeVisibleAsync();
        await Expect(Page.Locator("#new-todo-input")).ToBeVisibleAsync();

        await Page.Locator("#new-todo-input").FillAsync("Mobile Task");
        await Page.Locator("#myUniqueID").ClickAsync();
        await Expect(Page.GetByRole(AriaRole.List).GetByText("Mobile Task"))
            .ToBeVisibleAsync();

        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "screenshots/mobile-iphone15.png",
            FullPage = true,
        });
    }
}
```

**Alle Geräteprofile anzeigen:**
```csharp
foreach (var device in Playwright.Devices.Keys)
    Console.WriteLine(device);
```

**Aus PlaywrightDemos:** Mobile-Emulation + Video-Aufnahme ist seit [WDC 2023](https://github.com/norschel/PlaywrightDemos/blob/main/PlaywrightDemos/PlaywrightE2ETests_WDC2023.cs) ein fester Bestandteil der Demos.

</details>

---

## Exercise 9: Cross-Browser Testing

**Ziel:** Tests parallel in Chromium, Firefox und WebKit (Safari) ausführen.

**Aufgabe:**

1. Erweitere die Config um Firefox und WebKit
2. Führe alle Tests aus und beobachte die parallele Ausführung
3. Schreibe einen Test, der den Browser-Namen in der Ausgabe zeigt
4. **Bonus:** Füge Edge und Chrome als benannte Kanäle hinzu

<details>
<summary>💡 Lösungshinweis TypeScript</summary>

`playwright.config.ts`:
```typescript
projects: [
  { name: "chromium", use: { ...devices["Desktop Chrome"] } },
  { name: "firefox",  use: { ...devices["Desktop Firefox"] } },
  { name: "webkit",   use: { ...devices["Desktop Safari"] } },
  { name: "edge",     use: { ...devices["Desktop Edge"] } },
  { name: "mobile-chrome", use: { ...devices["Pixel 7"] } },
  { name: "mobile-safari", use: { ...devices["iPhone 15"] } },
],
```

Bestimmten Browser testen:
```bash
npx playwright test --project=firefox
npx playwright test --project=webkit
```

Browser-Name im Test:
```typescript
test("check browser", async ({ page, browserName }) => {
  console.log(`Läuft auf: ${browserName}`);
  await page.goto("/");
  await expect(page.getByRole("heading", { name: "TodoMatic" })).toBeVisible();
});
```

</details>

<details>
<summary>💡 Lösungshinweis C#</summary>

```csharp
// Parametrisierter Cross-Browser-Test – wie in PlaywrightDemos
[TestClass]
public class CrossBrowserTests
{
    [TestMethod]
    [DataRow("Chromium")]
    [DataRow("Firefox")]
    [DataRow("Webkit")]
    [DataRow("Edge")]
    [DataRow("Chrome")]
    public async Task AppLoadsInAllBrowsers(string browserName)
    {
        using var playwright = await Playwright.CreateAsync();

        var options = new BrowserTypeLaunchOptions { Headless = true };
        IBrowser browser = browserName switch
        {
            "Chromium" => await playwright.Chromium.LaunchAsync(options),
            "Firefox"  => await playwright.Firefox.LaunchAsync(options),
            "Webkit"   => await playwright.Webkit.LaunchAsync(options),
            "Edge"     => await playwright.Chromium.LaunchAsync(
                              options with { Channel = "msedge" }),
            "Chrome"   => await playwright.Chromium.LaunchAsync(
                              options with { Channel = "chrome" }),
            _          => throw new ArgumentException($"Unknown browser: {browserName}")
        };

        var page = await browser.NewPageAsync();
        await page.GotoAsync("http://localhost:3000");

        var title = await page.TitleAsync();
        StringAssert.Contains(title, "TodoMatic");

        Console.WriteLine($"✅ {browserName}: {title}");
        await browser.CloseAsync();
    }
}
```

**Direkt aus PlaywrightDemos:** Dieses `[DataRow]`-Pattern für Cross-Browser-Tests ist das Kernmuster aus [PlaywrightE2ETests_Basta2023.cs](https://github.com/norschel/PlaywrightDemos/blob/main/PlaywrightDemos/PlaywrightE2ETests_Basta2023.cs) und wird in allen späteren Konferenz-Demos verwendet.

```bash
# Bestimmten Browser testen
dotnet test --filter "TestCategory=firefox"
```

</details>

---

## Exercise 10: JavaScript in die Seite injizieren mit `page.evaluate()`

**Ziel:** Das fortgeschrittenste Feature – JavaScript direkt im Browser-Kontext ausführen. Inspiriert vom spektakulären Canvas-Overlay-Demo aus den [PlaywrightDemos BASTA! Spring 2026](https://github.com/norschel/PlaywrightDemos/blob/main/PlaywrightDemos/PlaywrightE2ETests_BastaSpring2026.cs).

**Aufgabe:**

**Teil A:** Lies alle aktuellen Task-Namen per `evaluate()` direkt aus dem DOM aus.  
**Teil B:** Injiziere ein rotes "TESTMODUS"-Banner in die Seite und prüfe es per Locator.  
**Teil C (Bonus):** Erstelle einen animierten Canvas-Overlay, mache einen Screenshot.

> **🌐 DevTools-Tipp:** Öffne Chrome DevTools (F12) in einem laufenden Test (mit `headless: false`). Tippe in der Konsole `$$('h4')` oder `document.querySelectorAll('[data-testid]')`, um Locatoren vor dem Test zu erkunden. Das spart Iterationen.

<details>
<summary>💡 Lösungshinweis TypeScript</summary>

```typescript
import { test, expect } from "@playwright/test";

test.use({
  geolocation: { latitude: 48.1372, longitude: 11.5755 },
  permissions: ["geolocation"],
  headless: false, // für DevTools-Debugging
});

test("read task names via evaluate", async ({ page }) => {
  await page.goto("/");

  const taskNames = await page.evaluate(() => {
    const headings = document.querySelectorAll('[role="list"] h4');
    return Array.from(headings).map((h) => h.textContent?.trim());
  });
  console.log("Aufgaben im DOM:", taskNames);
});

test("inject test mode banner", async ({ page }) => {
  await page.goto("/");

  await page.evaluate(() => {
    const banner = document.createElement("div");
    banner.id = "test-mode-banner";
    banner.textContent = "⚠️ TESTMODUS AKTIV";
    Object.assign(banner.style, {
      position: "fixed",
      top: "0",
      left: "0",
      right: "0",
      backgroundColor: "red",
      color: "white",
      textAlign: "center",
      padding: "8px",
      zIndex: "99999",
      fontSize: "18px",
      fontWeight: "bold",
    });
    document.body.prepend(banner);
  });

  await expect(page.locator("#test-mode-banner")).toBeVisible();
  await page.screenshot({ path: "tests/screenshots/test-mode.png" });
});

test("canvas overlay (PlaywrightDemos-inspired)", async ({ page }) => {
  await page.goto("/");

  await page.evaluate(() => {
    const canvas = document.createElement("canvas");
    canvas.width = 420;
    canvas.height = 90;
    Object.assign(canvas.style, {
      position: "fixed",
      bottom: "20px",
      right: "20px",
      zIndex: "9999",
      borderRadius: "10px",
      backgroundColor: "rgba(0,0,0,0.75)",
    });
    document.body.appendChild(canvas);

    const ctx = canvas.getContext("2d")!;
    ctx.fillStyle = "#fff";
    ctx.font = "bold 22px Arial";
    ctx.textAlign = "center";
    ctx.fillText("🎭 Playwright Demo", 210, 40);
    ctx.font = "13px Arial";
    ctx.fillStyle = "#aaa";
    ctx.fillText("norschel/PlaywrightDemos inspired", 210, 68);
  });

  await page.screenshot({ path: "tests/screenshots/canvas-overlay.png", fullPage: true });
});
```

</details>

<details>
<summary>💡 Lösungshinweis C#</summary>

```csharp
[TestClass]
public class EvaluateTests : PageTest
{
    [TestMethod]
    public async Task ReadTaskNamesViaEvaluate()
    {
        await Page.GotoAsync("http://localhost:3000");

        var taskNames = await Page.EvaluateAsync<string[]>(@"
            () => {
                const headings = document.querySelectorAll('[role=""list""] h4');
                return Array.from(headings).map(h => h.textContent?.trim());
            }
        ");
        Console.WriteLine($"Aufgaben: {string.Join(", ", taskNames)}");
    }

    [TestMethod]
    public async Task InjectTestModeBanner()
    {
        await Page.GotoAsync("http://localhost:3000");

        await Page.EvaluateAsync(@"
            () => {
                const banner = document.createElement('div');
                banner.id = 'test-mode-banner';
                banner.textContent = '⚠️ TESTMODUS AKTIV';
                Object.assign(banner.style, {
                    position: 'fixed', top: '0', left: '0', right: '0',
                    backgroundColor: 'red', color: 'white',
                    textAlign: 'center', padding: '8px',
                    zIndex: '99999', fontSize: '18px', fontWeight: 'bold'
                });
                document.body.prepend(banner);
            }
        ");

        await Expect(Page.Locator("#test-mode-banner")).ToBeVisibleAsync();
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "screenshots/test-mode.png",
        });
    }

    // Canvas-Overlay – analog zum BASTA! Spring 2026 Osterhasen-Demo
    [TestMethod]
    public async Task CanvasOverlay()
    {
        await Page.GotoAsync("http://localhost:3000");

        await Page.EvaluateAsync(@"
            () => {
                const canvas = document.createElement('canvas');
                canvas.width = 420; canvas.height = 90;
                Object.assign(canvas.style, {
                    position: 'fixed', bottom: '20px', right: '20px',
                    zIndex: '9999', borderRadius: '10px',
                    backgroundColor: 'rgba(0,0,0,0.75)'
                });
                document.body.appendChild(canvas);
                const ctx = canvas.getContext('2d');
                ctx.fillStyle = '#fff';
                ctx.font = 'bold 22px Arial';
                ctx.textAlign = 'center';
                ctx.fillText('🎭 Playwright Demo', 210, 40);
                ctx.font = '13px Arial';
                ctx.fillStyle = '#aaa';
                ctx.fillText('norschel/PlaywrightDemos inspired', 210, 68);
            }
        ");

        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "screenshots/canvas-overlay.png",
            FullPage = true,
        });
    }
}
```

**Direkt aus PlaywrightDemos:**  
Der [BASTA! Spring 2026 Demo](https://github.com/norschel/PlaywrightDemos/blob/main/PlaywrightDemos/PlaywrightE2ETests_BastaSpring2026.cs) injiziert per `EvaluateAsync` einen animierten Osterhasen (gezeichnet mit Canvas 2D API) und spielt "Häschen in der Grube" über die Web Audio API ab – alles live auf einer echten Konferenz-Website während des Vortrags.

</details>

---

## Exercise 11: CI/CD mit GitHub Actions

**Ziel:** Playwright-Tests in einer GitHub Actions-Pipeline automatisieren.

**Aufgabe:**

Erstelle `.github/workflows/playwright.yml` (TypeScript) bzw. `.github/workflows/playwright-dotnet.yml` (C#):

1. Trigger: Push und Pull Request auf `main`
2. Installiere Dependencies und Playwright-Browser
3. Führe Tests aus
4. Speichere HTML-Report und Artefakte (Screenshots, Videos, Traces)

<details>
<summary>💡 Lösungshinweis TypeScript (GitHub Actions)</summary>

```yaml
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

      - name: Setup Node.js
        uses: actions/setup-node@v4
        with:
          node-version: 20
          cache: "npm"

      - name: Install dependencies
        run: npm ci

      - name: Install Playwright browsers
        run: npx playwright install --with-deps

      - name: Run Playwright tests
        run: npx playwright test

      - name: Upload HTML Report
        uses: actions/upload-artifact@v4
        if: always()
        with:
          name: playwright-report
          path: playwright-report/
          retention-days: 14

      - name: Upload test artifacts (nur bei Fehler)
        uses: actions/upload-artifact@v4
        if: failure()
        with:
          name: test-results
          path: test-results/
          retention-days: 7
```

</details>

<details>
<summary>💡 Lösungshinweis C# / .NET (GitHub Actions)</summary>

```yaml
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

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 8.x

      - name: Setup Node.js (für die Todo-App)
        uses: actions/setup-node@v4
        with:
          node-version: 20

      - name: Install and start Todo App
        run: |
          npm ci
          npm run build
          npx serve dist -p 3000 &
        working-directory: ./todo-react-playwright

      - name: Build test project
        run: dotnet build TodoPlaywrightTests/

      - name: Install Playwright browsers
        run: pwsh TodoPlaywrightTests/bin/Debug/net8.0/playwright.ps1 install --with-deps

      - name: Run Playwright tests
        run: dotnet test TodoPlaywrightTests/ --logger trx --results-directory TestResults/
        env:
          PLAYWRIGHT_BASE_URL: http://localhost:3000

      - name: Upload TRX results
        uses: actions/upload-artifact@v4
        if: always()
        with:
          name: test-results-dotnet
          path: TestResults/
          retention-days: 14

      - name: Upload Playwright artifacts (Traces, Screenshots)
        uses: actions/upload-artifact@v4
        if: failure()
        with:
          name: playwright-artifacts
          path: |
            **/traces/
            **/screenshots/
            **/*.webm
          retention-days: 7
```

**Aus PlaywrightDemos:**  
Das [dotnet.yml](https://github.com/norschel/PlaywrightDemos/blob/main/.github/workflows/dotnet.yml) aus den PlaywrightDemos lädt `.webm`, `.png` und `.zip`-Traces als CI-Artefakte hoch und nutzt das `[TestCategory("CICD")]`-Tag, um gezielt nur produktionsreife Tests in der Pipeline auszuführen.

</details>

---

## Zusammenfassung: Gelerntes auf einen Blick

| Konzept | TypeScript API | C# API | Übung |
|---|---|---|---|
| Navigation | `page.goto()` | `Page.GotoAsync()` | 1–11 |
| ARIA-Locatoren | `getByRole()`, `getByTestId()` | `GetByRole()`, `GetByTestId()` | 1, 3, 4 |
| Formular-Interaktion | `fill()`, `click()`, `check()` | `FillAsync()`, `ClickAsync()` | 2, 3 |
| Locator-Chaining | `.filter({ hasText })` | `.Filter(new() { HasText })` | 3, 4 |
| Geolocation mocken | `test.use({ geolocation })` | `ContextOptions()` override | 2, 3, 8 |
| Netzwerk-Mocking | `page.route()` + `fulfill()` | `RouteAsync()` + `FulfillAsync()` | 5, 6 |
| Response-Manipulation | `route.fetch()` | `route.FetchAsync()` | 6 |
| Screenshots | `page.screenshot()` | `Page.ScreenshotAsync()` | 7 |
| Video | `video: "retain-on-failure"` | `playwright.config.json` | 7 |
| Trace Viewer | `show-trace trace.zip` | `playwright.ps1 show-trace` | 7 |
| Mobile Emulation | `devices["iPhone 15 Pro"]` | `Playwright.Devices[...]` | 8 |
| Cross-Browser | `projects` in Config | `[DataRow("Firefox")]` | 9 |
| JS-Injektion | `page.evaluate()` | `Page.EvaluateAsync()` | 10 |
| Codegen | `npx playwright codegen` | `pwsh playwright.ps1 codegen` | alle |
| Inspector | `PWDEBUG=1` / `page.pause()` | `PWDEBUG=1` / `PauseAsync()` | alle |
| CI/CD | GitHub Actions YAML | GitHub Actions YAML | 11 |

---

## Weiterführende Ressourcen

- 📖 [Playwright Dokumentation (TypeScript)](https://playwright.dev/docs/intro)
- 📖 [Playwright Dokumentation (.NET/C#)](https://playwright.dev/dotnet/docs/intro)
- 🎭 [norschel/PlaywrightDemos](https://github.com/norschel/PlaywrightDemos) – Konferenz-Demos mit fortgeschrittenen C#-Patterns
- 🛠 [Playwright Trace Viewer](https://playwright.dev/docs/trace-viewer)
- 🎬 [Playwright Codegen](https://playwright.dev/docs/codegen)
- 📱 [Emulierte Geräte-Liste](https://github.com/microsoft/playwright/blob/main/packages/playwright-core/src/server/deviceDescriptorsSource.json)
- 🌐 [Azure Playwright Testing Service](https://azure.microsoft.com/de-de/products/playwright-testing) – Cloud-Browser-Farm (in PlaywrightDemos ab IT-Tage 2025 demonstriert)
- 🔬 [Playwright Inspector & Debugger](https://playwright.dev/docs/debug)
- 📊 [Playwright HTML Reporter](https://playwright.dev/docs/test-reporters#html-reporter)
- 🧩 [Playwright VS Code Extension](https://marketplace.visualstudio.com/items?itemName=ms-playwright.playwright)

---

*HOL erstellt für das [harrybin/todo-react-playwright](https://github.com/harrybin/todo-react-playwright) Repo – basierend auf echten Konferenz-Demos von [Nico Orschel](https://github.com/norschel) (norschel/PlaywrightDemos, BASTA! / MDD / IT-Tage 2023–2026)*
