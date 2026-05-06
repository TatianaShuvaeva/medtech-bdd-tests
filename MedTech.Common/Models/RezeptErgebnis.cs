namespace MedTech.Common.Models;

// Modelle sind in Models/ ausgelagert (Patient.cs, Arzt.cs, Rezept.cs, AuditLogEintrag.cs, Laborwert.cs)

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