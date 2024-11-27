import { chromium, Browser, Page, test, expect } from "@playwright/test";

test.describe("TodoMatic Tests", () => {
  let browser: Browser;
  let page: Page;

  test.beforeAll(async () => {
    browser = await chromium.launch({ headless: false, slowMo: 5000 });
  });

  test.afterAll(async () => {
    await browser.close();
  });

  test("SimpleTest_AddElement", async () => {
    page = await browser.newPage({
      permissions: ["geolocation"],
      geolocation: { latitude: 37.7749, longitude: -122.4194 },
    });

    await page.goto("http://localhost:3000/todo-react/");
    await page.fill("#new-todo-input", "buy milk");
    await page.click("#myUniqueID");
    await expect(page.locator("text=buy milk")).toBeVisible();
  });

  test("SimpleTest_DeleteElement", async () => {
    page = await browser.newPage({
      permissions: ["geolocation"],
      geolocation: { latitude: 37.7749, longitude: -122.4194 },
    });

    await page.goto("http://localhost:3000/todo-react/");
    await page.fill("#new-todo-input", "buy milk");
    await page.click("#myUniqueID");
    await expect(page.locator("text=buy milk")).toBeVisible();

    const milkTask = page
      .locator(".MuiListItem-root")
      .filter({ hasText: "buy milk" })
      .getByText("Delete");
    await milkTask.scrollIntoViewIfNeeded();
    await milkTask.highlight();
    await milkTask.click();
  });

  test("SimpleTest_MockBackend", async () => {
    page = await browser.newPage({
      permissions: ["geolocation"],
      geolocation: { latitude: 37.7749, longitude: -122.4194 },
    });

    await page.route("**/remoteTasks.json", async (route) => {
      const response = await route.fetch();
      let body = await response.text();
      body = body.replace("another remote value", "buy coffee");
      body = body.replace("remote entry", "buy a steak");

      await route.fulfill({
        response,
        body,
        headers: {
          ...response.headers(),
          "Content-Type": "application/json",
        },
      });
    });

    await page.goto("http://localhost:3000/todo-react/");
    await page.locator("text=Load remote tasks").click();
    const steak = page.locator("text=steak");
    await steak.scrollIntoViewIfNeeded();
    await steak.highlight();
    await expect(page.locator("text=buy coffee")).toBeVisible();
  });

  test("SimpleTest_SiteLogo", async () => {
    page = await browser.newPage({
      permissions: ["geolocation"],
      geolocation: { latitude: 37.7749, longitude: -122.4194 },
    });

    await page.route("**/*.png", (route) =>
      route.fulfill({ path: "src-tests/typeScript/Playwright/DDC_logo.png" })
    );

    await page.route("**/remoteTasks.json", async (route) => {
      const response = await route.fetch();
      let body = await response.text();
      body = body.replace("another remote value", "buy coffee");
      body = body.replace("remote entry", "buy a steak");

      await route.fulfill({
        response,
        body,
        headers: {
          ...response.headers(),
          "Content-Type": "application/json",
        },
      });
    });

    await page.goto("http://localhost:3000/todo-react/");
    await page.locator("text=Load remote tasks").click();
    const steak = page.locator("text=steak");
    await steak.scrollIntoViewIfNeeded();
    await steak.highlight();
    await expect(page.locator("text=buy coffee")).toBeVisible();
  });
});
