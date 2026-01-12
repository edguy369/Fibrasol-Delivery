using Microsoft.Playwright;

namespace Fibrasol_Delivery.E2E.PageObjects;

public class SalesPersonsPage
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    // Selectors
    private const string NewSalesPersonButton = "button[data-bs-target='#salesPersonModal']";
    private const string DataTable = "#salesPersonsTable";
    private const string Modal = "#salesPersonModal";
    private const string NameInput = "#salesPersonName";
    private const string SaveButton = "#salesPersonModal .btn-primary-custom";
    private const string CancelButton = "#salesPersonModal .btn-secondary";
    private const string EditButton = ".btn-edit";
    private const string DeleteButton = ".btn-delete";
    private const string SearchInput = "#salesPersonsTable_filter input";
    private const string TableRows = "#salesPersonsTable tbody tr";
    private const string ReportLink = "a[href*='reportes']";

    public SalesPersonsPage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;
    }

    public async Task NavigateAsync()
    {
        await _page.GotoAsync($"{_baseUrl}/vendedores");
    }

    public async Task WaitForTableLoadAsync()
    {
        await _page.WaitForSelectorAsync($"{DataTable}_wrapper", new PageWaitForSelectorOptions { Timeout = 10000 });
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task ClickNewSalesPersonAsync()
    {
        await _page.ClickAsync(NewSalesPersonButton);
        await _page.WaitForSelectorAsync($"{Modal}.show");
    }

    public async Task FillNameAsync(string name)
    {
        await _page.FillAsync(NameInput, name);
    }

    public async Task ClickSaveAsync()
    {
        await _page.ClickAsync(SaveButton);
    }

    public async Task ClickCancelAsync()
    {
        await _page.ClickAsync(CancelButton);
    }

    public async Task<int> GetRowCountAsync()
    {
        var rows = await _page.QuerySelectorAllAsync(TableRows);
        return rows.Count;
    }

    public async Task ClickEditAsync(int rowIndex = 0)
    {
        var rows = await _page.QuerySelectorAllAsync(TableRows);
        if (rows.Count > rowIndex)
        {
            var editBtn = await rows[rowIndex].QuerySelectorAsync(EditButton);
            if (editBtn != null)
            {
                await editBtn.ClickAsync();
                await _page.WaitForSelectorAsync($"{Modal}.show");
            }
        }
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

    public async Task<bool> IsModalVisibleAsync()
    {
        var modal = await _page.QuerySelectorAsync($"{Modal}.show");
        return modal != null;
    }

    public async Task CreateSalesPersonAsync(string name)
    {
        await ClickNewSalesPersonAsync();
        await FillNameAsync(name);
        await ClickSaveAsync();
        await _page.WaitForSelectorAsync($"{Modal}.show", new PageWaitForSelectorOptions { State = WaitForSelectorState.Hidden });
    }

    public async Task<bool> HasSalesPersonWithNameAsync(string name)
    {
        var cells = await _page.QuerySelectorAllAsync($"{DataTable} tbody td");
        foreach (var cell in cells)
        {
            var text = await cell.TextContentAsync();
            if (text?.Contains(name) == true)
            {
                return true;
            }
        }
        return false;
    }

    public async Task ClickReportLinkAsync()
    {
        await _page.ClickAsync(ReportLink);
    }
}
