using Microsoft.Playwright;

namespace Fibrasol_Delivery.E2E.PageObjects;

public class DashboardPage
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    // Selectors
    private const string DashboardCards = ".dashboard-card";
    private const string ClientsCounter = "#clientsCount";
    private const string RidersCounter = "#driversCount";
    private const string OrdersCounter = "#ordersCount";
    private const string InvoicesCounter = "#invoicesCount";
    private const string SalesPersonsCounter = "#salesPersonsCount";
    private const string UnsignedOrdersTable = "#unsignedOrdersTable";
    private const string ViewOrderButton = ".btn-view";
    private const string PrintOrderButton = ".btn-print";
    private const string LastUpdateTime = "#lastUpdate";
    private const string UserDropdown = ".user-avatar";
    private const string LogoutButton = "button:has-text('Cerrar Sesión')";

    public DashboardPage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;
    }

    public async Task NavigateAsync()
    {
        await _page.GotoAsync($"{_baseUrl}/");
    }

    public async Task<int> GetDashboardCardCountAsync()
    {
        var cards = await _page.QuerySelectorAllAsync(DashboardCards);
        return cards.Count;
    }

    public async Task<string?> GetClientsCountAsync()
    {
        await _page.WaitForSelectorAsync(ClientsCounter);
        return await _page.TextContentAsync(ClientsCounter);
    }

    public async Task<string?> GetRidersCountAsync()
    {
        await _page.WaitForSelectorAsync(RidersCounter);
        return await _page.TextContentAsync(RidersCounter);
    }

    public async Task<string?> GetOrdersCountAsync()
    {
        await _page.WaitForSelectorAsync(OrdersCounter);
        return await _page.TextContentAsync(OrdersCounter);
    }

    public async Task<string?> GetInvoicesCountAsync()
    {
        await _page.WaitForSelectorAsync(InvoicesCounter);
        return await _page.TextContentAsync(InvoicesCounter);
    }

    public async Task<string?> GetSalesPersonsCountAsync()
    {
        await _page.WaitForSelectorAsync(SalesPersonsCounter);
        return await _page.TextContentAsync(SalesPersonsCounter);
    }

    public async Task<bool> IsUnsignedOrdersTableVisibleAsync()
    {
        var table = await _page.QuerySelectorAsync(UnsignedOrdersTable);
        return table != null && await table.IsVisibleAsync();
    }

    public async Task<int> GetUnsignedOrdersCountAsync()
    {
        var rows = await _page.QuerySelectorAllAsync($"{UnsignedOrdersTable} tbody tr");
        return rows.Count;
    }

    public async Task ClickViewFirstOrderAsync()
    {
        await _page.ClickAsync($"{UnsignedOrdersTable} tbody tr:first-child {ViewOrderButton}");
    }

    public async Task ClickPrintFirstOrderAsync()
    {
        var printButton = await _page.QuerySelectorAsync($"{UnsignedOrdersTable} tbody tr:first-child {PrintOrderButton}");
        if (printButton != null)
        {
            await printButton.ClickAsync();
        }
    }

    public async Task LogoutAsync()
    {
        await _page.ClickAsync(UserDropdown);
        await _page.ClickAsync(LogoutButton);
    }

    public async Task<bool> IsUserDropdownVisibleAsync()
    {
        var dropdown = await _page.QuerySelectorAsync(UserDropdown);
        return dropdown != null && await dropdown.IsVisibleAsync();
    }
}
