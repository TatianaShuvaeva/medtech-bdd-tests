using System.ComponentModel.DataAnnotations;

namespace MedTech.Tests.Infrastructure;

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
