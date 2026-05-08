// Exercise 10: JavaScript in die Seite injizieren mit EvaluateAsync()
using Microsoft.Playwright.MSTest;
// Teil A: Task-Namen per evaluate() direkt aus dem DOM auslesen
// Teil B: Rotes "TESTMODUS"-Banner injizieren und per Locator prüfen
// Teil C (Bonus): Canvas-Overlay – inspiriert vom BASTA! Spring 2026 Demo

[TestClass]
public class EvaluateTests : PageTest
{
    // Teil A: Task-Namen direkt aus dem DOM lesen
    [TestMethod]
    public async Task ReadTaskNamesViaEvaluate()
    {
        await Page.GotoAsync("http://localhost:3000");

        // Escaped quotes innerhalb des JS-Strings: "" = literal " in verbatim string
        var taskNames = await Page.EvaluateAsync<string[]>(@"
            () => {
                const headings = document.querySelectorAll('[role=""list""] h4');
                return Array.from(headings).map(h => h.textContent?.trim());
            }
        ");

        Console.WriteLine($"Aufgaben im DOM: {string.Join(", ", taskNames)}");
        // App startet mit 1 Aufgabe ("test") → taskNames darf nicht leer sein
        Assert.IsTrue(taskNames.Length >= 1, "Mindestens eine Aufgabe erwartet");
    }

    // Teil B: Rotes TESTMODUS-Banner injizieren und per Locator prüfen
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

        Directory.CreateDirectory("screenshots");
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "screenshots/test-mode.png",
        });
    }

    // Teil C (Bonus): Canvas-Overlay – analog zum BASTA! Spring 2026 Osterhasen-Demo
    // EvaluateAsync zeichnet per Canvas 2D API ein Overlay auf die Seite.
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

        Directory.CreateDirectory("screenshots");
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "screenshots/canvas-overlay.png",
            FullPage = true,
        });
    }
}
