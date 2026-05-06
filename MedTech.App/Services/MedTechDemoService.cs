namespace MedTech.App.Services;

public sealed class MedTechDemoService
{
    private static readonly IReadOnlyList<DemoPatient> Patienten =
    [
        new("P001", "Hans Schmidt", []),
        new("P002", "Anna Klein", ["Penicillin"]),
        new("P003", "Max Müller", []),
        new("P004", "Maria Schmidt", [])
    ];

    public IReadOnlyList<DemoPatient> SuchePatienten(string? suchbegriff)
    {
        if (string.IsNullOrWhiteSpace(suchbegriff))
        {
            return Patienten;
        }

        return Patienten
            .Where(patient => patient.Name.Contains(suchbegriff, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public RezeptPruefung Verschreibe(string patientId, string medikament, string dosierung)
    {
        var patient = Patienten.FirstOrDefault(eintrag => eintrag.PatientId == patientId);
        if (patient is null)
        {
            return new RezeptPruefung(false, "Patient nicht gefunden.", string.Empty, null);
        }

        var penicillinGruppe = new[] { "Amoxicillin", "Ampicillin", "Penicillin", "Oxacillin" };
        bool hatPenicillinAllergie = patient.Allergien
            .Any(allergie => allergie.Equals("Penicillin", StringComparison.OrdinalIgnoreCase));
        bool verschreibtPenicillinArt = penicillinGruppe
            .Any(eintrag => eintrag.Equals(medikament.Trim(), StringComparison.OrdinalIgnoreCase));

        if (hatPenicillinAllergie && verschreibtPenicillinArt)
        {
            return new RezeptPruefung(
                false,
                $"Patient hat Penicillin-Allergie! {medikament} ist ein Penicillin-Derivat.",
                "HOCH",
                null);
        }

        return new RezeptPruefung(
            true,
            null,
            string.Empty,
            $"Rezept für {medikament} ({dosierung}) wurde erfolgreich ausgestellt.");
    }
}

public sealed record DemoPatient(string PatientId, string Name, IReadOnlyList<string> Allergien);

public sealed record RezeptPruefung(
    bool Erfolgreich,
    string? AllergieWarnung,
    string WarnungSchweregrad,
    string? Erfolgsmeldung);