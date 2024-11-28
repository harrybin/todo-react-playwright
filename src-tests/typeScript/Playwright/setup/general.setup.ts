import { test as setup } from "@playwright/test";
import { authenticate } from "./auth";
setup("general setup", async ({ page, browser }) => {
  await authenticate(
    page,
    process.env.TEST_USER_NAME,
    process.env.TEST_USER_PASSWORD,
    process.env.TOTP_SECRET
  );
});
