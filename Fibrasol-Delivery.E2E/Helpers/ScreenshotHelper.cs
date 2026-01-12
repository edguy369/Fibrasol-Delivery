using Microsoft.Playwright;

namespace Fibrasol_Delivery.E2E.Helpers;

public static class ScreenshotHelper
{
    private static readonly string ScreenshotsDir = Path.Combine(
        Directory.GetCurrentDirectory(),
        "..", "..", "..",
        "screenshots"
    );

    static ScreenshotHelper()
    {
        if (!Directory.Exists(ScreenshotsDir))
        {
            Directory.CreateDirectory(ScreenshotsDir);
        }
    }

    public static async Task TakeScreenshotAsync(IPage page, string testName, string step)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var fileName = $"{testName}_{step}_{timestamp}.png";
        var filePath = Path.Combine(ScreenshotsDir, fileName);

        await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = filePath,
            FullPage = true
        });

        Console.WriteLine($"Screenshot saved: {filePath}");
    }

    public static async Task TakeScreenshotOnErrorAsync(IPage page, string testName)
    {
        await TakeScreenshotAsync(page, testName, "ERROR");
    }

    public static async Task TakeElementScreenshotAsync(IPage page, string selector, string testName, string step)
    {
        var element = await page.QuerySelectorAsync(selector);
        if (element != null)
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"{testName}_{step}_{timestamp}.png";
            var filePath = Path.Combine(ScreenshotsDir, fileName);

            await element.ScreenshotAsync(new ElementHandleScreenshotOptions
            {
                Path = filePath
            });

            Console.WriteLine($"Element screenshot saved: {filePath}");
        }
    }

    public static string GetScreenshotsDirectory() => ScreenshotsDir;
}
