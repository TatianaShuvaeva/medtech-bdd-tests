using Reqnroll;
using Reqnroll.BoDi;
using Microsoft.EntityFrameworkCore;
using MedTech.Common.Data;
using MedTech.Common.Services;

namespace MedTech.Tests.Hooks;

[Binding]
public sealed class DatabaseHooks
{
    private readonly IObjectContainer _container;
    private readonly ScenarioContext _scenarioContext;
    private MedTechDbContext? _dbContext;
    private string? _dbName;

    public DatabaseHooks(IObjectContainer container, ScenarioContext scenarioContext)
    {
        _container = container;
        _scenarioContext = scenarioContext;
    }

    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        TestContext.Progress.WriteLine("MedTech Testlauf gestartet");
        TestContext.Progress.WriteLine($"Start: {DateTime.Now:dd.MM.yyyy HH:mm}");
    }

    [BeforeScenario(Order = 0)]
    public void CreateScenarioDatabase()
    {
        _dbName = $"MedTechTest_{Guid.NewGuid()}";
        _scenarioContext["DbName"] = _dbName;

        DbContextOptions<MedTechDbContext> options;

        // Browser-Tests starten die App als eigenen Prozess → In-Memory-DB kann nicht geteilt werden.
        // Stattdessen SQLite-Datei verwenden, damit App und Test dieselbe DB lesen/schreiben.
        if (_scenarioContext.ScenarioInfo.Tags.Contains("browser"))
        {
            var dbPath = Path.Combine(Path.GetTempPath(), $"medtech_test_{Guid.NewGuid()}.db");
            _scenarioContext["DbPath"] = dbPath;
            options = new DbContextOptionsBuilder<MedTechDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;
        }
        else
        {
            options = new DbContextOptionsBuilder<MedTechDbContext>()
                .UseInMemoryDatabase(_dbName)
                .Options;
        }

        _dbContext = new MedTechDbContext(options);
        _dbContext.Database.EnsureCreated();

        _container.RegisterInstanceAs(_dbContext);
        _container.RegisterInstanceAs<IMedTechDbContext>(_dbContext);
        _container.RegisterInstanceAs(new RezeptService(_dbContext));
    }

    [BeforeScenario(Order = 1)]
    public void LogScenarioInformation()
    {
        var title = _scenarioContext.ScenarioInfo.Title;
        var tags = string.Join(", ", _scenarioContext.ScenarioInfo.Tags);

        Console.WriteLine($"Szenario: {title}");
        Console.WriteLine($"Tags: {tags}");
        Console.WriteLine($"InMemory-Datenbank: {_dbName}");
    }

    // Browser-Tests brauchen Seed-Daten in der Test-DB (App läuft als separater Prozess)
    [BeforeScenario("browser", Order = 2)]
    public void SeedTestDatenbank()
    {
        MedTechDbSeeder.Seed(_dbContext!);
    }

    [AfterScenario(Order = 20)]
    public void DisposeScenarioDatabase()
    {
        if (_scenarioContext.TestError is not null)
        {
            Console.WriteLine($"FEHLGESCHLAGEN: {_scenarioContext.TestError.Message}");
        }
        else
        {
            Console.WriteLine("BESTANDEN");
        }

        _dbContext?.Dispose();
        _dbContext = null;

        // SQLite-Datei aus Browser-Tests aufräumen (nach App- und Browser-Cleanup, Order = 20)
        if (_scenarioContext.TryGetValue("DbPath", out var dbPathObj) && dbPathObj is string dbPath)
        {
            try { File.Delete(dbPath); } catch { /* Datei ggf. noch gesperrt – ignorieren */ }
        }
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {
        Console.WriteLine("Testlauf abgeschlossen");
    }
}