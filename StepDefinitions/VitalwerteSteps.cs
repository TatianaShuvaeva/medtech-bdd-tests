using Reqnroll;
using FluentAssertions;
using MedTech.Tests.Infrastructure;

namespace MedTech.Tests.StepDefinitions;

[Binding]
public class VitalwerteSteps
{
    private readonly VitalwerteService _vitalwerteService = new();
    private string _klinischerStatus = string.Empty;

    [When("der Vitalwert {string} mit dem Messwert {string} in {string} erfasst wird")]
    public void WennVitalwertErfasst(string vitalwert, string messwert, string einheit)
    {
        _klinischerStatus = _vitalwerteService.ValidiereVitalwert(vitalwert, messwert, einheit);
    }

    [Then("sollte der klinische Status {string} lauten")]
    public void DannSollteKlinischerStatusSein(string erwarteterStatus)
    {
        _klinischerStatus.Should().Be(erwarteterStatus);
    }
}
