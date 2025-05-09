import { expect, type Frame, type Locator, type Page } from "@playwright/test";

export const BASE_URL = `https://outlook.office365.com/host/${process.env.APP_ID}/bookings`;

/// Get the frame (iFrame) of the power app
export async function getFrame(page: Page) {
  if (page.url() !== BASE_URL) await page.goto(BASE_URL);
  // wait for iframe container to appear in DOM
  await page.waitForSelector("#fullscreen-app-host");
  // initialise iframe
  const frame: Frame | null = await page.frame("fullscreen-app-host");
  // wait for iframe to complete loading power app
  await frame?.waitForLoadState();
  if (frame === null) throw new Error("Frame not found");
  return frame;
}

export async function getDropdownOptions(frame: Frame, dropdownId: string) {
  return await frame.$$eval(
    `${dropdownId} > option`,
    (els: HTMLOptionElement[]) => {
      let result = {};
      els.map((option) => (result[option.textContent!] = option.value));
      return result;
    }
  );
}

export async function getComboboxOptions(
  frame: Frame,
  locator: Locator,
  role: "option" | "menuitemcheckbox" = "option"
) {
  const count = await locator.getByRole(role).count();
  return await locator
    .getByRole(role)
    .evaluateAll((options: HTMLDivElement[]) => {
      let result = {};
      options.map(
        (option, index) =>
          (result[index] = { id: option.id, value: option.textContent })
      );
      return result;
    });
  //   `${dropdownId} > [role=option]`,
  //   (els: HTMLDivElement[]) => {
  //     let result = {};
  //     els.map((option, index) => (result[index] = option.id));
  //     return result;
  //   }
}

export const dateToString = (date: Date) =>
  date.toLocaleDateString("en-us", {
    weekday: "short",
    year: "numeric",
    month: "short",
    day: "numeric",
  }); // e,g, "Fri, Jul 2, 2021"
