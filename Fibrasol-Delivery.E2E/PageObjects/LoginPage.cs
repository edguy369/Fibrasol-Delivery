using Microsoft.Playwright;

namespace Fibrasol_Delivery.E2E.PageObjects;

public class LoginPage
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    // Selectors
    private const string EmailInput = "#_email";
    private const string PasswordInput = "#_password";
    private const string SubmitButton = "button._login";
    private const string PasswordToggle = "#togglePassword";
    private const string ErrorAlert = ".error-message";

    public LoginPage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;
    }

    public async Task NavigateAsync()
    {
        await _page.GotoAsync($"{_baseUrl}/login");
    }

    public async Task FillEmailAsync(string email)
    {
        await _page.FillAsync(EmailInput, email);
    }

    public async Task FillPasswordAsync(string password)
    {
        await _page.FillAsync(PasswordInput, password);
    }

    public async Task ClickSubmitAsync()
    {
        await _page.ClickAsync(SubmitButton);
    }

    public async Task TogglePasswordVisibilityAsync()
    {
        await _page.ClickAsync(PasswordToggle);
    }

    public async Task<bool> IsErrorVisibleAsync()
    {
        var error = await _page.QuerySelectorAsync(ErrorAlert);
        return error != null && await error.IsVisibleAsync();
    }

    public async Task<string?> GetErrorTextAsync()
    {
        var error = await _page.QuerySelectorAsync(ErrorAlert);
        return error != null ? await error.TextContentAsync() : null;
    }

    public async Task<string?> GetPasswordInputTypeAsync()
    {
        return await _page.GetAttributeAsync(PasswordInput, "type");
    }

    public async Task LoginAsync(string email, string password)
    {
        await FillEmailAsync(email);
        await FillPasswordAsync(password);
        await ClickSubmitAsync();
        // Wait for either redirect or error message
        await _page.WaitForTimeoutAsync(2000);
    }

    public async Task<bool> IsEmailInputVisibleAsync()
    {
        var element = await _page.QuerySelectorAsync(EmailInput);
        return element != null && await element.IsVisibleAsync();
    }

    public async Task<bool> IsPasswordInputVisibleAsync()
    {
        var element = await _page.QuerySelectorAsync(PasswordInput);
        return element != null && await element.IsVisibleAsync();
    }

    public async Task<bool> IsSubmitButtonVisibleAsync()
    {
        var element = await _page.QuerySelectorAsync(SubmitButton);
        return element != null && await element.IsVisibleAsync();
    }
}
