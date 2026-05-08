// Exercise 11: Page Object Model – Tests
// Zeigt, wie das POM die Tests deutlich kürzer und lesbarer macht.
// TestBase erbt Geolocation + BaseURL – kein ContextOptions()-Override nötig.

using TodoPlaywrightTests.Pages;

[TestClass]
public class PomTests : TestBase
{
    // Vollständiger Task-Lifecycle: hinzufügen → bearbeiten → abschließen → filtern → löschen
    [TestMethod]
    [TestCategory("CICD")]
    public async Task FullTaskLifecycle()
    {
        var todo = new TodoPage(Page);
        await todo.GotoAsync();

        await todo.AddTaskAsync("Einkaufen");
        await todo.EditTaskAsync("Einkaufen", "Einkaufen gehen");
        await todo.CompleteTaskAsync("Einkaufen gehen");
        await todo.SetFilterAsync("Completed");
        await Expect(todo.TaskItem("Einkaufen gehen")).ToBeVisibleAsync();
        await todo.SetFilterAsync("All");
        await todo.DeleteTaskAsync("Einkaufen gehen");

        // Nach dem Löschen der eigenen Aufgabe: zurück auf initiale 1 Aufgabe
        Assert.AreEqual(1, await todo.GetTaskCountAsync());
    }

    // Smoke Test via POM – zeigt, wie kurz ein Test mit POM werden kann
    [TestMethod]
    [TestCategory("CICD")]
    public async Task AddAndDeleteViaPom()
    {
        var todo = new TodoPage(Page);
        await todo.GotoAsync();

        await todo.AddTaskAsync("POM Task");
        Assert.AreEqual(2, await todo.GetTaskCountAsync());

        await todo.DeleteTaskAsync("POM Task");
        Assert.AreEqual(1, await todo.GetTaskCountAsync());
    }
}
