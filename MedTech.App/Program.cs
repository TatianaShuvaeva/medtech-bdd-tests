using MedTech.App.Components;
using MedTech.Common.Data;
using MedTech.Common.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var appUrl = Environment.GetEnvironmentVariable("MEDTECH_APP_URL")
    ?? builder.Configuration["App:Url"]
    ?? "http://localhost:3000";

builder.WebHost.UseUrls(appUrl);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Browser-Tests übergeben einen SQLite-Datei-Pfad, damit App und Test dieselbe DB teilen.
// Ohne diese Variable läuft die App mit einer In-Memory-Datenbank (normaler Betrieb).
var testDbPath = Environment.GetEnvironmentVariable("MEDTECH_TEST_DB_PATH");
if (!string.IsNullOrEmpty(testDbPath))
{
    builder.Services.AddDbContext<MedTechDbContext>(options =>
        options.UseSqlite($"Data Source={testDbPath}"));
}
else
{
    builder.Services.AddDbContext<MedTechDbContext>(options =>
        options.UseInMemoryDatabase("MedTechProd"));
}

builder.Services.AddScoped<IMedTechDbContext>(sp =>
    (IMedTechDbContext)sp.GetRequiredService<MedTechDbContext>());

builder.Services.AddScoped<RezeptService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MedTechDbContext>();
    MedTechDbSeeder.Seed(db);
}

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
public partial class Program { }
