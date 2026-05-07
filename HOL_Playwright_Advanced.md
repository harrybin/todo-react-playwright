# Hands-on Lab: Playwright End-to-End Testing mit der TodoMatic-App

> **Niveau:** Fortgeschrittene Bootcamp-Übung  
> **Dauer:** ca. 4–5 Stunden  
> **App-Repo:** [harrybin/todo-react-playwright](https://github.com/harrybin/todo-react-playwright)  
> **Referenz-Demos:** [norschel/PlaywrightDemos](https://github.com/norschel/PlaywrightDemos)

---

## 🌐 Sprachauswahl – TypeScript/JavaScript oder C# (.NET)

Dieses HOL unterstützt beide Technologie-Stacks vollständig. **Wähle einmalig deine Sprache** – alle Setup-Schritte, Debug-Tools und Lösungshinweise sind separat für jede Sprache aufklappbar.

| Deine Sprache | Was du öffnest / liest |
|---|---|
| 🟦 **TypeScript / JavaScript** | Blöcke mit 🟦 bzw. „💡 Lösungshinweis TypeScript" – alles andere überspringen |
| 🟣 **C# / .NET** | Blöcke mit 🟣 bzw. „💡 Lösungshinweis C# – MSTest/NUnit/xUnit" – alles andere überspringen |

> Die **Aufgabentexte** und **konzeptionellen Erklärungen** jeder Übung sind sprachunabhängig und für alle Teilnehmer relevant. Nur die konkreten Code-Beispiele und Setup-Schritte sind nach Sprache getrennt.

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

## Glossar: Playwright-Begriffe auf einen Blick

| Begriff | Bedeutung |
|---|---|
| **Locator** | Playwright-Selektor, der ein oder mehrere DOM-Elemente beschreibt. Anders als CSS-Selektoren sind Locatoren "lazy" (führen keine DOM-Suche bei Erstellung durch) und "strict" (schlagen fehl, wenn mehrere Elemente matchen). Beispiele: `getByRole()`, `getByTestId()`, `locator("#id")`. |
| **ARIA-Locator** | Locator, der auf barrierefreiheits-semantischen Attributen basiert (`role`, `name`, `label`). Robuster als CSS, weil er am UI aus Nutzersicht operiert: `getByRole("button", { name: "Add" })`. |
| **Fixture** | Wiederverwendbare Test-Ressource, die Playwright vor/nach jedem Test aufbaut und abbaut. Eingebaute Fixtures: `page`, `context`, `browser`, `browserName`. Mit `test.extend()` können eigene Fixtures definiert werden. |
| **`test.use()`** | Überschreibt Fixture-Konfiguration für alle Tests in einer Datei oder einem `describe`-Block (z. B. `geolocation`, `video`, `viewport`). |
| **`page.route()`** | Registriert einen Interceptor für HTTP-Requests. Der Callback kann den Request abfangen (`route.fulfill()`), weiterleiten (`route.continue()`) oder erst senden und dann modifizieren (`route.fetch()` + `route.fulfill()`). |
| **Trace** | Eine komprimierte `.zip`-Datei, die alle Playwright-Aktionen eines Tests aufzeichnet: DOM-Snapshots, Screenshots, Netzwerk-Requests, Konsolen-Ausgaben und Timing. Öffenbar im Trace Viewer. |
| **Trace Viewer** | Interaktiver Browser-basierter Viewer für Trace-Dateien. Zeigt einen Zeitstrahl aller Aktionen mit DOM-State zu jedem Schritt. Starten mit `npx playwright show-trace trace.zip`. |
| **Codegen** | Playwright Test Recorder – zeichnet manuelle Browser-Aktionen auf und generiert daraus TypeScript- oder C#-Testcode. Starten mit `npx playwright codegen http://localhost:3000`. |
| **Inspector** | Interaktiver Debugger, der öffnet, wenn `PWDEBUG=1` gesetzt oder `await page.pause()` aufgerufen wird. Zeigt den aktuellen DOM, ermöglicht schrittweise Ausführung und Locator-Exploration. |
| **UI Mode** | Modernes lokales Debugging-Tool (seit Playwright 1.32). Öffnet eine eigene Oberfläche mit Test-Baum, Watch-Mode, live DOM-Snapshot und Locator Picker. Start: `npx playwright test --ui`. |
| **PageTest** | Basisklasse für C#-Tests (`Microsoft.Playwright.MSTest.PageTest` / `NUnit.PageTest` / `Xunit.PageTest`). Stellt `Page`, `Context`, `Browser` und `Playwright` als Properties zur Verfügung. |
| **Page Object Model (POM)** | Design-Pattern: Locatoren und Aktionen für eine Seite werden in einer eigenen Klasse gekapselt. Tests nutzen nur die Methoden der Page-Object-Klasse, keine rohen Locatoren. Verbessert Wartbarkeit und Lesbarkeit. |
| **Headless / Headed** | *Headless*: Browser ohne sichtbares Fenster (Standard in CI). *Headed*: Browser mit sichtbarem Fenster (Standard lokal für Debugging). Umschalten: `npx playwright test --headed` / `HEADED=1 dotnet test`. |
| **`webServer`** | Konfiguration in `playwright.config.ts`, die einen lokalen Dev-Server automatisch vor den Tests startet und danach beendet. Entspricht `npm run dev`. Für C# gibt es kein Äquivalent – App muss manuell gestartet werden. |
| **Sharding** | Aufteilung der Test-Suite auf mehrere parallele Prozesse oder Maschinen. TypeScript: `--shard=1/4`. Azure Playwright Testing Service ermöglicht bis zu 50 parallele Shard-Container. |
| **`reuseExistingServer`** | In `playwright.config.ts webServer`: Bei `true` wird kein neuer Server gestartet, wenn Port 3000 bereits belegt ist. In CI sollte dieser Wert `false` sein (`!process.env.CI`), damit kein veralteter Server genutzt wird. |
| **MCP Server** | *Model Context Protocol Server* – ermöglicht KI-Assistenten (GitHub Copilot, Claude), Playwright-Browser-Tools per natürlicher Sprache zu steuern. Ergänzt den manuellen Testansatz für Exploration und Entwurf. |
| **Playwright CLI** | Token-effizientes CLI-Interface für Browser-Automatisierung (`@playwright/cli`). Stellt einzelne, fokussierte Befehle bereit (`open`, `click`, `fill`, `snapshot`, `route`, …), die Coding-Agents direkt aufrufen können, ohne großes Tool-Schema-Overhead. Gegenstück zum MCP Server: lieber viele kleine Befehle statt eines großen persistenten Tool-Kontexts. |

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

<details>
<summary>🟣 Nur relevant für C# / .NET – Visual Studio 2022</summary>

1. Stelle sicher, dass das **.NET 8 SDK** installiert ist
2. Öffne den **Test Explorer** (`Test → Test Explorer`)
3. Playwright-Tests erscheinen dort automatisch nach dem Build
4. Für den integrierten Debugger: Breakpoints setzen → Rechtsklick im Test Explorer → **Debug**

> **Tipp:** Visual Studio bietet keinen Playwright Codegen direkt, aber du kannst ihn per PowerShell-Skript aufrufen (siehe Übung 0B).

</details>

---

## Teil 0B: Projekt-Setup

<details open>
<summary>🟦 TypeScript / JavaScript – Setup</summary>

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
  retries: process.env.CI ? 2 : 0,
  reporter: [["html"], ["list"]],
  use: {
    baseURL: "http://localhost:3000",
    trace: "on-first-retry",
    screenshot: "only-on-failure",
    // Geolocation global – die App benötigt sie für addTask().
    // Tests ohne addTask() werden durch das Grant nicht beeinträchtigt.
    geolocation: { latitude: 48.1372, longitude: 11.5755 },
    permissions: ["geolocation"],
  },
  projects: [
    { name: "chromium", use: { ...devices["Desktop Chrome"] } },
  ],
  webServer: {
    command: "npm run dev",
    url: "http://localhost:3000",
    reuseExistingServer: !process.env.CI,
  },
});
```

```bash
mkdir tests
npx playwright test --list  # Setup prüfen
```

</details>

---

<details>
<summary>🟣 C# / .NET – Setup</summary>

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
# Playwright-Browser installieren – Pfad hängt vom Build-Ordner ab:
pwsh bin/Debug/net8.0/playwright.ps1 install   # .NET 8, Debug
# pwsh bin/Debug/net9.0/playwright.ps1 install # .NET 9
# Alternativ (framework-unabhängig):
# dotnet tool install --global Microsoft.Playwright.CLI && playwright install
```

**NUnit einrichten:**

```bash
dotnet new nunit -n TodoPlaywrightTests && cd TodoPlaywrightTests
dotnet add package Microsoft.Playwright.NUnit
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install   # Pfad wie oben
```

**xUnit einrichten:**

```bash
dotnet new xunit -n TodoPlaywrightTests && cd TodoPlaywrightTests
dotnet add package Microsoft.Playwright.Xunit
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install   # Pfad wie oben
```

**Gemeinsame Basisklasse** (`TestBase.cs`) – einmal definieren, von allen Testklassen erben:

> ℹ️ **Geolocation global:** Die TodoMatic-App benötigt `navigator.geolocation` für `addTask()`. Die Basisklasse setzt dies einmalig – alle abgeleiteten Testklassen erben die Konfiguration automatisch. Für Exercises, in denen keine Aufgabe hinzugefügt wird, schadet das Grant nicht.

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
        // Geolocation global – benötigt von addTask() in allen Tests
        Geolocation = new Geolocation { Latitude = 48.1372f, Longitude = 11.5755f },
        Permissions = new[] { "geolocation" },
    };
}
```

```csharp
// NUnit – Attribut und Namespace ändern, Rest identisch
using Microsoft.Playwright.NUnit;
[TestFixture]
public class TestBase : PageTest
{
    public override BrowserNewContextOptions ContextOptions() => new()
    {
        BaseURL = Environment.GetEnvironmentVariable("PLAYWRIGHT_BASE_URL")
                  ?? "http://localhost:3000",
        Geolocation = new Geolocation { Latitude = 48.1372f, Longitude = 11.5755f },
        Permissions = new[] { "geolocation" },
    };
}
```

```csharp
// xUnit – kein Klassenattribut
using Microsoft.Playwright.Xunit;
public class TestBase : PageTest
{
    public override BrowserNewContextOptions ContextOptions() => new()
    {
        BaseURL = Environment.GetEnvironmentVariable("PLAYWRIGHT_BASE_URL")
                  ?? "http://localhost:3000",
        Geolocation = new Geolocation { Latitude = 48.1372f, Longitude = 11.5755f },
        Permissions = new[] { "geolocation" },
    };
}
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

</details>

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

<details open>
<summary>🟦 TypeScript / JavaScript</summary>

```bash
# Codegen direkt starten
npx playwright codegen http://localhost:3000

# Oder in VS Code: Seitenleiste → Testing (Beaker) → "Record new" Button
```

</details>

<details>
<summary>🟣 C# / .NET</summary>

```powershell
# PowerShell
pwsh bin/Debug/net8.0/playwright.ps1 codegen http://localhost:3000
```

</details>

**Was passiert:**
- Ein Browser öffnet sich mit der App
- Ein separates Fenster zeigt den generierten Code in Echtzeit
- Klicke, tippe, navigiere – Codegen übersetzt alles in Test-Code
- Kopiere den generierten Code als Startpunkt für deine Tests

> **Übung:** Starte Codegen, füge eine neue Aufgabe hinzu, markiere sie als abgeschlossen, und lösche sie. Betrachte den generierten Code. Welche Locator-Strategien wählt Codegen automatisch?

---

### 🔍 Tool 2: Playwright Inspector (PWDEBUG)

Der Inspector erlaubt Step-by-Step-Debugging direkt im Browser.

<details open>
<summary>🟦 TypeScript / JavaScript</summary>

```bash
# Vor dem Test-Aufruf setzen
PWDEBUG=1 npx playwright test smoke.spec.ts

# Windows PowerShell:
$env:PWDEBUG=1; npx playwright test smoke.spec.ts
```

```typescript
// Alternativ im Code (hält den Test an):
await page.pause(); // öffnet den Inspector an dieser Stelle
```

</details>

<details>
<summary>🟣 C# / .NET</summary>

```bash
# Umgebungsvariable setzen, dann normal testen
$env:PWDEBUG=1; dotnet test --filter "SmokeTest"
```

```csharp
// Alternativ im Code (hält den Test an):
await Page.PauseAsync(); // öffnet den Inspector an dieser Stelle
```

</details>

**Features des Inspectors:**
- **Step over**: Test Schritt für Schritt ausführen
- **Locator Explorer**: Locatoren direkt auf der Seite ausprobieren
- **Pick locator**: Element anklicken → Inspector zeigt den besten Locator

---

### 📊 Tool 3: Playwright Trace Viewer

Der Trace Viewer ist ein vollständiger Zeitstrahl des Tests – mit DOM-Snapshots, Netzwerk-Requests und Screenshots zu jedem Schritt.

<details open>
<summary>🟦 TypeScript / JavaScript</summary>

```bash
npx playwright show-trace test-results/pfad-zum-test/trace.zip
```

```typescript
// playwright.config.ts – Trace aktivieren:
use: {
  trace: "on",               // immer
  // trace: "on-first-retry" // nur beim Retry (empfohlen für CI)
  // trace: "retain-on-failure"
}
```

</details>

<details>
<summary>🟣 C# / .NET</summary>

```powershell
pwsh bin/Debug/net8.0/playwright.ps1 show-trace test-results/trace.zip
```

```csharp
// Trace per Umgebungsvariable vor dem Test aktivieren:
Environment.SetEnvironmentVariable("PLAYWRIGHT_TRACE", "on");
// Oder manuell im Test (siehe Exercise 7)
```

</details>

**In VS Code:** Nach einem fehlgeschlagenen Test erscheint in der Test-Ergebnis-Ansicht ein **"Show Trace"**-Link, der den Trace direkt in VS Code öffnet.

---

### 🌐 Tool 4: Browser DevTools

Playwright kann die Browser DevTools für Debugging-Sessions aktivieren.

<details open>
<summary>🟦 TypeScript / JavaScript</summary>

```typescript
// Browser im sichtbaren Modus + DevTools öffnen
test.use({ headless: false, launchOptions: { devtools: true } });
```

</details>

<details>
<summary>🟣 C# / .NET</summary>

```csharp
// In playwright.config.json oder per LaunchOptions
public override BrowserTypeLaunchOptions LaunchOptions =>
    new() { Headless = false, Devtools = true };
```

</details>

**Nützlich für:**
- JavaScript-Fehler in der Konsole prüfen (`page.on('console', ...)`)
- Netzwerk-Traffic live beobachten
- CSS-Selektoren in der DevTools-Konsole ausprobieren: `$$('[data-testid]')`

---

### 🖥️ Tool 5: Playwright UI Mode

Der UI Mode (seit Playwright 1.32) ist das mächtigste lokale Debug-Werkzeug. Er öffnet eine eigene Oberfläche, in der du Tests verwalten, einzeln starten, in Echtzeit beobachten und direkt debuggen kannst.

**Starten:**

```bash
npx playwright test --ui

# Im package.json bereits vorkonfiguriert:
npm run test:ui
```

**Features auf einen Blick:**

| Feature | Beschreibung |
|---|---|
| **Test-Baum** | Alle Tests nach Dateien und Suites – per Klick einzeln starten |
| **Watch-Mode** | Datei speichern → Test läuft sofort neu (Live-Feedback) |
| **Timeline** | Visueller Zeitstrahl aller Aktionen, live während der Ausführung |
| **DOM-Snapshot** | Klick auf jeden Schritt → exakter DOM-Zustand zu dem Zeitpunkt |
| **Locator Picker** | Klick auf Element im Browser → UI Mode schlägt besten Locator vor |
| **Netzwerk-Tab** | HTTP-Requests und Responses live sehen |
| **Console** | Browser-Konsolenausgaben direkt eingebettet |

> **💡 Empfehlung:** Nutze den UI Mode als primäres Werkzeug während der Entwicklung – er vereint Codegen, Inspector und Trace Viewer in einer Oberfläche.

> **C# / .NET:** Der UI Mode ist aktuell nur für TypeScript/JavaScript verfügbar. Für C#-Tests bleibt `PWDEBUG=1` + Inspector der Standard-Workflow.

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

> 🤔 **Stop & Think:** Warum ist `getByRole("heading", { name: "TodoMatic" })` robuster als `locator("h2")`? Welche App-Änderung würde den ersten Locator überleben, den zweiten aber brechen?

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

> ℹ️ **Globale Konfiguration:** Die `playwright.config.ts` dieser HOL enthält bereits `geolocation` und `permissions` global in `use:`. Der `test.use()`-Block hier zeigt, wie man es **pro Datei** überschreibt – z. B. für einen anderen Ort. In späteren Exercises entfällt er.

```typescript
import { test, expect } from "@playwright/test";

// Beispiel: Geolocation per-Datei auf einen anderen Ort überschreiben
// (In dieser HOL nicht nötig – globale Config reicht aus)
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

> ℹ️ **Globale Konfiguration:** Die `TestBase`-Klasse setzt `Geolocation` und `Permissions` bereits. Alle Testklassen, die `TestBase` statt `PageTest` erweitern, erben diese Konfiguration – kein `ContextOptions()`-Override nötig.

```csharp
// TestBase erbt bereits Geolocation + BaseURL – kein Override nötig
[TestClass]
public class AddTaskTests : TestBase
{
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

> 🤔 **Stop & Think:** Was würde ohne Geolocation-Mock passieren? Würde der Test sofort fehlschlagen oder nach einem Timeout – und welches Timeout würde greifen?

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

// Kein test.use() nötig – Geolocation ist global in playwright.config.ts konfiguriert

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
// TestBase erbt Geolocation + BaseURL – kein ContextOptions()-Override nötig
[TestClass]
public class TaskManagementTests : TestBase
{
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

> 🤔 **Stop & Think:** Warum ist `.filter({ hasText })` / `.Filter(new() { HasText })` besser als einfach `getByText("Edit").click()`? Was passiert, wenn zwei Aufgaben gleichzeitig in der Liste sind?

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

// Kein test.use() nötig – Geolocation ist global in playwright.config.ts konfiguriert

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
// TestBase erbt Geolocation + BaseURL – kein ContextOptions()-Override nötig
[TestClass]
public class FilterTests : TestBase
{
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

> 🤔 **Stop & Think:** Der Filter zeigt immer die aktuelle Liste – aber wie könnte dieser Test fehlschlagen, wenn ein anderer parallel laufender Test ebenfalls Aufgaben hinzufügt? Wie verhindert Playwright das bei `fullyParallel: true`?

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

> 🤔 **Stop & Think:** Warum ist Test B (HTTP 500 simulieren) in einem echten Projekt besonders wichtig? Was passiert in der App, wenn der echte Server einen 500er zurückgibt – und wie könntest du das mit `page.on("console", ...)` überprüfen?

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

> 🤔 **Stop & Think:** Was ist der Unterschied zwischen `route.fulfill()` und `route.fetch()` + `route.fulfill(response, { body: ... })`? In welchem Szenario brauchst du zwingend die zweite Variante?

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
  // Geolocation aus globalem playwright.config.ts – kein Eintrag hier nötig
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

// Kein test.use() nötig – Geolocation ist global in playwright.config.ts konfiguriert

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
// TestBase erbt Geolocation + BaseURL – kein ContextOptions()-Override nötig
[TestClass]
public class TraceTests : TestBase
{
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

> 🤔 **Stop & Think:** Wann würdest du `trace: "on"` statt `trace: "on-first-retry"` in der Produktion wählen – und welchen Trade-off gehst du dabei ein (Speicherplatz, Performance)?

---

### Teil D: Visual Regression – `toHaveScreenshot()` (Bonus)

**Ziel:** Pixel-genaue Screenshot-Vergleiche automatisieren. Playwright speichert beim ersten Lauf Referenz-Screenshots ("Snapshots") und vergleicht bei jedem weiteren Lauf das aktuelle UI dagegen.

**Aufgabe:**
1. Schreibe einen Test, der einen Screenshot der Startseite mit `toHaveScreenshot()` vergleicht
2. Führe den Test zum ersten Mal aus – Playwright erstellt die Referenz-Datei automatisch
3. Ändere etwas am UI (z. B. Hintergrundfarbe in `src/styles.css`) und führe den Test erneut aus
4. Beobachte, wie Playwright einen Diff anzeigt und der Test fehlschlägt
5. Aktualisiere die Snapshots mit `--update-snapshots`

> 📚 **Docs:** [Visual Comparisons (TS)](https://playwright.dev/docs/test-snapshots) · [Visual Comparisons (.NET)](https://playwright.dev/dotnet/docs/test-snapshots)

<details>
<summary>💡 Lösungshinweis TypeScript</summary>

```typescript
import { test, expect } from "@playwright/test";

test("initial page matches screenshot", async ({ page }) => {
  await page.goto("/");

  // Beim ersten Lauf: Playwright erstellt tests/snapshots/initial-state.png
  // Bei jedem Folgelauf: Pixel-Vergleich gegen dieses Referenz-Bild
  await expect(page).toHaveScreenshot("initial-state.png", {
    fullPage: true,
    // Toleranz für Anti-Aliasing und Font-Rendering-Unterschiede
    maxDiffPixelRatio: 0.02,
  });
});

test("task list after adding item", async ({ page }) => {
  await page.goto("/");
  await page.locator("#new-todo-input").fill("Visual Regression Task");
  await page.locator("#myUniqueID").click();
  await expect(page.getByRole("list").getByText("Visual Regression Task")).toBeVisible();

  // Nur die Aufgabenliste vergleichen (nicht die ganze Seite)
  await expect(page.getByRole("list")).toHaveScreenshot("task-list.png");
});
```

```bash
# Ersten Lauf: Snapshots erstellen
npx playwright test visual.spec.ts

# Snapshots nach einer bewussten UI-Änderung aktualisieren
npx playwright test visual.spec.ts --update-snapshots

# Im HTML-Report: Diff-Bild zeigt rote Pixel für Abweichungen
npx playwright show-report
```

> **Wo werden Snapshots gespeichert?** In `tests/__snapshots__/<test-name>/<snapshot-name>.png`. Diese Dateien **committen** – sie sind deine Baseline und müssen im Repository versioniert sein.

</details>

<details>
<summary>💡 Lösungshinweis C#</summary>

```csharp
// TestBase erbt Geolocation + BaseURL
[TestClass]
public class VisualRegressionTests : TestBase
{
    [TestMethod]
    public async Task InitialPageMatchesSnapshot()
    {
        await Page.GotoAsync("/");

        // Screenshot aufnehmen und als Basis speichern
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "snapshots/initial-state.png",
            FullPage = true,
        });

        // Hinweis: Playwright .NET hat kein eingebautes toHaveScreenshot() –
        // für Pixel-Vergleich empfiehlt sich eine Bibliothek wie ImageSharp
        // oder der Vergleich im Playwright HTML-Report über die Screenshots.
        // Alternativ: externe Visual-Testing-Services (z. B. Percy, Applitools).
    }
}
```

> ℹ️ **C#-Hinweis:** `toHaveScreenshot()` ist aktuell nur in der TypeScript-API verfügbar. Für C#-Projekte gibt es keine eingebaute Pixel-Vergleichs-API – nutze externe Bibliotheken oder TypeScript für Visual Regression Tests.

</details>

> 🤔 **Stop & Think:** Wo ist Visual Regression Testing sinnvoll und wo kann es zur "Flakiness-Falle" werden? Wie gehst du mit Snapshot-Abweichungen durch Rendering-Unterschiede zwischen Betriebssystemen um?

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

// Nur das Geräteprofil setzen – Geolocation kommt aus playwright.config.ts global
test.use({
  ...devices["iPhone 15 Pro"],
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
// TestBase setzt BaseURL + Geolocation; wir überschreiben nur für das Geräteprofil
[TestClass]
public class MobileTests : TestBase
{
    public override BrowserNewContextOptions ContextOptions()
    {
        // iPhone 15 Pro Geräteprofil – BaseURL und Geolocation aus TestBase
        var baseOpts = base.ContextOptions();
        var iPhone = Playwright.Devices["iPhone 15 Pro"];
        return new BrowserNewContextOptions(iPhone)
        {
            BaseURL = baseOpts.BaseURL,
            Geolocation = baseOpts.Geolocation,
            Permissions = baseOpts.Permissions,
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

> 🤔 **Stop & Think:** Welche App-Verhaltensweisen würden auf einem echten Gerät anders sein als in der Emulation? Nenne drei konkrete Punkte, die die Emulation nicht abdecken kann.

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

        // Neue Instanz je Browser – BrowserTypeLaunchOptions ist eine reguläre C#-Klasse
        // ohne Copy-Infrastruktur für with-Ausdrücke (kein Record, kein Clone-Mechanismus).
        IBrowser browser = browserName switch
        {
            "Chromium" => await playwright.Chromium.LaunchAsync(
                              new() { Headless = true }),
            "Firefox"  => await playwright.Firefox.LaunchAsync(
                              new() { Headless = true }),
            "Webkit"   => await playwright.Webkit.LaunchAsync(
                              new() { Headless = true }),
            "Edge"     => await playwright.Chromium.LaunchAsync(
                              new() { Headless = true, Channel = "msedge" }),
            "Chrome"   => await playwright.Chromium.LaunchAsync(
                              new() { Headless = true, Channel = "chrome" }),
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

> 🤔 **Stop & Think:** Warum laufen die drei Browser in dieser Übung sequenziell (`[DataRow]`) statt parallel? Wie würdest du sie in GitHub Actions parallel in einer Matrix ausführen?

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

// Geolocation aus globalem playwright.config.ts; headless: false nur für lokales Debugging
test.use({
  headless: false, // für DevTools-Debugging – entfernen für CI
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

> 🤔 **Stop & Think:** Wann ist `page.evaluate()` die bessere Wahl gegenüber einem normalen Locator – und wann ist es ein Anti-Pattern, das du vermeiden solltest?

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

// Kein test.use() nötig – Geolocation ist global in playwright.config.ts konfiguriert

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
// TestBase erbt Geolocation + BaseURL – kein ContextOptions()-Override nötig
[TestClass]
public class PomTests : TestBase
{
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

**NUnit:** `[TestFixture]` + `[Test]` + `Microsoft.Playwright.NUnit.PageTest` als Basis für `TestBase`  
**xUnit:** kein Klassenattribut + `[Fact]` + `Microsoft.Playwright.Xunit.PageTest` als Basis für `TestBase`  
**Das `TodoPage`-Objekt selbst bleibt identisch** – es ist framework-unabhängig.

</details>

> 🤔 **Stop & Think:** Wenn du `#myUniqueID` im `TodoPage`-Konstruktor zu `#add-task-btn` änderst, wie viele Test-Dateien müsstest du anfassen? Was wäre der Unterschied, wenn du kein POM hättest?

---

## Exercise 12: CI/CD – GitHub Actions, Azure Pipelines und Docker

**Ziel:** Playwright-Tests in drei verschiedenen Ausführungsumgebungen automatisieren. Wähle die Variante, die zu deiner Infrastruktur passt.

> 📚 **Docs:** [Playwright in CI (Übersicht)](https://playwright.dev/docs/ci) · [GitHub Actions](https://playwright.dev/docs/ci-intro) · [Azure Pipelines](https://playwright.dev/docs/ci#azure-pipelines) · [Docker](https://playwright.dev/docs/docker) · [Playwright-Artefakte in CI](https://playwright.dev/docs/ci#artifacts)

**Hintergrund – Warum CI für Playwright-Tests?**

Lokale Playwright-Tests laufen mit sichtbarem Browser auf deiner Maschine. In CI/CD-Pipelines gibt es keinen Display, kein GUI. Playwright läuft dort **headless** (ohne sichtbares Browser-Fenster). Drei Dinge sind in CI besonders wichtig:

| Problem | Lösung |
|---|---|
| Browser nicht installiert | `npx playwright install --with-deps` / `playwright.ps1 install --with-deps` installiert Chromium, Firefox, WebKit + alle System-Abhängigkeiten |
| App muss laufen | `webServer` in `playwright.config.ts` (TS) startet die App automatisch; bei .NET: App separat starten (`npm run build && npx serve dist &`) |
| Keine Traces bei Fehler | Artefakte (`playwright-report`, `test-results`) hochladen – dann kannst du den Trace Viewer auf dem Artefakt öffnen |

**Aufgabe:** Implementiere die CI-Pipeline mit einer der drei Varianten (oder alle drei zum Vergleich):
1. Trigger auf Push und Pull Request auf `main`
2. App starten, Tests ausführen
3. Artefakte (Reports, Screenshots, Traces) speichern

---

### Variante A: GitHub Actions

> 📚 **Docs:** [GitHub Actions – Quickstart](https://docs.github.com/actions/writing-workflows/quickstart) · [actions/checkout](https://github.com/actions/checkout) · [actions/setup-node](https://github.com/actions/setup-node) · [actions/upload-artifact](https://github.com/actions/upload-artifact) · [Playwright CI Intro](https://playwright.dev/docs/ci-intro)

**Schlüsselkonzepte:**

| YAML-Element | Bedeutung |
|---|---|
| `on: push / pull_request` | Trigger: Pipeline läuft bei jedem Push auf `main` und bei allen Pull Requests |
| `runs-on: ubuntu-latest` | GitHub-gehosteter Runner (Linux VM mit Ubuntu) – kostenlos für öffentliche Repos |
| `actions/checkout@v4` | Klont das Repository in den Runner-Workspace |
| `actions/setup-node@v4` | Installiert Node.js in der angegebenen Version; `cache: npm` beschleunigt Folge-Runs |
| `npm ci` | Installiert Abhängigkeiten exakt nach `package-lock.json` – reproduzierbarer als `npm install` |
| `npx playwright install --with-deps` | Lädt Playwright-Browser-Binaries + alle Linux-Systemabhängigkeiten (libglib, libnspr, ...) herunter |
| `npx playwright test` | Führt alle Tests headless aus; Playwright startet die App via `webServer` in der Config automatisch |
| `upload-artifact` mit `if: always()` | Report wird auch bei Testfehlern hochgeladen – wichtig für Post-mortem Analyse |
| `upload-artifact` mit `if: failure()` | Traces und Screenshots nur bei Fehler hochladen spart Speicherplatz |
| `retention-days` | Wie lange Artefakte aufbewahrt werden (14 Tage für Report, 7 für Debug-Artefakte) |

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
      # 1. Repo auschecken
      - uses: actions/checkout@v4

      # 2. Node.js installieren (mit npm-Cache für schnellere Folge-Runs)
      - uses: actions/setup-node@v4
        with: { node-version: 20, cache: npm }

      # 3. Abhängigkeiten exakt nach package-lock.json installieren
      - run: npm ci

      # 4. Playwright-Browser + System-Abhängigkeiten installieren
      #    --with-deps: installiert auch libglib, libnss, libnspr usw. auf Ubuntu
      - run: npx playwright install --with-deps

      # 5. Tests ausführen (App startet automatisch via webServer in playwright.config.ts)
      - run: npx playwright test

      # 6. HTML-Report immer hochladen (auch bei Fehlern) → im Actions-Tab herunterladbar
      - uses: actions/upload-artifact@v4
        if: always()
        with: { name: playwright-report, path: playwright-report/, retention-days: 14 }

      # 7. Test-Artefakte (Traces, Screenshots) nur bei Fehlern hochladen
      - uses: actions/upload-artifact@v4
        if: failure()
        with: { name: test-results, path: test-results/, retention-days: 7 }
```

> **💡 Tipp:** Nach einem fehlgeschlagenen Run lade das Artefakt `playwright-report` herunter und öffne `index.html` lokal – du siehst den vollständigen Trace Viewer direkt im Browser.

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
      # 1. Repo auschecken
      - uses: actions/checkout@v4

      # 2. .NET 8 SDK installieren
      - uses: actions/setup-dotnet@v4
        with: { dotnet-version: 8.x }

      # 3. Node.js für die React-App benötigt
      - uses: actions/setup-node@v4
        with: { node-version: 20 }

      # 4. React-App bauen und als statischen Server starten (im Hintergrund &)
      #    npx serve dist startet einen einfachen HTTP-Server auf Port 3000
      - name: Build and start Todo App
        run: npm ci && npm run build && npx serve dist -p 3000 &
        working-directory: ./todo-react-playwright

      # 5. .NET-Testprojekt bauen
      - run: dotnet build TodoPlaywrightTests/

      # 6. Playwright-Browser installieren – Pfad hängt vom Build-Output-Ordner ab.
      #    Alternativ: dotnet tool install -g Microsoft.Playwright.CLI && playwright install --with-deps
      - run: pwsh TodoPlaywrightTests/bin/Debug/net8.0/playwright.ps1 install --with-deps

      # 7. Tests ausführen – TRX-Format für PublishTestResults kompatibel
      - run: dotnet test TodoPlaywrightTests/ --logger trx --results-directory TestResults/
        env: { PLAYWRIGHT_BASE_URL: "http://localhost:3000" }

      # 8. TRX-Ergebnisse immer hochladen
      - uses: actions/upload-artifact@v4
        if: always()
        with: { name: test-results-dotnet, path: TestResults/, retention-days: 14 }

      # 9. Playwright-Artefakte (Traces, Screenshots, Videos) nur bei Fehlern
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

> **Tipp aus PlaywrightDemos:** Nutze `[TestCategory("CICD")]` (MSTest) / `[Category("CICD")]` (NUnit) / `[Trait("Category","CICD")]` (xUnit), um nur produktionsreife Tests in CI auszuführen: `dotnet test --filter "TestCategory=CICD"`.

</details>

---

### Variante B: Azure Pipelines

> 📚 **Docs:** [Azure Pipelines – Einstieg](https://learn.microsoft.com/azure/devops/pipelines/get-started/pipelines-get-started) · [YAML-Schema Referenz](https://learn.microsoft.com/azure/devops/pipelines/yaml-schema/) · [PublishTestResults@2](https://learn.microsoft.com/azure/devops/pipelines/tasks/test/publish-test-results) · [PublishPipelineArtifact@1](https://learn.microsoft.com/azure/devops/pipelines/tasks/utility/publish-pipeline-artifact) · [Playwright Azure Pipelines](https://playwright.dev/docs/ci#azure-pipelines)

**Unterschiede zu GitHub Actions:**

| Konzept | GitHub Actions | Azure Pipelines |
|---|---|---|
| Trigger | `on: push` | `trigger: [main]` |
| Runner | `runs-on: ubuntu-latest` | `pool: { vmImage: ubuntu-latest }` |
| Steps | `- uses:` / `- run:` | `- task:` / `- script:` |
| Test-Ergebnisse | Artefakt herunterladen | `PublishTestResults@2` – integriert in Azure DevOps UI |
| Artefakte | `upload-artifact` | `PublishPipelineArtifact@1` |
| Fehler-Verhalten | `if: failure()` | `condition: failed()` / `continueOnError: true` |

> **`continueOnError: true`** ist bei Tests wichtig: Auch wenn Tests fehlschlagen (Exit-Code ≠ 0), soll die Pipeline weiterlaufen, um Ergebnisse und Artefakte zu veröffentlichen.

<details>
<summary>💡 TypeScript – Azure Pipelines</summary>

`azure-pipelines.yml`:

```yaml
trigger:
  - main

pool:
  vmImage: ubuntu-latest

steps:
  # 1. Node.js in der gewünschten Version bereitstellen
  - task: NodeTool@0
    inputs: { versionSpec: "20.x" }
    displayName: Install Node.js

  # 2. Abhängigkeiten installieren (npm ci = deterministisch, kein npm install)
  - script: npm ci
    displayName: Install dependencies

  # 3. Browser + System-Abhängigkeiten installieren
  - script: npx playwright install --with-deps
    displayName: Install Playwright browsers

  # 4. Tests mit JUnit-Reporter ausführen (für PublishTestResults kompatibel)
  #    continueOnError: true → Pipeline bricht nicht ab wenn Tests fehlschlagen
  - script: npx playwright test --reporter=junit,html
    displayName: Run Playwright tests
    continueOnError: true

  # 5. Testergebnisse in Azure DevOps Test-Dashboard veröffentlichen
  #    → sichtbar unter Pipelines > Tests > Runs
  - task: PublishTestResults@2
    condition: always()
    inputs:
      testResultsFormat: JUnit
      testResultsFiles: test-results/results.xml
      mergeTestResults: true
      testRunTitle: Playwright TypeScript Tests

  # 6. HTML-Report als Pipeline-Artefakt speichern
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

> **💡 Tipp:** Mit `PublishTestResults@2` erscheinen Testergebnisse direkt in der Azure DevOps Pipeline-UI unter dem Tab **Tests** – inklusive Fehlermeldungen, Dauer und Trend-Diagrammen. Das ist der Hauptvorteil gegenüber GitHub Actions, wo du den Report manuell herunterladen musst.

</details>

<details>
<summary>💡 C# / .NET – Azure Pipelines</summary>

```yaml
trigger:
  - main

pool:
  vmImage: ubuntu-latest

steps:
  # 1. .NET 8 SDK bereitstellen
  - task: UseDotNet@2
    inputs: { version: "8.x" }
    displayName: Install .NET 8

  # 2. Node.js für die React-App bereitstellen
  - task: NodeTool@0
    inputs: { versionSpec: "20.x" }
    displayName: Install Node.js

  # 3. React-App bauen und im Hintergrund starten
  #    workingDirectory: Pfad relativ zum Repo-Root
  - script: npm ci && npm run build && npx serve dist -p 3000 &
    displayName: Build and start Todo App
    workingDirectory: $(System.DefaultWorkingDirectory)/todo-react-playwright

  # 4. .NET Testprojekt kompilieren
  - script: dotnet build TodoPlaywrightTests/
    displayName: Build test project

  # 5. Playwright-Browser via PowerShell-Skript installieren
  #    Das Skript wird durch dotnet build automatisch in bin/Debug/net8.0/ generiert
  - script: pwsh TodoPlaywrightTests/bin/Debug/net8.0/playwright.ps1 install --with-deps
    displayName: Install Playwright browsers

  # 6. Tests ausführen – TRX für PublishTestResults, Agent.TempDirectory für Berechtigungen
  - script: >
      dotnet test TodoPlaywrightTests/
      --logger trx
      --results-directory $(Agent.TempDirectory)/TestResults
    displayName: Run Playwright tests
    continueOnError: true
    env:
      PLAYWRIGHT_BASE_URL: http://localhost:3000

  # 7. TRX-Ergebnisse in Azure DevOps Tests-Tab integrieren
  - task: PublishTestResults@2
    condition: always()
    inputs:
      testResultsFormat: VSTest
      testResultsFiles: $(Agent.TempDirectory)/TestResults/*.trx
      mergeTestResults: true
      testRunTitle: Playwright .NET Tests

  # 8. Traces bei Fehler als Artefakt speichern → lokal mit Trace Viewer öffnen
  - task: PublishPipelineArtifact@1
    condition: failed()
    inputs:
      targetPath: $(System.DefaultWorkingDirectory)/traces
      artifact: playwright-traces
      publishLocation: pipeline
```

> **💡 Tipp:** `$(Agent.TempDirectory)` und `$(System.DefaultWorkingDirectory)` sind [vordefinierte Azure Pipelines-Variablen](https://learn.microsoft.com/azure/devops/pipelines/build/variables) – immer diesen verwenden statt hardcoded Pfaden, damit die Pipeline auf verschiedenen Agenten funktioniert.

</details>

---

### Variante C: Docker-Container

> 📚 **Docs:** [Playwright Docker](https://playwright.dev/docs/docker) · [mcr.microsoft.com/playwright](https://mcr.microsoft.com/en-us/product/playwright/about) · [Dockerfile Best Practices](https://docs.docker.com/develop/develop-images/dockerfile_best-practices/) · [Multi-Stage Builds](https://docs.docker.com/build/building/multi-stage/) · [docker compose healthcheck](https://docs.docker.com/compose/how-tos/startup-order/)

**Warum Docker für Playwright-Tests?**

Docker garantiert reproduzierbare, isolierte Testläufe unabhängig vom Host-System. Das klassische Problem *"läuft bei mir, nicht in CI"* entfällt, weil Container exakt dieselbe Umgebung überall mitbringen.

| Vorteil | Erklärung |
|---|---|
| **Reproduzierbarkeit** | Gleiche Browser-Version, gleiche System-Bibliotheken auf jedem Host |
| **Isolation** | Tests beeinflussen sich nicht gegenseitig; kein Zustand auf dem Host |
| **Offizielles Image** | `mcr.microsoft.com/playwright` enthält Chromium, Firefox, WebKit + alle Abhängigkeiten – kein `--with-deps` nötig |
| **CI-Integration** | Jeder CI-Dienst (GitHub Actions, Azure Pipelines, Jenkins, ...) kann Docker-Container ausführen |

**Wichtige Konzepte:**

| Konzept | Erklärung |
|---|---|
| `FROM mcr.microsoft.com/playwright:vX.Y.Z-jammy` | Offizielles Microsoft-Image mit Ubuntu Jammy (22.04) und Playwright vorinstalliert. Version immer pinnen (z. B. `v1.52.0`) – nie `latest` in Produktion! |
| `.dockerignore` | Verhindert, dass `node_modules`, `.git` und Reports in den Build-Context kopiert werden → deutlich schnellere Image-Builds |
| `-v $(pwd)/playwright-report:/app/playwright-report` | Volume-Mount: Artefakte aus dem Container auf den Host-Dateisystem mappen – sonst sind sie nach `docker run --rm` weg |
| Multi-Stage Build | Stage 1 (SDK) baut die App / Tests; Stage 2 (Runtime) ist schlanker – kein Build-Toolchain im finalen Image |
| `--add-host=host-gateway:host-gateway` | Erlaubt dem Container, auf `host-gateway` (= Host-IP) zuzugreifen – nötig wenn die App auf dem Host läuft, nicht im Container |
| `healthcheck` in compose | Stellt sicher, dass der `tests`-Container erst startet, wenn die `app` wirklich HTTP-Anfragen beantwortet |

<details>
<summary>💡 TypeScript – Dockerfile + Ausführung</summary>

`Dockerfile`:

```dockerfile
# Offizielles Playwright-Image – alle Browser vorinstalliert, keine weitere Installation nötig
# Version pinnen für Reproduzierbarkeit; "jammy" = Ubuntu 22.04 LTS
FROM mcr.microsoft.com/playwright:v1.52.0-jammy

WORKDIR /app

# Zuerst nur package.json kopieren → Docker-Layer-Cache: npm ci nur bei Änderungen
COPY package*.json ./
RUN npm ci

# Dann erst den restlichen Code kopieren
COPY . .

# playwright.config.ts muss baseURL auf localhost:3000 zeigen
# Die App muss separat gestartet werden (kein webServer im Docker-Context)
CMD ["npx", "playwright", "test"]
```

`.dockerignore` – verhindert unnötig große Build-Kontexte:

```
node_modules
test-results
playwright-report
.git
*.md
```

```bash
# Image bauen
docker build -t todo-playwright-tests .

# Tests ausführen (headless, Ergebnisse im Container)
docker run --rm todo-playwright-tests

# Mit spezifischem Browser (Umgebungsvariable)
docker run --rm -e BROWSER=firefox todo-playwright-tests

# Ergebnisse auf Host-Dateisystem speichern (Volume-Mount)
docker run --rm \
  -v $(pwd)/test-results:/app/test-results \
  -v $(pwd)/playwright-report:/app/playwright-report \
  todo-playwright-tests

# Report lokal öffnen
npx playwright show-report playwright-report
```

> **💡 Tipp:** Das `mcr.microsoft.com/playwright`-Image enthält keine App – du musst die TodoMatic-App separat starten oder in `playwright.config.ts` einen `webServer`-Eintrag konfigurieren, der die App im Container selbst startet. Alternativ: `docker compose` (siehe unten).

</details>

<details>
<summary>💡 C# / .NET – Multi-Stage Dockerfile (inspiriert von PlaywrightDemos)</summary>

```dockerfile
# ── Stage 1: Build ─────────────────────────────────────────────────────────
# Vollständiges SDK-Image zum Kompilieren der Tests
FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
WORKDIR /app
COPY . .
# Nur bauen, nicht testen – Ausführung passiert in Stage 2
RUN dotnet build TodoPlaywrightTests/

# ── Stage 2: Test-Ausführung ────────────────────────────────────────────────
# Schlankes Playwright-Runtime-Image (Browser + Systemabhängigkeiten enthalten)
# Das dotnet/sdk-Image von Stage 1 ist NICHT im finalen Image enthalten → kleineres Image
FROM mcr.microsoft.com/playwright/dotnet:v1.52.0-jammy AS test
WORKDIR /app
# Nur die kompilierten Test-Binaries aus Stage 1 übernehmen
COPY --from=build /app/TodoPlaywrightTests/bin/Debug/net8.0 .
# host-gateway wird zur Laufzeit auf die Host-IP aufgelöst (via --add-host)
ENV PLAYWRIGHT_BASE_URL=http://host-gateway:3000
ENTRYPOINT ["dotnet", "test", "TodoPlaywrightTests.dll", \
            "--filter", "TestCategory=CICD", \
            "--logger", "trx;LogFileName=results.trx"]
```

```bash
# Image bauen (Stage 1 + Stage 2 werden automatisch hintereinander ausgeführt)
docker build -t todo-playwright-tests-dotnet .

# Voraussetzung: App auf dem Host läuft (npm run dev oder npm run build && npx serve dist)
# --add-host: Löst "host-gateway" auf die Host-IP auf (Linux: host-gateway = 172.17.0.1)
# -v: TRX-Ergebnisse auf den Host mappen
docker run --rm \
  --add-host=host-gateway:host-gateway \
  -e PLAYWRIGHT_BASE_URL=http://host-gateway:3000 \
  -v $(pwd)/TestResults:/app/TestResults \
  todo-playwright-tests-dotnet
```

**docker compose** – App und Tests als zusammenhängende Services:

```yaml
# docker-compose.test.yml
# Startet die React-App und die Tests in einer koordinierten Umgebung
services:
  app:
    build: { context: ./todo-react-playwright }
    ports: ["3000:3000"]
    # healthcheck: Tests starten erst, wenn die App HTTP 200 zurückgibt
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:3000"]
      interval: 5s
      retries: 10

  tests:
    build: { context: ./TodoPlaywrightTests }
    # depends_on mit condition: service_healthy → wartet auf healthcheck
    depends_on:
      app: { condition: service_healthy }
    environment:
      # Im compose-Netzwerk ist "app" der Hostname des App-Containers
      PLAYWRIGHT_BASE_URL: http://app:3000
    volumes:
      - ./TestResults:/app/TestResults
```

```bash
# Alles starten, Tests ausführen, Exit-Code von "tests" zurückgeben
# --exit-code-from tests: Pipeline schlägt fehl wenn Tests fehlschlagen
docker compose -f docker-compose.test.yml up --exit-code-from tests

# Aufräumen nach dem Run
docker compose -f docker-compose.test.yml down
```

> **💡 Tipp:** `--exit-code-from tests` ist entscheidend für CI: Ohne dieses Flag würde `docker compose up` immer mit Exit-Code 0 enden – die Pipeline wäre immer grün, auch wenn Tests fehlschlagen.

</details>

> 🤔 **Stop & Think:** Warum ist `if: always()` für den Report-Upload wichtiger als `if: failure()`? Welche Information geht verloren, wenn der Report nur bei Fehlern hochgeladen wird?

---

## Exercise 13 (Bonus): Azure Playwright Testing Service

**Ziel:** Tests auf einer Azure-verwalteten Browser-Farm ausführen – skalierbar, ohne eigene Browser-Infrastruktur.

> 📚 **Offizielle Dokumentation:**  
> [Azure Playwright Testing – Übersicht](https://learn.microsoft.com/azure/playwright-testing/overview-what-is-microsoft-playwright-testing) · [Quickstart TypeScript](https://learn.microsoft.com/azure/playwright-testing/quickstart-run-end-to-end-tests) · [Quickstart .NET](https://learn.microsoft.com/azure/playwright-testing/quickstart-run-end-to-end-tests?tabs=playwrightdotnet) · [Service-Konfiguration](https://learn.microsoft.com/azure/playwright-testing/how-to-manage-playwright-workspace) · [Reporting & Portal](https://learn.microsoft.com/azure/playwright-testing/how-to-use-reporting-feature) · [Sharding & Parallelität](https://learn.microsoft.com/azure/playwright-testing/concept-determine-optimal-configuration) · [GitHub Actions Integration](https://learn.microsoft.com/azure/playwright-testing/quickstart-automate-end-to-end-testing) · [PlaywrightDemos Referenzimplementierung](https://github.com/norschel/PlaywrightDemos/blob/main/PlaywrightDemos/AzurePlaywrightTests_BastaSpring2026.cs)

**Was ist der Azure Playwright Testing Service?**

| Aspekt | Lokale Tests / Self-hosted CI | Azure Playwright Testing Service |
|---|---|---|
| Browser-Installation | Manuell (`--with-deps`) auf jedem Agent | ✅ Vollständig verwaltet – keine Browser auf dem Agent nötig |
| Parallelität | Limitiert durch Agent-CPU/RAM | ✅ Bis zu 50 parallele Browser in Azure-Containern |
| Ergebnisse | Lokaler HTML-Report | ✅ Zentrales Dashboard im Azure Portal mit Trend-Analyse |
| Traces | Lokales Artefakt herunterladen | ✅ Traces direkt im Azure Portal browsern |
| Kosten | Agent-Infrastruktur | Pay-per-use (Browser-Minuten) |

**Voraussetzungen:**

1. **Azure-Abonnement** – [kostenloses Konto erstellen](https://azure.microsoft.com/free/) oder bestehendes nutzen
2. **Azure Playwright Testing Workspace** im Portal erstellt (Schritt 1 unten)
3. **Access Token** generiert (Schritt 2 unten)

---

### Schritt 1: Workspace im Azure Portal erstellen

1. Öffne das [Azure Portal](https://portal.azure.com) und suche nach **"Playwright Testing"**
2. Klicke **+ Create** und fülle aus:
   - **Subscription:** dein Azure-Abonnement
   - **Resource group:** neu oder bestehend (z. B. `rg-playwright-testing`)
   - **Name:** eindeutiger Workspace-Name (z. B. `pw-testing-demo`)
   - **Region:** wähle eine Region nahe deiner CI-Infrastruktur (z. B. `West Europe`)
3. Klicke **Review + Create** → **Create**
4. Nach der Bereitstellung: navigiere zum Workspace, öffne **Settings → Access tokens**
5. Klicke **+ Generate new token** und notiere:
   - Den **Service URL** (Format: `wss://westeurope.api.playwright.microsoft.com/accounts/<ID>/...`)
   - Den **Access Token**

> 💡 **Sicherheitshinweis:** Speichere Token niemals im Code. Nutze GitHub Actions Secrets (`Settings → Secrets → Actions`) oder Azure Pipelines Variable Groups.

---

### Schritt 2: Umgebungsvariablen konfigurieren

```bash
# Lokal: in ~/.bashrc / ~/.zshrc eintragen oder als Shell-Session-Variablen setzen
export PLAYWRIGHT_SERVICE_URL="wss://westeurope.api.playwright.microsoft.com/accounts/<ACCOUNT_ID>/authorize/accessToken"
export PLAYWRIGHT_SERVICE_ACCESS_TOKEN="<dein-token>"
```

In GitHub Actions als Secrets hinterlegen:

```yaml
# In Repository Settings → Secrets → New repository secret:
# Name: PLAYWRIGHT_SERVICE_URL
# Name: PLAYWRIGHT_SERVICE_ACCESS_TOKEN
```

> 📚 Siehe [Access Token verwalten](https://learn.microsoft.com/azure/playwright-testing/how-to-manage-access-tokens) und [GitHub Secrets](https://docs.github.com/actions/security-guides/encrypted-secrets)

---

<details>
<summary>💡 TypeScript – Azure Playwright Testing Service (Schritt-für-Schritt)</summary>

#### Schritt 3a: Paket installieren

```bash
# @azure/microsoft-playwright-testing: SDK für den Azure Playwright Testing Service
# --save-dev: nur für Entwicklung/Tests, nicht für Produktions-Bundle
npm install --save-dev @azure/microsoft-playwright-testing
```

> 📚 [NPM-Paket](https://www.npmjs.com/package/@azure/microsoft-playwright-testing) · [SDK Changelog](https://github.com/Azure/azure-sdk-for-js/blob/main/sdk/playwright-testing/microsoft-playwright-testing/CHANGELOG.md)

#### Schritt 3b: Service-Konfigurationsdatei erstellen

`playwright.service.config.ts` – **erweitert** die bestehende `playwright.config.ts`, überschreibt sie nicht:

```typescript
import { defineConfig } from "@playwright/test";
import { getServiceConfig, ServiceOS } from "@azure/microsoft-playwright-testing";
import config from "./playwright.config";

// getServiceConfig() injiziert Connect-URL, Token und Service-spezifische Einstellungen
// Es übernimmt alle Einstellungen aus config (playwright.config.ts) und überschreibt nur
// was für den Service nötig ist (z.B. connectOptions, Timeout-Anpassungen)
export default defineConfig(
  config,
  getServiceConfig(config, {
    // ServiceOS.LINUX: Browser laufen in Linux-Containern in Azure
    // ServiceOS.WINDOWS: Alternativ Windows-Container (für Edge-spezifische Tests)
    os: ServiceOS.LINUX,
    // runId: eindeutige ID für diesen Test-Run → erscheint im Azure Portal Dashboard
    // BUILD_ID: von CI-System gesetzt (GitHub: GITHUB_RUN_ID, Azure Pipelines: BUILD_BUILDID)
    runId: process.env.BUILD_ID ?? new Date().toISOString(),
  }),
  {
    reporter: [
      ["list"],                                          // Lokale Konsolenausgabe
      ["@azure/microsoft-playwright-testing/reporter"],  // Ergebnisse → Azure Portal
    ],
  }
);
```

> 📚 [`getServiceConfig` API-Referenz](https://learn.microsoft.com/azure/playwright-testing/quickstart-run-end-to-end-tests#create-playwright-service-configuration-file) · [ServiceOS Optionen](https://learn.microsoft.com/azure/playwright-testing/concept-determine-optimal-configuration#choose-the-right-os)

#### Schritt 3c: Tests ausführen

```bash
# Sicherstellen, dass die Umgebungsvariablen gesetzt sind (Schritt 2)
echo $PLAYWRIGHT_SERVICE_URL

# Tests mit Service-Konfiguration ausführen
# Playwright verbindet sich via WebSocket (wss://) zu Azure-Browsern
npx playwright test --config=playwright.service.config.ts

# Sharding: 4 parallele Shards in Azure (jeder Shard = eigener Browser-Container)
# Ideal für große Test-Suiten in CI – deutlich schneller als sequenziell
npx playwright test --config=playwright.service.config.ts --shard=1/4
npx playwright test --config=playwright.service.config.ts --shard=2/4
npx playwright test --config=playwright.service.config.ts --shard=3/4
npx playwright test --config=playwright.service.config.ts --shard=4/4

# Spezifischen Browser wählen (Chromium, Firefox oder WebKit)
npx playwright test --config=playwright.service.config.ts --project=chromium
```

#### Schritt 3d: In GitHub Actions integrieren

```yaml
# .github/workflows/playwright-azure.yml
name: Playwright Tests (Azure Service)
on:
  push: { branches: [main] }
  pull_request: { branches: [main] }

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with: { node-version: 20, cache: npm }
      - run: npm ci
      # Keine Browser-Installation nötig! Azure stellt sie bereit.
      # --with-deps entfällt komplett.
      - name: Run Playwright tests on Azure
        run: npx playwright test --config=playwright.service.config.ts
        env:
          # Secrets aus GitHub Repository Settings → Secrets → Actions
          PLAYWRIGHT_SERVICE_URL: ${{ secrets.PLAYWRIGHT_SERVICE_URL }}
          PLAYWRIGHT_SERVICE_ACCESS_TOKEN: ${{ secrets.PLAYWRIGHT_SERVICE_ACCESS_TOKEN }}
          BUILD_ID: ${{ github.run_id }}  # Eindeutige Run-ID für das Portal-Dashboard
```

> 💡 **Tipp:** Nach dem Run öffne das [Azure Playwright Testing Portal](https://playwright.microsoft.com/) und navigiere zu **Test Runs** – du siehst alle Runs mit Traces, Screenshots und Fehlerdetails ohne lokalen Download.

</details>

<details>
<summary>💡 C# / NUnit – Azure Playwright Testing Service (wie in PlaywrightDemos)</summary>

#### Schritt 3a: NuGet-Pakete installieren

```bash
# Microsoft.Playwright.NUnit: Basis-Integration (PlaywrightTest-Basisklasse)
dotnet add package Microsoft.Playwright.NUnit

# Azure.Developer.MicrosoftPlaywrightTesting.NUnit: Service-Integration
# Stellt PlaywrightServiceNUnitSetup und PlaywrightServiceTest bereit
dotnet add package Azure.Developer.MicrosoftPlaywrightTesting.NUnit
```

> 📚 [NuGet: Azure.Developer.MicrosoftPlaywrightTesting.NUnit](https://www.nuget.org/packages/Azure.Developer.MicrosoftPlaywrightTesting.NUnit) · [Quickstart .NET](https://learn.microsoft.com/azure/playwright-testing/quickstart-run-end-to-end-tests?tabs=playwrightdotnet)

#### Schritt 3b: Service-Setup registrieren

`PlaywrightServiceSetup.cs` – globales Setup, das den Service initialisiert:

```csharp
using Azure.Developer.MicrosoftPlaywrightTesting.NUnit;

// Parallelisierung auf Fixture-Ebene aktivieren – Voraussetzung für Service-Sharding
[assembly: NUnit.Framework.Parallelizable(NUnit.Framework.ParallelScope.Fixtures)]

// PlaywrightServiceNUnitSetup liest PLAYWRIGHT_SERVICE_URL und PLAYWRIGHT_SERVICE_ACCESS_TOKEN
// aus Umgebungsvariablen und konfiguriert den WebSocket-Connect zu Azure
[SetUpFixture]
public class PlaywrightServiceSetup : PlaywrightServiceNUnitSetup { }
```

#### Schritt 3c: Tests von PlaywrightServiceTest ableiten

`AzurePlaywrightTests.cs`:

```csharp
using Azure.Developer.MicrosoftPlaywrightTesting.NUnit;
using System.Text.RegularExpressions;

// PlaywrightServiceTest ersetzt die normale PlaywrightTest-Basisklasse
// Browser, Context und Page werden remote in Azure-Containern erstellt
[TestFixture]
public class AzurePlaywrightTests : PlaywrightServiceTest
{
    public IPage Page { get; private set; } = null!;

    [SetUp]
    public async Task SetUp()
    {
        // Context kommt von PlaywrightServiceTest – bereits mit Azure-Browser verbunden
        Page = await Context.NewPageAsync();
    }

    [TearDown]
    public async Task TearDown() => await Page.CloseAsync();

    [Test]
    [Category("CICD")]  // Nur Tests mit CICD-Kategorie in CI ausführen
    public async Task AppLoadsOnAzureService()
    {
        await Page.GotoAsync("http://localhost:3000");
        await Assertions.Expect(Page).ToHaveTitleAsync(new Regex("TodoMatic"));
        // Browser-Info aus Azure-Container loggen
        TestContext.Out.WriteLine($"Browser: {Page.Context.Browser?.BrowserType.Name}");
        TestContext.Out.WriteLine($"OS: {Environment.OSVersion}");
    }
}
```

#### Schritt 3d: Tests lokal ausführen

```bash
# Umgebungsvariablen setzen (Schritt 2) und dann:
export PLAYWRIGHT_SERVICE_URL="wss://westeurope.api.playwright.microsoft.com/..."
export PLAYWRIGHT_SERVICE_ACCESS_TOKEN="<dein-token>"

# Alle Tests ausführen
dotnet test TodoPlaywrightTests/

# Nur CICD-Kategorie (empfohlen für CI)
dotnet test TodoPlaywrightTests/ --filter "TestCategory=CICD"

# Mit TRX-Ergebnissen (für Azure Pipelines PublishTestResults)
dotnet test TodoPlaywrightTests/ --filter "TestCategory=CICD" \
  --logger trx --results-directory TestResults/
```

#### Schritt 3e: In Azure Pipelines integrieren

```yaml
# azure-pipelines.yml (Erweiterung von Variante B aus Exercise 12)
- script: >
    dotnet test TodoPlaywrightTests/
    --filter "TestCategory=CICD"
    --logger trx
    --results-directory $(Agent.TempDirectory)/TestResults
  displayName: Run Playwright tests on Azure Service
  # Keine Browser-Installation nötig – Azure übernimmt das
  env:
    # Als Secret-Variable in der Pipeline definiert (Library oder Inline als Secret)
    PLAYWRIGHT_SERVICE_URL: $(PLAYWRIGHT_SERVICE_URL)
    PLAYWRIGHT_SERVICE_ACCESS_TOKEN: $(PLAYWRIGHT_SERVICE_ACCESS_TOKEN)
    BUILD_ID: $(Build.BuildId)
```

> 📚 [Azure Pipelines Service Connection für Playwright](https://learn.microsoft.com/azure/playwright-testing/quickstart-automate-end-to-end-testing?tabs=github-actions-azure-devops)

#### Überblick der Vorteile

| Vorteil | Details |
|---|---|
| **Keine Browser-Installation** | Browser laufen in Azure-Containern – weder `--with-deps` noch `playwright.ps1 install` im CI-Agent nötig |
| **Bis zu 50 parallele Browser** | Sharding über mehrere Browser-Container ohne eigene Infrastruktur skalieren |
| **Zentrales Reporting** | Ergebnisse, Traces und Screenshots im [Azure Playwright Testing Portal](https://playwright.microsoft.com/) – kein Artefakt-Download nötig |
| **Trend-Analyse** | Test-Ergebnisse über Zeit im Portal visualisiert – Flakyness-Erkennung eingebaut |
| **Cross-Browser in Azure** | Chromium, Firefox und WebKit gleichzeitig in Azure ausführen |

</details>

> 🤔 **Stop & Think:** In welchem konkreten Szenario lohnt sich der Azure Playwright Testing Service gegenüber Self-hosted CI – und in welchem nicht? Denke an Projektgröße, Budget und Datenschutzanforderungen.

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

> 🤔 **Stop & Think:** Wo ersetzt der MCP Server menschliche Exploration sinnvoll – und wo solltest du trotzdem von Hand testen? Welche Locatoren oder Szenarien würde ein KI-Agent wahrscheinlich übersehen?

---

## Exercise 15 (Bonus): Playwright CLI – Token-effiziente Browser-Automatisierung

**Ziel:** Den offiziellen **Playwright CLI** (`@playwright/cli`) kennenlernen – ein schlankes Command-Line-Interface für Browser-Automatisierung, das speziell für Coding-Agents optimiert ist, aber auch manuell sehr praktisch ist.

> **Was ist playwright-cli, und was unterscheidet es vom MCP Server?**  
> Während der [MCP Server (Exercise 14)](https://github.com/microsoft/playwright-mcp) auf persistente Browser-Sessions und reichhaltige DOM-Introspection setzt, bietet `playwright-cli` bewusst token-effiziente CLI-Befehle als Skills. Jeder Befehl ist ein eigenständiger CLI-Aufruf – kein großes Tool-Schema wird ins LLM-Context geladen. Das macht `playwright-cli` zur besseren Wahl für **Coding-Agents mit begrenztem Context-Window** (z. B. beim gleichzeitigen Arbeiten mit großen Codebasen und Browser-Automatisierung).

> 📚 **Docs:** [github.com/microsoft/playwright-cli](https://github.com/microsoft/playwright-cli)

| | playwright-cli | Playwright MCP Server |
|---|---|---|
| **Primäre Zielgruppe** | Coding-Agents (Claude Code, GitHub Copilot) + manuelle Nutzung | Explorative Agenten, UI-Interaktion |
| **Token-Effizienz** | ✅ Hoch – kein Tool-Schema im Context | ⚠️ Mittel – Tool-Schemas und Accessibility Trees |
| **Persistenz** | Sessions mit `--persistent` (auch cross-restart) | Persistent innerhalb einer Sitzung |
| **Dashboard** | `playwright-cli show` (visuelles Session-Grid) | – |
| **Network-Mocking** | `playwright-cli route <pattern>` | Über MCP-Tool-Aufrufe |
| **Tracing/Video** | `tracing-start/stop`, `video-start/stop` | – |
| **Code-Erzeugung** | Kein eingebauter Recorder | `npx playwright codegen` separat |

---

### Voraussetzungen

| | |
|---|---|
| Node.js | 18+ |
| Betriebssystem | Linux, macOS, Windows |
| Coding Agent (optional) | Claude Code, GitHub Copilot Agent Mode, oder ein anderer Agent |

---

### Setup

```bash
# Global installieren
npm install -g @playwright/cli@latest

# Verfügbare Befehle anzeigen
playwright-cli --help

# Skills für Coding-Agents installieren
# (Claude Code, GitHub Copilot und andere lesen diese Skills automatisch)
playwright-cli install --skills

# Browser-Session starten und TodoMatic-App öffnen
playwright-cli open http://localhost:3000 --headed
```

> ℹ️ **Sessions:** `playwright-cli` hält eine Browser-Instanz im Hintergrund. Alle nachfolgenden Befehle sprechen dieselbe Instanz an – kein erneutes `open` nötig, solange die Session läuft.

---

### Teil A: Manuelle Erkundung der TodoMatic-App

**Aufgabe:** Erkunde die App Schritt für Schritt per CLI-Befehlen – ohne eine einzige Codezeile zu schreiben.

**Schritt 1 – App öffnen und Snapshot aufnehmen:**

```bash
playwright-cli open http://localhost:3000
playwright-cli snapshot
```

Der Snapshot zeigt die aktuelle Seitenstruktur mit Element-`ref`-IDs. Diese `ref`-Werte (z. B. `e12`, `e15`) sind **temporäre Bezeichner**, die `playwright-cli` für jeden Snapshot neu erzeugt – sie zeigen keine stabilen Selektoren, sondern numerische Platzhalter für die aktuelle DOM-Struktur. Nach einer Interaktion und erneutem Snapshot können sich die refs ändern.

> 💡 **Tipp:** Nach jedem Befehl gibt `playwright-cli` automatisch einen neuen Snapshot aus. Lies stets den aktuellen Snapshot, bevor du den nächsten Befehl absetzt.

**Schritt 2 – Eine Aufgabe hinzufügen:**

```bash
# Eingabefeld füllen (ref aus dem Snapshot, z. B. e12)
playwright-cli fill e12 "CLI-Test-Aufgabe"

# Geolocation wird benötigt – prüfe, ob ein Dialog erscheint
playwright-cli snapshot

# Add-Button klicken (ref des Buttons aus dem Snapshot)
playwright-cli click e15
playwright-cli snapshot
```

> 💡 **Tipp:** Nach jedem Befehl gibt `playwright-cli` automatisch einen neuen Snapshot aus. Die `ref`-Werte (`e12`, `e15` usw.) ändern sich nach DOM-Updates – lies immer den aktuellen Snapshot.

**Schritt 3 – Filter testen:**

```bash
# "Active"-Filter klicken
playwright-cli click e28   # ref des Active-Buttons aus dem Snapshot
playwright-cli snapshot

# "Completed"-Filter
playwright-cli click e31   # ref des Completed-Buttons
playwright-cli snapshot

# Screenshot der aktuellen Ansicht
playwright-cli screenshot --filename=filter-state.png
```

**Schritt 4 – Aufgabe bearbeiten und löschen:**

```bash
# "Edit"-Button der hinzugefügten Aufgabe klicken
playwright-cli click e42

# Inhalt des Eingabefelds ersetzen
playwright-cli fill e45 "CLI-Test-Aufgabe (bearbeitet)"

# "Save" klicken
playwright-cli click e48

# Snapshot zur Verifikation
playwright-cli snapshot

# Aufgabe löschen
playwright-cli click e51   # "Delete"-Button
playwright-cli snapshot
```

> 🔍 **Locator generieren:** Du kannst für jedes Element einen regulären Playwright-Locator-String erzeugen:
> ```bash
> playwright-cli generate-locator e12
> # Ausgabe: locator('#new-todo-input')   ← TypeScript/JavaScript-Syntax
> # C#-Äquivalent: Page.Locator("#new-todo-input")
> ```
> Das ist besonders nützlich, wenn du einen manuellen CLI-Workflow anschließend in einen echten Playwright-Test überführen möchtest.

<details>
<summary>💡 Lösungshinweis – Vollständiger Workflow</summary>

```bash
# Session starten
playwright-cli open http://localhost:3000

# Snapshot aufnehmen – refs merken
playwright-cli snapshot

# Aufgabe hinzufügen (refs ggf. anpassen)
playwright-cli fill e12 "CLI-Test-Aufgabe"
playwright-cli click e15

# Prüfen
playwright-cli snapshot

# Filter testen
playwright-cli click e28   # Active
playwright-cli click e31   # Completed
playwright-cli click e25   # All

# Screenshot
playwright-cli screenshot --filename=todo-cli-result.png

# Browser schließen
playwright-cli close
```

</details>

> 🤔 **Stop & Think:** Welchen Vorteil hat `playwright-cli generate-locator` gegenüber manueller Locator-Suche im DevTools-Inspector? In welchem Schritt des Test-Entwicklungsprozesses würdest du es einsetzen?

---

### Teil B: Network-Mocking und Tracing per CLI

**Aufgabe:** Nutze die CLI-eigenen Netzwerk- und Tracing-Features für Diagnose und Mocking.

**Tracing starten, Workflow ausführen, Trace öffnen:**

```bash
# Tracing starten
playwright-cli open http://localhost:3000
playwright-cli tracing-start

# Workflow ausführen (Aufgabe hinzufügen)
playwright-cli fill e12 "Trace-Aufgabe"
playwright-cli click e15
playwright-cli snapshot

# Tracing beenden und Datei speichern
playwright-cli tracing-stop
# → Trace-Datei im aktuellen Verzeichnis

# Trace Viewer öffnen
npx playwright show-trace trace.zip
```

**Network-Requests beobachten:**

```bash
playwright-cli open http://localhost:3000
playwright-cli requests
# Zeigt alle HTTP-Requests seit dem Laden der Seite

# "Load remote tasks"-Button klicken
playwright-cli click e99   # ref aus Snapshot
playwright-cli requests
# Neuer Request zu remoteTasks.json sollte erscheinen
```

**Network-Route mocken:**

```bash
# Für komplexe Payloads: Mock-JSON in eine Datei auslagern
cat > /tmp/mock-tasks.json << 'EOF'
[{"id":"mock-1","name":"Gemockte CLI-Aufgabe","completed":false}]
EOF

# remoteTasks.json mit der Mock-Datei abfangen
playwright-cli route "**/remoteTasks.json" --body=@/tmp/mock-tasks.json --content-type="application/json"

# Button klicken – gemockter Response wird geliefert
playwright-cli click e99
playwright-cli snapshot
```

<details>
<summary>💡 Lösungshinweis – Video-Aufnahme</summary>

```bash
# Video-Aufnahme starten
playwright-cli open http://localhost:3000
playwright-cli video-start workflow.webm
playwright-cli video-chapter "Task hinzufügen"

playwright-cli fill e12 "Video-Task"
playwright-cli click e15

playwright-cli video-chapter "Task löschen"
playwright-cli click e51   # Delete

# Video beenden – Datei workflow.webm wird gespeichert
playwright-cli video-stop
```

</details>

> 🤔 **Stop & Think:** Wann ist `playwright-cli route` dem `page.route()` in einem echten Playwright-Test vorzuziehen – und wann nicht? Denke an schnelle Ad-hoc-Tests vs. reproduzierbare Regressionstests.

---

### Teil C: playwright-cli als Coding-Agent-Skill

**Ziel:** Den Workflow verstehen, mit dem ein KI-Coding-Agent `playwright-cli` nutzt – entweder mit installierten Skills oder in der skills-losen Variante.

**Mit installierten Skills (Claude Code / GitHub Copilot Agent Mode):**

```bash
# Skills einmalig installieren
playwright-cli install --skills

# Danach im Coding-Agent:
# "Teste den 'Add Todo'-Workflow auf http://localhost:3000 mit playwright-cli."
```

**Ohne installierte Skills (Skills-less):**

Wenn keine Skills installiert sind, liest der Agent den Hilfetext selbst:

```
Teste den "Add Todo"-Workflow auf http://localhost:3000 mit playwright-cli.
Prüfe playwright-cli --help für verfügbare Befehle.
```

**Mehrere Sessions parallel:**

```bash
# Session für TodoMatic
playwright-cli -s=todo open http://localhost:3000

# Andere Session für Vergleichsapp
playwright-cli -s=demo open https://demo.playwright.dev/todomvc

# Sessions anzeigen
playwright-cli list

# Dashboard öffnen – zeigt alle Sessions als Live-Screencasts
playwright-cli show

# Alle Sessions schließen
playwright-cli close-all
```

> ℹ️ **C#-Entwickler:** `playwright-cli` ist ein Node.js-Tool und läuft unabhängig vom C#-Testprojekt. Du kannst es parallel zu deinen C#-Tests nutzen – z. B. für schnelle manuelle Verifikation, ohne den vollen `dotnet test`-Zyklus zu durchlaufen.

<details>
<summary>💡 Lösungshinweis – Locator aus CLI in C#-Test übernehmen</summary>

```bash
# CLI-Schritt: Locator generieren
playwright-cli open http://localhost:3000
playwright-cli snapshot
playwright-cli generate-locator e12
# → locator('#new-todo-input')
```

```csharp
// Diesen Locator direkt in den C#-Test übernehmen:
await Page.Locator("#new-todo-input").FillAsync("CLI-generierter Locator");
```

Der Workflow: CLI für Exploration → `generate-locator` für präzise Selektoren → C#-Test mit diesen Selektoren schreiben. Kein Raten, kein DevTools-Öffnen.

</details>

> 🤔 **Stop & Think:** Ein Coding-Agent kann `playwright-cli` eigenständig nutzen, ohne dass du jede Aktion vorgibst. Welche Risiken entstehen dabei, und wie kannst du sicherstellen, dass der Agent keine unerwünschten Seiteneffekte verursacht (z. B. Daten löschen)?

> **Sicherheitshinweis – Best Practices bei Agent-gesteuerten CLI-Sessions:**  
> - **Immer Testumgebung nutzen** – nie eine Produktion- oder Staging-URL ohne Freigabe übergeben  
> - **Session isolieren** – benannte Sessions (`-s=my-task`) verhindern, dass ein Agent in einen anderen Kontext schreibt  
> - **Prompt-Scope einschränken** – dem Agent nur die URL und den Scope mitteilen, den er benötigt  
> - **Dashboard nutzen** – `playwright-cli show` gibt dir jederzeit einen Live-Überblick, was der Agent gerade tut  
> - **Sessions nach Abschluss schließen** – `playwright-cli close-all` verhindert unbeabsichtigtes Weiterlaufen

---

### Hilfreiche Links zu playwright-cli

| Ressource | Link |
|---|---|
| GitHub-Repository | [github.com/microsoft/playwright-cli](https://github.com/microsoft/playwright-cli) |
| npm-Paket | [npmjs.com/@playwright/cli](https://www.npmjs.com/package/@playwright/cli) |
| Playwright MCP Server (Vergleich) | [github.com/microsoft/playwright-mcp](https://github.com/microsoft/playwright-mcp) |
| Claude Code Integration | [docs.anthropic.com/claude-code](https://docs.anthropic.com/en/docs/claude-code) |

---

## Zusammenfassung: Gelerntes auf einen Blick

| Konzept | TypeScript API | C# API | Framework | Übung |
|---|---|---|---|---|
| Navigation | `page.goto()` | `Page.GotoAsync()` | alle | 1–15 |
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
| **Playwright CLI** | `playwright-cli <cmd>` | *(Node.js-Tool, nutzbar neben C#)* | alle | **15** |

---

## Trouble-shooting – Häufige Fehler und Lösungen

Hier sind die häufigsten Stolpersteine beim Arbeiten mit dieser HOL:

---

### 🔴 "Add"-Button klicken, aber Aufgabe erscheint nicht

**Symptom:** `toBeVisible()` schlägt mit Timeout fehl – die Aufgabe erscheint nie in der Liste.

**Ursache:** Die App ruft `navigator.geolocation.getCurrentPosition()` auf und wartet auf ein Ergebnis. Ohne Geolocation-Grant hängt der Callback und die Aufgabe wird nie gespeichert.

**Lösung TypeScript:**
```typescript
// In playwright.config.ts (global – einmalig für alle Tests):
use: {
  geolocation: { latitude: 48.1372, longitude: 11.5755 },
  permissions: ["geolocation"],
}
```

**Lösung C#:**
```csharp
// In TestBase.ContextOptions():
Geolocation = new Geolocation { Latitude = 48.1372f, Longitude = 11.5755f },
Permissions = new[] { "geolocation" },
```

---

### 🔴 Port 3000 bereits belegt

**Symptom:** `Error: listen EADDRINUSE: address already in use :::3000` oder `webServer` startet nicht.

**Lösung:**
```bash
# Prozess auf Port 3000 finden und beenden (Linux/macOS):
lsof -ti:3000 | xargs kill -9

# Windows:
netstat -ano | findstr :3000
taskkill /PID <PID> /F

# Alternativ: App auf anderem Port starten
npm run dev -- --port 3001
# und in playwright.config.ts / PLAYWRIGHT_BASE_URL anpassen
```

---

### 🔴 `playwright.ps1` nicht gefunden

**Symptom:** `pwsh: cannot find 'bin/Debug/net8.0/playwright.ps1'`

**Ursachen und Lösungen:**
- `dotnet build` wurde noch nicht ausgeführt → `dotnet build` ausführen
- Falsches Target-Framework (z. B. net9.0 statt net8.0) → Pfad in der Konsole anpassen oder:
```bash
# Pfad automatisch finden (Linux/macOS):
pwsh $(find . -name 'playwright.ps1' -not -path '*/obj/*' | head -1) install

# Alternativ: Global Tool verwenden
dotnet tool install --global Microsoft.Playwright.CLI
playwright install
```

---

### 🟡 `npx playwright install --with-deps` schlägt in CI fehl

**Symptom:** `Error: Failed to install browsers` oder fehlende Linux-Bibliotheken.

**Lösung:** `--with-deps` ist für Linux-Umgebungen ohne vorinstallierte System-Bibliotheken gedacht. Stelle sicher, dass:
```yaml
# GitHub Actions / Azure Pipelines:
- run: npx playwright install --with-deps   # ✅ immer --with-deps in CI
# Nicht nur:
- run: npx playwright install               # ❌ fehlt System-Abhängigkeiten
```

Alternativ: Offizielles Docker-Image nutzen (`mcr.microsoft.com/playwright:v1.52.0-jammy`) – enthält alles vorinstalliert.

---

### 🟡 Test schlägt nach Page-Reload fehl – Aufgaben weg

**Symptom:** Test fügt Aufgaben hinzu, ruft `page.goto("/")` erneut auf und die Liste ist leer.

**Ursache:** Die App speichert Aufgaben nur im React-State – kein LocalStorage, keine Datenbank. Ein Reload setzt alles zurück.

**Lösung:** Alle Testschritte ohne erneutes `goto()` ausführen, oder Aufgaben nach jedem `goto()` neu hinzufügen.

---

### 🟡 `getByTestId("testID-All")` findet das Element nicht

**Symptom:** `strict mode violation: locator resolved to N elements` oder Element nicht gefunden.

**Mögliche Ursachen:**
1. App noch nicht vollständig geladen → `await page.waitForLoadState("networkidle")` einfügen
2. Filter-Buttons verwenden `data-testid`, nicht `id` – prüfe mit DevTools: `$$('[data-testid]')`
3. Tipp-Fehler: Groß/Kleinschreibung beachten (`testID-All` ≠ `testID-all`)

---

### 🟡 Locator matched mehrere Elemente

**Symptom:** `Error: strict mode violation – locator matched 2 elements`

**Lösung:** Locator mit `.filter()` eingrenzen:
```typescript
// ❌ zu breit – trifft alle Buttons mit Name "Delete"
page.getByRole("button", { name: "Delete" })

// ✅ zuerst das richtige Listenelement finden, dann den Button darin
page.getByRole("listitem").filter({ hasText: "Meine Aufgabe" })
    .getByRole("button", { name: "Delete" })
```

---

### 🟡 C#-Tests laufen, aber die App antwortet nicht

**Symptom:** `net::ERR_CONNECTION_REFUSED` oder Timeout beim `GotoAsync()`

**Ursache:** Im Gegensatz zu TypeScript (dort gibt es `webServer` in `playwright.config.ts`) startet C#-Tests die App nicht automatisch.

**Lösung:** App manuell starten, bevor du Tests ausführst:
```bash
# Terminal 1: App starten
npm run dev

# Terminal 2: Tests ausführen
dotnet test --settings playwright.runsettings
```

---

### 🟢 HTML-Report öffnet sich nicht im Browser

**Lösung:**
```bash
npx playwright show-report              # TypeScript
# Öffnet http://localhost:9323 – falls Port belegt:
npx playwright show-report --port 9324
```

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

### 🖥️ Playwright CLI

| Ressource | Link |
|---|---|
| `@playwright/cli` (npm) | [npmjs.com/@playwright/cli](https://www.npmjs.com/package/@playwright/cli) |
| GitHub-Repository | [github.com/microsoft/playwright-cli](https://github.com/microsoft/playwright-cli) |
| Vergleich CLI vs. MCP | [github.com/microsoft/playwright-cli – README](https://github.com/microsoft/playwright-cli#playwright-cli-vs-playwright-mcp) |

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
