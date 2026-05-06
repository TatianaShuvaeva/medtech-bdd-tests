using System.ComponentModel.DataAnnotations;

namespace MedTech.Common.Models;

public class Laborwert
{
    [Key]
    public int Id { get; set; }
    public int PatientIdFk { get; set; }
    public string Parameter { get; set; } = string.Empty;
    public string Wert { get; set; } = string.Empty;
    public string Einheit { get; set; } = string.Empty;
    public DateTime GemessenAmUtc { get; set; }
}
