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

    // Lokaler State des Szenarios
    private string _warnung = string.Empty;
    private bool _rezeptGespeichert = false;

    public RezeptSteps(ScenarioContext context, TestDbContext db)
    {
        _context = context;
        _db = db;
    }

    [Given("Dr. Weber is logged into the system")]
    public void GegebenDrWeberIstEingeloggt()
    {
        // Arzt in InMemory-DB suchen/anlegen
        var arzt = _db.Aerzte.FirstOrDefault(a => a.Name == "Dr. Weber")
                   ?? new Arzt { Name = "Dr. Weber", Fachrichtung = "Allgemeinmedizin" };

        _db.Aerzte.Add(arzt);
        _db.SaveChanges();

        _context["AktuellerArzt"] = arzt;
    }

    [Given("a patient {string} \\(ID: {string}\\) is open")]
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

    [Given("patient {string} has a documented allergy to {string}")]
    public void GegebenPatientHatAllergie(string patientName, string allergen)
    {
        var patient = _context.Get<Patient>("AktuellerPatient");
        patient.Allergien.Add(allergen);
        _db.Patienten.Update(patient); // Explizit markieren, da List<string> per HasConversion gespeichert wird
        _db.SaveChanges();
    }

    [Given("patient {string} is currently taking {string}")]
    public void GegebenPatientNimmtMedikament(string patientName, string medikament)
    {
        var patient = _context.Get<Patient>("AktuellerPatient");
        patient.AktiveMedikamente.Add(medikament);
        _db.SaveChanges();
    }

    [When("Dr. Weber prescribes {string} twice daily")]
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
    }

    [When("Dr. Weber tries to prescribe {string}")]
    public void WennDrWeberVersuchtZuVerschreiben(string medikament)
    {
        WennDrWeberVerschreibt(medikament);
    }

    [Then("the prescription should be saved")]
    public void DannSollteRezeptGespeichertSein()
    {
        _rezeptGespeichert.Should().BeTrue("Das Rezept sollte gespeichert worden sein");
    }

    [Then("the prescription should NOT be saved automatically")]
    public void DannSollteRezeptNichtAutomatischGespeichertSein()
    {
        _rezeptGespeichert.Should().BeFalse("Bei Allergie darf kein Rezept automatisch gespeichert werden");
    }

    [Then("a red allergy warning should appear: {string}")]
    public void DannSollteAllergieWarnungErscheinen(string erwartet)
    {
        _warnung.Should().Contain(erwartet,
            "Die Allergie-Warnung muss für den Arzt klar sichtbar sein");
    }

    [Then("an interaction warning should appear: {string}")]
    public void DannSollteInteraktionsWarnungErscheinen(string erwartet)
    {
        _warnung.Should().Contain(erwartet,
            "Bei kritischen Wechselwirkungen muss eine Warnung erscheinen");
    }

    [Then("Dr. Weber must confirm override with a reason")]
    public void DannMussDrWeberOverrideBestaetigen()
    {
        _rezeptGespeichert.Should().BeFalse("ohne manuelle Override-Bestaetigung darf nicht gespeichert werden");
        _warnung.Should().NotBeNullOrWhiteSpace("eine Override-Bestaetigung ist nur bei bestehender Warnung sinnvoll");
    }

    [Then("the severity should be marked as {string}")]
    public void DannSollteSchweregradusMarkiertSein(string erwartet)
    {
        var ergebnis = _context.Get<RezeptErgebnis>("Ergebnis");
        ergebnis.WarnungSchweregrad.Should().Be(erwartet);
    }

    [Then("the system should suggest {string}")]
    public void DannSollteSystemVorschlagen(string erwartet)
    {
        var ergebnis = _context.Get<RezeptErgebnis>("Ergebnis");
        ergebnis.Vorschlag.Should().Be(erwartet);
    }

    [Then("the medication should appear in the patient's active medications")]
    public void DannSollteMedikamentInAktivenMedikamentenErscheinen()
    {
        var patient = _context.Get<Patient>("AktuellerPatient");
        var aktualisiertPatient = _db.Patienten.Find(patient.Id);
        aktualisiertPatient!.AktiveMedikamente.Should().NotBeEmpty();
    }

    [Then("a prescription PDF should be generated")]
    public void DannSolltePdfGeneriertWerden()
    {
        var ergebnis = _context.Get<RezeptErgebnis>("Ergebnis");
        ergebnis.PdfPfad.Should().NotBeNullOrEmpty("Es muss ein druckbares PDF geben");
    }
}