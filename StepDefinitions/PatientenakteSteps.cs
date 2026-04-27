using System.Globalization;
using FluentAssertions;
using MedTech.Tests.Infrastructure;
using Reqnroll;

namespace MedTech.Tests.StepDefinitions;

[Binding]
public class PatientenakteSteps
{
    private readonly ScenarioContext _context;
    private readonly TestDbContext _db;

    public PatientenakteSteps(ScenarioContext context, TestDbContext db)
    {
        _context = context;
        _db = db;
    }

    [Given("patient {string} has blood pressure readings:")]
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
                Datum = DateOnly.ParseExact(row["Date"], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                Systolisch = int.Parse(row["Systolic"], CultureInfo.InvariantCulture),
                Diastolisch = int.Parse(row["Diastolic"], CultureInfo.InvariantCulture)
            })
            .ToList();

        _context["BP.Patient"] = patient;
        _context["BP.Readings"] = messungen;
    }

    [When("the doctor opens the blood pressure chart")]
    public void WhenTheDoctorOpensTheBloodPressureChart()
    {
        var messungen = _context.Get<List<BlutdruckMessung>>("BP.Readings");
        var service = new PatientenakteService();
        var ergebnis = service.AnalysiereBlutdruckverlauf(messungen);

        _context["BP.Result"] = ergebnis;
    }

    [Then("the average systolic should be {int}")]
    public void ThenTheAverageSystolicShouldBe(int erwartet)
    {
        var ergebnis = _context.Get<BlutdruckAnalyseErgebnis>("BP.Result");
        ergebnis.DurchschnittSystolisch.Should().Be(erwartet);
    }

    [Then("the trend should be marked as {string}")]
    public void ThenTheTrendShouldBeMarkedAs(string erwartet)
    {
        var ergebnis = _context.Get<BlutdruckAnalyseErgebnis>("BP.Result");
        ergebnis.Trend.Should().Be(erwartet);
    }

    [Then("a clinical alert should be shown: {string}")]
    public void ThenAClinicalAlertShouldBeShown(string erwartet)
    {
        var ergebnis = _context.Get<BlutdruckAnalyseErgebnis>("BP.Result");
        ergebnis.KlinischerHinweis.Should().Be(erwartet);
    }
}
