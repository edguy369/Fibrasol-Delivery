using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Fibrasol_Delivery.E2E.Helpers;
using Fibrasol_Delivery.E2E.PageObjects;

namespace Fibrasol_Delivery.E2E.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class AuthTests : PageTest
{
    private string BaseUrl => PlaywrightFixture.BaseUrl;
    private LoginPage _loginPage = null!;

    [SetUp]
    public void Setup()
    {
        _loginPage = new LoginPage(Page, BaseUrl);
    }

    [Test]
    public async Task Login_WithValidCredentials_RedirectsToDashboard()
    {
        // Arrange
        await _loginPage.NavigateAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Login_WithValidCredentials_RedirectsToDashboard), "01_LoginPage");

        // Act
        await _loginPage.LoginAsync("root@codingtipi.com", "root");
        // Wait for AJAX login to complete and redirect
        await Page.WaitForFunctionAsync("() => !window.location.href.includes('/login')", new PageWaitForFunctionOptions { Timeout = 30000 });
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Login_WithValidCredentials_RedirectsToDashboard), "02_AfterLogin");

        // Assert
        await Expect(Page).ToHaveURLAsync(BaseUrl + "/");
    }

    [Test]
    public async Task Login_WithInvalidCredentials_ShowsErrorMessage()
    {
        // Arrange
        await _loginPage.NavigateAsync();

        // Act
        await _loginPage.LoginAsync("invalid@email.com", "wrongpassword");
        await Page.WaitForTimeoutAsync(2000);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Login_WithInvalidCredentials_ShowsErrorMessage), "01_ErrorShown");

        // Assert
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/login");
        // Check if error message element is not hidden (d-none class removed)
        var errorMessage = Page.Locator(".error-message:not(.d-none)");
        await Expect(errorMessage).ToBeVisibleAsync();
    }

    [Test]
    public async Task Login_WithEmptyFields_StaysOnLoginPage()
    {
        // Arrange
        await _loginPage.NavigateAsync();

        // Act
        await _loginPage.ClickSubmitAsync();

        // Assert - HTML5 validation should prevent submission
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/login");
    }

    [Test]
    public async Task LoginPage_HasAllRequiredElements()
    {
        // Arrange & Act
        await _loginPage.NavigateAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(LoginPage_HasAllRequiredElements), "01_AllElements");

        // Assert
        Assert.Multiple(async () =>
        {
            Assert.That(await _loginPage.IsEmailInputVisibleAsync(), Is.True, "Email input should be visible");
            Assert.That(await _loginPage.IsPasswordInputVisibleAsync(), Is.True, "Password input should be visible");
            Assert.That(await _loginPage.IsSubmitButtonVisibleAsync(), Is.True, "Submit button should be visible");
        });
    }

    [Test]
    public async Task Login_PasswordToggle_ChangesInputType()
    {
        // Arrange
        await _loginPage.NavigateAsync();
        await _loginPage.FillPasswordAsync("testpassword");

        // Assert initial state
        var initialType = await _loginPage.GetPasswordInputTypeAsync();
        Assert.That(initialType, Is.EqualTo("password"));

        // Act - Toggle password visibility
        await _loginPage.TogglePasswordVisibilityAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Login_PasswordToggle_ChangesInputType), "01_PasswordVisible");

        // Assert - Password should be visible
        var visibleType = await _loginPage.GetPasswordInputTypeAsync();
        Assert.That(visibleType, Is.EqualTo("text"));
    }

    [Test]
    public async Task Logout_AfterLogin_RedirectsToLoginPage()
    {
        // Arrange - Login first
        await TestHelpers.LoginAsync(Page);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Logout_AfterLogin_RedirectsToLoginPage), "01_LoggedIn");

        // Act - Logout
        await TestHelpers.LogoutAsync(Page);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Logout_AfterLogin_RedirectsToLoginPage), "02_AfterLogout");

        // Assert
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/login");
    }
}
