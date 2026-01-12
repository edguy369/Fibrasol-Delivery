using Microsoft.Playwright;

namespace Fibrasol_Delivery.E2E.PageObjects;

public class SalesReportPage
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    // Selectors
    private const string StartDateInput = "#startDate, input[name='StartDate']";
    private const string EndDateInput = "#endDate, input[name='EndDate']";
    private const string QuickPeriodSelect = "#quickPeriod, select[name='QuickPeriod']";
    private const string GenerateButton = "#generateReport, button:has-text('Generar')";
    private const string ExportButton = "#exportCsv, button:has-text('Exportar')";
    private const string RefreshButton = "#refresh, button:has-text('Actualizar')";

    // Statistics cards
    private const string TotalSalesCard = "#totalSales, .stat-card:has-text('Total')";
    private const string AverageCard = "#averageSales, .stat-card:has-text('Promedio')";
    private const string TopSellerCard = "#topSeller, .stat-card:has-text('Top')";
    private const string TotalSellersCard = "#totalSellers, .stat-card:has-text('Vendedores')";

    // Chart and table
    private const string SalesChart = "#salesChart, canvas";
    private const string DataTable = "#dataTable, .table";
    private const string TableRows = "#dataTable tbody tr, .table tbody tr";

    public SalesReportPage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;
    }

    public async Task NavigateAsync()
    {
        await _page.GotoAsync($"{_baseUrl}/vendedores/reportes");
    }

    public async Task WaitForPageLoadAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task SetStartDateAsync(string date)
    {
        await _page.FillAsync(StartDateInput, date);
    }

    public async Task SetEndDateAsync(string date)
    {
        await _page.FillAsync(EndDateInput, date);
    }

    public async Task SelectQuickPeriodAsync(string periodLabel)
    {
        await _page.SelectOptionAsync(QuickPeriodSelect, new SelectOptionValue { Label = periodLabel });
    }

    public async Task ClickGenerateReportAsync()
    {
        await _page.ClickAsync(GenerateButton);
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task ClickExportAsync()
    {
        var downloadTask = _page.WaitForDownloadAsync();
        await _page.ClickAsync(ExportButton);
        await downloadTask;
    }

    public async Task ClickRefreshAsync()
    {
        await _page.ClickAsync(RefreshButton);
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<bool> IsTotalSalesCardVisibleAsync()
    {
        var card = await _page.QuerySelectorAsync(TotalSalesCard);
        return card != null && await card.IsVisibleAsync();
    }

    public async Task<bool> IsAverageCardVisibleAsync()
    {
        var card = await _page.QuerySelectorAsync(AverageCard);
        return card != null && await card.IsVisibleAsync();
    }

    public async Task<bool> IsTopSellerCardVisibleAsync()
    {
        var card = await _page.QuerySelectorAsync(TopSellerCard);
        return card != null && await card.IsVisibleAsync();
    }

    public async Task<bool> IsTotalSellersCardVisibleAsync()
    {
        var card = await _page.QuerySelectorAsync(TotalSellersCard);
        return card != null && await card.IsVisibleAsync();
    }

    public async Task<bool> IsChartVisibleAsync()
    {
        var chart = await _page.QuerySelectorAsync(SalesChart);
        return chart != null && await chart.IsVisibleAsync();
    }

    public async Task<bool> IsTableVisibleAsync()
    {
        var table = await _page.QuerySelectorAsync(DataTable);
        return table != null && await table.IsVisibleAsync();
    }

    public async Task<int> GetTableRowCountAsync()
    {
        var rows = await _page.QuerySelectorAllAsync(TableRows);
        return rows.Count;
    }

    public async Task<string?> GetTotalSalesValueAsync()
    {
        return await _page.TextContentAsync(TotalSalesCard);
    }

    public async Task<string?> GetStartDateValueAsync()
    {
        return await _page.InputValueAsync(StartDateInput);
    }

    public async Task<string?> GetEndDateValueAsync()
    {
        return await _page.InputValueAsync(EndDateInput);
    }

    public async Task GenerateReportForPeriodAsync(string startDate, string endDate)
    {
        await SetStartDateAsync(startDate);
        await SetEndDateAsync(endDate);
        await ClickGenerateReportAsync();
    }

    public async Task GenerateReportForQuickPeriodAsync(string periodLabel)
    {
        await SelectQuickPeriodAsync(periodLabel);
        await ClickGenerateReportAsync();
    }
}
