using MedTech.App.Components;
using MedTech.App.Services;

var builder = WebApplication.CreateBuilder(args);
var appUrl = Environment.GetEnvironmentVariable("MEDTECH_APP_URL")
    ?? builder.Configuration["App:Url"]
    ?? "http://localhost:3000";

builder.WebHost.UseUrls(appUrl);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddSingleton<MedTechDemoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
