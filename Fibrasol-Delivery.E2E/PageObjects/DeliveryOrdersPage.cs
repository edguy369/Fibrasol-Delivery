using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace Fibrasol_Delivery.E2E.PageObjects;

public class DeliveryOrdersPage
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    // Selectors
    private const string NewOrderButton = "a[href='/constancias/0']";
    private const string DataTable = "#deliveryOrdersTable";
    private const string TableRows = "#deliveryOrdersTable tbody tr";
    private const string ViewButton = ".btn-view";
    private const string PrintButton = ".btn-print";
    private const string DeleteButton = ".btn-delete";
    private const string SearchInput = "#deliveryOrdersTable_filter input";
    private const string StatusBadge = ".status-badge";

    public DeliveryOrdersPage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;
    }

    public async Task NavigateAsync()
    {
        await _page.GotoAsync($"{_baseUrl}/constancias");
    }

    public async Task WaitForTableLoadAsync()
    {
        await _page.WaitForSelectorAsync($"{DataTable}_wrapper", new PageWaitForSelectorOptions { Timeout = 10000 });
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task ClickNewOrderAsync()
    {
        await _page.ClickAsync(NewOrderButton);
        await _page.WaitForURLAsync(new Regex(@"/constancias/0"));
    }

    public async Task<int> GetRowCountAsync()
    {
        var rows = await _page.QuerySelectorAllAsync(TableRows);
        // DataTables shows 1 row with "No data" message when empty
        if (rows.Count == 1)
        {
            var firstRow = rows[0];
            var emptyCell = await firstRow.QuerySelectorAsync(".dataTables_empty");
            if (emptyCell != null) return 0;
        }
        return rows.Count;
    }

    public async Task ClickViewAsync(int rowIndex = 0)
    {
        var rows = await _page.QuerySelectorAllAsync(TableRows);
        if (rows.Count > rowIndex)
        {
            var viewBtn = await rows[rowIndex].QuerySelectorAsync(ViewButton);
            if (viewBtn != null)
            {
                await viewBtn.ClickAsync();
            }
        }
    }

    public async Task<IPage?> ClickPrintAsync(int rowIndex = 0)
    {
        var rows = await _page.QuerySelectorAllAsync(TableRows);
        if (rows.Count > rowIndex)
        {
            var printBtn = await rows[rowIndex].QuerySelectorAsync(PrintButton);
            if (printBtn != null)
            {
                var popupTask = _page.Context.WaitForPageAsync();
                await printBtn.ClickAsync();
                return await popupTask;
            }
        }
        return null;
    }

    public async Task ClickDeleteAsync(int rowIndex = 0)
    {
        var rows = await _page.QuerySelectorAllAsync(TableRows);
        if (rows.Count > rowIndex)
        {
            var deleteBtn = await rows[rowIndex].QuerySelectorAsync(DeleteButton);
            if (deleteBtn != null)
            {
                await deleteBtn.ClickAsync();
            }
        }
    }

    public async Task SearchAsync(string searchText)
    {
        await _page.FillAsync(SearchInput, searchText);
        await _page.WaitForTimeoutAsync(500);
    }

    public async Task<string?> GetOrderStatusAsync(int rowIndex = 0)
    {
        var rows = await _page.QuerySelectorAllAsync(TableRows);
        if (rows.Count > rowIndex)
        {
            var badge = await rows[rowIndex].QuerySelectorAsync(StatusBadge);
            if (badge != null)
            {
                return await badge.TextContentAsync();
            }
        }
        return null;
    }

    public async Task<string?> GetOrderTotalAsync(int rowIndex = 0)
    {
        var rows = await _page.QuerySelectorAllAsync(TableRows);
        if (rows.Count > rowIndex)
        {
            var cells = await rows[rowIndex].QuerySelectorAllAsync("td");
            if (cells.Count >= 4)
            {
                return await cells[3].TextContentAsync();
            }
        }
        return null;
    }

    public async Task<int?> GetOrderIdAsync(int rowIndex = 0)
    {
        var rows = await _page.QuerySelectorAllAsync(TableRows);
        if (rows.Count > rowIndex)
        {
            var cells = await rows[rowIndex].QuerySelectorAllAsync("td");
            if (cells.Count > 0)
            {
                var idText = await cells[0].TextContentAsync();
                if (int.TryParse(idText, out var id))
                {
                    return id;
                }
            }
        }
        return null;
    }
}
