using Microsoft.Playwright.NUnit;
using Fibrasol_Delivery.E2E.Helpers;
using Fibrasol_Delivery.E2E.PageObjects;

namespace Fibrasol_Delivery.E2E.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class SalesPersonTests : PageTest
{
    private string BaseUrl => PlaywrightFixture.BaseUrl;
    private SalesPersonsPage _salesPersonsPage = null!;

    [SetUp]
    public async Task Setup()
    {
        _salesPersonsPage = new SalesPersonsPage(Page, BaseUrl);
        await TestHelpers.LoginAsync(Page);
    }

    [Test]
    public async Task SalesPersons_NavigateToPage()
    {
        // Act
        await _salesPersonsPage.NavigateAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(SalesPersons_NavigateToPage), "01_SalesPersonsPage");

        // Assert
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/vendedores");
    }

    [Test]
    public async Task SalesPersons_DataTable_IsVisible()
    {
        // Arrange
        await _salesPersonsPage.NavigateAsync();

        // Act
        await _salesPersonsPage.WaitForTableLoadAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(SalesPersons_DataTable_IsVisible), "01_DataTableLoaded");

        // Assert
        var dataTable = Page.Locator("#salesPersonsTable");
        await Expect(dataTable).ToBeVisibleAsync();
    }

    [Test]
    public async Task SalesPersons_CreateNew_Success()
    {
        // Arrange
        await _salesPersonsPage.NavigateAsync();
        await _salesPersonsPage.WaitForTableLoadAsync();
        var newSalesPersonName = $"Test Vendedor {DateTime.Now.Ticks}";

        // Act
        await _salesPersonsPage.ClickNewSalesPersonAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(SalesPersons_CreateNew_Success), "01_ModalOpened");

        await _salesPersonsPage.FillNameAsync(newSalesPersonName);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(SalesPersons_CreateNew_Success), "02_FormFilled");

        await _salesPersonsPage.ClickSaveAsync();
        await Page.WaitForTimeoutAsync(1000);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(SalesPersons_CreateNew_Success), "03_AfterSave");

        // Assert
        var hasSalesPerson = await _salesPersonsPage.HasSalesPersonWithNameAsync(newSalesPersonName);
        Assert.That(hasSalesPerson, Is.True, $"Sales person '{newSalesPersonName}' should be in the table");
    }

    [Test]
    public async Task SalesPersons_Edit_Success()
    {
        // Arrange - First create a sales person to edit
        await _salesPersonsPage.NavigateAsync();
        await _salesPersonsPage.WaitForTableLoadAsync();
        var originalName = $"EditMe {DateTime.Now.Ticks}";
        await _salesPersonsPage.CreateSalesPersonAsync(originalName);
        await Page.WaitForTimeoutAsync(1000);

        var updatedName = $"Updated Vendedor {DateTime.Now.Ticks}";

        // Act
        await _salesPersonsPage.ClickEditAsync(0);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(SalesPersons_Edit_Success), "01_EditModalOpened");

        await _salesPersonsPage.FillNameAsync(updatedName);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(SalesPersons_Edit_Success), "02_NameUpdated");

        await _salesPersonsPage.ClickSaveAsync();
        await Page.WaitForTimeoutAsync(1000);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(SalesPersons_Edit_Success), "03_AfterSave");

        // Assert
        var hasSalesPerson = await _salesPersonsPage.HasSalesPersonWithNameAsync(updatedName);
        Assert.That(hasSalesPerson, Is.True, $"Updated sales person '{updatedName}' should be in the table");
    }

    [Test]
    public async Task SalesPersons_Delete_Success()
    {
        // Arrange - First create a sales person to delete
        await _salesPersonsPage.NavigateAsync();
        await _salesPersonsPage.WaitForTableLoadAsync();
        var salesPersonToDelete = $"DeleteMe {DateTime.Now.Ticks}";
        await _salesPersonsPage.CreateSalesPersonAsync(salesPersonToDelete);
        await Page.WaitForTimeoutAsync(1000);

        var initialCount = await _salesPersonsPage.GetRowCountAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(SalesPersons_Delete_Success), "01_BeforeDelete");

        // Act
        Page.Dialog += async (_, dialog) => await dialog.AcceptAsync();
        await _salesPersonsPage.ClickDeleteAsync(0);
        await Page.WaitForTimeoutAsync(1000);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(SalesPersons_Delete_Success), "02_AfterDelete");

        // Assert
        var finalCount = await _salesPersonsPage.GetRowCountAsync();
        Assert.That(finalCount, Is.LessThan(initialCount), "Row count should decrease after delete");
    }

    [Test]
    public async Task SalesPersons_Search_FiltersTable()
    {
        // Arrange
        await _salesPersonsPage.NavigateAsync();
        await _salesPersonsPage.WaitForTableLoadAsync();

        var uniqueName = $"SearchVendedor{DateTime.Now.Ticks}";
        await _salesPersonsPage.CreateSalesPersonAsync(uniqueName);
        await Page.WaitForTimeoutAsync(1000);

        // Act
        await _salesPersonsPage.SearchAsync(uniqueName);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(SalesPersons_Search_FiltersTable), "01_AfterSearch");

        // Assert
        var rowCount = await _salesPersonsPage.GetRowCountAsync();
        Assert.That(rowCount, Is.GreaterThanOrEqualTo(1), "Should find at least one result");
    }

    [Test]
    public async Task SalesPersons_Modal_OpensOnNewClick()
    {
        // Arrange
        await _salesPersonsPage.NavigateAsync();
        await _salesPersonsPage.WaitForTableLoadAsync();

        // Act
        await _salesPersonsPage.ClickNewSalesPersonAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(SalesPersons_Modal_OpensOnNewClick), "01_ModalVisible");

        // Assert
        var isModalVisible = await _salesPersonsPage.IsModalVisibleAsync();
        Assert.That(isModalVisible, Is.True, "Modal should be visible after clicking New button");
    }
}
