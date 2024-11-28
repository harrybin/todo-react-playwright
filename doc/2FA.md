## How a test-user with MFA is created/used

For running the test with your or the test user you ned to to the following steps:

1. set your system or at least your browser to use english language
2. ensure your MFA can be done by using an Authentcator with code (TOTP):

   - go to: https://mysignins.microsoft.com/security-info
   - create a `new sign-in method` --> `Authenticator app`
     - choose `I want to use a different authenticator app`
     - on "_Set up your acount_" simply hit `Next`
     - click on `Can't scan image?` to get the **Secret Key** shown
     - copy that secret key to your `.env` file as `TOTP_SECRET`
       ![MFA-Auth-Creation.gif](./readme-assests/MFA-Auth-Creation.gif)
     - finally scan that QR-Code using your Authenticator-App and active it
     - ensure that this authenticator is set as `default` if possible ot the only auth method for the user (alternatively, in case you like to run the test only once a while, you can uncomment `headless: false` around line 35 in `playwright.config.ts`, run a test, do manual 2nd-Factor-Auth and comment out the line again)

   ▶️ if this is done correctly and at least one test run, there will be a file `./playwright/.auth/user.json` containing auth cookie information

## Issues / Remarks found

During test creation the following impediments were found:

- **ID**s: **don't use element IDs for identifying**. IDs (like _PAPrefix0ee7fe8d392a5afield-429\_\_control_) seem to be stabel for some time, but for sure they change upon version changes in the app or even depend on the order the controls are created. A working way, is to identify the data-control by `data-control-name` and then within locate the HTML **input/button/select** etc. element.
- **options** showing up in the style of an **radio button**, like the "Shareholding Entities" or the list of Generators don't have a selection state visible in the HTML DOM

![radio button style](./readme-assests/radioButton.png)

> The visual selection is done by using a SVG image and the content of that image is changed by system. (Even the data-Url of the image does not change). So there is no chance to determine which element in such a list is seleted.

- **comboboxes** are a custom fluentUI implemention. Their popup (popover) is a global DIV with `role=listbox` and further DIVs as options inside. The assoziation of the listbox to the input with `role=combobox` it's used for is done by the ID of the listbox DIV, which set as `aria-controls` ID on the input. This ID for sure changes depending on the order different combobox are triggered.
