using System.Globalization;
using FluentAssertions;
using MedTech.Common.Data;
using MedTech.Common.Models;
using MedTech.Tests.Infrastructure;
using Reqnroll;

namespace MedTech.Tests.StepDefinitions;

[Binding]
public class PatientenakteSteps
{
    private readonly ScenarioContext _context;
    private readonly MedTechDbContext _db;

    public PatientenakteSteps(ScenarioContext context, MedTechDbContext db)
    {
        _context = context;
        _db = db;
    }

    [Given("Patient {string} hat folgende Blutdruckmessungen:")]
    public void GivenPatientHasBloodPressureReadings(string name, Table table)
    {
        var patient = new Patient
        {
            PatientId = $"BP-{Guid.NewGuid():N}",
            Name = name,
            Allergien = new List<string>(),
            AktiveMedikamente = new List<string>()
        };

        _db.Patienten.Add(patient);
        _db.SaveChanges();

        var messungen = table.Rows
            .Select(row => new BlutdruckMessung
            {
                Datum = DateOnly.ParseExact(row["Datum"], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                Systolisch = int.Parse(row["Systolisch"], CultureInfo.InvariantCulture),
                Diastolisch = int.Parse(row["Diastolisch"], CultureInfo.InvariantCulture)
            })
            .ToList();

        _context["BP.Patient"] = patient;
        _context["BP.Readings"] = messungen;
    }

    [When("der Arzt das Blutdruckdiagramm öffnet")]
    public void WhenTheDoctorOpensTheBloodPressureChart()
    {
        var messungen = _context.Get<List<BlutdruckMessung>>("BP.Readings");
        var service = new PatientenakteService();
        var ergebnis = service.AnalysiereBlutdruckverlauf(messungen);

        _context["BP.Result"] = ergebnis;
    }

    [Then("sollte der durchschnittliche Systolwert {int} betragen")]
    public void ThenTheAverageSystolicShouldBe(int erwartet)
    {
        var ergebnis = _context.Get<BlutdruckAnalyseErgebnis>("BP.Result");
        ergebnis.DurchschnittSystolisch.Should().Be(erwartet);
    }

    [Then("der Trend sollte als {string} markiert sein")]
    public void ThenTheTrendShouldBeMarkedAs(string erwartet)
    {
        var ergebnis = _context.Get<BlutdruckAnalyseErgebnis>("BP.Result");
        ergebnis.Trend.Should().Be(erwartet);
    }

    [Then("ein klinischer Hinweis sollte angezeigt werden: {string}")]
    public void ThenAClinicalAlertShouldBeShown(string erwartet)
    {
        var ergebnis = _context.Get<BlutdruckAnalyseErgebnis>("BP.Result");
        ergebnis.KlinischerHinweis.Should().Be(erwartet);
    }
}
