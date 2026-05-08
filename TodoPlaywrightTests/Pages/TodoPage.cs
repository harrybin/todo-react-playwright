// Exercise 11: Page Object Model – TodoPage
// Kapselt alle Locatoren und Aktionen für die TodoMatic-Seite.
// Framework-unabhängig: wird von MSTest, NUnit und xUnit gleich verwendet.
//
// Vorteile:
//   - Wartbarkeit: Selektor ändert sich → nur EINE Stelle anpassen
//   - Wiederverwendung: addTask(), deleteTask() etc. in vielen Tests nutzbar
//   - Lesbarkeit: Tests beschreiben WAS getestet wird, nicht WIE das DOM navigiert wird

namespace TodoPlaywrightTests.Pages;

public class TodoPage(IPage page)
{
    private readonly IPage _page = page;

    // ── Statische Locatoren als Properties (lazy evaluation) ─────────────────
    public ILocator AddInput        => _page.Locator("#new-todo-input");
    public ILocator AddButton       => _page.Locator("#myUniqueID");
    public ILocator TaskCount       => _page.Locator("#list-heading");
    public ILocator FilterAll       => _page.GetByTestId("testID-All");
    public ILocator FilterActive    => _page.GetByTestId("testID-Active");
    public ILocator FilterCompleted => _page.GetByTestId("testID-Completed");
    public ILocator TaskList        => _page.GetByRole(AriaRole.List);
    public ILocator LoadRemoteButton => _page.GetByRole(AriaRole.Button, new() { Name = "Load remote tasks" });

    // ── Dynamische Locatoren als Methoden ─────────────────────────────────────
    public ILocator TaskItem(string name) =>
        _page.GetByRole(AriaRole.Listitem).Filter(new() { HasText = name });

    // ── Aktionen ─────────────────────────────────────────────────────────────
    public Task GotoAsync() => _page.GotoAsync("/");

    public async Task AddTaskAsync(string name)
    {
        await AddInput.FillAsync(name);
        await AddButton.ClickAsync();
        await Assertions.Expect(TaskList.GetByText(name)).ToBeVisibleAsync();
    }

    public async Task DeleteTaskAsync(string name)
    {
        await TaskItem(name).GetByRole(AriaRole.Button, new() { Name = "Delete" }).ClickAsync();
        await Assertions.Expect(TaskList.GetByText(name)).Not.ToBeVisibleAsync();
    }

    public async Task EditTaskAsync(string oldName, string newName)
    {
        var item = TaskItem(oldName);
        await item.GetByRole(AriaRole.Button, new() { Name = "Edit" }).ClickAsync();
        await item.GetByRole(AriaRole.Textbox).FillAsync(newName);
        await item.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
        await Assertions.Expect(TaskList.GetByText(newName)).ToBeVisibleAsync();
    }

    // MUI-Checkbox: ClickAsync() statt CheckAsync(), da MUI-Ripple-Wrapper
    // CheckAsync() (State-Verifikation) stören kann.
    public Task CompleteTaskAsync(string name) =>
        TaskItem(name).GetByRole(AriaRole.Checkbox).ClickAsync();

    public Task SetFilterAsync(string filter)
    {
        var btn = filter switch
        {
            "Active"    => FilterActive,
            "Completed" => FilterCompleted,
            _           => FilterAll,
        };
        return btn.ClickAsync();
    }

    public async Task<int> GetTaskCountAsync()
    {
        var text = await TaskCount.TextContentAsync();
        return int.Parse(Regex.Match(text ?? "0", @"\d+").Value);
    }
}
