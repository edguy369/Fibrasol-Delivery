using Microsoft.Playwright;

namespace Fibrasol_Delivery.E2E.Helpers;

public static class TestHelpers
{
    public static string BaseUrl => PlaywrightFixture.BaseUrl;

    public static async Task LoginAsync(IPage page, string email = "root@codingtipi.com", string password = "root")
    {
        await page.GotoAsync($"{BaseUrl}/login");
        await page.FillAsync("#_email", email);
        await page.FillAsync("#_password", password);
        await page.ClickAsync("button._login");
        // Wait for AJAX login to complete and redirect
        await page.WaitForFunctionAsync("() => !window.location.href.includes('/login')", new PageWaitForFunctionOptions { Timeout = 30000 });
    }

    public static async Task LogoutAsync(IPage page)
    {
        await page.ClickAsync(".user-avatar");
        await page.WaitForSelectorAsync(".dropdown-menu.show");
        await page.ClickAsync("button:has-text('Cerrar Sesión')");
        await page.WaitForURLAsync($"{BaseUrl}/login");
    }

    public static async Task WaitForDataTableAsync(IPage page, string tableSelector = "#dataTable")
    {
        await page.WaitForSelectorAsync($"{tableSelector}_wrapper");
        await page.WaitForFunctionAsync($"() => document.querySelector('{tableSelector} tbody tr') !== null");
    }

    public static async Task WaitForModalAsync(IPage page, string modalSelector = "#modal")
    {
        await page.WaitForSelectorAsync($"{modalSelector}.show");
    }

    public static async Task CloseModalAsync(IPage page, string modalSelector = "#modal")
    {
        await page.ClickAsync($"{modalSelector} .btn-close, {modalSelector} [data-bs-dismiss='modal']");
        await page.WaitForSelectorAsync($"{modalSelector}.show", new PageWaitForSelectorOptions { State = WaitForSelectorState.Hidden });
    }

    public static void SetupConfirmDelete(IPage page)
    {
        page.Dialog += async (_, dialog) =>
        {
            await dialog.AcceptAsync();
        };
    }

    public static void SetupCancelDelete(IPage page)
    {
        page.Dialog += async (_, dialog) =>
        {
            await dialog.DismissAsync();
        };
    }

    public static async Task WaitForToastAsync(IPage page, string? containsText = null)
    {
        if (containsText != null)
        {
            await page.WaitForSelectorAsync($".toast:has-text('{containsText}')");
        }
        else
        {
            await page.WaitForSelectorAsync(".toast.show");
        }
    }

    public static async Task NavigateToAsync(IPage page, string menuText)
    {
        await page.ClickAsync($".navbar-nav >> text={menuText}");
    }

    public static async Task<int> GetTableRowCountAsync(IPage page, string tableSelector = "#dataTable")
    {
        var rows = await page.QuerySelectorAllAsync($"{tableSelector} tbody tr");
        return rows.Count;
    }
}
