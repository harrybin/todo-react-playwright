# Hands-on Lab: Playwright End-to-End Testing mit der TodoMatic-App

> **Niveau:** Fortgeschrittene Bootcamp-Übung  
> **Dauer:** ca. 4–5 Stunden  
> **App-Repo:** [harrybin/todo-react-playwright](https://github.com/harrybin/todo-react-playwright)  
> **Referenz-Demos:** [norschel/PlaywrightDemos](https://github.com/norschel/PlaywrightDemos)

---

## Lernziele

Nach dieser HOL kannst du:

- Playwright-Tests in TypeScript/JavaScript **und** C# (.NET) aufsetzen – mit **MSTest, NUnit oder xUnit**
- Tests **code-driven** (bevorzugt) und per **Codegen** erstellen – und beide Ansätze gezielt einsetzen
- Lokatorstrategien (ARIA-Roles, `data-testid`, CSS, Text) gezielt einsetzen
- Browser-APIs wie Geolocation mocken
- Netzwerk-Requests abfangen und manipulieren
- **Page Object Model** für wartbare, wiederverwendbare Tests aufbauen
- **Screenshots, Videos und Traces** als Diagnosewerkzeuge nutzen
- Tests cross-browser und mit Mobile-Emulation ausführen
- JavaScript direkt ins DOM injizieren via `page.evaluate()`
- **Codegen, Trace Viewer, Inspector und Browser DevTools** als Debugging-Werkzeuge einsetzen
- Tests in **GitHub Actions**, **Azure Pipelines** und **Docker** betreiben
- Den **Azure Playwright Testing Service** für Cloud-Ausführung nutzen
- Den **Playwright MCP Server** einrichten und mit GitHub Copilot Agent zur browser-gesteuerten Test-Generierung nutzen

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

### Setup: C# / .NET – Testframework wählen

Du kannst zwischen **MSTest**, **NUnit** und **xUnit** wählen. Alle drei werden von `Microsoft.Playwright` offiziell unterstützt.

| | MSTest | NUnit | xUnit |
|---|---|---|---|
| NuGet-Paket | `Microsoft.Playwright.MSTest` | `Microsoft.Playwright.NUnit` | `Microsoft.Playwright.Xunit` |
| Template | `dotnet new mstest` | `dotnet new nunit` | `dotnet new xunit` |
| Testklasse | `[TestClass]` | `[TestFixture]` | *(kein Attribut)* |
| Testmethode | `[TestMethod]` | `[Test]` | `[Fact]` |
| Parametrisiert | `[DataRow("val")]` | `[TestCase("val")]` | `[Theory]`+`[InlineData("val")]` |
| Setup | `[TestInitialize]` | `[SetUp]` | Konstruktor / `IAsyncLifetime` |
| CI-Tag | `[TestCategory("CICD")]` | `[Category("CICD")]` | `[Trait("Category","CICD")]` |

**Empfehlung:** Wähle das Framework, das in deinem Team bereits genutzt wird. Alle Exercises zeigen MSTest als primäre Lösung; NUnit und xUnit als ausklappbare Alternativen.

**MSTest einrichten:**

```bash
dotnet new mstest -n TodoPlaywrightTests && cd TodoPlaywrightTests
dotnet add package Microsoft.Playwright.MSTest
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install
```

**NUnit einrichten:**

```bash
dotnet new nunit -n TodoPlaywrightTests && cd TodoPlaywrightTests
dotnet add package Microsoft.Playwright.NUnit
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install
```

**xUnit einrichten:**

```bash
dotnet new xunit -n TodoPlaywrightTests && cd TodoPlaywrightTests
dotnet add package Microsoft.Playwright.Xunit
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install
```

**Gemeinsame Basisklasse** (`TestBase.cs`) – einmal definieren, von allen Testklassen erben:

```csharp
// MSTest
using Microsoft.Playwright.MSTest;
[TestClass]
public class TestBase : PageTest
{
    public override BrowserNewContextOptions ContextOptions() => new()
    {
        BaseURL = Environment.GetEnvironmentVariable("PLAYWRIGHT_BASE_URL")
                  ?? "http://localhost:3000",
    };
}
```

```csharp
// NUnit – Attribut und Namespace ändern, Rest identisch
using Microsoft.Playwright.NUnit;
[TestFixture]
public class TestBase : PageTest { /* ContextOptions() wie oben */ }
```

```csharp
// xUnit – kein Klassenattribut
using Microsoft.Playwright.Xunit;
public class TestBase : PageTest { /* ContextOptions() wie oben */ }
```

**`.runsettings` – Browser, Headless-Modus und Timeouts konfigurieren:**

Erstelle `playwright.runsettings` im Projektstamm:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <TestRunParameters>
    <!-- Browser: chromium | firefox | webkit -->
    <Parameter name="playwright:browser" value="chromium" />
    <Parameter name="playwright:headless" value="true" />
    <!-- Timeout je Aktion in ms -->
    <Parameter name="playwright:timeout" value="30000" />
    <!-- Basis-URL der App -->
    <Parameter name="playwright:baseUrl" value="http://localhost:3000" />
    <!-- SlowMo für Debugging (ms zwischen Aktionen) -->
    <!-- <Parameter name="playwright:slowMo" value="500" /> -->
  </TestRunParameters>
  <MSTest>
    <Parallelize>
      <Workers>4</Workers>
      <Scope>ClassLevel</Scope>
    </Parallelize>
  </MSTest>
</RunSettings>
```

```bash
dotnet test --settings playwright.runsettings
```

**Wichtige Umgebungsvariablen:**

```bash
BROWSER=firefox dotnet test           # Browser wechseln
HEADED=1 dotnet test                  # Sichtbarer Modus
PLAYWRIGHT_TRACE=on dotnet test       # Trace immer aufzeichnen
PLAYWRIGHT_BASE_URL=http://localhost:3000 dotnet test
```

> **⚠️ Wichtig:** Starte die TodoMatic-App (`npm run dev`) **manuell**, bevor du C#-Tests ausführst. Im Gegensatz zu TypeScript gibt es für .NET kein eingebautes `webServer`-Äquivalent – die App muss separat gestartet werden.

---

## Teil 0C: App-Konfiguration – Was du über die TodoMatic-App wissen musst

> ⚠️ **Lies diesen Abschnitt vor den Übungen.** Er erklärt App-Verhaltensweisen, die direkt bestimmen, wie du Tests schreiben musst.

### Starten der App

```bash
npm run dev          # Startet Vite Dev-Server auf Port 3000, öffnet Browser
npm run dev -- --open false   # Ohne Browser-Öffnung (besser für Test-Runs)
```

Port und Base-URL sind in `vite.config.js` als `base: "http://localhost:3000/"` und in `package.json` als `"dev": "vite --port 3000 --open"` fest konfiguriert.

### Initialer App-Zustand: 1 vorhandene Aufgabe

Die App startet **nicht leer**. In `src/main.tsx` ist eine Aufgabe fest einprogrammiert:

```typescript
const DATA: Task[] = [{
  id: "todo-iYhueLHTq-6wprHhsXYF6",
  name: "test",
  time: "2024-11-18T16:12:44.160Z",
  location: { latitude: 49.6370557, longitude: 6.9014314 },
  completed: false,
}];
ReactDOM.createRoot(rootElement).render(<App tasks={DATA} />);
```

**Konsequenzen für Tests:**

| Situation | Was passiert |
|---|---|
| Seite frisch laden | `"1 task remaining"` ist sichtbar |
| Aufgabe hinzufügen | Zähler geht von 1 → 2 |
| Test benötigt leeren Zustand | Initiale Aufgabe `"test"` zuerst löschen |
| Seite neu laden (F5 / `page.reload()`) | Zustand resettet auf 1 initiale Aufgabe |

### Statische Dateien

```
public/getsitelogo.png   → http://localhost:3000/getsitelogo.png   (Logo im Header)
public/remoteTasks.json  → http://localhost:3000/remoteTasks.json  (Remote-Tasks-Button)
```

**"Load remote tasks"** ersetzt die **gesamte** aktuelle Task-Liste durch die 2 Einträge aus `remoteTasks.json`.

### Geolocation ist Pflicht beim Hinzufügen von Aufgaben

```typescript
// src/App.tsx – addTask ruft navigator.geolocation auf
function addTask(name: string) {
  navigator.geolocation.getCurrentPosition((position) => {
    // Aufgabe wird NUR hier erstellt – ohne erfolgreichen Callback: nichts passiert
    setTasks([...tasks, { id: "todo-" + nanoid(), name, ...coords }]);
  });
}
```

**Ohne Geolocation-Grant** wird keine Aufgabe gespeichert – kein Fehler, keine Meldung. Das ist der häufigste Fehler beim ersten Test! Lösung: immer `permissions: ["geolocation"]` + `geolocation: { latitude, longitude }` konfigurieren (siehe Exercise 2).

### Keine Persistenz

- Kein LocalStorage, keine Datenbank, kein Backend
- `page.reload()` = kompletter State-Reset
- Tests sind voneinander isoliert – ideal für parallele Ausführung

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

| Element | Locator-Hinweis | Wichtiger Hinweis |
|---|---|---|
| Seitentitel | `<h2>` mit Text "TodoMatic" | |
| Eingabefeld neue Aufgabe | `id="new-todo-input"` | |
| Hinzufügen-Button | `id="myUniqueID"` (Text: "Add") | ⚠️ Braucht Geolocation-Grant! |
| Filter-Buttons | `data-testid="testID-All/Active/Completed"` | `aria-pressed="true"` = aktiver Filter |
| Aufgaben-Liste | `role="list"` | Startet mit 1 Aufgabe ("test") |
| Aufgaben-Zähler | `id="list-heading"` | "N tasks remaining" – beginnt bei 1 |
| Logo-Bild | `alt="Site Logo"` | HTTP-Anfrage: `getsitelogo.png` |
| Remote-Tasks-Button | Text "Load remote tasks" | Lädt + **ersetzt** Liste mit `remoteTasks.json` |
| Bearbeiten-Button | Text "Edit" | Öffnet Inline-Edit-Mode |
| Löschen-Button | Text "Delete" | |
| Speichern-Button (Edit) | Text "Save" | |
| Abbrechen-Button (Edit) | Text "Cancel" | |

---

## Teil 2: Code-Driven vs. Codegen – Zwei Wege zum Test

Bevor du mit den Exercises beginnst, lerne die zwei grundlegenden Ansätze kennen, wie du einen Playwright-Test erstellen kannst:

| | 💻 Code-Driven | 🎬 Codegen (Recorder) |
|---|---|---|
| **Vorgehen** | Test von Hand schreiben | Browser-Interaktionen aufzeichnen |
| **Ergebnis** | Sauberer, wartbarer Code | Funktionierender Code, aber oft redundant |
| **Locator-Qualität** | Du wählst bewusst die beste Strategie | Playwright wählt automatisch – oft zu fragil |
| **API-Kenntnisse** | Werden aktiv gelernt | Bleiben oberflächlich |
| **Einsatz** | ✅ Für alle produktiven Tests | ✅ Für Locator-Discovery und Quick-Starts |
| **Trainerpräferenz** | ⭐ Bevorzugt | Als Hilfsmittel |

### 💻 Ansatz 1: Code-Driven (vom Trainer empfohlen)

Du schreibst den Test direkt, basierend auf dem Wissen über die App-Struktur und die Playwright-API. Das erzwingt Verständnis der Locator-Strategien und führt zu wartbarem Code.

**Vorgehensweise:**
1. App im Browser öffnen, DOM mit DevTools erkunden (`F12`)
2. Locatoren manuell im Inspector / DevTools-Konsole testen: `$$('[data-testid]')`
3. Test mit der `@playwright/test`-API von Hand schreiben
4. Mit `PWDEBUG=1` debuggen

```typescript
// ✅ Code-Driven: Bewusste Locator-Wahl
test("smoke test – code-driven", async ({ page }) => {
  await page.goto("/");
  // Semantisch robust: ARIA-Rolle, nicht CSS-Klasse
  await expect(page.getByRole("heading", { name: "TodoMatic" })).toBeVisible();
  // data-testid – stabil gegenüber UI-Änderungen
  await expect(page.getByTestId("testID-All")).toBeVisible();
  // ID – eindeutig, direkt aus dem Quellcode bekannt
  await expect(page.locator("#new-todo-input")).toBeVisible();
});
```

### 🎬 Ansatz 2: Codegen (Recorder)

Playwright zeichnet Interaktionen auf und generiert Code. Sinnvoll, um Locatoren schnell zu entdecken oder einen ersten Entwurf zu generieren.

```bash
# TypeScript
npx playwright codegen http://localhost:3000

# C# (.NET)
pwsh bin/Debug/net8.0/playwright.ps1 codegen http://localhost:3000

# VS Code: Testing-Seitenleiste → "Record new"-Button
```

Typische Codegen-Ausgabe für denselben Test:

```typescript
// ⚠️ Codegen-Output: funktioniert, ist aber oft zu spezifisch
test("smoke test – codegen", async ({ page }) => {
  await page.goto("http://localhost:3000/");
  // Codegen wählt manchmal fragile Selektoren
  await expect(page.locator("h2")).toContainText("TodoMatic");
  // Oder: rollenbasiert, aber mit absolutem URL
  await page.locator("button").filter({ hasText: "All" }).click();
});
```

### 🔄 Empfohlener Workflow: das Beste aus beiden Welten

1. **Codegen starten** → Interaktionen aufzeichnen → Locatoren beobachten
2. **Generierten Code als Inspiration verwenden** – aber nie blind übernehmen
3. **Code manuell verbessern**: fragile Selektoren durch ARIA-Rollen / `data-testid` ersetzen
4. **Page Object** daraus bauen (Exercise 4), damit Locatoren nur einmal definiert werden

> **Übung:** Starte Codegen, füge eine Aufgabe hinzu und lösche sie. Vergleiche den generierten Code mit dem Code-Driven-Ansatz aus Exercise 1. Welche Unterschiede erkennst du?

---

## Exercise 1: Smoke Test – Seiteninhalt prüfen

**Ziel:** Grundstruktur eines Playwright-Tests verstehen. Prüfe, ob die Seite korrekt lädt und alle wichtigen UI-Elemente sichtbar sind.

> 📚 **Docs:** [Writing Tests (TS)](https://playwright.dev/docs/writing-tests) · [Writing Tests (.NET)](https://playwright.dev/dotnet/docs/writing-tests) · [Locators](https://playwright.dev/docs/locators) · [Assertions](https://playwright.dev/docs/test-assertions) · [page.goto()](https://playwright.dev/docs/api/class-page#page-goto)

**Aufgabe:**

Schreibe den Test **code-driven** (ohne Codegen):
1. Die App öffnet
2. Den Browser-Tab-Titel prüft
3. Die Überschrift "TodoMatic" prüft
4. Das Eingabefeld und den "Add"-Button prüft
5. Alle drei Filter-Buttons prüft
6. Den initialen Aufgaben-Zähler prüft ("1 task remaining")

> **💡 Ablauf:** Öffne zuerst `http://localhost:3000` im Browser und drücke `F12`. Erkunde das DOM und suche die richtigen Locatoren. Schreibe dann den Test von Hand. Nutze **danach** Codegen zum Vergleich – was hat Codegen anders gewählt?

<details>
<summary>💡 Lösungshinweis TypeScript</summary>

Erstelle `tests/smoke.spec.ts`:

```typescript
import { test, expect } from "@playwright/test";

test("smoke test - app loads correctly", async ({ page }) => {
  await page.goto("/");

  // Browser-Titel
  await expect(page).toHaveTitle(/TodoMatic/);

  // Haupt-Überschrift via ARIA-Rolle (robust, barrierefrei)
  await expect(page.getByRole("heading", { name: "TodoMatic" })).toBeVisible();

  // Formular via ID (aus dem Quellcode bekannt)
  await expect(page.locator("#new-todo-input")).toBeVisible();
  await expect(page.locator("#myUniqueID")).toBeVisible();

  // Filter-Buttons via data-testid (bewusst stabil gewählt)
  await expect(page.getByTestId("testID-All")).toBeVisible();
  await expect(page.getByTestId("testID-Active")).toBeVisible();
  await expect(page.getByTestId("testID-Completed")).toBeVisible();

  // Initialer Zähler – App startet mit 1 Aufgabe!
  await expect(page.locator("#list-heading")).toContainText("1 task remaining");
});
```

```bash
npx playwright test smoke.spec.ts
```

</details>

<details>
<summary>💡 Lösungshinweis C# – MSTest</summary>

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

        await Expect(Page).ToHaveTitleAsync(new Regex("TodoMatic"));
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "TodoMatic" }))
            .ToBeVisibleAsync();
        await Expect(Page.Locator("#new-todo-input")).ToBeVisibleAsync();
        await Expect(Page.Locator("#myUniqueID")).ToBeVisibleAsync();
        await Expect(Page.GetByTestId("testID-All")).ToBeVisibleAsync();
        await Expect(Page.GetByTestId("testID-Active")).ToBeVisibleAsync();
        await Expect(Page.GetByTestId("testID-Completed")).ToBeVisibleAsync();
        // App startet mit 1 Aufgabe – nicht mit 0!
        await Expect(Page.Locator("#list-heading")).ToContainTextAsync("1 task remaining");
    }
}
```

**Visual Studio:** Test Explorer → Rebuild → Test ausführen  
**VS Code:** Testing-Seitenleiste → Einzelnen Test starten

```bash
dotnet test --filter "AppLoadsCorrectly"
```

</details>

<details>
<summary>💡 Lösungshinweis C# – NUnit</summary>

```csharp
using Microsoft.Playwright.NUnit;

[TestFixture]
public class SmokeTests : PageTest
{
    [Test]
    public async Task AppLoadsCorrectly()
    {
        await Page.GotoAsync("http://localhost:3000");
        await Expect(Page).ToHaveTitleAsync(new Regex("TodoMatic"));
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "TodoMatic" }))
            .ToBeVisibleAsync();
        await Expect(Page.Locator("#new-todo-input")).ToBeVisibleAsync();
        await Expect(Page.GetByTestId("testID-All")).ToBeVisibleAsync();
        await Expect(Page.Locator("#list-heading")).ToContainTextAsync("1 task remaining");
    }
}
```

```bash
dotnet test --filter "AppLoadsCorrectly"
```

</details>

<details>
<summary>💡 Lösungshinweis C# – xUnit</summary>

```csharp
using Microsoft.Playwright.Xunit;

public class SmokeTests : PageTest
{
    [Fact]
    public async Task AppLoadsCorrectly()
    {
        await Page.GotoAsync("http://localhost:3000");
        await Expect(Page).ToHaveTitleAsync(new Regex("TodoMatic"));
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "TodoMatic" }))
            .ToBeVisibleAsync();
        await Expect(Page.Locator("#new-todo-input")).ToBeVisibleAsync();
        await Expect(Page.GetByTestId("testID-All")).ToBeVisibleAsync();
        await Expect(Page.Locator("#list-heading")).ToContainTextAsync("1 task remaining");
    }
}
```

```bash
dotnet test --filter "FullyQualifiedName~SmokeTests"
```

</details>

---

## Exercise 2: Geolocation mocken und Aufgabe hinzufügen

**Ziel:** Browser-APIs mocken. Die App ruft `navigator.geolocation.getCurrentPosition` beim Hinzufügen auf – ohne Mock passiert nichts.

> 📚 **Docs:** [Emulation – Geolocation (TS)](https://playwright.dev/docs/emulation#geolocation) · [Emulation – Geolocation (.NET)](https://playwright.dev/dotnet/docs/emulation#geolocation) · [Permissions](https://playwright.dev/docs/emulation#permissions) · [test.use() / Fixtures](https://playwright.dev/docs/test-fixtures)

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

## Exercise 7: Screenshots, Video und Traces – Diagnose-Werkzeuge aktiv nutzen

**Ziel:** Die drei wichtigsten Diagnose-Werkzeuge von Playwright gezielt einsetzen – für lokales Debugging und CI-Fehleranalyse.

**Warum wichtig?** In Headless-CI-Umgebungen kann man nicht live zuschauen. Screenshots, Videos und Traces sind die einzigen Beweise dafür, was im fehlgeschlagenen Test passiert ist.

---

### Teil A: Screenshots

**Aufgabe:**
1. Konfiguriere `playwright.config.ts` / `playwright.runsettings` für automatische Screenshots bei Fehlern
2. Schreibe einen Test, der nach einer Interaktion einen manuellen Full-Page-Screenshot aufnimmt
3. Schreibe einen Test, der absichtlich fehlschlägt – prüfe, dass der Screenshot in `test-results/` erscheint
4. **Bonus:** Screenshot eines einzelnen Elements (z. B. nur die Filterschaltflächen)

<details>
<summary>💡 Lösungshinweis TypeScript</summary>

`playwright.config.ts` – automatisch bei Fehler:

```typescript
use: {
  screenshot: "only-on-failure",  // nur bei Fehler
  // screenshot: "on",            // immer
}
```

Manueller Screenshot im Test:

```typescript
import { test, expect } from "@playwright/test";

test("screenshot after adding task", async ({ page }) => {
  await page.goto("/");

  // Full-Page-Screenshot (inkl. nicht sichtbarem Bereich)
  await page.screenshot({ path: "tests/screenshots/initial-state.png", fullPage: true });

  await page.locator("#new-todo-input").fill("Screenshot Task");
  await page.screenshot({ path: "tests/screenshots/after-typing.png" });
});

test("screenshot of single element", async ({ page }) => {
  await page.goto("/");
  // Nur die Filter-Buttons
  const filters = page.locator("[data-testid^='testID']").first();
  await filters.screenshot({ path: "tests/screenshots/filter-area.png" });
});

test("intentionally failing – check screenshot", async ({ page }) => {
  await page.goto("/");
  // Dieser Test schlägt fehl → Screenshot landet in test-results/
  await expect(page.locator("#nonexistent")).toBeVisible({ timeout: 1000 });
});
```

```bash
npx playwright test --reporter=html
npx playwright show-report  # Screenshots in "Attachments" sichtbar
```

</details>

<details>
<summary>💡 Lösungshinweis C# – MSTest</summary>

`playwright.runsettings` (automatisch):

```xml
<Parameter name="playwright:screenshot" value="only-on-failure" />
```

Oder `playwright.config.json`:

```json
{ "use": { "screenshot": "only-on-failure" } }
```

Manuell im Test:

```csharp
[TestMethod]
public async Task ScreenshotAfterAddingTask()
{
    await Page.GotoAsync("http://localhost:3000");

    // Full-Page-Screenshot
    await Page.ScreenshotAsync(new PageScreenshotOptions
    {
        Path = "screenshots/initial-state.png",
        FullPage = true,
    });

    await Page.Locator("#new-todo-input").FillAsync("Screenshot Task");
    await Page.ScreenshotAsync(new PageScreenshotOptions
        { Path = "screenshots/after-typing.png" });
}

[TestMethod]
public async Task ScreenshotOfSingleElement()
{
    await Page.GotoAsync("http://localhost:3000");
    // Nur die Filter-Schaltflächen
    await Page.GetByTestId("testID-All").ScreenshotAsync(new LocatorScreenshotOptions
        { Path = "screenshots/filter-button.png" });
}
```

**In Visual Studio / VS Code** – Screenshot als Testergebnis-Anhang:

```csharp
// MSTest
TestContext.AddResultFile("screenshots/initial-state.png");

// NUnit
TestContext.AddTestAttachment("screenshots/initial-state.png", "Initial State");
```

</details>

<details>
<summary>💡 Lösungshinweis C# – NUnit / xUnit</summary>

**NUnit:**

```csharp
[TestFixture]
public class ScreenshotTests : PageTest
{
    [Test]
    public async Task TakeScreenshot()
    {
        await Page.GotoAsync("http://localhost:3000");
        await Page.ScreenshotAsync(new PageScreenshotOptions
            { Path = "screenshots/state.png", FullPage = true });
        // Als Anhang an NUnit-Ergebnis
        TestContext.AddTestAttachment("screenshots/state.png", "App State");
    }
}
```

**xUnit** (Ausgabe per `ITestOutputHelper`):

```csharp
public class ScreenshotTests : PageTest
{
    private readonly ITestOutputHelper _output;
    public ScreenshotTests(ITestOutputHelper output) => _output = output;

    [Fact]
    public async Task TakeScreenshot()
    {
        await Page.GotoAsync("http://localhost:3000");
        var path = "screenshots/state.png";
        await Page.ScreenshotAsync(new PageScreenshotOptions { Path = path, FullPage = true });
        _output.WriteLine($"Screenshot: {path}");
    }
}
```

</details>

---

### Teil B: Video-Aufnahme

**Aufgabe:**
1. Aktiviere Video-Aufnahme für einen Test
2. Führe einen vollständigen Task-Workflow aus (hinzufügen, bearbeiten, löschen)
3. Finde die `.webm`-Datei in `test-results/` und öffne sie
4. Konfiguriere `retain-on-failure` – prüfe, dass bei bestehendem Test kein Video entsteht, bei fehlschlagendem schon

<details>
<summary>💡 Lösungshinweis TypeScript</summary>

```typescript
import { test, expect } from "@playwright/test";

// Video für diese Test-Datei aktivieren
test.use({
  video: "on",                   // immer aufzeichnen
  // video: "retain-on-failure", // nur bei Fehler behalten (CI-Empfehlung)
  geolocation: { latitude: 48.1372, longitude: 11.5755 },
  permissions: ["geolocation"],
});

test("record task workflow", async ({ page }) => {
  await page.goto("/");
  await page.locator("#new-todo-input").fill("Video Task");
  await page.locator("#myUniqueID").click();
  await expect(page.getByRole("list").getByText("Video Task")).toBeVisible();

  // Edit
  const item = page.getByRole("listitem").filter({ hasText: "Video Task" });
  await item.getByRole("button", { name: "Edit" }).click();
  await item.getByRole("textbox").fill("Video Task bearbeitet");
  await item.getByRole("button", { name: "Save" }).click();

  // Delete
  await page.getByRole("listitem").filter({ hasText: "Video Task bearbeitet" })
    .getByRole("button", { name: "Delete" }).click();
});
```

Global in `playwright.config.ts`:

```typescript
use: { video: "retain-on-failure" }
```

Videos landen in `test-results/<test-name>/video.webm`. Im HTML-Report sind sie direkt eingebettet:

```bash
npx playwright show-report
```

</details>

<details>
<summary>💡 Lösungshinweis C# (alle Frameworks)</summary>

Per Umgebungsvariable (alle Frameworks):

```bash
PLAYWRIGHT_VIDEO=on dotnet test
PLAYWRIGHT_VIDEO=retain-on-failure dotnet test
```

Per `playwright.config.json`:

```json
{ "use": { "video": "retain-on-failure" } }
```

Videos werden in `test-results/` abgelegt. Pfad im Test ausgeben:

```csharp
// MSTest
[TestCleanup]
public void SaveVideoPath()
{
    // Video-Pfad ist im Context verfügbar
    TestContext.WriteLine($"Video: test-results/{TestContext.TestName}/video.webm");
}
```

</details>

---

### Teil C: Trace Viewer – vollständiger Zeitstrahl

**Aufgabe:**
1. Aktiviere Traces manuell im Test mit `context.tracing.start()`
2. Führe einen Add-Task-Workflow durch
3. Stoppe und speichere den Trace als `.zip`
4. Öffne ihn mit `npx playwright show-trace` / `playwright.ps1 show-trace`
5. Navigiere durch DOM-Snapshots → erkunde den **Network**-Tab → sieh dir jeden Schritt im **Actions**-Panel an

<details>
<summary>💡 Lösungshinweis TypeScript</summary>

```typescript
import { test, expect } from "@playwright/test";

test.use({
  geolocation: { latitude: 48.1372, longitude: 11.5755 },
  permissions: ["geolocation"],
});

test("manual trace – add and delete task", async ({ page, context }) => {
  // Trace starten (screenshots + DOM-Snapshots + Quellcode)
  await context.tracing.start({ screenshots: true, snapshots: true, sources: true });

  await page.goto("/");
  await page.locator("#new-todo-input").fill("Trace Task");
  await page.locator("#myUniqueID").click();
  await expect(page.getByRole("list").getByText("Trace Task")).toBeVisible();
  await page.getByRole("listitem").filter({ hasText: "Trace Task" })
    .getByRole("button", { name: "Delete" }).click();

  // Trace stoppen und als ZIP speichern
  await context.tracing.stop({ path: "tests/traces/add-delete-trace.zip" });
});
```

```bash
# Trace öffnen (öffnet Browser mit Trace-Viewer)
npx playwright show-trace tests/traces/add-delete-trace.zip

# In VS Code: nach fehlgeschlagenem Test → "Show Trace"-Link in Testing-Seitenleiste
```

Global in `playwright.config.ts` aktivieren:

```typescript
use: { trace: "on-first-retry" }  // nur beim Retry – CI-Empfehlung
// use: { trace: "on" }           // immer
```

</details>

<details>
<summary>💡 Lösungshinweis C# – MSTest</summary>

```csharp
[TestClass]
public class TraceTests : PageTest
{
    public override BrowserNewContextOptions ContextOptions() => new()
    {
        BaseURL = "http://localhost:3000",
        Geolocation = new Geolocation { Latitude = 48.1372f, Longitude = 11.5755f },
        Permissions = new[] { "geolocation" },
    };

    [TestMethod]
    public async Task ManualTrace()
    {
        await Context.Tracing.StartAsync(new TracingStartOptions
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true,
        });

        await Page.GotoAsync("/");
        await Page.Locator("#new-todo-input").FillAsync("Trace Task");
        await Page.Locator("#myUniqueID").ClickAsync();
        await Expect(Page.GetByRole(AriaRole.List).GetByText("Trace Task")).ToBeVisibleAsync();

        await Context.Tracing.StopAsync(new TracingStopOptions
            { Path = "traces/add-task-trace.zip" });

        // Als Testergebnis-Anhang in Visual Studio sichtbar machen
        TestContext.AddResultFile("traces/add-task-trace.zip");
    }
}
```

```powershell
# Trace öffnen
pwsh bin/Debug/net8.0/playwright.ps1 show-trace traces/add-task-trace.zip
```

**NUnit:**

```csharp
[TestFixture]
public class TraceTests : PageTest
{
    [Test]
    public async Task ManualTrace()
    {
        await Context.Tracing.StartAsync(new() { Screenshots = true, Snapshots = true });
        await Page.GotoAsync("http://localhost:3000");
        // ... Testschritte ...
        await Context.Tracing.StopAsync(new() { Path = "traces/trace.zip" });
        TestContext.AddTestAttachment("traces/trace.zip", "Playwright Trace");
    }
}
```

**xUnit:**

```csharp
public class TraceTests : PageTest
{
    private readonly ITestOutputHelper _output;
    public TraceTests(ITestOutputHelper output) => _output = output;

    [Fact]
    public async Task ManualTrace()
    {
        await Context.Tracing.StartAsync(new() { Screenshots = true, Snapshots = true });
        await Page.GotoAsync("http://localhost:3000");
        // ... Testschritte ...
        await Context.Tracing.StopAsync(new() { Path = "traces/trace.zip" });
        _output.WriteLine("Trace: traces/trace.zip");
    }
}
```

> **Aus PlaywrightDemos:** Tracing ist seit [BASTA! 2024](https://github.com/norschel/PlaywrightDemos/blob/main/PlaywrightDemos/PlaywrightE2ETests_Basta2024.cs) das Standard-Debugging-Werkzeug in allen Konferenz-Demos – unverzichtbar in CI/CD, wo kein lokaler Browser verfügbar ist.

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

## Exercise 11: Page Object Model – Wartbarkeit und Wiederverwendung

**Ziel:** Locatoren und Aktionen in einer Klasse kapseln, statt sie in jedem Test zu wiederholen. Das liefert drei konkrete Vorteile:

1. **Wartbarkeit:** Ändert sich z. B. `#myUniqueID` zu `#add-btn`, wird nur **eine Stelle** angepasst – alle Tests laufen sofort wieder
2. **Wiederverwendung:** `addTask()`, `deleteTask()` usw. werden von vielen Tests genutzt – kein Copy-Paste, kein Drift
3. **Lesbarkeit:** Tests beschreiben *Was* getestet wird, nicht *Wie* das DOM navigiert wird

**Aufgabe:**

1. Erstelle eine `TodoPage`-Klasse mit Locatoren als Properties und Aktionen als Methoden
2. Schreibe die Tests aus Exercise 2 und 3 damit neu – beobachte, wie viel kürzer sie werden
3. Schreibe einen neuen Test für den vollständigen Task-Lifecycle (hinzufügen → bearbeiten → abschließen → filtern → löschen) in wenigen, gut lesbaren Zeilen

<details>
<summary>💡 Lösungshinweis TypeScript – Page Object</summary>

`tests/pages/TodoPage.ts`:

```typescript
import { type Page, type Locator, expect } from "@playwright/test";

export class TodoPage {
  // ── Locatoren als readonly Properties ──────────────────────────────────────
  // Einmal definiert – wenn sich ein Selektor ändert, nur hier anpassen.
  readonly addInput: Locator;
  readonly addButton: Locator;
  readonly taskCount: Locator;
  readonly filterAll: Locator;
  readonly filterActive: Locator;
  readonly filterCompleted: Locator;
  readonly loadRemoteButton: Locator;

  constructor(private readonly page: Page) {
    this.addInput         = page.locator("#new-todo-input");
    this.addButton        = page.locator("#myUniqueID");
    this.taskCount        = page.locator("#list-heading");
    this.filterAll        = page.getByTestId("testID-All");
    this.filterActive     = page.getByTestId("testID-Active");
    this.filterCompleted  = page.getByTestId("testID-Completed");
    this.loadRemoteButton = page.getByRole("button", { name: "Load remote tasks" });
  }

  // ── Dynamische Locatoren als Methoden ──────────────────────────────────────
  taskItem(name: string): Locator {
    return this.page.getByRole("listitem").filter({ hasText: name });
  }
  taskList(): Locator { return this.page.getByRole("list"); }

  // ── Aktionen ───────────────────────────────────────────────────────────────
  async goto(): Promise<void> { await this.page.goto("/"); }

  async addTask(name: string): Promise<void> {
    await this.addInput.fill(name);
    await this.addButton.click();
    await expect(this.taskList().getByText(name)).toBeVisible();
  }

  async deleteTask(name: string): Promise<void> {
    await this.taskItem(name).getByRole("button", { name: "Delete" }).click();
    await expect(this.taskList().getByText(name)).not.toBeVisible();
  }

  async editTask(oldName: string, newName: string): Promise<void> {
    const item = this.taskItem(oldName);
    await item.getByRole("button", { name: "Edit" }).click();
    await item.getByRole("textbox").fill(newName);
    await item.getByRole("button", { name: "Save" }).click();
    await expect(this.taskList().getByText(newName)).toBeVisible();
  }

  async completeTask(name: string): Promise<void> {
    await this.taskItem(name).getByRole("checkbox").check();
  }

  async setFilter(filter: "All" | "Active" | "Completed"): Promise<void> {
    const btn = { All: this.filterAll, Active: this.filterActive,
                  Completed: this.filterCompleted }[filter];
    await btn.click();
  }

  async getTaskCount(): Promise<number> {
    const text = await this.taskCount.textContent();
    return parseInt(text?.match(/\d+/)?.[0] ?? "0");
  }
}
```

`tests/pom.spec.ts`:

```typescript
import { test, expect } from "@playwright/test";
import { TodoPage } from "./pages/TodoPage";

test.use({
  geolocation: { latitude: 48.1372, longitude: 11.5755 },
  permissions: ["geolocation"],
});

test("full task lifecycle – lesbarer dank POM", async ({ page }) => {
  const todo = new TodoPage(page);
  await todo.goto();

  await todo.addTask("Einkaufen");
  await todo.editTask("Einkaufen", "Einkaufen gehen");
  await todo.completeTask("Einkaufen gehen");
  await todo.setFilter("Completed");
  await expect(todo.taskItem("Einkaufen gehen")).toBeVisible();
  await todo.setFilter("All");
  await todo.deleteTask("Einkaufen gehen");
  expect(await todo.getTaskCount()).toBe(1); // zurück auf initiale Aufgabe
});
```

**Wartbarkeit prüfen:** Ändere im `TodoPage`-Konstruktor `"#myUniqueID"` zu `"#add-task-btn"` – alle Tests schlagen fehl. Ändere es zurück – alle Tests laufen wieder. Kein einziger Test wurde angefasst.

</details>

<details>
<summary>💡 Lösungshinweis C# – Page Object (framework-unabhängig)</summary>

`Pages/TodoPage.cs` – wird von MSTest, NUnit und xUnit gleich verwendet:

```csharp
using Microsoft.Playwright;

public class TodoPage
{
    private readonly IPage _page;

    // ── Locatoren als Properties ───────────────────────────────────────────────
    // Lazy evaluation: kein DOM-Lookup beim Erstellen des Page Objects
    public ILocator AddInput        => _page.Locator("#new-todo-input");
    public ILocator AddButton       => _page.Locator("#myUniqueID");
    public ILocator TaskCount       => _page.Locator("#list-heading");
    public ILocator FilterAll       => _page.GetByTestId("testID-All");
    public ILocator FilterActive    => _page.GetByTestId("testID-Active");
    public ILocator FilterCompleted => _page.GetByTestId("testID-Completed");
    public ILocator TaskList        => _page.GetByRole(AriaRole.List);

    public TodoPage(IPage page) => _page = page;

    // ── Dynamische Locatoren ───────────────────────────────────────────────────
    public ILocator TaskItem(string name) =>
        _page.GetByRole(AriaRole.Listitem).Filter(new() { HasText = name });

    // ── Aktionen ───────────────────────────────────────────────────────────────
    public Task GotoAsync() => _page.GotoAsync("http://localhost:3000");

    public async Task AddTaskAsync(string name)
    {
        await AddInput.FillAsync(name);
        await AddButton.ClickAsync();
        await Assertions.Expect(TaskList.GetByText(name)).ToBeVisibleAsync();
    }

    public async Task DeleteTaskAsync(string name)
    {
        await TaskItem(name).GetByRole(AriaRole.Button, new() { Name = "Delete" }).ClickAsync();
        await Assertions.Expect(TaskList.GetByText(name)).Not.ToBeVisibleAsync();
    }

    public async Task EditTaskAsync(string oldName, string newName)
    {
        var item = TaskItem(oldName);
        await item.GetByRole(AriaRole.Button, new() { Name = "Edit" }).ClickAsync();
        await item.GetByRole(AriaRole.Textbox).FillAsync(newName);
        await item.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
        await Assertions.Expect(TaskList.GetByText(newName)).ToBeVisibleAsync();
    }

    public Task CompleteTaskAsync(string name) =>
        TaskItem(name).GetByRole(AriaRole.Checkbox).CheckAsync();

    public Task SetFilterAsync(string filter)
    {
        var btn = filter switch
        {
            "Active"    => FilterActive,
            "Completed" => FilterCompleted,
            _           => FilterAll,
        };
        return btn.ClickAsync();
    }

    public async Task<int> GetTaskCountAsync()
    {
        var text = await TaskCount.TextContentAsync();
        return int.Parse(Regex.Match(text ?? "0", @"\d+").Value);
    }
}
```

Tests (MSTest – NUnit/xUnit analog):

```csharp
[TestClass]
public class PomTests : PageTest
{
    public override BrowserNewContextOptions ContextOptions() => new()
    {
        BaseURL = "http://localhost:3000",
        Geolocation = new Geolocation { Latitude = 48.1372f, Longitude = 11.5755f },
        Permissions = new[] { "geolocation" },
    };

    [TestMethod]   // NUnit: [Test]   xUnit: [Fact]
    public async Task FullTaskLifecycle()
    {
        var todo = new TodoPage(Page);
        await todo.GotoAsync();

        await todo.AddTaskAsync("Einkaufen");
        await todo.EditTaskAsync("Einkaufen", "Einkaufen gehen");
        await todo.CompleteTaskAsync("Einkaufen gehen");
        await todo.SetFilterAsync("Completed");
        await Expect(todo.TaskItem("Einkaufen gehen")).ToBeVisibleAsync();
        await todo.SetFilterAsync("All");
        await todo.DeleteTaskAsync("Einkaufen gehen");
        Assert.AreEqual(1, await todo.GetTaskCountAsync());
    }
}
```

**NUnit:** `[TestFixture]` + `[Test]` + `Microsoft.Playwright.NUnit.PageTest`  
**xUnit:** kein Klassenattribut + `[Fact]` + `Microsoft.Playwright.Xunit.PageTest`  
**Das `TodoPage`-Objekt selbst bleibt identisch** – es ist framework-unabhängig.

</details>

---

## Exercise 12: CI/CD – GitHub Actions, Azure Pipelines und Docker

**Ziel:** Playwright-Tests in drei verschiedenen Ausführungsumgebungen automatisieren. Wähle die Variante, die zu deiner Infrastruktur passt.

**Aufgabe:** Implementiere die CI-Pipeline mit einer der drei Varianten (oder alle drei zum Vergleich):
1. Trigger auf Push und Pull Request auf `main`
2. App starten, Tests ausführen
3. Artefakte (Reports, Screenshots, Traces) speichern

---

### Variante A: GitHub Actions

<details>
<summary>💡 TypeScript – GitHub Actions</summary>

`.github/workflows/playwright.yml`:

```yaml
name: Playwright Tests (TypeScript)
on:
  push:    { branches: [main] }
  pull_request: { branches: [main] }

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with: { node-version: 20, cache: npm }
      - run: npm ci
      - run: npx playwright install --with-deps
      - run: npx playwright test
      - uses: actions/upload-artifact@v4
        if: always()
        with: { name: playwright-report, path: playwright-report/, retention-days: 14 }
      - uses: actions/upload-artifact@v4
        if: failure()
        with: { name: test-results, path: test-results/, retention-days: 7 }
```

</details>

<details>
<summary>💡 C# / .NET – GitHub Actions</summary>

```yaml
name: Playwright Tests (.NET)
on:
  push:    { branches: [main] }
  pull_request: { branches: [main] }

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with: { dotnet-version: 8.x }
      - uses: actions/setup-node@v4
        with: { node-version: 20 }
      - name: Build and start Todo App
        run: npm ci && npm run build && npx serve dist -p 3000 &
        working-directory: ./todo-react-playwright
      - run: dotnet build TodoPlaywrightTests/
      - run: pwsh TodoPlaywrightTests/bin/Debug/net8.0/playwright.ps1 install --with-deps
      - run: dotnet test TodoPlaywrightTests/ --logger trx --results-directory TestResults/
        env: { PLAYWRIGHT_BASE_URL: "http://localhost:3000" }
      - uses: actions/upload-artifact@v4
        if: always()
        with: { name: test-results-dotnet, path: TestResults/, retention-days: 14 }
      - uses: actions/upload-artifact@v4
        if: failure()
        with:
          name: playwright-artifacts
          path: |
            **/traces/
            **/screenshots/
            **/*.webm
          retention-days: 7
```

**Tipp aus PlaywrightDemos:** Nutze `[TestCategory("CICD")]` (MSTest) / `[Category("CICD")]` (NUnit) / `[Trait("Category","CICD")]` (xUnit), um nur produktionsreife Tests in CI auszuführen: `dotnet test --filter "TestCategory=CICD"`.

</details>

---

### Variante B: Azure Pipelines

<details>
<summary>💡 TypeScript – Azure Pipelines</summary>

`azure-pipelines.yml`:

```yaml
trigger:
  - main

pool:
  vmImage: ubuntu-latest

steps:
  - task: NodeTool@0
    inputs: { versionSpec: "20.x" }
    displayName: Install Node.js

  - script: npm ci
    displayName: Install dependencies

  - script: npx playwright install --with-deps
    displayName: Install Playwright browsers

  - script: npx playwright test --reporter=junit,html
    displayName: Run Playwright tests
    continueOnError: true

  - task: PublishTestResults@2
    condition: always()
    inputs:
      testResultsFormat: JUnit
      testResultsFiles: test-results/results.xml
      mergeTestResults: true
      testRunTitle: Playwright TypeScript Tests

  - task: PublishPipelineArtifact@1
    condition: always()
    inputs:
      targetPath: playwright-report
      artifact: playwright-report
      publishLocation: pipeline
```

JUnit-Reporter in `playwright.config.ts` aktivieren:

```typescript
reporter: [["html"], ["junit", { outputFile: "test-results/results.xml" }]],
```

</details>

<details>
<summary>💡 C# / .NET – Azure Pipelines</summary>

```yaml
trigger:
  - main

pool:
  vmImage: ubuntu-latest

steps:
  - task: UseDotNet@2
    inputs: { version: "8.x" }
    displayName: Install .NET 8

  - task: NodeTool@0
    inputs: { versionSpec: "20.x" }
    displayName: Install Node.js

  - script: npm ci && npm run build && npx serve dist -p 3000 &
    displayName: Build and start Todo App
    workingDirectory: $(System.DefaultWorkingDirectory)/todo-react-playwright

  - script: dotnet build TodoPlaywrightTests/
    displayName: Build test project

  - script: pwsh TodoPlaywrightTests/bin/Debug/net8.0/playwright.ps1 install --with-deps
    displayName: Install Playwright browsers

  - script: >
      dotnet test TodoPlaywrightTests/
      --logger trx
      --results-directory $(Agent.TempDirectory)/TestResults
    displayName: Run Playwright tests
    continueOnError: true
    env:
      PLAYWRIGHT_BASE_URL: http://localhost:3000

  - task: PublishTestResults@2
    condition: always()
    inputs:
      testResultsFormat: VSTest
      testResultsFiles: $(Agent.TempDirectory)/TestResults/*.trx
      mergeTestResults: true
      testRunTitle: Playwright .NET Tests

  - task: PublishPipelineArtifact@1
    condition: failed()
    inputs:
      targetPath: $(System.DefaultWorkingDirectory)/traces
      artifact: playwright-traces
      publishLocation: pipeline
```

</details>

---

### Variante C: Docker-Container

Docker garantiert reproduzierbare, isolierte Testläufe unabhängig vom Host-System. Microsoft stellt offizielle Playwright-Images mit vorinstallierten Browsern bereit.

<details>
<summary>💡 TypeScript – Dockerfile + Ausführung</summary>

`Dockerfile`:

```dockerfile
# Offizielles Playwright-Image – alle Browser vorinstalliert
FROM mcr.microsoft.com/playwright:v1.52.0-jammy
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
CMD ["npx", "playwright", "test"]
```

`.dockerignore`:

```
node_modules
test-results
playwright-report
.git
```

```bash
docker build -t todo-playwright-tests .

# Tests ausführen
docker run --rm todo-playwright-tests

# Mit spezifischem Browser
docker run --rm -e BROWSER=firefox todo-playwright-tests

# Ergebnisse aus Container extrahieren
docker run --rm \
  -v $(pwd)/test-results:/app/test-results \
  -v $(pwd)/playwright-report:/app/playwright-report \
  todo-playwright-tests
```

</details>

<details>
<summary>💡 C# / .NET – Multi-Stage Dockerfile (inspiriert von PlaywrightDemos)</summary>

`Dockerfile`:

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
WORKDIR /app
COPY . .
RUN dotnet build TodoPlaywrightTests/

# Stage 2: Test-Ausführung im Playwright-Runtime-Image
# (Browser + Systemabhängigkeiten bereits enthalten)
FROM mcr.microsoft.com/playwright/dotnet:v1.52.0-jammy AS test
WORKDIR /app
COPY --from=build /app/TodoPlaywrightTests/bin/Debug/net8.0 .
ENV PLAYWRIGHT_BASE_URL=http://host-gateway:3000
ENTRYPOINT ["dotnet", "test", "TodoPlaywrightTests.dll", \
            "--filter", "TestCategory=CICD", \
            "--logger", "trx;LogFileName=results.trx"]
```

```bash
docker build -t todo-playwright-tests-dotnet .

# App auf Host muss laufen (npm run dev)
docker run --rm \
  --add-host=host-gateway:host-gateway \
  -e PLAYWRIGHT_BASE_URL=http://host-gateway:3000 \
  -v $(pwd)/TestResults:/app/TestResults \
  todo-playwright-tests-dotnet
```

**docker compose** – App und Tests zusammen:

```yaml
# docker-compose.test.yml
services:
  app:
    build: { context: ./todo-react-playwright }
    ports: ["3000:3000"]
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:3000"]
      interval: 5s
      retries: 10
  tests:
    build: { context: ./TodoPlaywrightTests }
    depends_on:
      app: { condition: service_healthy }
    environment:
      PLAYWRIGHT_BASE_URL: http://app:3000
    volumes:
      - ./TestResults:/app/TestResults
```

```bash
docker compose -f docker-compose.test.yml up --exit-code-from tests
```

</details>

---

## Exercise 13 (Bonus): Azure Playwright Testing Service

**Ziel:** Tests auf einer Azure-verwalteten Browser-Farm ausführen – skalierbar, ohne eigene Browser-Infrastruktur. Das Enterprise-Muster aus den [PlaywrightDemos ab IT-Tage 2025](https://github.com/norschel/PlaywrightDemos/blob/main/PlaywrightDemos/AzurePlaywrightTests_BastaSpring2026.cs).

**Voraussetzungen:** Azure-Abonnement, Playwright Testing Workspace im Azure Portal erstellt.

<details>
<summary>💡 TypeScript – Azure Playwright Testing Service</summary>

```bash
npm install --save-dev @azure/microsoft-playwright-testing
```

`playwright.service.config.ts`:

```typescript
import { defineConfig } from "@playwright/test";
import { getServiceConfig, ServiceOS } from "@azure/microsoft-playwright-testing";
import config from "./playwright.config";

export default defineConfig(
  config,
  getServiceConfig(config, {
    os: ServiceOS.LINUX,
    runId: process.env.BUILD_ID ?? new Date().toISOString(),
  }),
  {
    reporter: [
      ["list"],
      ["@azure/microsoft-playwright-testing/reporter"],
    ],
  }
);
```

```bash
export PLAYWRIGHT_SERVICE_URL="wss://eastus.api.playwright.microsoft.com/accounts/<ID>/authorize/accessToken"
export PLAYWRIGHT_SERVICE_ACCESS_TOKEN="<token>"

# Tests in Azure ausführen
npx playwright test --config=playwright.service.config.ts

# Sharding – 4 parallele Browser in Azure
npx playwright test --config=playwright.service.config.ts --shard=1/4
```

</details>

<details>
<summary>💡 C# / NUnit – Azure Playwright Testing Service (wie in PlaywrightDemos)</summary>

```bash
dotnet add package Microsoft.Playwright.NUnit
dotnet add package Azure.Developer.MicrosoftPlaywrightTesting.NUnit
```

`PlaywrightServiceSetup.cs`:

```csharp
using Azure.Developer.MicrosoftPlaywrightTesting.NUnit;
[assembly: NUnit.Framework.Parallelizable(NUnit.Framework.ParallelScope.Fixtures)]

[SetUpFixture]
public class PlaywrightServiceSetup : PlaywrightServiceNUnitSetup { }
```

`AzurePlaywrightTests.cs`:

```csharp
using Azure.Developer.MicrosoftPlaywrightTesting.NUnit;

[TestFixture]
public class AzurePlaywrightTests : PlaywrightServiceTest
{
    public IPage Page { get; private set; } = null!;

    [SetUp]
    public async Task SetUp() => Page = await Context.NewPageAsync();

    [TearDown]
    public async Task TearDown() => await Page.CloseAsync();

    [Test]
    [Category("CICD")]
    public async Task AppLoadsOnAzureService()
    {
        await Page.GotoAsync("http://localhost:3000");
        await Assertions.Expect(Page).ToHaveTitleAsync(new Regex("TodoMatic"));
        TestContext.Out.WriteLine($"Browser: {Page.Context.Browser?.BrowserType.Name}");
    }
}
```

```bash
export PLAYWRIGHT_SERVICE_URL="wss://eastus.api.playwright.microsoft.com/..."
export PLAYWRIGHT_SERVICE_ACCESS_TOKEN="<token>"
dotnet test --filter "TestCategory=CICD"
```

**Vorteile:**
- Browser laufen in Azure-Containern – keine Browser-Installation im CI-Agent nötig
- Bis zu 50 parallele Browser ohne eigene Infrastruktur
- Ergebnisse und Traces direkt im Azure Portal sichtbar

</details>

---

## Exercise 14 (Bonus): Playwright MCP Server – KI-gesteuerte Browser-Automatisierung

**Ziel:** Den offiziellen **Playwright MCP Server** (`@playwright/mcp`) einrichten und über **GitHub Copilot Agent Mode** in VS Code nutzen, um die TodoMatic-App zu erkunden, Locatoren zu entdecken und Testentwürfe zu generieren.

> **Was ist der Playwright MCP Server?**  
> Das [Model Context Protocol (MCP)](https://modelcontextprotocol.io/) ist ein offener Standard, mit dem KI-Assistenten externe Tools (Browser, Datenbanken, APIs) steuern können. Der `@playwright/mcp`-Server stellt einem LLM (z. B. GitHub Copilot, Claude, ChatGPT) Playwright-Browser-Tools zur Verfügung – Navigieren, Klicken, Formulare ausfüllen, Screenshots aufnehmen, DOM abfragen – alles per natürlicher Sprache. Das **ergänzt** den code-driven Ansatz: du nutzt den MCP Server für Exploration und Entwurf, schreibst den finalen Test dann selbst von Hand (wie in Teil 2 empfohlen).

---

### Teil A: Setup

#### Voraussetzungen

| | |
|---|---|
| IDE | Visual Studio Code (MCP-Integration über `.vscode/mcp.json`) |
| GitHub Copilot | Abonnement aktiv, **Agent Mode** aktiviert (`Chat: Agent Mode` in VS Code Settings) |
| Node.js | 18+ (für `npx @playwright/mcp`) |

> ℹ️ **C# / Visual Studio:** Der Playwright MCP Server ist Node.js-basiert und wird in VS Code genutzt. Visual Studio 2022 hat aktuell keine direkte MCP-Integration. C#-Entwickler können den MCP Server dennoch in VS Code parallel nutzen – der generierte Test-Entwurf wird dann in C# übersetzt.

#### Schritt 1: MCP Server in VS Code konfigurieren

Erstelle (oder ergänze) die Datei `.vscode/mcp.json` im Projektstamm:

```json
{
  "servers": {
    "playwright": {
      "type": "stdio",
      "command": "npx",
      "args": ["@playwright/mcp@latest"]
    }
  }
}
```

> **Alternativ global** (einmalig installieren, dann schneller):
> ```bash
> npm install -g @playwright/mcp
> ```
> Dann in `.vscode/mcp.json` statt `npx @playwright/mcp@latest` einfach `playwright-mcp` als `command` angeben.

#### Schritt 2: Agent Mode aktivieren

1. In VS Code: **Einstellungen** → `Chat: Agent Mode` → aktivieren  
   (oder `Ctrl+,` → suche `chat.agent`)
2. Im Copilot Chat-Panel: oben rechts auf **`⚙ Agent`** umschalten
3. Du siehst jetzt eine Liste verfügbarer Tools – dort erscheint `playwright` mit seinen Browser-Aktionen

#### Schritt 3: App starten

```bash
npm run dev   # App muss laufen, bevor der MCP Server sie steuern kann
```

---

### Teil B: Übungsaufgaben

#### Aufgabe 1 – App explorieren und Locatoren entdecken

**Aufgabe:** Gib dem Copilot Agent folgenden Prompt und beobachte, wie der MCP Server die TodoMatic-App im Hintergrund öffnet und navigiert:

```
Öffne http://localhost:3000 im Browser und beschreibe mir alle interaktiven Elemente
auf der Seite mit ihren Locatoren (ARIA-Rolle, data-testid, ID).
```

> **Was passiert:** Der Playwright MCP Server navigiert zur App, analysiert das DOM via `browser_snapshot` und liefert dir eine strukturierte Liste aller Elemente – inklusive der Locatoren, die du in eigenen Tests verwenden kannst.

<details>
<summary>💡 Lösungshinweis</summary>

Der Agent wird intern u.a. folgende MCP-Tools nutzen:
- `browser_navigate` → Öffnet `http://localhost:3000`
- `browser_snapshot` → Liest den Accessibility-Baum (ähnlich wie Playwright Inspector)
- Antwortet mit einer Liste wie:
  ```
  - Eingabefeld: role=textbox, name="What needs to be done?"
  - Add-Button: role=button, name="Add"
  - Filter: data-testid="testID-All", "testID-Active", "testID-Completed"
  - Aufgaben-Zähler: id="list-heading", Text "1 task remaining"
  ```

Diese Locatoren entsprechen exakt dem, was du in Übung 1–5 manuell erarbeitet hast – hier siehst du den Unterschied zwischen Exploration (MCP) und festem Test-Code (code-driven).

</details>

---

#### Aufgabe 2 – Geführte Interaktion: Aufgabe hinzufügen

**Aufgabe:** Lasse den Copilot Agent eine neue Aufgabe in der App anlegen:

```
Gehe zur TodoMatic-App auf http://localhost:3000.
Füge eine neue Aufgabe mit dem Namen "MCP Demo Task" hinzu.
Mache danach einen Screenshot und zeige mir, ob die Aufgabe in der Liste erscheint.
```

> **⚠️ Wichtig:** Die App benötigt Geolocation (siehe Teil 0C). Der MCP Server nutzt chromium mit Standard-Permissions – die Aufgabe wird daher **nicht gespeichert**, wenn Geolocation nicht gewährt wurde. Beobachte, was passiert, und notiere, wie du dieses Problem in einem echten Test (Exercise 2) löst.

<details>
<summary>💡 Lösungshinweis</summary>

Der Agent wird Folgendes intern ausführen:
```
browser_navigate("http://localhost:3000")
browser_click(element: "What needs to be done?" input)
browser_type(text: "MCP Demo Task")
browser_click(element: "Add" button)
browser_screenshot()
```

Das Problem: `navigator.geolocation.getCurrentPosition()` schlägt ohne Berechtigung still fehl → kein Eintrag erscheint.

**Lesson learned:** Der MCP Server ist gut für Exploration, aber für produktive Tests brauchst du die explizite Geolocation-Konfiguration via `playwright.config.ts` oder `ContextOptions()` (wie in Exercise 2).

</details>

---

#### Aufgabe 3 – Testentwurf generieren lassen

**Aufgabe:** Bitte den Copilot Agent, basierend auf seiner Exploration einen TypeScript-Playwright-Test zu schreiben:

```
Basierend auf der TodoMatic-App auf http://localhost:3000: Schreibe mir einen
Playwright-Test in TypeScript, der prüft, dass alle drei Filter-Buttons ("All",
"Active", "Completed") sichtbar sind und angeklickt werden können.
Verwende getByTestId() für die Filter-Buttons.
```

> **Ziel:** Vergleiche den generierten Testentwurf mit deiner eigenen Lösung aus Exercise 5. Was hat der Agent gut gemacht? Was würdest du ändern?

<details>
<summary>💡 Lösungshinweis</summary>

Ein guter generierter Entwurf sollte ungefähr so aussehen:

```typescript
import { test, expect } from "@playwright/test";

test("filter buttons are visible and clickable", async ({ page }) => {
  await page.goto("/");

  const allFilter    = page.getByTestId("testID-All");
  const activeFilter = page.getByTestId("testID-Active");
  const doneFilter   = page.getByTestId("testID-Completed");

  await expect(allFilter).toBeVisible();
  await expect(activeFilter).toBeVisible();
  await expect(doneFilter).toBeVisible();

  await activeFilter.click();
  await expect(activeFilter).toHaveAttribute("aria-pressed", "true");

  await doneFilter.click();
  await expect(doneFilter).toHaveAttribute("aria-pressed", "true");

  await allFilter.click();
  await expect(allFilter).toHaveAttribute("aria-pressed", "true");
});
```

**Typische Agent-Fehler, die du korrigieren musst:**
- Fehlende `baseURL`-Konfiguration (absoluter statt relativer URL)
- Geolocation nicht berücksichtigt (wenn der Agent vorher gescheitert ist)
- Locatoren via CSS statt `getByTestId()` / `getByRole()` (fragiler)

**Für C#-Entwickler:** Kopiere den generierten TypeScript-Test und bitte Copilot:  
*"Übersetze diesen Playwright TypeScript-Test in C# mit NUnit"* – und vergleiche mit deiner Lösung aus Exercise 5.

</details>

---

#### Aufgabe 4 – Screenshot-Vergleich: MCP vs. Trace Viewer

**Aufgabe:** Führe folgende zwei Schritte aus und vergleiche die Diagnosewerkzeuge:

1. Lasse den MCP Agent die Seite öffnen und einen Screenshot machen:
   ```
   Öffne http://localhost:3000, warte bis die Seite geladen ist, und mache dann
   einen Screenshot der gesamten Seite.
   ```
2. Führe danach `npx playwright test --trace on` aus und öffne den Trace Viewer.

> **Reflexionsfrage:** Wann ist ein MCP-Screenshot (spontane Exploration) sinnvoll, und wann ist der Trace Viewer (vollständige Testreproduktion) das bessere Werkzeug?

<details>
<summary>💡 Lösungshinweis</summary>

| | MCP Server Screenshot | Trace Viewer |
|---|---|---|
| **Wann** | Schnelle Exploration, Ad-hoc-Diagnose | Nach Testlauf, Fehleranalyse |
| **Aufruf** | Natürlicher Prompt an Copilot | `npx playwright show-trace trace.zip` |
| **Inhalt** | Einzelbild der aktuellen Seite | Vollständiger Zeitstrahl: DOM, Network, Console, Screenshots |
| **Persistenz** | Nur im Chat sichtbar | ZIP-Datei, teilbar, in CI archivierbar |
| **Empfehlung** | Schneller DOM-Check während der Entwicklung | Post-mortem Analyse fehlgeschlagener Tests in CI |

</details>

---

### Teil C: Weiterführende MCP-Prompts

Experimentiere mit diesen Prompts für tiefere Erkundung:

```
# DOM-Struktur einer Todo-Karte analysieren
Klicke in der TodoMatic-App den "Edit"-Button der ersten Aufgabe an
und beschreibe die Eingabefelder, die erscheinen.

# Netzwerk-Aktivität beobachten
Klicke auf "Load remote tasks" in der TodoMatic-App und beschreibe,
welche HTTP-Anfrage ausgelöst wird und was die Antwort enthält.

# Responsiveness testen
Setze das Browser-Fenster auf 375x667 (iPhone SE) und mache einen
Screenshot der TodoMatic-App. Ist die App mobil nutzbar?
```

---

### Hilfreiche Links zum MCP Server

| Ressource | Link |
|---|---|
| Offizielles npm-Paket | [npmjs.com/@playwright/mcp](https://www.npmjs.com/package/@playwright/mcp) |
| GitHub-Repository | [github.com/microsoft/playwright-mcp](https://github.com/microsoft/playwright-mcp) |
| MCP-Protokoll Spezifikation | [modelcontextprotocol.io](https://modelcontextprotocol.io/) |
| VS Code MCP-Konfiguration | [code.visualstudio.com/docs/copilot/chat/mcp-servers](https://code.visualstudio.com/docs/copilot/chat/mcp-servers) |
| GitHub Copilot Agent Mode | [docs.github.com/copilot/agent-mode](https://docs.github.com/en/copilot/using-github-copilot/agents/using-github-copilot-agent-mode) |

---

## Zusammenfassung: Gelerntes auf einen Blick

| Konzept | TypeScript API | C# API | Framework | Übung |
|---|---|---|---|---|
| Navigation | `page.goto()` | `Page.GotoAsync()` | alle | 1–14 |
| ARIA-Locatoren | `getByRole()`, `getByTestId()` | `GetByRole()`, `GetByTestId()` | alle | 1, 3 |
| Formular | `fill()`, `click()`, `check()` | `FillAsync()`, `ClickAsync()` | alle | 2, 3 |
| Locator-Chaining | `.filter({ hasText })` | `.Filter(new() { HasText })` | alle | 3, 4 |
| Geolocation mock | `test.use({ geolocation })` | `ContextOptions()` override | alle | 2, 11 |
| Network mock | `page.route()` + `fulfill()` | `RouteAsync()` + `FulfillAsync()` | alle | 5, 6 |
| Response-Manipulation | `route.fetch()` | `route.FetchAsync()` | alle | 6 |
| **Screenshots** | `page.screenshot()` | `ScreenshotAsync()` | alle | **7** |
| **Video** | `video: "retain-on-failure"` | `PLAYWRIGHT_VIDEO=on` | alle | **7** |
| **Trace Viewer** | `show-trace trace.zip` | `playwright.ps1 show-trace` | alle | **7** |
| Mobile Emulation | `devices["iPhone 15 Pro"]` | `Playwright.Devices[...]` | alle | 8 |
| Cross-Browser TS | `projects` in Config | – | TypeScript | 9 |
| Cross-Browser C# | – | `[DataRow]`/`[TestCase]`/`[InlineData]` | MSTest/NUnit/xUnit | 9 |
| JS-Injektion | `page.evaluate()` | `Page.EvaluateAsync()` | alle | 10 |
| **Page Object Model** | Klasse + Properties + Methoden | Klasse + Properties + Methoden | alle | **11** |
| Codegen | `npx playwright codegen` | `pwsh playwright.ps1 codegen` | alle | Teil 2 |
| Inspector | `PWDEBUG=1` / `page.pause()` | `PWDEBUG=1` / `PauseAsync()` | alle | Teil 1 |
| **Code-Driven** | Von Hand schreiben | Von Hand schreiben | alle | **Teil 2** |
| GitHub Actions | YAML | YAML | alle | 12 |
| **Azure Pipelines** | YAML | YAML | alle | **12** |
| **Docker** | Dockerfile | Multi-Stage Dockerfile | alle | **12** |
| **Azure PW Service** | `playwright.service.config.ts` | `PlaywrightServiceTest` (NUnit) | TS / NUnit | **13** |
| **Playwright MCP Server** | `@playwright/mcp` + MCP-Config | *(Node.js-basiert, kein C# SDK)* | VS Code + Copilot | **14** |

---

## Weiterführende Ressourcen

### 🎭 Playwright – Kern-Dokumentation

| Ressource | Link |
|---|---|
| Getting Started (TypeScript) | [playwright.dev/docs/intro](https://playwright.dev/docs/intro) |
| Getting Started (.NET / C#) | [playwright.dev/dotnet/docs/intro](https://playwright.dev/dotnet/docs/intro) |
| API-Referenz (TypeScript) | [playwright.dev/docs/api/class-playwright](https://playwright.dev/docs/api/class-playwright) |
| API-Referenz (.NET) | [playwright.dev/dotnet/docs/api/class-playwright](https://playwright.dev/dotnet/docs/api/class-playwright) |
| Release Notes | [playwright.dev/docs/release-notes](https://playwright.dev/docs/release-notes) |
| GitHub-Repository | [github.com/microsoft/playwright](https://github.com/microsoft/playwright) |

### 🔍 Locatoren & Assertions

| Ressource | Link |
|---|---|
| Locators (TS) | [playwright.dev/docs/locators](https://playwright.dev/docs/locators) |
| Locators (.NET) | [playwright.dev/dotnet/docs/locators](https://playwright.dev/dotnet/docs/locators) |
| Assertions (TS) | [playwright.dev/docs/test-assertions](https://playwright.dev/docs/test-assertions) |
| Assertions (.NET) | [playwright.dev/dotnet/docs/test-assertions](https://playwright.dev/dotnet/docs/test-assertions) |
| Best Practices Locatoren | [playwright.dev/docs/best-practices](https://playwright.dev/docs/best-practices) |
| ARIA-Roles (MDN) | [developer.mozilla.org/ARIA/Roles](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Roles) |

### 🌐 Browser-Features & Mocking

| Ressource | Link |
|---|---|
| Geolocation & Permissions | [playwright.dev/docs/emulation#geolocation](https://playwright.dev/docs/emulation#geolocation) |
| Network Mocking (TS) | [playwright.dev/docs/network](https://playwright.dev/docs/network) |
| Network Mocking (.NET) | [playwright.dev/dotnet/docs/network](https://playwright.dev/dotnet/docs/network) |
| Mobile Emulation | [playwright.dev/docs/emulation](https://playwright.dev/docs/emulation) |
| Emulierte Geräte-Liste | [github.com – deviceDescriptorsSource.json](https://github.com/microsoft/playwright/blob/main/packages/playwright-core/src/server/deviceDescriptorsSource.json) |
| JavaScript Evaluate | [playwright.dev/docs/evaluating](https://playwright.dev/docs/evaluating) |

### 🐛 Debugging & Diagnose-Werkzeuge

| Ressource | Link |
|---|---|
| Playwright Inspector & Debugger | [playwright.dev/docs/debug](https://playwright.dev/docs/debug) |
| Trace Viewer | [playwright.dev/docs/trace-viewer](https://playwright.dev/docs/trace-viewer) |
| Trace Viewer Intro (Guide) | [playwright.dev/docs/trace-viewer-intro](https://playwright.dev/docs/trace-viewer-intro) |
| Codegen (Test-Recorder) | [playwright.dev/docs/codegen](https://playwright.dev/docs/codegen) |
| VS Code Extension | [marketplace.visualstudio.com – Playwright Test](https://marketplace.visualstudio.com/items?itemName=ms-playwright.playwright) |
| Screenshots | [playwright.dev/docs/screenshots](https://playwright.dev/docs/screenshots) |
| Videos | [playwright.dev/docs/videos](https://playwright.dev/docs/videos) |

### 🤖 Playwright MCP Server

| Ressource | Link |
|---|---|
| `@playwright/mcp` (npm) | [npmjs.com/@playwright/mcp](https://www.npmjs.com/package/@playwright/mcp) |
| GitHub-Repository | [github.com/microsoft/playwright-mcp](https://github.com/microsoft/playwright-mcp) |
| MCP-Protokoll Spezifikation | [modelcontextprotocol.io](https://modelcontextprotocol.io/) |
| VS Code MCP-Server Konfiguration | [code.visualstudio.com – MCP Servers](https://code.visualstudio.com/docs/copilot/chat/mcp-servers) |
| GitHub Copilot Agent Mode | [docs.github.com – Agent Mode](https://docs.github.com/en/copilot/using-github-copilot/agents/using-github-copilot-agent-mode) |

### 🧪 Testing-Frameworks (.NET)

| Ressource | Link |
|---|---|
| Microsoft.Playwright.MSTest | [nuget.org/packages/Microsoft.Playwright.MSTest](https://www.nuget.org/packages/Microsoft.Playwright.MSTest) |
| Microsoft.Playwright.NUnit | [nuget.org/packages/Microsoft.Playwright.NUnit](https://www.nuget.org/packages/Microsoft.Playwright.NUnit) |
| Microsoft.Playwright.Xunit | [nuget.org/packages/Microsoft.Playwright.Xunit](https://www.nuget.org/packages/Microsoft.Playwright.Xunit) |
| NUnit Dokumentation | [docs.nunit.org](https://docs.nunit.org/) |
| xUnit Dokumentation | [xunit.net/docs](https://xunit.net/docs/getting-started/netcore/cmdline) |
| MSTest Dokumentation | [learn.microsoft.com – MSTest](https://learn.microsoft.com/dotnet/core/testing/unit-testing-with-mstest) |
| `.runsettings` Referenz | [learn.microsoft.com – Configure unit tests](https://learn.microsoft.com/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file) |

### 📊 Reporter & CI/CD

| Ressource | Link |
|---|---|
| HTML Reporter | [playwright.dev/docs/test-reporters#html-reporter](https://playwright.dev/docs/test-reporters#html-reporter) |
| Alle Reporter (TS) | [playwright.dev/docs/test-reporters](https://playwright.dev/docs/test-reporters) |
| Playwright in GitHub Actions | [playwright.dev/docs/ci-intro](https://playwright.dev/docs/ci-intro) |
| Playwright in Azure Pipelines | [playwright.dev/docs/ci#azure-pipelines](https://playwright.dev/docs/ci#azure-pipelines) |
| Playwright Docker-Images | [playwright.dev/docs/docker](https://playwright.dev/docs/docker) |
| Docker Hub – mcr.microsoft.com/playwright | [mcr.microsoft.com/product/playwright](https://mcr.microsoft.com/en-us/product/playwright/about) |

### ☁️ Azure Playwright Testing Service

| Ressource | Link |
|---|---|
| Übersicht | [learn.microsoft.com – Was ist der Microsoft Playwright Testing Service?](https://learn.microsoft.com/azure/playwright-testing/overview-what-is-microsoft-playwright-testing) |
| Schnellstart TypeScript | [learn.microsoft.com – Quickstart TS](https://learn.microsoft.com/azure/playwright-testing/quickstart-run-end-to-end-tests) |
| Schnellstart .NET | [learn.microsoft.com – Quickstart .NET](https://learn.microsoft.com/azure/playwright-testing/quickstart-run-end-to-end-tests-dotnet) |
| NuGet-Paket | [nuget.org/packages/Azure.Developer.MicrosoftPlaywrightTesting.NUnit](https://www.nuget.org/packages/Azure.Developer.MicrosoftPlaywrightTesting.NUnit) |
| npm-Paket | [npmjs.com/@azure/microsoft-playwright-testing](https://www.npmjs.com/package/@azure/microsoft-playwright-testing) |

### 🎓 Lernressourcen & Community

| Ressource | Link |
|---|---|
| norschel/PlaywrightDemos | [github.com/norschel/PlaywrightDemos](https://github.com/norschel/PlaywrightDemos) – Konferenz-Demos BASTA!/MDD/IT-Tage 2023–2026 |
| harrybin/todo-react-playwright | [github.com/harrybin/todo-react-playwright](https://github.com/harrybin/todo-react-playwright) – HOL-App |
| Playwright Learning Path (Microsoft) | [learn.microsoft.com – Playwright](https://learn.microsoft.com/training/modules/build-with-playwright/) |
| Playwright YouTube Channel | [youtube.com – Playwright](https://www.youtube.com/@Playwrightdev) |
| Playwright Discord | [aka.ms/playwright/discord](https://aka.ms/playwright/discord) |

---

*HOL erstellt für das [harrybin/todo-react-playwright](https://github.com/harrybin/todo-react-playwright) Repo – basierend auf echten Konferenz-Demos von [Nico Orschel](https://github.com/norschel) (norschel/PlaywrightDemos, BASTA! / MDD / IT-Tage 2023–2026)*
