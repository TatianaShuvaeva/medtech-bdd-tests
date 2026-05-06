using System.ComponentModel.DataAnnotations;

namespace MedTech.Common.Models;

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
