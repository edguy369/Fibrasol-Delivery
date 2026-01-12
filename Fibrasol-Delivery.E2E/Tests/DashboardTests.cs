using Microsoft.Playwright.NUnit;
using Fibrasol_Delivery.E2E.Helpers;
using Fibrasol_Delivery.E2E.PageObjects;

namespace Fibrasol_Delivery.E2E.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class DashboardTests : PageTest
{
    private string BaseUrl => PlaywrightFixture.BaseUrl;
    private DashboardPage _dashboardPage = null!;

    [SetUp]
    public async Task Setup()
    {
        _dashboardPage = new DashboardPage(Page, BaseUrl);
        await TestHelpers.LoginAsync(Page);
    }

    [Test]
    public async Task Dashboard_LoadsAfterLogin()
    {
        // Assert
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/");
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Dashboard_LoadsAfterLogin), "01_DashboardLoaded");
    }

    [Test]
    public async Task Dashboard_ShowsMetricCards()
    {
        // Act
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Dashboard_ShowsMetricCards), "01_MetricCards");

        // Assert - Should have dashboard cards
        var cards = Page.Locator(".card, .dashboard-card");
        await Expect(cards.First).ToBeVisibleAsync();
    }

    [Test]
    public async Task Dashboard_ClientsCounter_IsVisible()
    {
        // Act
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        await Page.WaitForTimeoutAsync(2000); // Wait for AJAX to load counters

        // Assert
        var clientsText = await _dashboardPage.GetClientsCountAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Dashboard_ClientsCounter_IsVisible), "01_ClientsCounter");

        Assert.That(clientsText, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task Dashboard_RidersCounter_IsVisible()
    {
        // Act
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        await Page.WaitForTimeoutAsync(2000);

        // Assert
        var ridersText = await _dashboardPage.GetRidersCountAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Dashboard_RidersCounter_IsVisible), "01_RidersCounter");

        Assert.That(ridersText, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task Dashboard_OrdersCounter_IsVisible()
    {
        // Act
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        await Page.WaitForTimeoutAsync(2000);

        // Assert
        var ordersText = await _dashboardPage.GetOrdersCountAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Dashboard_OrdersCounter_IsVisible), "01_OrdersCounter");

        Assert.That(ordersText, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task Dashboard_InvoicesCounter_IsVisible()
    {
        // Act
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        await Page.WaitForTimeoutAsync(2000);

        // Assert
        var invoicesText = await _dashboardPage.GetInvoicesCountAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Dashboard_InvoicesCounter_IsVisible), "01_InvoicesCounter");

        Assert.That(invoicesText, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task Dashboard_UnsignedOrdersTable_IsVisible()
    {
        // Act
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Dashboard_UnsignedOrdersTable_IsVisible), "01_UnsignedOrdersTable");

        // Assert
        var isTableVisible = await _dashboardPage.IsUnsignedOrdersTableVisibleAsync();
        Assert.That(isTableVisible, Is.True);
    }

    [Test]
    public async Task Dashboard_UserDropdown_IsVisible()
    {
        // Assert
        var isVisible = await _dashboardPage.IsUserDropdownVisibleAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Dashboard_UserDropdown_IsVisible), "01_UserDropdown");

        Assert.That(isVisible, Is.True);
    }
}
