using Microsoft.Playwright;

namespace Fibrasol_Delivery.E2E.PageObjects;

public class DeliveryOrderStatusPage
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    // Selectors
    private const string NewStatusButton = "button[data-bs-target='#deliveryStatusModal']";
    private const string DataTable = "#deliveryStatusesTable";
    private const string Modal = "#deliveryStatusModal";
    private const string NameInput = "#deliveryStatusName";
    private const string SaveButton = "#deliveryStatusModal .btn-primary-custom";
    private const string CancelButton = "#deliveryStatusModal .btn-secondary";
    private const string EditButton = ".btn-edit";
    private const string DeleteButton = ".btn-delete";
    private const string SearchInput = "#deliveryStatusesTable_filter input";
    private const string TableRows = "#deliveryStatusesTable tbody tr";

    public DeliveryOrderStatusPage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;
    }

    public async Task NavigateAsync()
    {
        await _page.GotoAsync($"{_baseUrl}/estados-entrega");
    }

    public async Task WaitForTableLoadAsync()
    {
        await _page.WaitForSelectorAsync($"{DataTable}_wrapper", new PageWaitForSelectorOptions { Timeout = 10000 });
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task ClickNewStatusAsync()
    {
        await _page.ClickAsync(NewStatusButton);
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

    public async Task CreateStatusAsync(string name)
    {
        await ClickNewStatusAsync();
        await FillNameAsync(name);
        await ClickSaveAsync();
        await _page.WaitForSelectorAsync($"{Modal}.show", new PageWaitForSelectorOptions { State = WaitForSelectorState.Hidden });
    }

    public async Task<bool> HasStatusWithNameAsync(string name)
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
}
