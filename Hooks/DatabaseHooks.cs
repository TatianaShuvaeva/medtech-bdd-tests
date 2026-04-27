using Reqnroll.BoDi;
using Microsoft.EntityFrameworkCore;
using Reqnroll;
using MedTech.Tests.Infrastructure;

namespace MedTech.Tests.Hooks;

[Binding]
public sealed class DatabaseHooks
{
    private readonly IObjectContainer _container;
    private TestDbContext? _dbContext;

    public DatabaseHooks(IObjectContainer container)
    {
        _container = container;
    }

    [BeforeScenario(Order = 0)]
    public void CreateScenarioDatabase()
    {
        var dbName = $"MedTechTest_{Guid.NewGuid()}";

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        _dbContext = new TestDbContext(options);
        _dbContext.Database.EnsureCreated();

        _container.RegisterInstanceAs(_dbContext);
        _container.RegisterInstanceAs(new RezeptService(_dbContext));
    }

    [AfterScenario(Order = 1000)]
    public void DisposeScenarioDatabase()
    {
        if (_dbContext is null) return;

        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
        _dbContext = null;
    }
}