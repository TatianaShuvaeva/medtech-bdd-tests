using System.Net.Http.Json;
using FluentAssertions;
using MedTech.Tests.Pages;
using Reqnroll;

namespace MedTech.Tests.StepDefinitions;

[Binding]
public class UIDataLayerSteps
{
    private static readonly string AppUrl =
        Environment.GetEnvironmentVariable("MEDTECH_UI_BASE_URL")?.TrimEnd('/')
        ?? "http://localhost:3000";

    private static readonly HttpClient Http = new();

    private readonly PatientenlistePage _patientenlistePage;
    private readonly RezeptPage _rezeptPage;
    private readonly ScenarioContext _context;

    public UIDataLayerSteps(
        PatientenlistePage patientenlistePage,
        RezeptPage rezeptPage,
        ScenarioContext context)
    {
        _patientenlistePage = patientenlistePage;
        _rezeptPage = rezeptPage;
        _context = context;
    }


    [Given("der API-Zustand wurde zurückgesetzt")]
    public async Task GegebenApiZustandZurueckgesetzt()
    {
        var response = await Http.DeleteAsync($"{AppUrl}/api/rezepte/reset");
        response.IsSuccessStatusCode.Should().BeTrue(
            "Der Reset-Endpunkt muss erreichbar sein (ist die App gestartet?)");
    }

    [Given("Patient {string} ist in der Patientenliste ausgewählt")]
    public async Task GegebenPatientAusgewaehlt(string name)
    {
        _context["GesuchterPatient"] = name;
        await _patientenlistePage.SucheNachPatient(name);
        await _patientenlistePage.ÖffnePatientenakte(name);
    }


    [Then("der API-Endpunkt {string} sollte ein Rezept für Patient {string} enthalten")]
    public async Task DannSollteApiRezeptFuerPatientEnthalten(string endpunkt, string patientName)
    {
        var rezepte = await Http.GetFromJsonAsync<List<ApiRezept>>($"{AppUrl}{endpunkt}");
        rezepte.Should().NotBeNull();
        rezepte!.Should().Contain(r => r.PatientName == patientName,
            $"Nach einer erfolgreichen UI-Verschreibung muss '{patientName}' im API-Endpunkt erscheinen");
    }

    [Then("der API-Endpunkt {string} sollte kein Rezept für Patient {string} enthalten")]
    public async Task DannSollteApiKeinRezeptFuerPatientEnthalten(string endpunkt, string patientName)
    {
        var rezepte = await Http.GetFromJsonAsync<List<ApiRezept>>($"{AppUrl}{endpunkt}");
        rezepte.Should().NotBeNull();
        rezepte!.Should().NotContain(r => r.PatientName == patientName,
            $"Bei einer abgewiesenen Verschreibung darf '{patientName}' NICHT im API-Endpunkt erscheinen");
    }


    private sealed record ApiRezept(
        string PatientId,
        string PatientName,
        string Medikament,
        string Dosierung,
        DateTime AusgestelltAmUtc);
}
