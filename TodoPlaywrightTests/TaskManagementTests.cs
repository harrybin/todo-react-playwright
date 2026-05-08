// Exercise 3: Aufgabe bearbeiten, löschen und Zähler prüfen
// Schlüsselkonzept: .Filter(new() { HasText }) für präzises Locator-Chaining
// in dynamisch gerenderten Listen.
// TestBase erbt Geolocation + BaseURL – kein ContextOptions()-Override nötig.

[TestClass]
public class TaskManagementTests : TestBase
{
    // Hilfsmethode: Aufgabe hinzufügen und sofort prüfen
    private async Task AddTaskAsync(string name)
    {
        await Page.Locator("#new-todo-input").FillAsync(name);
        await Page.Locator("#myUniqueID").ClickAsync();
        await Expect(Page.GetByRole(AriaRole.List).GetByText(name)).ToBeVisibleAsync();
    }

    // Test A: Aufgabe bearbeiten
    [TestMethod]
    [TestCategory("CICD")]
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

    // Test B: Aufgabe löschen
    [TestMethod]
    [TestCategory("CICD")]
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

    // Test C: CRUD-Zyklus mit Zähler prüfen
    [TestMethod]
    [TestCategory("CICD")]
    public async Task AddAndDeleteTask_CheckCount()
    {
        await Page.GotoAsync("/");

        // Ausgangszustand: App startet mit 1 Aufgabe
        await Expect(Page.Locator("#list-heading")).ToContainTextAsync("1 task remaining");

        // Neue Aufgabe anlegen
        await Page.Locator("#new-todo-input").FillAsync("Smoke Bonus Task");
        await Page.Locator("#myUniqueID").ClickAsync();

        // Nach dem Hinzufügen
        var newItem = Page.GetByRole(AriaRole.Listitem).Filter(new() { HasText = "Smoke Bonus Task" });
        await Expect(newItem).ToBeVisibleAsync();
        await Expect(Page.Locator("#list-heading")).ToContainTextAsync("2 tasks remaining");

        // Aufgabe löschen – Delete-Button des richtigen ListItems ansteuern
        await newItem.GetByRole(AriaRole.Button, new() { Name = "Delete" }).ClickAsync();

        // Nach dem Löschen
        await Expect(newItem).Not.ToBeVisibleAsync();
        await Expect(Page.Locator("#list-heading")).ToContainTextAsync("1 task remaining");
    }
}
