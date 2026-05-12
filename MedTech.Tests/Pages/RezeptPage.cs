using Microsoft.Playwright;

namespace MedTech.Tests.Pages;

public class RezeptPage : BasePage
{
    private const string MedikamentInput = "[data-testid='medication-input']";
    private const string DosierungInput = "[data-testid='dosage-input']";
    private const string VerschreibenButton = "[data-testid='prescribe-button']";
    private const string AllergieWarnung = "[data-testid='allergy-warning']";
    private const string WechselwirkungWarnung = "[data-testid='interaction-warning']";
    private const string ErfolgsMeldung = "[data-testid='success-message']";
    private const string WarnungSchweregrad = "[data-testid='warning-severity']";

    public RezeptPage(IPage page) : base(page) { }

    public async Task GibeMedikamentEin(string medikament)
    {
        await GibeEin(MedikamentInput, medikament);
        await WarteBlazerRendering();
    }

    public async Task GibeDosierungEin(string dosierung)
        => await GibeEin(DosierungInput, dosierung);

    public async Task KlickeVerschreiben()
    {
        await KlickeAuf(VerschreibenButton);
        await WarteBlazerRendering();
    }

    public async Task<bool> IstAllergieWarnungSichtbar()
    {
        try
        {
            await Page.Locator(AllergieWarnung).WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
            return true;
        }
        catch { return false; }
    }

    public async Task<bool> IstWechselwirkungWarnungSichtbar()
        => await Page.Locator(WechselwirkungWarnung).IsVisibleAsync();

    public async Task<string> LeseAllergieWarnungsText()
        => await LeseText(AllergieWarnung);

    public async Task<string> LeseWarnungSchweregrad()
        => await LeseText(WarnungSchweregrad);

    public async Task<bool> IstErfolgsMeldungSichtbar()
    {
        try
        {
            await Page.Locator(ErfolgsMeldung).WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
            return true;
        }
        catch { return false; }
    }

    public async Task<byte[]> MacheScreenshot()
        => await Page.ScreenshotAsync(new() { FullPage = true });
}
