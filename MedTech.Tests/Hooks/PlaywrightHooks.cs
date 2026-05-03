using System.Diagnostics;
using Microsoft.Playwright;
using Reqnroll;
using Reqnroll.BoDi;
using MedTech.Tests.Pages;

namespace MedTech.Tests.Hooks;

[Binding]
public class PlaywrightHooks
{
    private static readonly SemaphoreSlim AppStartLock = new(1, 1);
    private static Process? _appProcess;
    private static bool _appStartedByTests;

    private readonly IObjectContainer _container;
    private readonly ScenarioContext _context;

    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _browserContext;
    private IPage? _page;

    private static readonly string AppUrl =
        Environment.GetEnvironmentVariable("MEDTECH_UI_BASE_URL")?.TrimEnd('/')
        ?? "http://localhost:3000";

    public PlaywrightHooks(IObjectContainer container, ScenarioContext context)
    {
        _container = container;
        _context = context;
    }

    [BeforeScenario("browser", Order = 10)]
    public static async Task DemoAppSicherstellen()
    {
        if (await IstAppErreichbar())
        {
            return;
        }

        await AppStartLock.WaitAsync();
        try
        {
            if (await IstAppErreichbar())
            {
                return;
            }

            var projektPfad = FindeAppProjektPfad();
            if (projektPfad is null)
            {
                Assert.Fail("MedTech.App.csproj konnte für die Browser-Tests nicht gefunden werden.");
                return;
            }

            Console.WriteLine($"Starte Demo-App automatisch: {projektPfad}");

            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{projektPfad}\" --no-launch-profile",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = Path.GetDirectoryName(projektPfad) ?? AppContext.BaseDirectory
            };

            startInfo.Environment["MEDTECH_APP_URL"] = AppUrl;
            startInfo.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";

            _appProcess = new Process { StartInfo = startInfo };
            _appProcess.OutputDataReceived += (_, args) =>
            {
                if (!string.IsNullOrWhiteSpace(args.Data))
                {
                    Console.WriteLine($"[MedTech.App] {args.Data}");
                }
            };
            _appProcess.ErrorDataReceived += (_, args) =>
            {
                if (!string.IsNullOrWhiteSpace(args.Data))
                {
                    Console.WriteLine($"[MedTech.App] {args.Data}");
                }
            };

            _appProcess.Start();
            _appProcess.BeginOutputReadLine();
            _appProcess.BeginErrorReadLine();
            _appStartedByTests = true;

            var erreichbar = await WarteAufAppStart();
            if (!erreichbar)
            {
                Assert.Fail($"Die Demo-App konnte nicht unter {AppUrl} gestartet werden.");
            }
        }
        finally
        {
            AppStartLock.Release();
        }
    }

    [BeforeScenario("browser", Order = 20)]
    public async Task BrowserStarten()
    {
        _playwright = await Playwright.CreateAsync();

        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = Environment.GetEnvironmentVariable("CI") == "true",
            SlowMo = 100,
        });

        _browserContext = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new() { Width = 1920, Height = 1080 },
            Locale = "de-DE",
        });

        _page = await _browserContext.NewPageAsync();

        // Page Objects im DI-Container registrieren
        _container.RegisterInstanceAs(_page);
        _container.RegisterInstanceAs(new PatientenlistePage(_page, AppUrl));
        _container.RegisterInstanceAs(new RezeptPage(_page));
    }

    [AfterScenario("browser")]
    public async Task NachBrowserSzenario()
    {
        // Bei Fehler: Screenshot speichern
        if (_context.TestError != null && _page != null)
        {
            var screenshot = await _page.ScreenshotAsync(new() { FullPage = true });
            var verzeichnis = "TestResults/screenshots";
            Directory.CreateDirectory(verzeichnis);
            var sicherName = string.Concat(_context.ScenarioInfo.Title
                .Split(Path.GetInvalidFileNameChars()));
            var pfad = $"{verzeichnis}/{sicherName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            await File.WriteAllBytesAsync(pfad, screenshot);
            Console.WriteLine($"Screenshot gespeichert: {pfad}");
        }

        if (_browserContext != null) await _browserContext.CloseAsync();
        if (_browser != null) await _browser.CloseAsync();
        _playwright?.Dispose();
    }

    [AfterTestRun]
    public static void DemoAppBeenden()
    {
        if (!_appStartedByTests || _appProcess is null)
        {
            return;
        }

        try
        {
            if (!_appProcess.HasExited)
            {
                _appProcess.Kill(entireProcessTree: true);
                _appProcess.WaitForExit(5000);
            }
        }
        finally
        {
            _appProcess.Dispose();
            _appProcess = null;
            _appStartedByTests = false;
        }
    }

    private static async Task<bool> WarteAufAppStart()
    {
        for (var versuch = 0; versuch < 30; versuch++)
        {
            if (_appProcess is { HasExited: true })
            {
                return false;
            }

            if (await IstAppErreichbar())
            {
                return true;
            }

            await Task.Delay(1000);
        }

        return false;
    }

    private static async Task<bool> IstAppErreichbar()
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
            using var response = await client.GetAsync($"{AppUrl}/patienten");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private static string? FindeAppProjektPfad()
    {
        var envPfad = Environment.GetEnvironmentVariable("MEDTECH_UI_PROJECT_PATH");
        if (!string.IsNullOrWhiteSpace(envPfad) && File.Exists(envPfad))
        {
            return envPfad;
        }

        var kandidaten = new[]
        {
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "MedTech.App", "MedTech.App.csproj")),
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "MedTech.App", "MedTech.App.csproj")),
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "MedTech.App", "MedTech.App.csproj"))
        };

        return kandidaten.FirstOrDefault(File.Exists);
    }
}
