// Gemeinsame Basisklasse für alle Testklassen.
// Setzt BaseURL, Geolocation (für addTask()) und Permissions einmalig –
// alle abgeleiteten Klassen erben diese Konfiguration automatisch.
// HINWEIS: Kein [TestClass] hier – TestBase ist nur eine Basisklasse,
// keine eigenständige Test-Klasse.
using Microsoft.Playwright.MSTest;

public class TestBase : PageTest
{
    public override BrowserNewContextOptions ContextOptions() => new()
    {
        BaseURL = Environment.GetEnvironmentVariable("PLAYWRIGHT_BASE_URL")
                  ?? "http://localhost:3000",
        // Geolocation global – benötigt von addTask() in allen Tests
        Geolocation = new Geolocation { Latitude = 52.1205f, Longitude = 11.6276f },
        Permissions = ["geolocation"],
    };
}
