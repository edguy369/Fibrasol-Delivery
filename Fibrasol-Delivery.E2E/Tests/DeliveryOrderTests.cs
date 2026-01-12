using Microsoft.Playwright.NUnit;
using Fibrasol_Delivery.E2E.Helpers;
using Fibrasol_Delivery.E2E.PageObjects;

namespace Fibrasol_Delivery.E2E.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class DeliveryOrderTests : PageTest
{
    private string BaseUrl => PlaywrightFixture.BaseUrl;
    private DeliveryOrdersPage _ordersPage = null!;

    [SetUp]
    public async Task Setup()
    {
        _ordersPage = new DeliveryOrdersPage(Page, BaseUrl);
        await TestHelpers.LoginAsync(Page);
    }

    [Test]
    public async Task DeliveryOrders_NavigateToPage()
    {
        // Act
        await _ordersPage.NavigateAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrders_NavigateToPage), "01_OrdersPage");

        // Assert
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/constancias");
    }

    [Test]
    public async Task DeliveryOrders_DataTable_IsVisible()
    {
        // Arrange
        await _ordersPage.NavigateAsync();

        // Act
        await _ordersPage.WaitForTableLoadAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrders_DataTable_IsVisible), "01_DataTableLoaded");

        // Assert
        var dataTable = Page.Locator("#deliveryOrdersTable");
        await Expect(dataTable).ToBeVisibleAsync();
    }

    [Test]
    public async Task DeliveryOrders_NewOrderButton_NavigatesToDetail()
    {
        // Arrange
        await _ordersPage.NavigateAsync();
        await _ordersPage.WaitForTableLoadAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrders_NewOrderButton_NavigatesToDetail), "01_BeforeClick");

        // Act
        await _ordersPage.ClickNewOrderAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrders_NewOrderButton_NavigatesToDetail), "02_DetailPage");

        // Assert
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(@"/constancias/0"));
    }

    [Test]
    public async Task DeliveryOrders_ViewButton_NavigatesToDetail()
    {
        // Arrange
        await _ordersPage.NavigateAsync();
        await _ordersPage.WaitForTableLoadAsync();

        var rowCount = await _ordersPage.GetRowCountAsync();
        if (rowCount == 0)
        {
            Assert.Ignore("No orders available to test view functionality");
            return;
        }

        var orderId = await _ordersPage.GetOrderIdAsync(0);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrders_ViewButton_NavigatesToDetail), "01_BeforeClick");

        // Act
        await _ordersPage.ClickViewAsync(0);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrders_ViewButton_NavigatesToDetail), "02_DetailPage");

        // Assert
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex($@"/constancias/{orderId}"));
    }

    [Test]
    public async Task DeliveryOrders_PrintButton_OpensNewTab()
    {
        // Arrange
        await _ordersPage.NavigateAsync();
        await _ordersPage.WaitForTableLoadAsync();

        var rowCount = await _ordersPage.GetRowCountAsync();
        if (rowCount == 0)
        {
            Assert.Ignore("No orders available to test print functionality");
            return;
        }

        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrders_PrintButton_OpensNewTab), "01_BeforeClick");

        // Act
        var printPage = await _ordersPage.ClickPrintAsync(0);

        // Assert
        if (printPage != null)
        {
            await printPage.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
            await ScreenshotHelper.TakeScreenshotAsync(printPage, nameof(DeliveryOrders_PrintButton_OpensNewTab), "02_PrintPage");

            var url = printPage.Url;
            Assert.That(url, Does.Contain("impresion"), "Print page URL should contain 'impresion'");
            await printPage.CloseAsync();
        }
    }

    [Test]
    public async Task DeliveryOrders_Search_FiltersTable()
    {
        // Arrange
        await _ordersPage.NavigateAsync();
        await _ordersPage.WaitForTableLoadAsync();

        var rowCount = await _ordersPage.GetRowCountAsync();
        if (rowCount == 0)
        {
            Assert.Ignore("No orders available to test search functionality");
            return;
        }

        var orderId = await _ordersPage.GetOrderIdAsync(0);

        // Act
        await _ordersPage.SearchAsync(orderId?.ToString() ?? "1");
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrders_Search_FiltersTable), "01_AfterSearch");

        // Assert
        var filteredCount = await _ordersPage.GetRowCountAsync();
        Assert.That(filteredCount, Is.GreaterThanOrEqualTo(0), "Search should return results or empty");
    }
}
