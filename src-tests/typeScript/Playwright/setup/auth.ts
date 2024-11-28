import { expect, type Browser, type Page } from "@playwright/test";
import "dotenv/config";
import { BASE_URL } from "../utils/helper";
import { authenticator } from "otplib";

export const AUTH_FILE = "playwright/.auth/user.json";

export async function authenticate(
  page: Page,
  user: string = process.env.TEST_USER_NAME!,
  password: string = process.env.TEST_USER_PASSWORD!,
  totpSecret: string = process.env.TOTP_SECRET!
) {
  // we need to wait until "networkidle" to catch the redirect to login.microsoft.com
  console.log("# Used environment: " + BASE_URL);
  await page.goto(BASE_URL, { waitUntil: "networkidle" });
  // check if new need to login or not. If login is not required, we are redirected to a login.microsoft.com page
  if (page.url() === BASE_URL) return; //no login required

  console.log("Authentication needed...");
  await page.locator("#i0116").fill(user);
  console.log("authenticatiing for: " + user);
  await page.locator("#i0116").press("Enter");
  await page.locator("#i0118").fill(password);
  await page.locator("#idSIButton9").click(); // pressing Enter on #i0118 does not work
  // case the auth was done before from too many machines
  const suspiciousActivity = await page.getByText(
    "Suspicious activity detected"
  );
  const approveWithApp = await page.getByText("Approve sign in request");
  // usual case to enter TOTP
  const enterTopCode = await page.getByRole("textbox", { name: "Enter code" });
  await expect(
    suspiciousActivity.or(enterTopCode).or(approveWithApp)
  ).toBeVisible();
  // handle the special case
  if (await suspiciousActivity.isVisible()) {
    await page.locator("#idSIButton9").click();
    await page.getByText("Use a verification code").click();
  }

  // change from approve with app to use TOTP
  if (await approveWithApp.isVisible()) {
    await page.locator("#signInAnotherWay").click();
    await page.getByText("Use a verification code").click();
  }

  // go on with usual case
  if (totpSecret === undefined)
    throw new Error("TOTP_SECRET is not set in .env file");
  const token = authenticator.generate(totpSecret);

  const otpTextBox = page.getByRole("textbox", { name: "Enter code" });
  otpTextBox.fill(token);
  await page.locator("#idSubmit_SAOTCC_Continue").click(); // verify button

  // stay singned in
  //await page.locator("#KmsiCheckboxField").check();
  await page.getByRole("button", { name: "Yes" }).click();
  // wait for the final URL to ensure that the cookies are actually set.
  await page.waitForURL(BASE_URL);
  console.log("Authentication successfull!");
  await page.context().storageState({ path: AUTH_FILE });
}
