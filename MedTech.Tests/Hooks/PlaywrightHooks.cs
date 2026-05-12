using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Reqnroll;
using Reqnroll.BoDi;
using MedTech.Common.Data;
using MedTech.Tests.Pages;

namespace MedTech.Tests.Hooks;

[Binding]
public class PlaywrightHooks
{
    private static MedTechWebAppFactory? _factory;
    private static string? _appUrl;

    private readonly IObjectContainer _container;
    private readonly ScenarioContext _context;

    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _browserContext;
    private IPage? _page;

    public PlaywrightHooks(IObjectContainer container, ScenarioContext context)
    {
        _container = container;
        _context = context;
    }

    [BeforeScenario("browser", Order = 5)]
    public static void AppFactoryStarten(ScenarioContext context)
    {
        // DB-Name aus DatabaseHooks übernehmen (muss im ScenarioContext gesetzt sein)
        var dbName = context.Get<string>("DbName");

        _factory = new MedTechWebAppFactory(dbName);

        // Test-Server auf einem freien Port starten und Seed-Daten einfügen
        var client = _factory.CreateClient();
        _appUrl = _factory.Server.BaseAddress.ToString().TrimEnd('/');

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MedTechDbContext>();
        MedTechDbSeeder.Seed(db);
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
        _container.RegisterInstanceAs(new PatientenlistePage(_page, _appUrl!));
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
}
