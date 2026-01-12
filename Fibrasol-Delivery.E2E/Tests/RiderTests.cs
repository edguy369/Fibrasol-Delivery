using Microsoft.Playwright.NUnit;
using Fibrasol_Delivery.E2E.Helpers;
using Fibrasol_Delivery.E2E.PageObjects;

namespace Fibrasol_Delivery.E2E.Tests;

[NonParallelizable]  // CRUD tests need sequential execution for data consistency
[TestFixture]
public class RiderTests : PageTest
{
    private string BaseUrl => PlaywrightFixture.BaseUrl;
    private RidersPage _ridersPage = null!;

    [SetUp]
    public async Task Setup()
    {
        _ridersPage = new RidersPage(Page, BaseUrl);
        await TestHelpers.LoginAsync(Page);
    }

    [Test]
    public async Task Riders_NavigateToPage()
    {
        // Act
        await _ridersPage.NavigateAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Riders_NavigateToPage), "01_RidersPage");

        // Assert
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/conductores");
    }

    [Test]
    public async Task Riders_DataTable_IsVisible()
    {
        // Arrange
        await _ridersPage.NavigateAsync();

        // Act
        await _ridersPage.WaitForTableLoadAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Riders_DataTable_IsVisible), "01_DataTableLoaded");

        // Assert
        var dataTable = Page.Locator("#ridersTable");
        await Expect(dataTable).ToBeVisibleAsync();
    }

    [Test]
    public async Task Riders_CreateNew_Success()
    {
        // Arrange
        await _ridersPage.NavigateAsync();
        await _ridersPage.WaitForTableLoadAsync();
        var newRiderName = $"Test Rider {DateTime.Now.Ticks}";

        // Act
        await _ridersPage.ClickNewRiderAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Riders_CreateNew_Success), "01_ModalOpened");

        await _ridersPage.FillNameAsync(newRiderName);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Riders_CreateNew_Success), "02_FormFilled");

        await _ridersPage.ClickSaveAsync();
        await Page.WaitForTimeoutAsync(1000);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Riders_CreateNew_Success), "03_AfterSave");

        // Assert
        var hasRider = await _ridersPage.HasRiderWithNameAsync(newRiderName);
        Assert.That(hasRider, Is.True, $"Rider '{newRiderName}' should be in the table");
    }

    [Test]
    public async Task Riders_Edit_Success()
    {
        // Arrange - First create a rider to edit
        await _ridersPage.NavigateAsync();
        await _ridersPage.WaitForTableLoadAsync();
        var originalName = $"EditMe {DateTime.Now.Ticks}";
        await _ridersPage.CreateRiderAsync(originalName);
        await Page.WaitForTimeoutAsync(1000);

        var updatedName = $"Updated Rider {DateTime.Now.Ticks}";

        // Act
        await _ridersPage.ClickEditAsync(0);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Riders_Edit_Success), "01_EditModalOpened");

        await _ridersPage.FillNameAsync(updatedName);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Riders_Edit_Success), "02_NameUpdated");

        await _ridersPage.ClickSaveAsync();
        await Page.WaitForTimeoutAsync(1000);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Riders_Edit_Success), "03_AfterSave");

        // Assert
        var hasRider = await _ridersPage.HasRiderWithNameAsync(updatedName);
        Assert.That(hasRider, Is.True, $"Updated rider '{updatedName}' should be in the table");
    }

    [Test]
    public async Task Riders_Delete_Success()
    {
        // Arrange - First create a rider to delete
        await _ridersPage.NavigateAsync();
        await _ridersPage.WaitForTableLoadAsync();
        var riderToDelete = $"DeleteMe {DateTime.Now.Ticks}";
        await _ridersPage.CreateRiderAsync(riderToDelete);
        await Page.WaitForTimeoutAsync(1000);

        var initialCount = await _ridersPage.GetRowCountAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Riders_Delete_Success), "01_BeforeDelete");

        // Act
        Page.Dialog += async (_, dialog) => await dialog.AcceptAsync();
        await _ridersPage.ClickDeleteAsync(0);
        await Page.WaitForTimeoutAsync(1000);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Riders_Delete_Success), "02_AfterDelete");

        // Assert
        var finalCount = await _ridersPage.GetRowCountAsync();
        Assert.That(finalCount, Is.LessThan(initialCount), "Row count should decrease after delete");
    }

    [Test]
    public async Task Riders_Search_FiltersTable()
    {
        // Arrange
        await _ridersPage.NavigateAsync();
        await _ridersPage.WaitForTableLoadAsync();

        var uniqueName = $"SearchRider{DateTime.Now.Ticks}";
        await _ridersPage.CreateRiderAsync(uniqueName);
        await Page.WaitForTimeoutAsync(1000);

        // Act
        await _ridersPage.SearchAsync(uniqueName);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Riders_Search_FiltersTable), "01_AfterSearch");

        // Assert
        var rowCount = await _ridersPage.GetRowCountAsync();
        Assert.That(rowCount, Is.GreaterThanOrEqualTo(1), "Should find at least one result");
    }

    [Test]
    public async Task Riders_Modal_OpensOnNewClick()
    {
        // Arrange
        await _ridersPage.NavigateAsync();
        await _ridersPage.WaitForTableLoadAsync();

        // Act
        await _ridersPage.ClickNewRiderAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Riders_Modal_OpensOnNewClick), "01_ModalVisible");

        // Assert
        var isModalVisible = await _ridersPage.IsModalVisibleAsync();
        Assert.That(isModalVisible, Is.True, "Modal should be visible after clicking New button");
    }
}
