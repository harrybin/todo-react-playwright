// Exercise 6: Response-Manipulation – Logo durch Testbild ersetzen
using Microsoft.Playwright.MSTest;
// Fortgeschrittenes Route-Pattern: echten Request abschicken, dann nur den Body ersetzen.
// Inspiriert vom "Holiday Theme"-Demo der norschel/PlaywrightDemos.
// Das Testbild liegt unter fixtures/test-logo.png und wird beim Build ins Output-Dir kopiert.

[TestClass]
public class ImageMockTests : PageTest
{
    // Pfad zur Fixture-Datei (wird via CopyToOutputDirectory bereitgestellt)
    private static readonly string LogoFixturePath = Path.Combine("fixtures", "test-logo.png");

    // Test A: Logo direkt durch Testbild ersetzen (route.fulfill ohne echten Request)
    [TestMethod]
    [TestCategory("CICD")]
    public async Task ReplaceLogo()
    {
        var logoBytes = await File.ReadAllBytesAsync(LogoFixturePath);

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

    // Test B (Bonus): Echter Request + Body-Ersatz
    // route.FetchAsync() schickt den echten Request ab; danach wird nur der Body ersetzt.
    // Das ist die Kernidee des "Santa Hat"-Demos aus dem IT-Tage 2025.
    [TestMethod]
    public async Task ReplaceLogoBodyAfterRealFetch()
    {
        var logoBytes = await File.ReadAllBytesAsync(LogoFixturePath);

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
