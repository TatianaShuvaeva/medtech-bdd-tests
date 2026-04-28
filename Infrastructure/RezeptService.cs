namespace MedTech.Tests.Infrastructure;

public class RezeptService
{
    private readonly TestDbContext _db;

    public RezeptService(TestDbContext db)
    {
        _db = db;
    }

    public RezeptErgebnis VerschreibeMedikament(Patient patient, Arzt arzt, string medikament, string? dosierung)
    {
        // Allergie-Check (Kurs-Szenario: Penicillin -> Amoxicillin)
        if (patient.Allergien.Any(a => a.Equals("Penicillin", StringComparison.OrdinalIgnoreCase)) &&
            medikament.Contains("Amoxicillin", StringComparison.OrdinalIgnoreCase))
        {
            return new RezeptErgebnis
            {
                ErfolgreichGespeichert = false,
                Warnung = "Patient ist allergisch gegen Penicillin-Klasse-Antibiotika",
                WarnungSchweregrad = "HOCH"
            };
        }

        // Interaktions-Check (Kurs-Szenario: Warfarin + Aspirin)
        if (patient.AktiveMedikamente.Any(m => m.Equals("Warfarin", StringComparison.OrdinalIgnoreCase)) &&
            medikament.Contains("Aspirin", StringComparison.OrdinalIgnoreCase))
        {
            return new RezeptErgebnis
            {
                ErfolgreichGespeichert = false,
                Warnung = "Erhöhtes Blutungsrisiko mit Warfarin",
                WarnungSchweregrad = "HOCH",
                Vorschlag = "Alternative in Betracht ziehen: Paracetamol"
            };
        }

        // Erfolgreich speichern
        if (!patient.AktiveMedikamente.Contains(medikament, StringComparer.OrdinalIgnoreCase))
        {
            patient.AktiveMedikamente.Add(medikament);
        }

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
        _db.SaveChanges();

        return new RezeptErgebnis
        {
            ErfolgreichGespeichert = true,
            PdfPfad = pdfPfad
        };
    }
}