using Microsoft.Playwright.NUnit;
using Fibrasol_Delivery.E2E.Helpers;
using Fibrasol_Delivery.E2E.PageObjects;

namespace Fibrasol_Delivery.E2E.Tests;

[NonParallelizable]  // CRUD tests need sequential execution for data consistency
[TestFixture]
public class DeliveryOrderStatusTests : PageTest
{
    private string BaseUrl => PlaywrightFixture.BaseUrl;
    private DeliveryOrderStatusPage _statusPage = null!;

    [SetUp]
    public async Task Setup()
    {
        _statusPage = new DeliveryOrderStatusPage(Page, BaseUrl);
        await TestHelpers.LoginAsync(Page);
    }

    [Test]
    public async Task DeliveryOrderStatus_NavigateToPage()
    {
        // Act
        await _statusPage.NavigateAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderStatus_NavigateToPage), "01_StatusPage");

        // Assert
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(@"/estados"));
    }

    [Test]
    public async Task DeliveryOrderStatus_DataTable_IsVisible()
    {
        // Arrange
        await _statusPage.NavigateAsync();

        // Act
        await _statusPage.WaitForTableLoadAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderStatus_DataTable_IsVisible), "01_DataTableLoaded");

        // Assert
        var dataTable = Page.Locator("#deliveryStatusesTable");
        await Expect(dataTable).ToBeVisibleAsync();
    }

    [Test]
    public async Task DeliveryOrderStatus_CreateNew_Success()
    {
        // Arrange
        await _statusPage.NavigateAsync();
        await _statusPage.WaitForTableLoadAsync();
        var newStatusName = $"Test Status {DateTime.Now.Ticks}";

        // Act
        await _statusPage.ClickNewStatusAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderStatus_CreateNew_Success), "01_ModalOpened");

        await _statusPage.FillNameAsync(newStatusName);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderStatus_CreateNew_Success), "02_FormFilled");

        await _statusPage.ClickSaveAsync();
        await Page.WaitForTimeoutAsync(1000);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderStatus_CreateNew_Success), "03_AfterSave");

        // Assert
        var hasStatus = await _statusPage.HasStatusWithNameAsync(newStatusName);
        Assert.That(hasStatus, Is.True, $"Status '{newStatusName}' should be in the table");
    }

    [Test]
    public async Task DeliveryOrderStatus_Edit_Success()
    {
        // Arrange - First create a status to edit
        await _statusPage.NavigateAsync();
        await _statusPage.WaitForTableLoadAsync();
        var originalName = $"EditMe {DateTime.Now.Ticks}";
        await _statusPage.CreateStatusAsync(originalName);
        await Page.WaitForTimeoutAsync(1000);

        var updatedName = $"Updated Status {DateTime.Now.Ticks}";

        // Act
        await _statusPage.ClickEditAsync(0);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderStatus_Edit_Success), "01_EditModalOpened");

        await _statusPage.FillNameAsync(updatedName);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderStatus_Edit_Success), "02_NameUpdated");

        await _statusPage.ClickSaveAsync();
        await Page.WaitForTimeoutAsync(1000);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderStatus_Edit_Success), "03_AfterSave");

        // Assert
        var hasStatus = await _statusPage.HasStatusWithNameAsync(updatedName);
        Assert.That(hasStatus, Is.True, $"Updated status '{updatedName}' should be in the table");
    }

    [Test]
    public async Task DeliveryOrderStatus_Delete_Success()
    {
        // Arrange - First create a status to delete
        await _statusPage.NavigateAsync();
        await _statusPage.WaitForTableLoadAsync();
        var statusToDelete = $"DeleteMe {DateTime.Now.Ticks}";
        await _statusPage.CreateStatusAsync(statusToDelete);
        await Page.WaitForTimeoutAsync(1000);

        var initialCount = await _statusPage.GetRowCountAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderStatus_Delete_Success), "01_BeforeDelete");

        // Act
        Page.Dialog += async (_, dialog) => await dialog.AcceptAsync();
        await _statusPage.ClickDeleteAsync(0);
        await Page.WaitForTimeoutAsync(1000);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderStatus_Delete_Success), "02_AfterDelete");

        // Assert
        var finalCount = await _statusPage.GetRowCountAsync();
        Assert.That(finalCount, Is.LessThan(initialCount), "Row count should decrease after delete");
    }

    [Test]
    public async Task DeliveryOrderStatus_Search_FiltersTable()
    {
        // Arrange
        await _statusPage.NavigateAsync();
        await _statusPage.WaitForTableLoadAsync();

        var uniqueName = $"SearchStatus{DateTime.Now.Ticks}";
        await _statusPage.CreateStatusAsync(uniqueName);
        await Page.WaitForTimeoutAsync(1000);

        // Act
        await _statusPage.SearchAsync(uniqueName);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderStatus_Search_FiltersTable), "01_AfterSearch");

        // Assert
        var rowCount = await _statusPage.GetRowCountAsync();
        Assert.That(rowCount, Is.GreaterThanOrEqualTo(1), "Should find at least one result");
    }

    [Test]
    public async Task DeliveryOrderStatus_Modal_OpensOnNewClick()
    {
        // Arrange
        await _statusPage.NavigateAsync();
        await _statusPage.WaitForTableLoadAsync();

        // Act
        await _statusPage.ClickNewStatusAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(DeliveryOrderStatus_Modal_OpensOnNewClick), "01_ModalVisible");

        // Assert
        var isModalVisible = await _statusPage.IsModalVisibleAsync();
        Assert.That(isModalVisible, Is.True, "Modal should be visible after clicking New button");
    }
}
