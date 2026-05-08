// Exercise 9: Cross-Browser Testing
// Tests werden via [DataRow] parametrisiert – Chromium, Firefox, WebKit, Edge, Chrome.
// Jeder Test-Aufruf startet eine eigene Browser-Instanz.
// Diese Klasse erbt NICHT von PageTest/TestBase – Browser werden manuell instanziiert.

[TestClass]
public class CrossBrowserTests
{
    [TestMethod]
    [DataRow("Chromium")]
    [DataRow("Firefox")]
    [DataRow("Webkit")]
    public async Task AppLoadsInAllBrowsers(string browserName)
    {
        using var playwright = await Playwright.CreateAsync();

        // Neue Instanz je Browser – BrowserTypeLaunchOptions ist kein Record,
        // daher kein with-Ausdruck verfügbar (kein Clone-Mechanismus).
        IBrowser browser = browserName switch
        {
            "Chromium" => await playwright.Chromium.LaunchAsync(new() { Headless = true }),
            "Firefox"  => await playwright.Firefox.LaunchAsync(new() { Headless = true }),
            "Webkit"   => await playwright.Webkit.LaunchAsync(new() { Headless = true }),
            "Edge"     => await playwright.Chromium.LaunchAsync(
                              new() { Headless = true, Channel = "msedge" }),
            "Chrome"   => await playwright.Chromium.LaunchAsync(
                              new() { Headless = true, Channel = "chrome" }),
            _          => throw new ArgumentException($"Unknown browser: {browserName}"),
        };

        var page = await browser.NewPageAsync();
        await page.GotoAsync("http://localhost:3000");

        var title = await page.TitleAsync();
        StringAssert.Contains(title, "TodoMatic");

        Console.WriteLine($"✅ {browserName}: {title}");
        await browser.CloseAsync();
    }
}
