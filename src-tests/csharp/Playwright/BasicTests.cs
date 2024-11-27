using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace TodoMatic;

[TestClass]
public class Test1 : PageTest
{
  [TestMethod]
  public async Task SimpleTest_AddElement()
  {
    IBrowser browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
    {
      Headless = false,
      SlowMo = 5000
    });

    IPage page = await browser.NewPageAsync(new BrowserNewPageOptions
    {
      Permissions = ["geolocation"],
      Geolocation = new Geolocation
      {
        Latitude = 37.7749F,
        Longitude = -122.4194F
      }
    });

    await page.GotoAsync("http://localhost:3000/");

    await page.FillAsync("id=new-todo-input", "buy milk");
    await page.ClickAsync("id=myUniqueID");

    await Expect(page.Locator("text=buy milk")).ToBeVisibleAsync();
  }

  [TestMethod]
  public async Task SimpleTest_DeleteElement()
  {
    IBrowser browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
    {
      Headless = false,
      SlowMo = 1000
    });

    IPage page = await browser.NewPageAsync(new BrowserNewPageOptions
    {
      Permissions = ["geolocation"],
      Geolocation = new Geolocation
      {
        Latitude = 37.7749F,
        Longitude = -122.4194F
      }
    });

    await page.GotoAsync("http://localhost:3000/");

    await page.FillAsync("id=new-todo-input", "buy milk");
    await page.ClickAsync("id=myUniqueID");

    await Expect(page.Locator("text=buy milk")).ToBeVisibleAsync();

    var milkTask = page.Locator(".MuiListItem-root").
        Filter(new() { HasText = "buy milk" }).
        GetByText("Delete");
    await milkTask.ScrollIntoViewIfNeededAsync();
    await milkTask.HighlightAsync();
    await milkTask.ClickAsync();
  }

  [TestMethod]
  public async Task SimpleTest_MockBackend()
  {
    IBrowser browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
    {
      Headless = false,
      SlowMo = 5000
    });

    IPage page = await browser.NewPageAsync(new BrowserNewPageOptions
    {
      Permissions = ["geolocation"],
      Geolocation = new Geolocation
      {
        Latitude = 37.7749F,
        Longitude = -122.4194F
      }
    });

    await page.RouteAsync("**/remoteTasks.json", async route =>
    {
      var response = await route.FetchAsync();

      var body = await response.TextAsync();
      body = body.Replace("another remote value", "buy coffee");
      body = body.Replace("remote entry", "buy a steak");

      await route.FulfillAsync(new RouteFulfillOptions
      {
        Response = response,
        Body = body,
        Headers = new Dictionary<string, string>(response.Headers)
        {
          ["Content-Type"] = "application/json",
        }
      });
    });

    await page.GotoAsync("http://localhost:3000/");
    await page.Locator("text='Load remote tasks'").ClickAsync();

    var steak = page.Locator("text=steak");
    await steak.ScrollIntoViewIfNeededAsync();
    await steak.HighlightAsync();

    await Expect(page.Locator("text=buy coffee")).ToBeVisibleAsync();
  }

  [TestMethod]
  [DeploymentItem("DDC_logo.png")]
  public async Task SimpleTest_SiteLogo()
  {
    IBrowser browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
    {
      Headless = false,
      SlowMo = 5000
    });

    IPage page = await browser.NewPageAsync(new BrowserNewPageOptions
    {
      Permissions = ["geolocation"],
      Geolocation = new Geolocation
      {
        Latitude = 37.7749F,
        Longitude = -122.4194F
      }
    });

    await page.RouteAsync("**/*.png", route => route.FulfillAsync(new()
    {
      Path = "DDC_logo.png"
    }));

    await page.RouteAsync("**/remoteTasks.json", async route =>
    {
      var response = await route.FetchAsync();

      var body = await response.TextAsync();
      body = body.Replace("another remote value", "buy coffee");
      body = body.Replace("remote entry", "buy a steak");

      await route.FulfillAsync(new RouteFulfillOptions
      {
        Response = response,
        Body = body,
        Headers = new Dictionary<string, string>(response.Headers)
        {
          ["Content-Type"] = "application/json",
        }
      });
    });

    await page.GotoAsync("http://localhost:3000/");
    await page.Locator("text='Load remote tasks'").ClickAsync();

    var steak = page.Locator("text=steak");
    await steak.ScrollIntoViewIfNeededAsync();
    await steak.HighlightAsync();

    await Expect(page.Locator("text=buy coffee")).ToBeVisibleAsync();
  }
}
