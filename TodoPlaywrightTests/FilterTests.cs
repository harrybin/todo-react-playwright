// Exercise 4: Filter-Funktionalität testen
// Prüft, dass All/Active/Completed die Liste korrekt filtern und
// aria-pressed den aktiven Button korrekt kennzeichnet.
// TestBase erbt Geolocation + BaseURL – kein ContextOptions()-Override nötig.

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
    [TestCategory("CICD")]
    public async Task FilterButtonsWorkCorrectly()
    {
        await Page.GotoAsync("/");

        await AddTaskAsync("Task A");
        await AddTaskAsync("Task B");
        await AddTaskAsync("Task C");

        // MUI-Checkbox: ClickAsync() statt CheckAsync(), da MUI die Ripple-Animation
        // als wrapper rendert – CheckAsync() verifiziert den State und schlägt manchmal fehl.
        await Page.GetByRole(AriaRole.Listitem)
            .Filter(new() { HasText = "Task A" })
            .GetByRole(AriaRole.Checkbox)
            .ClickAsync();

        // --- Completed-Filter ---
        await Page.GetByTestId("testID-Completed").ClickAsync();
        // aria-pressed="true" zeigt den aktiven Filter
        await Expect(Page.GetByTestId("testID-Completed"))
            .ToHaveAttributeAsync("aria-pressed", "true");
        await Expect(Page.GetByRole(AriaRole.List).GetByText("Task A")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.List).GetByText("Task B")).Not.ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.List).GetByText("Task C")).Not.ToBeVisibleAsync();

        // --- Active-Filter ---
        await Page.GetByTestId("testID-Active").ClickAsync();
        await Expect(Page.GetByRole(AriaRole.List).GetByText("Task B")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.List).GetByText("Task C")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.List).GetByText("Task A")).Not.ToBeVisibleAsync();

        // --- All-Filter: alle drei sichtbar ---
        await Page.GetByTestId("testID-All").ClickAsync();
        foreach (var name in new[] { "Task A", "Task B", "Task C" })
        {
            await Expect(Page.GetByRole(AriaRole.List).GetByText(name)).ToBeVisibleAsync();
        }
    }
}
