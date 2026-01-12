using System.Diagnostics;
using Microsoft.Playwright;

namespace Fibrasol_Delivery.E2E;

[SetUpFixture]
public class PlaywrightFixture
{
    private static Process? _dockerProcess;

    public static string BaseUrl { get; } = "http://localhost:5003";
    public static IPlaywright? Playwright { get; private set; }
    public static IBrowser? Browser { get; private set; }

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        await StartDockerCompose();
        await WaitForAppToBeReady();

        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
    }

    [OneTimeTearDown]
    public async Task GlobalTeardown()
    {
        if (Browser != null)
        {
            await Browser.CloseAsync();
        }

        Playwright?.Dispose();

        await StopDockerCompose();
    }

    private static async Task StartDockerCompose()
    {
        var projectRoot = GetProjectRoot();

        Console.WriteLine("Starting Docker Compose for tests...");

        var stopProcess = new ProcessStartInfo
        {
            FileName = "docker",
            Arguments = "compose -f docker-compose.test.yml down -v",
            WorkingDirectory = projectRoot,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using (var process = Process.Start(stopProcess))
        {
            await process!.WaitForExitAsync();
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = "docker",
            Arguments = "compose -f docker-compose.test.yml up -d --build",
            WorkingDirectory = projectRoot,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        _dockerProcess = Process.Start(startInfo);
        await _dockerProcess!.WaitForExitAsync();

        if (_dockerProcess.ExitCode != 0)
        {
            var error = await _dockerProcess.StandardError.ReadToEndAsync();
            throw new Exception($"Failed to start Docker Compose: {error}");
        }

        Console.WriteLine("Docker Compose started successfully");
    }

    private static async Task StopDockerCompose()
    {
        var projectRoot = GetProjectRoot();

        Console.WriteLine("Stopping Docker Compose...");

        var stopInfo = new ProcessStartInfo
        {
            FileName = "docker",
            Arguments = "compose -f docker-compose.test.yml down -v",
            WorkingDirectory = projectRoot,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using var process = Process.Start(stopInfo);
        await process!.WaitForExitAsync();

        Console.WriteLine("Docker Compose stopped");
    }

    private static async Task WaitForAppToBeReady()
    {
        Console.WriteLine("Waiting for app to be ready...");

        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(10);
        var maxAttempts = 90; // Increased from 60 to allow more time for Docker startup
        var attempt = 0;

        while (attempt < maxAttempts)
        {
            try
            {
                var response = await httpClient.GetAsync($"{BaseUrl}/login");
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"App is ready after {attempt + 1} attempts");
                    // Additional wait to ensure MySQL is fully initialized
                    Console.WriteLine("Waiting additional time for database to stabilize...");
                    await Task.Delay(5000);
                    return;
                }
            }
            catch
            {
                // App not ready yet
            }

            attempt++;
            await Task.Delay(3000); // Increased from 2000 to 3000ms
        }

        throw new Exception($"App did not become ready after {maxAttempts} attempts");
    }

    private static string GetProjectRoot()
    {
        var currentDir = Directory.GetCurrentDirectory();

        while (!File.Exists(Path.Combine(currentDir, "docker-compose.test.yml")))
        {
            var parent = Directory.GetParent(currentDir);
            if (parent == null)
            {
                throw new Exception("Could not find project root with docker-compose.test.yml");
            }
            currentDir = parent.FullName;
        }

        return currentDir;
    }
}
