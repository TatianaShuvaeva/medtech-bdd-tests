using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MedTech.Common.Data;
using Microsoft.AspNetCore.Hosting;

namespace MedTech.Tests.Hooks;

/// <summary>
/// Startet die Blazor-App im Test-Prozess und überschreibt die DB-Registrierung
/// mit der Test-InMemory-DB (damit App und Tests dieselbe DB teilen).
/// Wird für Integrationstests ohne Browser verwendet.
/// </summary>
public class MedTechWebAppFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName;

    public MedTechWebAppFactory(string dbName)
    {
        _dbName = dbName;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Vorhandene DB-Registrierung entfernen
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<MedTechDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            // Test-InMemory-DB registrieren (gleicher Name wie im DatabaseHooks!)
            services.AddDbContext<MedTechDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            services.AddScoped<IMedTechDbContext>(sp =>
                sp.GetRequiredService<MedTechDbContext>());
        });
    }
}