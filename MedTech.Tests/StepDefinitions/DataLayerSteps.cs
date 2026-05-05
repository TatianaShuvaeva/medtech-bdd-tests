using Reqnroll;
using NUnit.Framework;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MedTech.Tests.Infrastructure;

namespace MedTech.Tests.StepDefinitions;

[Binding]
public class DataLayerSteps
{
    private readonly TestDbContext _db;

    public DataLayerSteps(TestDbContext db)
    {
        _db = db;
    }

    
    [Then("sollte das Rezept in der Datenbank persistiert sein")]
    public void DannSollteRezeptInDbPersistiertSein()
    {
        var rezepte = _db.Rezepte.ToList();
        rezepte.Should().NotBeEmpty("Der RezeptService muss das Rezept in der DB persistieren");
    }

    [Then("das persistierte Rezept sollte folgende Felder enthalten:")]
    public void DannSolltePersistiertesRezeptFolgendeFelder(DataTable tabelle)
    {
        var rezept = _db.Rezepte.FirstOrDefault();
        rezept.Should().NotBeNull("Es muss ein Rezept in der DB vorhanden sein");

        foreach (var zeile in tabelle.Rows)
        {
            var feld = zeile["Feld"];
            var erwartet = zeile["Wert"];

            switch (feld)
            {
                case "Medikament":
                    rezept!.Medikament.Should().Be(erwartet,
                        $"Das Feld '{feld}' muss korrekt persistiert sein");
                    break;
                case "Dosierung":
                    rezept!.Dosierung.Should().Be(erwartet,
                        $"Das Feld '{feld}' muss korrekt persistiert sein");
                    break;
                default:
                    Assert.Fail($"Unbekanntes Rezept-Feld in der Tabelle: '{feld}'");
                    break;
            }
        }
    }

    [Then("sollte kein Rezept in der Datenbank persistiert sein")]
    public void DannSollteKeinRezeptInDbPersistiertSein()
    {
        _db.Rezepte.Should().BeEmpty(
            "Bei einer Allergie darf der Service kein Rezept in die DB schreiben");
    }

    
    [Then("sollte das Audit-Log direkt in der DB einen Eintrag mit Aktion {string} enthalten")]
    public void DannSollteAuditLogEintragMitAktionVorhanden(string aktion)
    {
        var eintraege = _db.AuditLog.Where(e => e.Aktion == aktion).ToList();
        eintraege.Should().NotBeEmpty(
            $"Der RezeptService muss einen Audit-Log-Eintrag mit Aktion '{aktion}' in der DB anlegen");
    }

    [Then("der Audit-Log-Eintrag sollte Benutzer {string} mit Lizenznummer {string} protokolliert haben")]
    public void DannSollteAuditLogBenutzerUndLizenzEnthalten(string benutzer, string lizenznummer)
    {
        var eintrag = _db.AuditLog.FirstOrDefault(e =>
            e.Benutzer == benutzer && e.Lizenznummer == lizenznummer);

        eintrag.Should().NotBeNull(
            $"Der Audit-Log muss Benutzer '{benutzer}' mit Lizenznummer '{lizenznummer}' enthalten");
        eintrag!.EntityTyp.Should().Be("Rezept",
            "Der EntityTyp muss 'Rezept' sein (MDR-Anforderung)");
    }

    [Then("sollte kein Audit-Log-Eintrag in der Datenbank vorhanden sein")]
    public void DannSollteKeinAuditLogEintragVorhanden()
    {
        _db.AuditLog.Should().BeEmpty(
            "Bei einer abgelehnten Verschreibung (Allergie) darf kein Audit-Log-Eintrag entstehen");
    }
}
