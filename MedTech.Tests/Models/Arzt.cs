using System.ComponentModel.DataAnnotations;

namespace MedTech.Tests.Infrastructure;

public class Arzt
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Fachrichtung { get; set; } = string.Empty;
    public string Lizenznummer { get; set; } = string.Empty;
}
