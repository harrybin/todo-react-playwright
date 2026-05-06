# Hands-on Lab: Playwright End-to-End Testing mit der TodoMatic-App

> **Niveau:** Fortgeschrittene Bootcamp-Übung  
> **Dauer:** ca. 3–4 Stunden  
> **Repo:** [harrybin/todo-react-playwright](https://github.com/harrybin/todo-react-playwright)  
> **Referenz-Demos:** [norschel/PlaywrightDemos](https://github.com/norschel/PlaywrightDemos)

---

## Lernziele

Nach dieser HOL kannst du:

- Playwright-Tests aufsetzen und in ein Node.js-Projekt integrieren
- Lokatorstrategien (ARIA-Roles, `data-testid`, CSS, Text) gezielt einsetzen
- Browser-APIs wie Geolocation mocken
- Netzwerk-Requests abfangen und manipulieren
- Screenshots, Videos und Traces aufzeichnen
- Tests cross-browser und mit Mobile-Emulation ausführen
- JavaScript direkt ins DOM injizieren via `page.evaluate()`
- Playwright in einer GitHub Actions CI/CD-Pipeline betreiben

---

## Voraussetzungen

- Node.js 18+ installiert
- Git-Grundkenntnisse
- Grundkenntnisse in TypeScript/JavaScript
- Grundkenntnisse in React (hilfreich, aber nicht zwingend)

---

## Setup

### 1. Repository klonen und App starten

```bash
git clone https://github.com/harrybin/todo-react-playwright.git
cd todo-react-playwright
npm install
npm run dev
```

Die App läuft nun auf **http://localhost:3000**.  
Mach dich kurz mit der App vertraut: Aufgaben hinzufügen, bearbeiten, löschen, filtern und per Button Remote-Aufgaben laden.

### 2. Playwright installieren

```bash
npm install --save-dev @playwright/test
npx playwright install
```

### 3. Playwright-Konfiguration anlegen

Erstelle `playwright.config.ts` im Projektstamm:

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

Erstelle das Verzeichnis für Tests:

```bash
mkdir tests
```

Teste dein Setup:

```bash
npx playwright test --list
```

---

## Übersicht der App-Struktur

| Element | Locator-Hinweis |
|---|---|
| Seitentitel | `<h2>` mit Text "TodoMatic" |
| Eingabefeld neue Aufgabe | `id="new-todo-input"` |
| Hinzufügen-Button | `id="myUniqueID"` (Text: "Add") |
| Filter-Buttons | `data-testid="testID-All"`, `testID-Active`, `testID-Completed` |
| Aufgaben-Liste | `role="list"` mit `aria-labelledby="list-heading"` |
| Aufgaben-Anzahl Heading | `id="list-heading"` |
| Logo-Bild | `alt="Site Logo"` |
| Remote-Tasks-Button | Text "Load remote tasks" |
| Bearbeiten-Button (je Task) | Text "Edit" |
| Löschen-Button (je Task) | Text "Delete" |
| Speichern-Button (Edit-Mode) | Text "Save" |
| Abbrechen-Button (Edit-Mode) | Text "Cancel" |
| Checkbox je Task | `role="checkbox"` |

> **Hinweis zur Geolocation:** Das Hinzufügen einer Aufgabe ruft `navigator.geolocation.getCurrentPosition` auf. Du musst die Geolocation-API in Tests mocken, sonst schlägt der `addTask`-Aufruf lautlos fehl.

---

## Exercise 1: Smoke Test – Seiteninhalt prüfen

**Ziel:** Verstehen, wie ein einfacher Playwright-Test aufgebaut ist. Prüfe, ob die Seite korrekt lädt und alle wichtigen UI-Elemente sichtbar sind.

**Aufgabe:**

Erstelle `tests/smoke.spec.ts` und schreibe einen Test, der:

1. Die App unter `http://localhost:3000` öffnet
2. Den Seitentitel (Browser-Tab) prüft
3. Die Überschrift "TodoMatic" auf der Seite prüft
4. Das Eingabefeld und den "Add"-Button prüft
5. Die drei Filter-Buttons ("All", "Active", "Completed") prüft

**Laufbefehl:**

```bash
npx playwright test smoke.spec.ts
```

<details>
<summary>💡 Lösungshinweis</summary>

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

  // Filter-Buttons
  await expect(page.getByTestId("testID-All")).toBeVisible();
  await expect(page.getByTestId("testID-Active")).toBeVisible();
  await expect(page.getByTestId("testID-Completed")).toBeVisible();
});
```

**Warum `getByRole` für die Überschrift?**  
ARIA-Rollen-Locators sind die robustesten und barrierefreiheitsfreundlichsten. Sie funktionieren unabhängig von CSS-Klassen und IDs, solange die semantische Rolle stimmt. Das entspricht dem Ansatz aus den [PlaywrightDemos](https://github.com/norschel/PlaywrightDemos), wo `GetByRole` konsequent gegenüber CSS-Selektoren bevorzugt wird.

</details>

---

## Exercise 2: Geolocation mocken und Aufgabe hinzufügen

**Ziel:** Browser-APIs mocken und Formulare befüllen. Die App nutzt `navigator.geolocation`, was in Playwright-Tests explizit freigegeben werden muss.

**Aufgabe:**

Erstelle `tests/add-task.spec.ts` und schreibe einen Test, der:

1. Die Geolocation auf einen festen Standort (z. B. Latitude 48.1372, Longitude 11.5755 für München) mockt
2. Eine neue Aufgabe mit dem Namen "Playwright lernen" hinzufügt
3. Prüft, dass die Aufgabe in der Liste erscheint
4. Prüft, dass sich der Zähler in der Überschrift um 1 erhöht hat

**Hinweis:** Nutze `browserContext.setGeolocation()` und `browserContext.grantPermissions(['geolocation'])` im `test.use()`-Block oder direkt im Test.

<details>
<summary>💡 Lösungshinweis</summary>

```typescript
import { test, expect } from "@playwright/test";

test.use({
  geolocation: { latitude: 48.1372, longitude: 11.5755 },
  permissions: ["geolocation"],
});

test("add a new task", async ({ page }) => {
  await page.goto("/");

  // Initiale Anzahl auslesen
  const headingText = await page.locator("#list-heading").textContent();
  const initialCount = parseInt(headingText?.match(/\d+/)?.[0] ?? "0");

  // Neue Aufgabe eingeben
  await page.locator("#new-todo-input").fill("Playwright lernen");
  await page.locator("#myUniqueID").click();

  // Aufgabe in der Liste prüfen
  await expect(
    page.getByRole("list").getByText("Playwright lernen")
  ).toBeVisible();

  // Zähler hat sich erhöht
  await expect(page.locator("#list-heading")).toContainText(
    `${initialCount + 1}`
  );
});
```

**Warum Geolocation mocken?**  
Die `addTask`-Funktion in `App.tsx` ruft `navigator.geolocation.getCurrentPosition` auf. Ohne Grant hängt der Callback und die Aufgabe wird nie hinzugefügt. In echten Tests sollte man Browser-APIs immer mocken, um Tests stabil und deterministisch zu halten.

</details>

---

## Exercise 3: Aufgabe bearbeiten und löschen

**Ziel:** Interaktion mit dynamisch gerenderten Elementen. Edit-Mode aktivieren, Inhalt ändern, speichern und anschließend löschen.

**Aufgabe:**

Erweitere `tests/add-task.spec.ts` um zwei weitere Tests:

**Test A – Aufgabe bearbeiten:**
1. Füge eine Aufgabe "Alte Bezeichnung" hinzu
2. Klicke auf den "Edit"-Button der Aufgabe
3. Gib im Bearbeitungsfeld den neuen Namen "Neue Bezeichnung" ein
4. Klicke auf "Save"
5. Prüfe, dass "Neue Bezeichnung" in der Liste erscheint und "Alte Bezeichnung" nicht mehr sichtbar ist

**Test B – Aufgabe löschen:**
1. Füge eine Aufgabe "Zu löschende Aufgabe" hinzu
2. Klicke auf "Delete"
3. Prüfe, dass die Aufgabe aus der Liste verschwunden ist

**Herausforderung:** Wie sprichst du den Edit-Button eines *bestimmten* Listeneintrags an, ohne andere Einträge zu beeinflussen?

<details>
<summary>💡 Lösungshinweis</summary>

```typescript
import { test, expect } from "@playwright/test";

test.use({
  geolocation: { latitude: 48.1372, longitude: 11.5755 },
  permissions: ["geolocation"],
});

test("edit a task", async ({ page }) => {
  await page.goto("/");

  await page.locator("#new-todo-input").fill("Alte Bezeichnung");
  await page.locator("#myUniqueID").click();

  // Gezielt den Edit-Button im richtigen ListItem finden
  const taskItem = page
    .getByRole("listitem")
    .filter({ hasText: "Alte Bezeichnung" });
  await taskItem.getByRole("button", { name: "Edit" }).click();

  // Im Edit-Mode: Textfeld befüllen und speichern
  await taskItem.getByRole("textbox").fill("Neue Bezeichnung");
  await taskItem.getByRole("button", { name: "Save" }).click();

  await expect(page.getByRole("list").getByText("Neue Bezeichnung")).toBeVisible();
  await expect(page.getByRole("list").getByText("Alte Bezeichnung")).not.toBeVisible();
});

test("delete a task", async ({ page }) => {
  await page.goto("/");

  await page.locator("#new-todo-input").fill("Zu löschende Aufgabe");
  await page.locator("#myUniqueID").click();

  const taskItem = page
    .getByRole("listitem")
    .filter({ hasText: "Zu löschende Aufgabe" });
  await taskItem.getByRole("button", { name: "Delete" }).click();

  await expect(
    page.getByRole("list").getByText("Zu löschende Aufgabe")
  ).not.toBeVisible();
});
```

**Schlüsselkonzept – Locator-Chaining mit `.filter()`:**  
`.filter({ hasText: "..." })` schränkt einen breiten Locator auf Elemente ein, die bestimmten Text enthalten. Das `.And()`-Muster ist das C#-Äquivalent in den [PlaywrightDemos](https://github.com/norschel/PlaywrightDemos/blob/main/PlaywrightDemos/PlaywrightE2ETests_DDC2024.cs). Wichtig: Immer so spezifisch wie nötig locaten, aber so generisch wie möglich – das macht Tests wartbar.

</details>

---

## Exercise 4: Filter-Funktionalität testen

**Ziel:** Zustandsabhängige UI-Tests. Prüfe, dass die Filter-Buttons (All / Active / Completed) die Aufgabenliste korrekt filtern.

**Aufgabe:**

Erstelle `tests/filter.spec.ts` und schreibe einen Test, der:

1. Drei Aufgaben hinzufügt: "Task A", "Task B", "Task C"
2. "Task A" als abgeschlossen markiert (Checkbox anklicken)
3. Auf den "Completed"-Filter klickt und prüft:
   - "Task A" ist sichtbar
   - "Task B" und "Task C" sind nicht sichtbar
4. Auf den "Active"-Filter klickt und prüft:
   - "Task B" und "Task C" sind sichtbar
   - "Task A" ist nicht sichtbar
5. Auf "All" zurückschaltet und prüft, dass alle drei Aufgaben sichtbar sind
6. Prüfe außerdem das `aria-pressed`-Attribut des aktiven Filter-Buttons

<details>
<summary>💡 Lösungshinweis</summary>

```typescript
import { test, expect } from "@playwright/test";

test.use({
  geolocation: { latitude: 48.1372, longitude: 11.5755 },
  permissions: ["geolocation"],
});

async function addTask(page: import("@playwright/test").Page, name: string) {
  await page.locator("#new-todo-input").fill(name);
  await page.locator("#myUniqueID").click();
  // Warten bis die Aufgabe in der Liste erscheint
  await expect(page.getByRole("list").getByText(name)).toBeVisible();
}

test("filter buttons work correctly", async ({ page }) => {
  await page.goto("/");

  await addTask(page, "Task A");
  await addTask(page, "Task B");
  await addTask(page, "Task C");

  // Task A als abgeschlossen markieren
  const taskA = page.getByRole("listitem").filter({ hasText: "Task A" });
  await taskA.getByRole("checkbox").check();

  // Completed-Filter
  await page.getByTestId("testID-Completed").click();
  await expect(page.getByTestId("testID-Completed")).toHaveAttribute(
    "aria-pressed",
    "true"
  );
  await expect(page.getByRole("list").getByText("Task A")).toBeVisible();
  await expect(page.getByRole("list").getByText("Task B")).not.toBeVisible();
  await expect(page.getByRole("list").getByText("Task C")).not.toBeVisible();

  // Active-Filter
  await page.getByTestId("testID-Active").click();
  await expect(page.getByRole("list").getByText("Task B")).toBeVisible();
  await expect(page.getByRole("list").getByText("Task C")).toBeVisible();
  await expect(page.getByRole("list").getByText("Task A")).not.toBeVisible();

  // Alle anzeigen
  await page.getByTestId("testID-All").click();
  await expect(page.getByRole("list").getByText("Task A")).toBeVisible();
  await expect(page.getByRole("list").getByText("Task B")).toBeVisible();
  await expect(page.getByRole("list").getByText("Task C")).toBeVisible();
});
```

**Tipp:** Das `aria-pressed`-Attribut ist ein echter Barrierefreiheits-Check – du testest nicht nur Optik, sondern auch semantisch korrektes HTML.

</details>

---

## Exercise 5: Netzwerk-Mocking – Remote-Tasks abfangen

**Ziel:** Netzwerk-Requests abfangen und durch Testdaten ersetzen. Verwende `page.route()`, um die `remoteTasks.json`-Anfrage zu mocken.

**Hintergrund:**  
Der "Load remote tasks"-Button in der App ruft `fetch("remoteTasks.json")` auf. In Tests wollen wir kontrollierte Testdaten verwenden – unabhängig vom echten Server.

**Aufgabe:**

Erstelle `tests/network.spec.ts`:

**Test A – Erfolgreiches Laden von gemockten Remote-Tasks:**
1. Mocke die GET-Anfrage an `**/remoteTasks.json` und liefere ein JSON-Array mit zwei definierten Testaufgaben zurück
2. Klicke auf "Load remote tasks"
3. Prüfe, dass genau deine gemockten Aufgaben in der Liste erscheinen

**Test B – Fehlerfall simulieren (HTTP 500):**
1. Mocke die Anfrage so, dass sie HTTP 500 zurückgibt
2. Klicke auf "Load remote tasks"
3. Prüfe, dass die App korrekt reagiert (z. B. keine Aufgaben geladen werden oder ein Fehlerhinweis erscheint)

**Test C – Anfrage verzögern (Slow Network):**
1. Mocke die Anfrage mit einer künstlichen Verzögerung von 2 Sekunden
2. Prüfe das Verhalten der App während der Ladezeit

<details>
<summary>💡 Lösungshinweis</summary>

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
  // Route MUSS vor dem page.goto() registriert werden
  await page.route("**/remoteTasks.json", (route) =>
    route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify(mockTasks),
    })
  );

  await page.goto("/");
  await page.getByRole("button", { name: "Load remote tasks" }).click();

  await expect(
    page.getByRole("list").getByText("Gemockte Aufgabe 1")
  ).toBeVisible();
  await expect(
    page.getByRole("list").getByText("Gemockte Aufgabe 2")
  ).toBeVisible();
});

test("load remote tasks - server error", async ({ page }) => {
  await page.route("**/remoteTasks.json", (route) =>
    route.fulfill({ status: 500, body: "Internal Server Error" })
  );

  await page.goto("/");
  await page.getByRole("button", { name: "Load remote tasks" }).click();

  // App sollte keine Aufgaben aus dem Fehler-Response laden
  await expect(
    page.getByRole("list").getByText("Gemockte Aufgabe")
  ).not.toBeVisible();
});

test("load remote tasks - slow network", async ({ page }) => {
  await page.route("**/remoteTasks.json", async (route) => {
    await new Promise((r) => setTimeout(r, 2000)); // 2s Verzögerung
    await route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify(mockTasks),
    });
  });

  await page.goto("/");
  await page.getByRole("button", { name: "Load remote tasks" }).click();

  // Aufgaben erscheinen nach der Verzögerung
  await expect(
    page.getByRole("list").getByText("Gemockte Aufgabe 1")
  ).toBeVisible({ timeout: 5000 });
});
```

**Vertiefung aus PlaywrightDemos:**  
In den [norschel/PlaywrightDemos](https://github.com/norschel/PlaywrightDemos/blob/main/PlaywrightDemos/PlaywrightE2ETests_IT_Tage_2025.cs) wird das fortgeschrittene `route.fetch()`-Muster gezeigt: erst den echten Request absenden, dann den Response-Body ersetzen. Das ist sinnvoll, wenn du reale HTTP-Header brauchst, aber die Daten kontrollieren willst.

</details>

---

## Exercise 6: Logo-Bild durch Testbild ersetzen (Response-Manipulation)

**Ziel:** Das fortgeschrittenste Netzwerk-Mocking-Pattern: Den echten Response empfangen, aber den Body durch lokale Testdaten ersetzen – inspiriert von den spektakulären "Holiday Theme"-Demos aus den [PlaywrightDemos](https://github.com/norschel/PlaywrightDemos).

**Aufgabe:**

1. Erstelle ein kleines Test-PNG (z. B. 150×50 px, einfarbig) unter `tests/fixtures/test-logo.png`
2. Schreibe in `tests/network.spec.ts` einen Test, der:
   - Die Anfrage an `**/getsitelogo.png` abfängt
   - Das Test-Logo als Response zurückliefert
   - Prüft, dass das `<img>`-Element mit `alt="Site Logo"` sichtbar ist
3. **Bonusaufgabe:** Verwende `route.fetch()`, um den echten Request durchzulassen, aber den Body durch dein Test-Logo zu ersetzen

<details>
<summary>💡 Lösungshinweis</summary>

```typescript
import { test, expect } from "@playwright/test";
import { readFileSync } from "fs";
import { join } from "path";

test("replace logo with test image", async ({ page }) => {
  const logoBytes = readFileSync(join(__dirname, "fixtures", "test-logo.png"));

  await page.route("**/getsitelogo.png", (route) =>
    route.fulfill({
      status: 200,
      contentType: "image/png",
      body: logoBytes,
    })
  );

  await page.goto("/");

  const logo = page.getByRole("img", { name: "Site Logo" });
  await expect(logo).toBeVisible();
});

// Fortgeschrittene Variante: Echter Request + Body-Ersatz
test("replace logo body after real fetch", async ({ page }) => {
  const logoBytes = readFileSync(join(__dirname, "fixtures", "test-logo.png"));

  await page.route("**/getsitelogo.png", async (route) => {
    const response = await route.fetch(); // echten Request abschicken
    await route.fulfill({
      response,                // originale Headers übernehmen
      body: logoBytes,         // nur Body ersetzen
    });
  });

  await page.goto("/");
  await expect(page.getByRole("img", { name: "Site Logo" })).toBeVisible();
});
```

**Verbindung zu PlaywrightDemos:**  
Dieses Pattern ist die JavaScript-Entsprechung des "Santa Hat"-Demos aus den [IT-Tage 2025 Demos](https://github.com/norschel/PlaywrightDemos/blob/main/PlaywrightDemos/PlaywrightE2ETests_IT_Tage_2025.cs), wo Speaker-Fotos durch Weihnachtsmützen-Bilder ersetzt wurden. Gleiches Prinzip, anderer Anwendungsfall.

</details>

---

## Exercise 7: Screenshots, Video und Traces

**Ziel:** Diagnosewerkzeuge von Playwright kennenlernen – essenziell für CI/CD-Debugging.

### Teil A: Screenshot bei Fehler

**Aufgabe:**

1. Konfiguriere `playwright.config.ts` so, dass bei einem fehlgeschlagenen Test automatisch ein Screenshot gespeichert wird
2. Schreibe einen Test, der absichtlich fehlschlägt (z. B. erwarte Text, der nicht vorhanden ist)
3. Inspiziere den generierten Screenshot in `test-results/`

<details>
<summary>💡 Lösungshinweis</summary>

In `playwright.config.ts`:

```typescript
use: {
  screenshot: "only-on-failure",   // Screenshot nur bei Fehler
  // oder: screenshot: "on",       // immer
},
```

Manuell im Test:

```typescript
test("take screenshot on demand", async ({ page }) => {
  await page.goto("/");
  await page.screenshot({ path: "tests/screenshots/app-state.png", fullPage: true });
});
```

</details>

### Teil B: Video-Aufnahme

**Aufgabe:**

1. Aktiviere die Video-Aufnahme für einen bestimmten Test (oder global in der Config)
2. Führe den Test aus und finde die `.webm`-Datei im `test-results/`-Ordner
3. Spiele das Video im Browser ab

<details>
<summary>💡 Lösungshinweis</summary>

In `playwright.config.ts`:

```typescript
use: {
  video: "retain-on-failure",  // Video nur bei Fehler behalten
  // oder: video: "on",        // immer aufzeichnen
},
```

Per `test.use()` nur für einzelne Tests:

```typescript
test.use({ video: "on" });

test("record video of task workflow", async ({ page }) => {
  await page.goto("/");
  // ... Interaktionen
});
```

</details>

### Teil C: Playwright Trace Viewer

**Aufgabe:**

1. Aktiviere Traces (`trace: "on"`)
2. Führe einen Test aus
3. Öffne den Trace Viewer mit `npx playwright show-trace test-results/.../trace.zip`
4. Navigiere durch die einzelnen Test-Schritte und inspiziere DOM-Snapshots und Netzwerk-Requests

<details>
<summary>💡 Lösungshinweis</summary>

In `playwright.config.ts`:

```typescript
use: {
  trace: "on",              // Trace immer aufzeichnen
  // oder: "on-first-retry" // Trace nur beim ersten Retry
},
```

Manuell im Test steuern:

```typescript
test("manual trace control", async ({ page, context }) => {
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

**Aus PlaywrightDemos:**  
Tracing wurde ab BASTA! 2024 in die [Demos](https://github.com/norschel/PlaywrightDemos/blob/main/PlaywrightDemos/PlaywrightE2ETests_Basta2024.cs) integriert und ist seitdem das primäre Debugging-Werkzeug. Für CI/CD unersetzlich, da man ohne lokalen Browser Fehlschläge nachvollziehen kann.

</details>

---

## Exercise 8: Mobile Device Emulation

**Ziel:** Die App auf Mobilgeräten testen. Playwright kann Hunderte von Geräteprofilen (Viewport, User-Agent, Touch-Events, Pixel-Ratio) simulieren.

**Aufgabe:**

1. Schreibe in `tests/mobile.spec.ts` einen Test mit iPhone 15 Pro Emulation
2. Prüfe, dass alle Hauptelemente (Überschrift, Formular, Filter) auch auf dem mobilen Viewport sichtbar und bedienbar sind
3. Mache einen Screenshot, um die mobile Darstellung zu dokumentieren
4. **Bonusaufgabe:** Vergleiche das Verhalten zwischen iPhone (Portrait) und iPad (Landscape)

<details>
<summary>💡 Lösungshinweis</summary>

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
  await expect(page.locator("#myUniqueID")).toBeVisible();

  // Aufgabe auf Mobile hinzufügen
  await page.locator("#new-todo-input").fill("Mobile Task");
  await page.locator("#myUniqueID").click();
  await expect(page.getByRole("list").getByText("Mobile Task")).toBeVisible();

  await page.screenshot({
    path: "tests/screenshots/mobile-iphone15.png",
    fullPage: true,
  });
});
```

Alle verfügbaren Gerätedefinitionen anzeigen:

```typescript
import { devices } from "@playwright/test";
console.log(Object.keys(devices)); // Liste aller Geräteprofile
```

**Aus PlaywrightDemos:**  
Mobile-Emulation ist seit den [WDC 2023 Demos](https://github.com/norschel/PlaywrightDemos/blob/main/PlaywrightDemos/PlaywrightE2ETests_WDC2023.cs) ein fester Bestandteil. Die Kombination aus Geräte-Preset + Video-Recording ermöglicht es, das mobile UX exakt zu dokumentieren.

</details>

---

## Exercise 9: Cross-Browser Testing

**Ziel:** Tests auf mehreren Browsern gleichzeitig ausführen.

**Aufgabe:**

1. Erweitere `playwright.config.ts` um Firefox und WebKit (Safari)
2. Führe deine Tests aus und beobachte, wie Playwright sie parallel in allen Browsern ausführt
3. **Bonusaufgabe:** Schreibe einen parametrisierten Test, der den Browser-Namen in der Logausgabe anzeigt

<details>
<summary>💡 Lösungshinweis</summary>

In `playwright.config.ts`:

```typescript
projects: [
  { name: "chromium", use: { ...devices["Desktop Chrome"] } },
  { name: "firefox",  use: { ...devices["Desktop Firefox"] } },
  { name: "webkit",   use: { ...devices["Desktop Safari"] } },
  { name: "edge",     use: { ...devices["Desktop Edge"] } },
  // Mobile Browser
  { name: "mobile-chrome", use: { ...devices["Pixel 7"] } },
  { name: "mobile-safari", use: { ...devices["iPhone 15"] } },
],
```

Nur einen bestimmten Browser testen:

```bash
npx playwright test --project=firefox
npx playwright test --project=webkit
```

Browser-Information im Test abrufen:

```typescript
test("check browser name", async ({ page, browserName }) => {
  console.log(`Running on: ${browserName}`);
  await page.goto("/");
  await expect(page.getByRole("heading", { name: "TodoMatic" })).toBeVisible();
});
```

</details>

---

## Exercise 10: JavaScript in die Seite injizieren mit `page.evaluate()`

**Ziel:** Das fortgeschrittenste Playwright-Feature: JavaScript direkt im Browser-Kontext ausführen, DOM manipulieren und Browser-APIs aufrufen.

**Aufgabe:**

**Teil A – DOM auslesen:**  
Lies mit `page.evaluate()` alle aktuellen Aufgaben-Namen aus der DOM-Struktur aus (nicht über Playwright-Locatoren, sondern direkt per JavaScript).

**Teil B – DOM manipulieren:**  
Injiziere ein rotes Banner in die Seite mit dem Text "⚠️ TESTMODUS AKTIV" und prüfe, dass es sichtbar ist.

**Teil C – Seiten-Zustand prüfen:**  
Lese den aktuellen `localStorage`-Inhalt aus (auch wenn die App ihn nicht nutzt – als Übung für echte Apps mit State-Persistence).

**Bonusaufgabe (inspiriert von PlaywrightDemos):**  
Injiziere einen animierten Canvas-Overlay. Erstelle mit `page.evaluate()` ein Canvas-Element und zeige darauf einen Text an. Mache danach einen Screenshot.

<details>
<summary>💡 Lösungshinweis</summary>

**Teil A – Task-Namen auslesen:**

```typescript
test("read task names via evaluate", async ({ page }) => {
  await page.goto("/");

  // Alle heading-Texte aus der Aufgabenliste
  const taskNames = await page.evaluate(() => {
    const headings = document.querySelectorAll('[role="list"] h4');
    return Array.from(headings).map((h) => h.textContent?.trim());
  });

  console.log("Aufgaben:", taskNames);
});
```

**Teil B – Banner injizieren:**

```typescript
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
```

**Teil C – localStorage:**

```typescript
test("read localStorage", async ({ page }) => {
  await page.goto("/");

  const storage = await page.evaluate(() => {
    return Object.fromEntries(
      Object.keys(localStorage).map((key) => [key, localStorage.getItem(key)])
    );
  });

  console.log("localStorage:", storage);
});
```

**Bonusaufgabe – Canvas-Overlay (in Anlehnung an PlaywrightDemos):**

```typescript
test("canvas overlay via evaluate", async ({ page }) => {
  await page.goto("/");

  await page.evaluate(() => {
    const canvas = document.createElement("canvas");
    canvas.width = 400;
    canvas.height = 100;
    Object.assign(canvas.style, {
      position: "fixed",
      bottom: "20px",
      right: "20px",
      zIndex: "9999",
      border: "2px solid #333",
      borderRadius: "8px",
      backgroundColor: "rgba(0,0,0,0.7)",
    });
    document.body.appendChild(canvas);

    const ctx = canvas.getContext("2d")!;
    ctx.fillStyle = "white";
    ctx.font = "bold 24px Arial";
    ctx.textAlign = "center";
    ctx.fillText("🎭 Playwright Demo", 200, 45);
    ctx.font = "14px Arial";
    ctx.fillStyle = "#aaa";
    ctx.fillText("norschel/PlaywrightDemos inspired", 200, 75);
  });

  await page.screenshot({
    path: "tests/screenshots/canvas-overlay.png",
    fullPage: true,
  });
});
```

**Aus PlaywrightDemos:**  
Dieses Pattern ist der Höhepunkt der [BASTA! Spring 2026 Demos](https://github.com/norschel/PlaywrightDemos/blob/main/PlaywrightDemos/PlaywrightE2ETests_BastaSpring2026.cs): Animierte Osterhasen und Schneeflocken werden per `EvaluateAsync` live auf Konferenz-Websites gezeichnet – alles über Playwright gesteuert.

</details>

---

## Exercise 11: CI/CD mit GitHub Actions

**Ziel:** Playwright-Tests in einer GitHub Actions-Pipeline automatisieren, Artefakte (Screenshots, Videos, Traces, HTML-Report) speichern.

**Aufgabe:**

Erstelle `.github/workflows/playwright.yml`:

1. Der Workflow soll bei jedem Push und Pull Request auf `main` ausgelöst werden
2. Installiere Node.js, Abhängigkeiten und Playwright-Browser
3. Starte die App und führe die Tests aus
4. Speichere den HTML-Report und die Test-Artefakte (Screenshots, Videos, Traces)
5. **Bonusaufgabe:** Füge eine Test-Matrix für mehrere Node.js-Versionen hinzu

<details>
<summary>💡 Lösungshinweis</summary>

```yaml
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

      - name: Upload test artifacts (screenshots/videos/traces)
        uses: actions/upload-artifact@v4
        if: failure()
        with:
          name: test-results
          path: test-results/
          retention-days: 7
```

**Aus PlaywrightDemos:**  
Das [dotnet.yml](https://github.com/norschel/PlaywrightDemos/blob/main/.github/workflows/dotnet.yml) der PlaywrightDemos lädt `*.webm`, `*.png` und `*.zip`-Artefakte hoch und nutzt das `CICD`-Tag, um gezielt nur die aktuellen Tests in der Pipeline auszuführen. In JavaScript-Projekten übernehmen Playwright-Tags (`--grep`) diese Funktion.

</details>

---

## Zusammenfassung: Gelerntes auf einen Blick

| Konzept | Playwright-API | Übung |
|---|---|---|
| Navigation | `page.goto()` | 1–11 |
| ARIA-Locatoren | `getByRole()`, `getByTestId()` | 1, 3, 4 |
| Formular-Interaktion | `fill()`, `click()`, `check()` | 2, 3 |
| Locator-Chaining | `.filter()`, `.getByRole()` auf Elternelement | 3, 4 |
| Geolocation mocken | `test.use({ geolocation, permissions })` | 2, 3, 8 |
| Netzwerk-Mocking | `page.route()`, `route.fulfill()` | 5, 6 |
| Response-Manipulation | `route.fetch()` + `route.fulfill({ response })` | 6 |
| Screenshots | `page.screenshot()`, `screenshot: "only-on-failure"` | 7 |
| Video-Aufnahme | `video: "retain-on-failure"` | 7 |
| Trace Viewer | `context.tracing`, `show-trace` | 7 |
| Mobile Emulation | `devices["iPhone 15 Pro"]` | 8 |
| Cross-Browser | `projects` in Config | 9 |
| JS-Injektion | `page.evaluate()` | 10 |
| CI/CD | GitHub Actions Workflow | 11 |

---

## Weiterführende Ressourcen

- 📖 [Playwright Dokumentation](https://playwright.dev/docs/intro)
- 🎭 [norschel/PlaywrightDemos](https://github.com/norschel/PlaywrightDemos) – Konferenz-Demos mit fortgeschrittenen Patterns
- 🛠 [Playwright Trace Viewer](https://playwright.dev/docs/trace-viewer)
- 📱 [Emulierte Geräte-Liste](https://github.com/microsoft/playwright/blob/main/packages/playwright-core/src/server/deviceDescriptorsSource.json)
- 🌐 [Azure Playwright Testing Service](https://azure.microsoft.com/de-de/products/playwright-testing) – Cloud-Browser-Farm für skalierte Test-Ausführung
- 🔬 [Playwright Inspector](https://playwright.dev/docs/inspector) – Interaktiver Debugger
- 📊 [Playwright HTML Reporter](https://playwright.dev/docs/test-reporters#html-reporter)

---

*HOL erstellt für das todo-react-playwright Repo – basierend auf echten Konferenz-Demos von [Nico Orschel](https://github.com/norschel) (norschel/PlaywrightDemos, BASTA! / MDD / IT-Tage 2023–2026)*
