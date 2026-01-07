using Microsoft.Playwright.NUnit;
using Fibrasol_Delivery.E2E.Helpers;
using Fibrasol_Delivery.E2E.PageObjects;

namespace Fibrasol_Delivery.E2E.Tests;

[NonParallelizable]  // Complex form tests need sequential execution
[TestFixture]
public class DeliveryOrderDetailTests : PageTest
{
    private string BaseUrl => PlaywrightFixture.BaseUrl;
    private DeliveryOrderDetailPage _detailPage = null!;

    [SetUp]
    public async Task Setup()
    {
        _detailPage = new DeliveryOrderDetailPage(Page, BaseUrl);
        await TestHelpers.LoginAsync(Page);
    }

    [Test]
    public async Task DeliveryOrderDetail_NewOrder_PageLoads()
    {
        // Act
        await _detailPage.NavigateAsync(0);
        await _detailPage.WaitForPageLoadAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderDetail_NewOrder_PageLoads), "01_NewOrderPage");

        // Assert
        var isLoaded = await _detailPage.IsPageLoadedAsync();
        Assert.That(isLoaded, Is.True, "Detail page should be loaded");
    }

    [Test]
    public async Task DeliveryOrderDetail_StatusDropdown_IsVisible()
    {
        // Arrange
        await _detailPage.NavigateAsync(0);
        await _detailPage.WaitForPageLoadAsync();

        // Assert
        var statusSelect = Page.Locator("#orderStatus");
        await Expect(statusSelect).ToBeVisibleAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderDetail_StatusDropdown_IsVisible), "01_StatusDropdown");
    }

    [Test]
    public async Task DeliveryOrderDetail_AddBackorder_IncreasesCount()
    {
        // Arrange
        await _detailPage.NavigateAsync(0);
        await _detailPage.WaitForPageLoadAsync();
        var initialCount = await _detailPage.GetBackorderCountAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderDetail_AddBackorder_IncreasesCount), "01_BeforeAdd");

        // Act
        await _detailPage.ClickAddBackorderAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderDetail_AddBackorder_IncreasesCount), "02_AfterAdd");

        // Assert
        var newCount = await _detailPage.GetBackorderCountAsync();
        Assert.That(newCount, Is.GreaterThan(initialCount), "Backorder count should increase");
    }

    [Test]
    public async Task DeliveryOrderDetail_RidersSelect_IsVisible()
    {
        // Arrange
        await _detailPage.NavigateAsync(0);
        await _detailPage.WaitForPageLoadAsync();

        // Assert
        var ridersSelect = Page.Locator("#orderRiders");
        await Expect(ridersSelect).ToBeVisibleAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderDetail_RidersSelect_IsVisible), "01_RidersSelect");
    }

    [Test]
    public async Task DeliveryOrderDetail_SaveButton_IsVisible()
    {
        // Arrange
        await _detailPage.NavigateAsync(0);
        await _detailPage.WaitForPageLoadAsync();

        // Assert
        var saveButton = Page.Locator(".btn-primary-custom:has-text('Guardar')");
        await Expect(saveButton).ToBeVisibleAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderDetail_SaveButton_IsVisible), "01_SaveButton");
    }

    [Test]
    public async Task DeliveryOrderDetail_CancelButton_NavigatesBack()
    {
        // Arrange
        await _detailPage.NavigateAsync(0);
        await _detailPage.WaitForPageLoadAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderDetail_CancelButton_NavigatesBack), "01_BeforeCancel");

        // Act
        await _detailPage.ClickCancelAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderDetail_CancelButton_NavigatesBack), "02_AfterCancel");

        // Assert
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/constancias");
    }

    [Test]
    public async Task DeliveryOrderDetail_AddInvoice_ToBackorder()
    {
        // Arrange
        await _detailPage.NavigateAsync(0);
        await _detailPage.WaitForPageLoadAsync();

        // Add a backorder first
        await _detailPage.ClickAddBackorderAsync();
        await Page.WaitForTimeoutAsync(500);
        var initialInvoiceCount = await _detailPage.GetInvoiceCountAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderDetail_AddInvoice_ToBackorder), "01_BeforeAddInvoice");

        // Act
        await _detailPage.ClickAddInvoiceAsync(0);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderDetail_AddInvoice_ToBackorder), "02_AfterAddInvoice");

        // Assert
        var newInvoiceCount = await _detailPage.GetInvoiceCountAsync();
        Assert.That(newInvoiceCount, Is.GreaterThan(initialInvoiceCount), "Invoice count should increase");
    }

    [Test]
    public async Task DeliveryOrderDetail_TotalField_IsVisible()
    {
        // Arrange
        await _detailPage.NavigateAsync(0);
        await _detailPage.WaitForPageLoadAsync();

        // Assert
        var totalInput = Page.Locator("#orderTotal");
        await Expect(totalInput).ToBeVisibleAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderDetail_TotalField_IsVisible), "01_TotalField");
    }

    [Test]
    public async Task DeliveryOrderDetail_ExistingOrder_LoadsData()
    {
        // First, get an existing order ID from the list
        var ordersPage = new DeliveryOrdersPage(Page, BaseUrl);
        await ordersPage.NavigateAsync();
        await ordersPage.WaitForTableLoadAsync();

        var rowCount = await ordersPage.GetRowCountAsync();
        if (rowCount == 0)
        {
            Assert.Ignore("No existing orders to test");
            return;
        }

        var orderId = await ordersPage.GetOrderIdAsync(0);
        if (orderId == null)
        {
            Assert.Ignore("Could not get order ID");
            return;
        }

        // Act
        await _detailPage.NavigateAsync(orderId.Value);
        await _detailPage.WaitForPageLoadAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderDetail_ExistingOrder_LoadsData), "01_ExistingOrderLoaded");

        // Assert
        var isLoaded = await _detailPage.IsPageLoadedAsync();
        Assert.That(isLoaded, Is.True, "Existing order should be loaded");
    }

    [Test]
    public async Task DeliveryOrderDetail_BackorderFields_AreVisible()
    {
        // Arrange
        await _detailPage.NavigateAsync(0);
        await _detailPage.WaitForPageLoadAsync();

        // Add a backorder
        await _detailPage.ClickAddBackorderAsync();
        await Page.WaitForTimeoutAsync(500);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderDetail_BackorderFields_AreVisible), "01_BackorderFields");

        // Assert
        var backorderCount = await _detailPage.GetBackorderCountAsync();
        Assert.That(backorderCount, Is.GreaterThan(0), "Should have at least one backorder");
    }

    [Test]
    public async Task DeliveryOrderDetail_DeleteBackorder_DecreasesCount()
    {
        // Arrange
        await _detailPage.NavigateAsync(0);
        await _detailPage.WaitForPageLoadAsync();

        // Add two backorders
        await _detailPage.ClickAddBackorderAsync();
        await Page.WaitForTimeoutAsync(300);
        await _detailPage.ClickAddBackorderAsync();
        await Page.WaitForTimeoutAsync(300);

        var initialCount = await _detailPage.GetBackorderCountAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderDetail_DeleteBackorder_DecreasesCount), "01_BeforeDelete");

        // Act
        await _detailPage.DeleteBackorderAsync(0);
        await Page.WaitForTimeoutAsync(500);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderDetail_DeleteBackorder_DecreasesCount), "02_AfterDelete");

        // Assert
        var newCount = await _detailPage.GetBackorderCountAsync();
        Assert.That(newCount, Is.LessThan(initialCount), "Backorder count should decrease");
    }

    [Test]
    public async Task DeliveryOrderDetail_InvoiceFields_AreVisible()
    {
        // Arrange
        await _detailPage.NavigateAsync(0);
        await _detailPage.WaitForPageLoadAsync();

        // Add backorder and invoice
        await _detailPage.ClickAddBackorderAsync();
        await Page.WaitForTimeoutAsync(300);
        await _detailPage.ClickAddInvoiceAsync(0);
        await Page.WaitForTimeoutAsync(300);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderDetail_InvoiceFields_AreVisible), "01_InvoiceFields");

        // Assert
        var invoiceCount = await _detailPage.GetInvoiceCountAsync();
        Assert.That(invoiceCount, Is.GreaterThan(0), "Should have at least one invoice");
    }

    [Test]
    public async Task DeliveryOrderDetail_DeleteInvoice_DecreasesCount()
    {
        // Arrange
        await _detailPage.NavigateAsync(0);
        await _detailPage.WaitForPageLoadAsync();

        // Add backorder and two invoices
        await _detailPage.ClickAddBackorderAsync();
        await Page.WaitForTimeoutAsync(300);
        await _detailPage.ClickAddInvoiceAsync(0);
        await Page.WaitForTimeoutAsync(300);
        await _detailPage.ClickAddInvoiceAsync(0);
        await Page.WaitForTimeoutAsync(300);

        var initialCount = await _detailPage.GetInvoiceCountAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderDetail_DeleteInvoice_DecreasesCount), "01_BeforeDelete");

        // Act
        await _detailPage.DeleteInvoiceAsync(0);
        await Page.WaitForTimeoutAsync(500);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderDetail_DeleteInvoice_DecreasesCount), "02_AfterDelete");

        // Assert
        var newCount = await _detailPage.GetInvoiceCountAsync();
        Assert.That(newCount, Is.LessThan(initialCount), "Invoice count should decrease");
    }

    [Test]
    public async Task DeliveryOrderDetail_FormLayout_IsCorrect()
    {
        // Arrange
        await _detailPage.NavigateAsync(0);
        await _detailPage.WaitForPageLoadAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderDetail_FormLayout_IsCorrect), "01_FullLayout");

        // Assert - Check main form elements exist
        var statusSelect = Page.Locator("#orderStatus");
        var ridersSelect = Page.Locator("#orderRiders");
        var addBackorderBtn = Page.Locator("button:has-text('Agregar Comanda')");
        var saveBtn = Page.Locator(".btn-primary-custom:has-text('Guardar')");

        await Expect(statusSelect).ToBeVisibleAsync();
        await Expect(ridersSelect).ToBeVisibleAsync();
        await Expect(saveBtn).ToBeVisibleAsync();
    }
}
