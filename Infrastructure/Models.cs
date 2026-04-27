using System.ComponentModel.DataAnnotations;

namespace MedTech.Tests.Infrastructure;

public class Arzt
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Fachrichtung { get; set; } = string.Empty;
}

public class Patient
{
    [Key]
    public int Id { get; set; }
    public string PatientId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    // Werden in TestDbContext als CSV konvertiert
    public List<string> Allergien { get; set; } = new();
    public List<string> AktiveMedikamente { get; set; } = new();
}

public class Rezept
{
    [Key]
    public int Id { get; set; }
    public int PatientIdFk { get; set; }
    public int ArztIdFk { get; set; }
    public string Medikament { get; set; } = string.Empty;
    public string? Dosierung { get; set; }
    public DateTime ErstelltAmUtc { get; set; }
    public string PdfPfad { get; set; } = string.Empty;
}

public class RezeptErgebnis
{
    public bool ErfolgreichGespeichert { get; set; }
    public string? Warnung { get; set; }
    public string WarnungSchweregrad { get; set; } = string.Empty;
    public string Vorschlag { get; set; } = string.Empty;
    public string PdfPfad { get; set; } = string.Empty;
}

public class BlutdruckMessung
{
    public DateOnly Datum { get; set; }
    public int Systolisch { get; set; }
    public int Diastolisch { get; set; }
}

public class BlutdruckAnalyseErgebnis
{
    public int DurchschnittSystolisch { get; set; }
    public string Trend { get; set; } = string.Empty;
    public string KlinischerHinweis { get; set; } = string.Empty;
}