// Exercise 8: Mobile Device Emulation
// Testet die App auf einem iPhone 15 Pro Viewport (Playwright.Devices-Profil).
// TestBase wird nicht verwendet – wir überschreiben ContextOptions() komplett,
// um das Geräteprofil zu kombinieren und gleichzeitig BaseURL + Geolocation beizubehalten.

[TestClass]
public class MobileTests : TestBase
{
    public override BrowserNewContextOptions ContextOptions()
    {
        // iPhone 15 Pro Geräteprofil – BaseURL und Geolocation aus TestBase übernehmen
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
    [TestCategory("CICD")]
    public async Task AppWorksOnMobile()
    {
        await Page.GotoAsync("/");

        // Hauptelemente auf dem mobilen Viewport prüfen
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "TodoMatic" }))
            .ToBeVisibleAsync();
        await Expect(Page.Locator("#new-todo-input")).ToBeVisibleAsync();
        await Expect(Page.GetByTestId("testID-All")).ToBeVisibleAsync();

        // Aufgabe auf mobilem Gerät hinzufügen
        await Page.Locator("#new-todo-input").FillAsync("Mobile Task");
        await Page.Locator("#myUniqueID").ClickAsync();
        await Expect(Page.GetByRole(AriaRole.List).GetByText("Mobile Task"))
            .ToBeVisibleAsync();

        // Screenshot als Nachweis
        Directory.CreateDirectory("screenshots");
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "screenshots/mobile-iphone15.png",
            FullPage = true,
        });
        Console.WriteLine("Screenshot gespeichert: screenshots/mobile-iphone15.png");
    }
}
