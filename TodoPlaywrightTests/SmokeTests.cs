// Exercise 1: Smoke Test – Seiteninhalt prüfen
// Prüft, ob die App korrekt lädt und alle wichtigen UI-Elemente sichtbar sind.

[TestClass]
public class SmokeTests : TestBase
{
    [TestMethod]
    [TestCategory("CICD")]
    public async Task AppLoadsCorrectly()
    {
        await Page.GotoAsync("/");

        // Browser-Titel prüfen
        await Expect(Page).ToHaveTitleAsync(new Regex("TodoMatic"));

        // Haupt-Überschrift via ARIA-Rolle (robust, barrierefrei)
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "TodoMatic" }))
            .ToBeVisibleAsync();

        // Formular-Elemente via ID (aus dem Quellcode bekannt)
        await Expect(Page.Locator("#new-todo-input")).ToBeVisibleAsync();
        await Expect(Page.Locator("#myUniqueID")).ToBeVisibleAsync();

        // Filter-Buttons via data-testid (bewusst stabil gewählt)
        await Expect(Page.GetByTestId("testID-All")).ToBeVisibleAsync();
        await Expect(Page.GetByTestId("testID-Active")).ToBeVisibleAsync();
        await Expect(Page.GetByTestId("testID-Completed")).ToBeVisibleAsync();

        // Initialer Zähler – App startet immer mit 1 Aufgabe ("test")!
        await Expect(Page.Locator("#list-heading")).ToContainTextAsync("1 task remaining");
    }
}
