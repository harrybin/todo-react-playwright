// Exercise 7: Screenshots, Video und Traces – Diagnose-Werkzeuge aktiv nutzen
// Teil A: Screenshots (manuell und per Element)
// Teil C: Trace Viewer (manueller Trace-Zyklus)
// Video-Aufnahme (Teil B) wird über Umgebungsvariable PLAYWRIGHT_VIDEO=on gesteuert.
// TestBase erbt Geolocation + BaseURL – kein ContextOptions()-Override nötig.

[TestClass]
public class DiagnosticsTests : TestBase
{
    // Teil A – Manueller Full-Page-Screenshot
    [TestMethod]
    public async Task ScreenshotAfterAddingTask()
    {
        Directory.CreateDirectory("screenshots");

        await Page.GotoAsync("/");

        // Full-Page-Screenshot (inkl. nicht sichtbarem Bereich)
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "screenshots/initial-state.png",
            FullPage = true,
        });

        await Page.Locator("#new-todo-input").FillAsync("Screenshot Task");
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "screenshots/after-typing.png",
        });

        // Ergebnis-Pfade in die Testausgabe loggen (in CI über TestResults sichtbar)
        Console.WriteLine($"Screenshot gespeichert: screenshots/initial-state.png");
        Console.WriteLine($"Screenshot gespeichert: screenshots/after-typing.png");
    }

    // Teil A Bonus – Screenshot eines einzelnen Elements (nur die Filter-Buttons)
    [TestMethod]
    public async Task ScreenshotOfSingleElement()
    {
        Directory.CreateDirectory("screenshots");

        await Page.GotoAsync("/");
        await Page.GetByTestId("testID-All").ScreenshotAsync(new LocatorScreenshotOptions
        {
            Path = "screenshots/filter-button.png",
        });

        Console.WriteLine("Screenshot gespeichert: screenshots/filter-button.png");
    }

    // Teil C – Manueller Trace (Start → Aktionen → Stop → ZIP speichern)
    [TestMethod]
    public async Task ManualTrace()
    {
        Directory.CreateDirectory("traces");

        // Trace starten (screenshots + DOM-Snapshots + Quellcode)
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

        // Trace Task löschen
        await Page.GetByRole(AriaRole.Listitem)
            .Filter(new() { HasText = "Trace Task" })
            .GetByRole(AriaRole.Button, new() { Name = "Delete" })
            .ClickAsync();

        // Trace stoppen und als ZIP speichern
        await Context.Tracing.StopAsync(new TracingStopOptions
        {
            Path = "traces/add-task-trace.zip",
        });

        // Als Testergebnis-Anhang loggen
        Console.WriteLine("Trace gespeichert: traces/add-task-trace.zip");
        // Öffnen mit: pwsh bin/Debug/<net-version>/playwright.ps1 show-trace traces/add-task-trace.zip
    }
}
