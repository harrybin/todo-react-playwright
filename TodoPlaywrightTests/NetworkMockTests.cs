// Exercise 5: Netzwerk-Mocking – Remote-Tasks abfangen
using Microsoft.Playwright.MSTest;
// page.RouteAsync() abfängt HTTP-Anfragen und ersetzt sie durch Testdaten.
// Kein TestBase – diese Tests setzen keine Geolocation voraus.

[TestClass]
public class NetworkMockTests : PageTest
{
    private static readonly string MockTasksJson = JsonSerializer.Serialize(new[]
    {
        new
        {
            id = "todo-mock-1",
            name = "Gemockte Aufgabe 1",
            time = "2024-01-01T10:00:00.000Z",
            location = new { latitude = 48.1372, longitude = 11.5755 },
            completed = false,
        },
        new
        {
            id = "todo-mock-2",
            name = "Gemockte Aufgabe 2",
            time = "2024-01-01T11:00:00.000Z",
            location = new { latitude = 52.52, longitude = 13.405 },
            completed = true,
        },
    });

    // Test A: Route mit zwei definierten Testaufgaben – prüfe, dass nur diese erscheinen
    [TestMethod]
    [TestCategory("CICD")]
    public async Task LoadRemoteTasks_Success()
    {
        // Route VOR goto registrieren!
        await Page.RouteAsync("**/remoteTasks.json", route => route.FulfillAsync(
            new RouteFulfillOptions
            {
                Status = 200,
                ContentType = "application/json",
                Body = MockTasksJson,
            }));

        await Page.GotoAsync("http://localhost:3000");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Load remote tasks" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.List).GetByText("Gemockte Aufgabe 1"))
            .ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.List).GetByText("Gemockte Aufgabe 2"))
            .ToBeVisibleAsync();
    }

    // Test B: HTTP 500 simulieren – App sollte keine gemockten Tasks anzeigen
    [TestMethod]
    [TestCategory("CICD")]
    public async Task LoadRemoteTasks_ServerError()
    {
        await Page.RouteAsync("**/remoteTasks.json", route => route.FulfillAsync(
            new RouteFulfillOptions { Status = 500, Body = "Internal Server Error" }));

        await Page.GotoAsync("http://localhost:3000");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Load remote tasks" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.List).GetByText("Gemockte"))
            .Not.ToBeVisibleAsync();
    }

    // Test C: 2-Sekunden-Verzögerung simulieren – Tasks erscheinen nach Warten
    [TestMethod]
    [TestCategory("CICD")]
    public async Task LoadRemoteTasks_SlowNetwork()
    {
        await Page.RouteAsync("**/remoteTasks.json", async route =>
        {
            await Task.Delay(2000);
            await route.FulfillAsync(new RouteFulfillOptions
            {
                Status = 200,
                ContentType = "application/json",
                Body = MockTasksJson,
            });
        });

        await Page.GotoAsync("http://localhost:3000");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Load remote tasks" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.List).GetByText("Gemockte Aufgabe 1"))
            .ToBeVisibleAsync(new() { Timeout = 5000 });
    }
}
