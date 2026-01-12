using Microsoft.Playwright.NUnit;
using Fibrasol_Delivery.E2E.Helpers;
using Fibrasol_Delivery.E2E.PageObjects;

namespace Fibrasol_Delivery.E2E.Tests;

[NonParallelizable]  // CRUD tests need sequential execution for data consistency
[TestFixture]
public class ClientTests : PageTest
{
    private string BaseUrl => PlaywrightFixture.BaseUrl;
    private ClientsPage _clientsPage = null!;

    [SetUp]
    public async Task Setup()
    {
        _clientsPage = new ClientsPage(Page, BaseUrl);
        await TestHelpers.LoginAsync(Page);
    }

    [Test]
    public async Task Clients_NavigateToPage()
    {
        // Act
        await _clientsPage.NavigateAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Clients_NavigateToPage), "01_ClientsPage");

        // Assert
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/clientes");
    }

    [Test]
    public async Task Clients_DataTable_IsVisible()
    {
        // Arrange
        await _clientsPage.NavigateAsync();

        // Act
        await _clientsPage.WaitForTableLoadAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Clients_DataTable_IsVisible), "01_DataTableLoaded");

        // Assert
        var dataTable = Page.Locator("#clientsTable");
        await Expect(dataTable).ToBeVisibleAsync();
    }

    [Test]
    public async Task Clients_CreateNew_Success()
    {
        // Arrange
        await _clientsPage.NavigateAsync();
        await _clientsPage.WaitForTableLoadAsync();
        var initialCount = await _clientsPage.GetRowCountAsync();
        var newClientName = $"Test Client {DateTime.Now.Ticks}";

        // Act
        await _clientsPage.ClickNewClientAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Clients_CreateNew_Success), "01_ModalOpened");

        await _clientsPage.FillNameAsync(newClientName);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Clients_CreateNew_Success), "02_FormFilled");

        await _clientsPage.ClickSaveAsync();
        await Page.WaitForTimeoutAsync(1000);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Clients_CreateNew_Success), "03_AfterSave");

        // Assert
        var hasClient = await _clientsPage.HasClientWithNameAsync(newClientName);
        Assert.That(hasClient, Is.True, $"Client '{newClientName}' should be in the table");
    }

    [Test]
    public async Task Clients_Edit_Success()
    {
        // Arrange - First create a client to edit
        await _clientsPage.NavigateAsync();
        await _clientsPage.WaitForTableLoadAsync();
        var originalName = $"EditMe {DateTime.Now.Ticks}";
        await _clientsPage.CreateClientAsync(originalName);
        await Page.WaitForTimeoutAsync(1000);

        var updatedName = $"Updated Client {DateTime.Now.Ticks}";

        // Act
        await _clientsPage.ClickEditAsync(0);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Clients_Edit_Success), "01_EditModalOpened");

        await _clientsPage.FillNameAsync(updatedName);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Clients_Edit_Success), "02_NameUpdated");

        await _clientsPage.ClickSaveAsync();
        await Page.WaitForTimeoutAsync(1000);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Clients_Edit_Success), "03_AfterSave");

        // Assert
        var hasClient = await _clientsPage.HasClientWithNameAsync(updatedName);
        Assert.That(hasClient, Is.True, $"Updated client '{updatedName}' should be in the table");
    }

    [Test]
    public async Task Clients_Delete_Success()
    {
        // Arrange - First create a client to delete
        await _clientsPage.NavigateAsync();
        await _clientsPage.WaitForTableLoadAsync();
        var clientToDelete = $"DeleteMe {DateTime.Now.Ticks}";
        await _clientsPage.CreateClientAsync(clientToDelete);
        await Page.WaitForTimeoutAsync(1000);

        var initialCount = await _clientsPage.GetRowCountAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Clients_Delete_Success), "01_BeforeDelete");

        // Act - Setup dialog handler and delete
        Page.Dialog += async (_, dialog) => await dialog.AcceptAsync();
        await _clientsPage.ClickDeleteAsync(0);
        await Page.WaitForTimeoutAsync(1000);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Clients_Delete_Success), "02_AfterDelete");

        // Assert
        var finalCount = await _clientsPage.GetRowCountAsync();
        Assert.That(finalCount, Is.LessThan(initialCount), "Row count should decrease after delete");
    }

    [Test]
    public async Task Clients_Search_FiltersTable()
    {
        // Arrange
        await _clientsPage.NavigateAsync();
        await _clientsPage.WaitForTableLoadAsync();

        // Create a client with unique name
        var uniqueName = $"SearchTest{DateTime.Now.Ticks}";
        await _clientsPage.CreateClientAsync(uniqueName);
        await Page.WaitForTimeoutAsync(1000);

        // Act
        await _clientsPage.SearchAsync(uniqueName);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Clients_Search_FiltersTable), "01_AfterSearch");

        // Assert
        var rowCount = await _clientsPage.GetRowCountAsync();
        Assert.That(rowCount, Is.GreaterThanOrEqualTo(1), "Should find at least one result");
    }

    [Test]
    public async Task Clients_Modal_OpensOnNewClick()
    {
        // Arrange
        await _clientsPage.NavigateAsync();
        await _clientsPage.WaitForTableLoadAsync();

        // Act
        await _clientsPage.ClickNewClientAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Clients_Modal_OpensOnNewClick), "01_ModalVisible");

        // Assert
        var isModalVisible = await _clientsPage.IsModalVisibleAsync();
        Assert.That(isModalVisible, Is.True, "Modal should be visible after clicking New button");
    }
}
