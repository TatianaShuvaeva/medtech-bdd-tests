using Microsoft.Playwright;

namespace MedTech.Tests.Pages;

public abstract class BasePage
{
    protected readonly IPage Page;

    protected BasePage(IPage page)
    {
        Page = page;
    }

    protected async Task<ILocator> WarteFürElement(string selector)
    {
        var element = Page.Locator(selector);
        await element.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        return element;
    }

    protected async Task KlickeAuf(string selector)
    {
        await (await WarteFürElement(selector)).ClickAsync();
    }

    protected async Task GibeEin(string selector, string text)
    {
        var element = await WarteFürElement(selector);
        await element.ClearAsync();
        await element.FillAsync(text);
    }

    protected async Task<string> LeseText(string selector)
    {
        return await Page.Locator(selector).InnerTextAsync();
    }

    // Blazor-spezifisch: Warte auf Blazor-Rendering
    protected async Task WarteBlazerRendering()
    {
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}
