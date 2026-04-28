using Reqnroll;
using NUnit.Framework;
using FluentAssertions;
using MedTech.Tests.Infrastructure;

namespace MedTech.Tests.StepDefinitions;

[Binding]
public class RezeptSteps
{
    // Reqnroll's Dependency Injection
    private readonly ScenarioContext _context;
    private readonly TestDbContext _db;
    private readonly AuditLogService _auditLogService;

    // Lokaler State des Szenarios
    private string _warnung = string.Empty;
    private bool _rezeptGespeichert = false;

    public RezeptSteps(ScenarioContext context, TestDbContext db)
    {
        _context = context;
        _db = db;
        _auditLogService = new AuditLogService(_db);
    }

    private bool IstAuditRelevant => _context.ScenarioInfo.Tags.Contains("audit-relevant");

    [Given("Dr. Weber ist im System angemeldet")]
    public void GegebenDrWeberIstEingeloggt()
    {
        // Arzt in InMemory-DB suchen/anlegen
        var arzt = _db.Aerzte.FirstOrDefault(a => a.Name == "Dr. Weber")
                   ?? new Arzt { Name = "Dr. Weber", Fachrichtung = "Allgemeinmedizin" };

        _db.Aerzte.Add(arzt);
        _db.SaveChanges();

        _context["AktuellerArzt"] = arzt;
    }

    [Given("Patient {string} \\(ID: {string}\\) ist geöffnet")]
    public void GegebenPatientIstGeoeffnet(string name, string patientId)
    {
        var patient = new Patient
        {
            PatientId = patientId,
            Name = name,
            Allergien = new List<string>(),
            AktiveMedikamente = new List<string>()
        };

        _db.Patienten.Add(patient);
        _db.SaveChanges();

        _context["AktuellerPatient"] = patient;
    }

    [Given("Patient {string} hat eine dokumentierte Allergie gegen {string}")]
    public void GegebenPatientHatAllergie(string patientName, string allergen)
    {
        var patient = _context.Get<Patient>("AktuellerPatient");
        patient.Allergien.Add(allergen);
        _db.Patienten.Update(patient); // Explizit markieren, da List<string> per HasConversion gespeichert wird
        _db.SaveChanges();
    }

    [Given("Patient {string} nimmt aktuell {string} ein")]
    public void GegebenPatientNimmtMedikament(string patientName, string medikament)
    {
        var patient = _context.Get<Patient>("AktuellerPatient");
        patient.AktiveMedikamente.Add(medikament);
        _db.SaveChanges();
    }

    [When("Dr. Weber {string} zweimal täglich verschreibt")]
    [When("Dr. Weber {string} verschreibt")]
    public void WennDrWeberVerschreibt(string medikament)
    {
        var patient = _context.Get<Patient>("AktuellerPatient");
        var arzt = _context.Get<Arzt>("AktuellerArzt");

        // Business-Logik (Service aufrufen)
        var rezeptService = new RezeptService(_db);
        var ergebnis = rezeptService.VerschreibeMedikament(patient, arzt, medikament, "2x täglich");

        _context["Ergebnis"] = ergebnis;
        _rezeptGespeichert = ergebnis.ErfolgreichGespeichert;
        _warnung = ergebnis.Warnung ?? string.Empty;

        if (IstAuditRelevant)
        {
            var details = ergebnis.ErfolgreichGespeichert
                ? $"Rezept erfolgreich gespeichert: {medikament}"
                : $"Rezept nicht gespeichert: {ergebnis.Warnung ?? "ohne Warnung"}";

            _auditLogService.LogPrescription(
                arzt,
                patient,
                medikament,
                details,
                _context.ScenarioInfo.Title);
        }
    }

    [When("Dr. Weber versucht {string} zu verschreiben")]
    public void WennDrWeberVersuchtZuVerschreiben(string medikament)
    {
        WennDrWeberVerschreibt(medikament);
    }

    [Then("sollte das Rezept gespeichert werden")]
    public void DannSollteRezeptGespeichertSein()
    {
        _rezeptGespeichert.Should().BeTrue("Das Rezept sollte gespeichert worden sein");
    }

    [Then("das Rezept sollte NICHT automatisch gespeichert werden")]
    public void DannSollteRezeptNichtAutomatischGespeichertSein()
    {
        _rezeptGespeichert.Should().BeFalse("Bei Allergie darf kein Rezept automatisch gespeichert werden");
    }

    [Then("sollte eine rote Allergie-Warnung erscheinen: {string}")]
    public void DannSollteAllergieWarnungErscheinen(string erwartet)
    {
        _warnung.Should().Contain(erwartet,
            "Die Allergie-Warnung muss für den Arzt klar sichtbar sein");
    }

    [Then("sollte eine Wechselwirkungs-Warnung erscheinen: {string}")]
    public void DannSollteInteraktionsWarnungErscheinen(string erwartet)
    {
        _warnung.Should().Contain(erwartet,
            "Bei kritischen Wechselwirkungen muss eine Warnung erscheinen");
    }

    [Then("Dr. Weber muss die Überschreibung mit einem Grund bestätigen")]
    public void DannMussDrWeberOverrideBestaetigen()
    {
        _rezeptGespeichert.Should().BeFalse("ohne manuelle Override-Bestaetigung darf nicht gespeichert werden");
        _warnung.Should().NotBeNullOrWhiteSpace("eine Override-Bestaetigung ist nur bei bestehender Warnung sinnvoll");
    }

    [Then("der Schweregrad sollte als {string} markiert sein")]
    public void DannSollteSchweregradusMarkiertSein(string erwartet)
    {
        var ergebnis = _context.Get<RezeptErgebnis>("Ergebnis");
        ergebnis.WarnungSchweregrad.Should().Be(erwartet);
    }

    [Then("das System sollte {string} vorschlagen")]
    public void DannSollteSystemVorschlagen(string erwartet)
    {
        var ergebnis = _context.Get<RezeptErgebnis>("Ergebnis");
        ergebnis.Vorschlag.Should().Be(erwartet);
    }

    [Then("das Medikament sollte in den aktiven Medikamenten des Patienten erscheinen")]
    public void DannSollteMedikamentInAktivenMedikamentenErscheinen()
    {
        var patient = _context.Get<Patient>("AktuellerPatient");
        var aktualisiertPatient = _db.Patienten.Find(patient.Id);
        aktualisiertPatient!.AktiveMedikamente.Should().NotBeEmpty();
    }

    [Then("eine Rezept-PDF sollte generiert werden")]
    public void DannSolltePdfGeneriertWerden()
    {
        var ergebnis = _context.Get<RezeptErgebnis>("Ergebnis");
        ergebnis.PdfPfad.Should().NotBeNullOrEmpty("Es muss ein druckbares PDF geben");
    }
}