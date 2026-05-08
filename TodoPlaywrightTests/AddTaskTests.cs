// Exercise 2: Geolocation mocken und Aufgabe hinzufügen
// TestBase erbt bereits Geolocation + BaseURL – kein ContextOptions()-Override nötig.

[TestClass]
public class AddTaskTests : TestBase
{
    [TestMethod]
    [TestCategory("CICD")]
    public async Task AddNewTask()
    {
        await Page.GotoAsync("/");

        // Ausgangszähler auslesen (App startet mit 1)
        var headingText = await Page.Locator("#list-heading").TextContentAsync();
        var initialCount = int.Parse(Regex.Match(headingText ?? "0", @"\d+").Value);

        // Neue Aufgabe hinzufügen
        await Page.Locator("#new-todo-input").FillAsync("Playwright lernen");
        await Page.Locator("#myUniqueID").ClickAsync();

        // Aufgabe muss in der Liste erscheinen
        await Expect(
            Page.GetByRole(AriaRole.List).GetByText("Playwright lernen")
        ).ToBeVisibleAsync();

        // Zähler muss um 1 gestiegen sein
        await Expect(Page.Locator("#list-heading"))
            .ToContainTextAsync($"{initialCount + 1}");
    }
}
