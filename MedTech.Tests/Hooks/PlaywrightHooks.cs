using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Microsoft.Playwright;
using Reqnroll;
using Reqnroll.BoDi;
using MedTech.Tests.Pages;

namespace MedTech.Tests.Hooks;

[Binding]
public class PlaywrightHooks
{
    private readonly IObjectContainer _container;
    private readonly ScenarioContext _context;

    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _browserContext;
    private IPage? _page;

    // Wird pro Szenario gestartet (kein statischer Zustand mehr)
    private Process? _appProcess;
    private string _appUrl = string.Empty;

    public PlaywrightHooks(IObjectContainer container, ScenarioContext context)
    {
        _container = container;
        _context = context;
    }

    /// <summary>
    /// Startet die Demo-App pro Szenario auf einem freien Port und übergibt die SQLite-DB.
    /// Wenn MEDTECH_UI_BASE_URL gesetzt ist, wird diese App direkt verwendet (Entwickler-Modus).
    /// </summary>
    [BeforeScenario("browser", Order = 10)]
    public async Task DemoAppSicherstellen()
    {
        // Entwickler-Modus: extern gestartete App verwenden
        var manualUrl = Environment.GetEnvironmentVariable("MEDTECH_UI_BASE_URL")?.TrimEnd('/');
        if (!string.IsNullOrEmpty(manualUrl))
        {
            _appUrl = manualUrl;
            _context["AppUrl"] = _appUrl;
            Console.WriteLine($"[PlaywrightHooks] Verwende manuell gestartete App: {_appUrl}");
            return;
        }

        // Pro-Szenario-Start: freien Port + SQLite-Pfad weitergeben
        var port = FindFreePort();
        _appUrl = $"http://localhost:{port}";
        _context["AppUrl"] = _appUrl;

        var projektPfad = FindeAppProjektPfad();
        if (projektPfad is null)
        {
            Assert.Fail("MedTech.App.csproj konnte für die Browser-Tests nicht gefunden werden.");
            return;
        }

        Console.WriteLine($"[PlaywrightHooks] Starte App auf {_appUrl} (Projekt: {projektPfad})");

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

        startInfo.Environment["MEDTECH_APP_URL"] = _appUrl;
        startInfo.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";

        // SQLite-Datei für gemeinsame DB übergeben (gesetzt von DatabaseHooks für @browser-Szenarien)
        if (_context.TryGetValue("DbPath", out var dbPathObj) && dbPathObj is string dbPath)
        {
            startInfo.Environment["MEDTECH_TEST_DB_PATH"] = dbPath;
            Console.WriteLine($"[PlaywrightHooks] SQLite-DB: {dbPath}");
        }

        _appProcess = new Process { StartInfo = startInfo };
        _appProcess.OutputDataReceived += (_, args) =>
        {
            if (!string.IsNullOrWhiteSpace(args.Data))
                Console.WriteLine($"[MedTech.App] {args.Data}");
        };
        _appProcess.ErrorDataReceived += (_, args) =>
        {
            if (!string.IsNullOrWhiteSpace(args.Data))
                Console.WriteLine($"[MedTech.App] {args.Data}");
        };

        _appProcess.Start();
        _appProcess.BeginOutputReadLine();
        _appProcess.BeginErrorReadLine();

        var erreichbar = await WarteAufAppStart(_appUrl);
        if (!erreichbar)
            Assert.Fail($"Die Demo-App konnte nicht unter {_appUrl} gestartet werden.");
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

        var appUrl = _context.Get<string>("AppUrl");

        _container.RegisterInstanceAs(_page);
        _container.RegisterInstanceAs(new PatientenlistePage(_page, appUrl));
        _container.RegisterInstanceAs(new RezeptPage(_page));
    }

    // Order = 10 → läuft vor DatabaseHooks.DisposeScenarioDatabase (Order = 20),
    // damit der App-Prozess die SQLite-Datei freigibt, bevor sie gelöscht wird.
    [AfterScenario("browser", Order = 10)]
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

        // App-Prozess stoppen (nur wenn er von uns gestartet wurde)
        if (_appProcess != null)
        {
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
            }
        }
    }

    private static int FindFreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private static async Task<bool> WarteAufAppStart(string appUrl)
    {
        for (var versuch = 0; versuch < 30; versuch++)
        {
            if (await IstAppErreichbar(appUrl))
                return true;
            await Task.Delay(1000);
        }
        return false;
    }

    private static async Task<bool> IstAppErreichbar(string appUrl)
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
            using var response = await client.GetAsync($"{appUrl}/patienten");
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
            return envPfad;

        var kandidaten = new[]
        {
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "MedTech.App", "MedTech.App.csproj")),
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "MedTech.App", "MedTech.App.csproj")),
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "MedTech.App", "MedTech.App.csproj"))
        };

        return kandidaten.FirstOrDefault(File.Exists);
    }
}
