namespace MedTech.Tests.Infrastructure;

public class RezeptService
{
    private readonly IMedTechDbContext _db;

    // Bekannte Kreuzallergien (Allergen → betroffene Medikamente)
    private readonly Dictionary<string, string[]> _kreuzallergien = new()
    {
        ["Penicillin"] = ["Amoxicillin", "Ampicillin", "Piperacillin"],
        ["Sulfonamide"] = ["Trimethoprim-Sulfamethoxazol"]
    };

    // Bekannte Wechselwirkungen (aktives Medikament → gefährliche Kombinationen + Meldung)
    private readonly Dictionary<string, (string[] Medikamente, string Warnung, string Vorschlag)> _interaktionen = new()
    {
        ["Warfarin"] = (["Aspirin", "Ibuprofen", "Naproxen"],
            "Erhöhtes Blutungsrisiko mit Warfarin",
            "Alternative in Betracht ziehen: Paracetamol"),
        ["Metformin"] = (["Alkohol", "Röntgenkontrastmittel"],
            "Erhöhte Laktatazidose-Gefahr mit Metformin", ""),
        ["MAO-Hemmer"] = (["SSRI", "Tramadol", "Triptane"],
            "Gefahr eines Serotoninsyndroms", "")
    };

    public RezeptService(IMedTechDbContext db) => _db = db;

    public RezeptErgebnis VerschreibeMedikament(
        Patient patient, Arzt arzt, string medikament, string? dosierung)
    {
        // 1. Allergie-Check
        foreach (var allergen in patient.Allergien)
        {
            if (_kreuzallergien.TryGetValue(allergen, out var kreuzallergene) &&
                kreuzallergene.Any(k => medikament.Contains(k, StringComparison.OrdinalIgnoreCase)))
            {
                return new RezeptErgebnis
                {
                    ErfolgreichGespeichert = false,
                    Warnung = $"Patient ist allergisch gegen {allergen}-Klasse-Antibiotika",
                    WarnungSchweregrad = "HOCH"
                };
            }
        }

        // 2. Wechselwirkungscheck
        foreach (var aktivMedikament in patient.AktiveMedikamente)
        {
            if (_interaktionen.TryGetValue(aktivMedikament, out var interaktion) &&
                interaktion.Medikamente.Any(m => medikament.Contains(m, StringComparison.OrdinalIgnoreCase)))
            {
                return new RezeptErgebnis
                {
                    ErfolgreichGespeichert = false,
                    Warnung = interaktion.Warnung,
                    WarnungSchweregrad = "HOCH",
                    Vorschlag = interaktion.Vorschlag
                };
            }
        }

        // 3. Patient aktualisieren (aktive Medikamente)
        if (!patient.AktiveMedikamente.Contains(medikament, StringComparer.OrdinalIgnoreCase))
            patient.AktiveMedikamente.Add(medikament);

        var pdfPfad = $@"C:\Temp\rezepte\rezept_{patient.PatientId}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";

        var rezept = new Rezept
        {
            PatientIdFk = patient.Id,
            ArztIdFk = arzt.Id,
            Medikament = medikament,
            Dosierung = dosierung,
            ErstelltAmUtc = DateTime.UtcNow,
            PdfPfad = pdfPfad
        };

        _db.Patienten.Update(patient);
        _db.Rezepte.Add(rezept);
        _db.SaveChanges(); // rezept.Id wird hier gesetzt

        // 4. Audit-Log (MDR-Pflicht: jede Verschreibung muss protokolliert werden)
        _db.AuditLog.Add(new AuditLogEintrag
        {
            Aktion = "REZEPT_ERSTELLT",
            Benutzer = arzt.Name,
            Lizenznummer = arzt.Lizenznummer,
            EntityTyp = "Rezept",
            EntityId = rezept.Id,
            Änderungen = $"Medikament: {medikament}, Dosierung: {dosierung}",
            Zeitstempel = DateTime.UtcNow
        });
        _db.SaveChanges();

        return new RezeptErgebnis
        {
            ErfolgreichGespeichert = true,
            PdfPfad = pdfPfad
        };
    }
}