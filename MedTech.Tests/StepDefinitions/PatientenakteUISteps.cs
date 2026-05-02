using FluentAssertions;
using MedTech.Tests.Pages;
using Reqnroll;

namespace MedTech.Tests.StepDefinitions;

[Binding]
public class PatientenakteUISteps
{
    private readonly PatientenlistePage _patientenlistePage;
    private readonly RezeptPage _rezeptPage;
    private readonly ScenarioContext _context;

    public PatientenakteUISteps(
        PatientenlistePage patientenlistePage,
        RezeptPage rezeptPage,
        ScenarioContext context)
    {
        _patientenlistePage = patientenlistePage;
        _rezeptPage = rezeptPage;
        _context = context;
    }

    [Given(@"Dr\. Müller ist in der Blazor-Anwendung eingeloggt")]
    public async Task GegebenDrMüllerIstEingeloggt()
    {
        await _patientenlistePage.NavigiereZurPatientenliste();
    }

    [When(@"sie nach Patient ""(.*)"" sucht")]
    public async Task WennSieNachPatientSucht(string name)
    {
        _context["GesuchterPatient"] = name;
        await _patientenlistePage.SucheNachPatient(name);
    }

    // "Und die Patientenakte angezeigt werden" nach einem Wenn-Schritt → wird als When interpretiert
    [When(@"die Patientenakte angezeigt werden")]
    public async Task WennDiePatientenakteAngezeigtWerden()
    {
        var gesuchterPatient = _context.Get<string>("GesuchterPatient");
        await _patientenlistePage.ÖffnePatientenakte(gesuchterPatient);
    }

    [Then(@"sollte die Patientenakte angezeigt werden")]
    public async Task DannSolltePatientenakteAngezeigtWerden()
    {
        (await _patientenlistePage.AnzahlSuchergebnisse())
            .Should().BeGreaterThan(0, "Es sollte mindestens ein Suchergebnis geben");

        var gesuchterPatient = _context.Get<string>("GesuchterPatient");
        await _patientenlistePage.ÖffnePatientenakte(gesuchterPatient);

        (await _patientenlistePage.IstPatientenakteGeöffnet())
            .Should().BeTrue("Die Patientenakte sollte geöffnet sein");
    }

    [Then(@"sollte mindestens ein Suchergebnis angezeigt werden")]
    public async Task DannSollteErgebnisAngezeigt()
    {
        (await _patientenlistePage.AnzahlSuchergebnisse())
            .Should().BeGreaterThan(0, "Es sollte mindestens ein Suchergebnis geben");
    }

    [Then(@"der erste Treffer sollte ""(.*)"" enthalten")]
    public async Task DannSollteErsterTrefferEnthalten(string erwarteterName)
    {
        var ersterTreffer = await _patientenlistePage.ErsterSuchertreffer();
        ersterTreffer.Should().Contain(erwarteterName);
    }

    [When(@"der Arzt ""(.*)"" in der UI verschreibt")]
    public async Task WennArztVerschreibtInUI(string medikament)
    {
        await _rezeptPage.GibeMedikamentEin(medikament);
        await _rezeptPage.GibeDosierungEin("2x täglich");
        await _rezeptPage.KlickeVerschreiben();
    }

    [Then(@"sollte eine rote Allergie-Warnung in der UI erscheinen")]
    public async Task DannSollteAllergieWarnungErscheinen()
    {
        (await _rezeptPage.IstAllergieWarnungSichtbar())
            .Should().BeTrue("Die Allergie-Warnung muss für den Arzt sichtbar sein");
    }

    [Then(@"der Schweregrad der Warnung sollte ""(.*)"" sein")]
    public async Task DannSollteSchweregrad(string erwarteterSchweregrad)
    {
        var schweregrad = await _rezeptPage.LeseWarnungSchweregrad();
        schweregrad.Should().Be(erwarteterSchweregrad);
    }

    [Then(@"eine Erfolgsmeldung sollte in der UI erscheinen")]
    public async Task DannSollteErfolgsmeldungErscheinen()
    {
        (await _rezeptPage.IstErfolgsMeldungSichtbar())
            .Should().BeTrue("Eine Erfolgsmeldung muss angezeigt werden");
    }

    [Then(@"keine Allergie-Warnung sollte erscheinen")]
    public async Task DannSollteKeineWarnungErscheinen()
    {
        (await _rezeptPage.IstAllergieWarnungSichtbar())
            .Should().BeFalse("Es sollte keine Allergie-Warnung angezeigt werden");
    }
}
