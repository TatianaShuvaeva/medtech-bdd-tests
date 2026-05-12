using FluentAssertions;
using MedTech.Common.Data;
using MedTech.Tests.Infrastructure;
using MedTech.Tests.Pages;
using Reqnroll;

namespace MedTech.Tests.StepDefinitions;

[Binding]
public class PatientenakteUISteps
{
    private readonly PatientenlistePage _patientenlistePage;
    private readonly RezeptPage _rezeptPage;
    private readonly ScenarioContext _context;
    private readonly MedTechDbContext _db;



    public PatientenakteUISteps(
        PatientenlistePage patientenlistePage,
        RezeptPage rezeptPage,
        ScenarioContext context, MedTechDbContext db)
    {
        _db = db;
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

        // Patient-ID aus der DB nachschlagen und im Context speichern
        var patient = _db.Patienten.FirstOrDefault(p => p.Name == gesuchterPatient);
        if (patient != null)
        {
            _context["GesuchterPatientId"] = patient.Id;
        }
    }

    [Then(@"sollte die Patientenakte angezeigt werden")]
    public async Task DannSolltePatientenakteAngezeigtWerden()
    {
        // Arrange
        (await _patientenlistePage.AnzahlSuchergebnisse())
            .Should().BeGreaterThan(0, "Es sollte mindestens ein Suchergebnis geben");

        var gesuchterPatient = _context.Get<string>("GesuchterPatient");

        // Act
        await _patientenlistePage.ÖffnePatientenakte(gesuchterPatient);

        // Assert
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
        _context["Medikament"] = medikament;

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

    [Then(@"in der Datenbank korrekt gespeichert ist")]
    public async Task DannSollteInDatenbankKorrektGespeichertSein()
    {
        var gesuchterPatientId = _context.Get<int>("GesuchterPatientId");
        var medikament = _context.Get<string>("Medikament");

        var eintrag = _db.Rezepte.FirstOrDefault(r =>
            r.Medikament == medikament
                && r.PatientIdFk == gesuchterPatientId
                && r.Dosierung == "2x täglich");

        eintrag.Should().NotBeNull("Das Rezept sollte in der Datenbank korrekt gespeichert sein");
    }

    [Given(@"Patient ""(.*)"" existiert in der Datenbank")]
    public void GegebenPatientExistiertInDB(string name)
    {
        var patient = _db.Patienten.FirstOrDefault(p => p.Name == name);
        patient.Should().NotBeNull($"Patient {name} muss in der Test-DB vorhanden sein");
        _context["GesuchterPatientId"] = patient!.Id;
    }

    [Given(@"der Patient hat bisher (\d+) Rezept\(e\)")]
    public void GegebenPatientHatRezepte(int anzahl)
    {
        var patientId = _context.Get<int>("GesuchterPatientId");
        var aktuelleAnzahl = _db.Rezepte.Count(r => r.PatientIdFk == patientId);
        aktuelleAnzahl.Should().Be(anzahl);
        _context["RezeptAnzahlVorher"] = aktuelleAnzahl;
    }
    
    [Then(@"sollte die Rezeptanzahl des Patienten um 1 gestiegen sein")]
    public void DannSollteRezeptAnzahlGestiegen()
    {
        var patientId = _context.Get<int>("GesuchterPatientId");
        var vorher = _context.Get<int>("RezeptAnzahlVorher");
        var nachher = _db.Rezepte.Count(r => r.PatientIdFk == patientId);
        nachher.Should().Be(vorher + 1);
    }

    [Then(@"sollte die Rezeptanzahl des Patienten NICHT gestiegen sein")]
    public void DannSollteRezeptAnzahlNichtGestiegen()
    {
        var patientId = _context.Get<int>("GesuchterPatientId");
        var vorher = _context.Get<int>("RezeptAnzahlVorher");
        var nachher = _db.Rezepte.Count(r => r.PatientIdFk == patientId);
        nachher.Should().Be(vorher, "Die Allergie-Warnung muss das Speichern des Rezepts verhindern");
    }
}
