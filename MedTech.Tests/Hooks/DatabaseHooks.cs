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

        var options = new DbContextOptionsBuilder<MedTechDbContext>()
            .UseInMemoryDatabase(_dbName)
            .Options;

        _dbContext = new MedTechDbContext(options);
        _dbContext.Database.EnsureCreated();

        _container.RegisterInstanceAs(_dbContext);
        _container.RegisterInstanceAs(new RezeptService(_dbContext as IMedTechDbContext));
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

    [AfterScenario]
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
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {
        Console.WriteLine("Testlauf abgeschlossen");
    }
}