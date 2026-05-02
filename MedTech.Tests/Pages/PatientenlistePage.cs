using Microsoft.Playwright;

namespace MedTech.Tests.Pages;

public class PatientenlistePage : BasePage
{
    private const string SuchfeldSelector = "[data-testid='patient-search-input']";
    private const string SuchergebnisSelector = "[data-testid='patient-result-item']";
    private const string PatientenakteSelector = "[data-testid='patient-record']";
    private const string FehlerMeldungSelector = "[data-testid='error-message']";

    private readonly string _basisUrl;

    public PatientenlistePage(IPage page, string basisUrl) : base(page)
    {
        _basisUrl = basisUrl;
    }

    public async Task NavigiereZurPatientenliste()
    {
        await Page.GotoAsync($"{_basisUrl}/patienten");
        await WarteBlazerRendering();
    }

    public async Task SucheNachPatient(string suchbegriff)
    {
        await GibeEin(SuchfeldSelector, suchbegriff);
        // Blazor debounced Search — kurz warten
        await Page.WaitForTimeoutAsync(500);
        await WarteBlazerRendering();
    }

    public async Task<int> AnzahlSuchergebnisse()
    {
        return await Page.Locator(SuchergebnisSelector).CountAsync();
    }

    public async Task<string> ErsterSuchertreffer()
    {
        return await Page.Locator(SuchergebnisSelector).First.InnerTextAsync();
    }

    public async Task ÖffnePatientenakte(string patientName)
    {
        var link = Page.Locator(SuchergebnisSelector)
                       .Filter(new() { HasText = patientName });
        await link.ClickAsync();
        await WarteBlazerRendering();
    }

    public async Task<bool> IstPatientenakteGeöffnet()
    {
        return await Page.Locator(PatientenakteSelector).IsVisibleAsync();
    }

    public async Task<bool> IstFehlermeldungSichtbar()
    {
        return await Page.Locator(FehlerMeldungSelector).IsVisibleAsync();
    }
}
