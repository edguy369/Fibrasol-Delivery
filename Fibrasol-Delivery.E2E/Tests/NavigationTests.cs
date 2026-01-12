using Microsoft.Playwright.NUnit;
using Fibrasol_Delivery.E2E.Helpers;

namespace Fibrasol_Delivery.E2E.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class NavigationTests : PageTest
{
    private string BaseUrl => PlaywrightFixture.BaseUrl;

    [SetUp]
    public async Task Setup()
    {
        await TestHelpers.LoginAsync(Page);
    }

    [Test]
    public async Task Navigation_ToClients_FromMenu()
    {
        // Act
        await Page.ClickAsync(".navbar-nav >> text=Clientes");
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Navigation_ToClients_FromMenu), "01_ClientsPage");

        // Assert
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/clientes");
    }

    [Test]
    public async Task Navigation_ToSalesPersons_FromMenu()
    {
        // Act
        await Page.ClickAsync(".navbar-nav >> text=Vendedores");
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Navigation_ToSalesPersons_FromMenu), "01_SalesPersonsPage");

        // Assert
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/vendedores");
    }

    [Test]
    public async Task Navigation_ToSalesReport_FromMenu()
    {
        // Act
        await Page.ClickAsync(".navbar-nav >> text=Reporte de ventas");
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Navigation_ToSalesReport_FromMenu), "01_ReportPage");

        // Assert
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(@"/vendedores/reportes"));
    }

    [Test]
    public async Task Navigation_ToRiders_FromMenu()
    {
        // Act
        await Page.ClickAsync(".navbar-nav >> text=Conductores");
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Navigation_ToRiders_FromMenu), "01_RidersPage");

        // Assert
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/conductores");
    }

    [Test]
    public async Task Navigation_ToDeliveryOrders_FromMenu()
    {
        // Act
        await Page.ClickAsync(".navbar-nav >> text=Constancias");
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Navigation_ToDeliveryOrders_FromMenu), "01_DeliveryOrdersPage");

        // Assert
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/constancias");
    }

    [Test]
    public async Task Navigation_ToDeliveryOrderStatus_FromMenu()
    {
        // Act
        await Page.ClickAsync(".navbar-nav >> text=Estados");
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Navigation_ToDeliveryOrderStatus_FromMenu), "01_StatusPage");

        // Assert
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(@"/estados"));
    }

    [Test]
    public async Task Navigation_ToHome_ByClickingLogo()
    {
        // Arrange - Navigate away from home first
        await Page.GotoAsync($"{BaseUrl}/clientes");

        // Act
        await Page.ClickAsync(".navbar-brand-custom");
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Navigation_ToHome_ByClickingLogo), "01_HomePage");

        // Assert
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/");
    }

    [Test]
    public async Task Navigation_UserDropdown_IsVisible()
    {
        // Assert
        var userDropdown = Page.Locator(".user-avatar");
        await Expect(userDropdown).ToBeVisibleAsync();
        await ScreenshotHelper.TakeScreenshotAsync(Page, nameof(Navigation_UserDropdown_IsVisible), "01_UserDropdown");
    }
}
