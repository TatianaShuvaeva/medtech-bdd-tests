using System.ComponentModel.DataAnnotations;

namespace MedTech.Tests.Infrastructure;

/// <summary>
/// MDR-konformer Audit-Log-Eintrag: jede klinisch relevante Aktion wird hier protokolliert.
/// </summary>
public class AuditLogEintrag
{
    [Key]
    public int Id { get; set; }
    public string Aktion { get; set; } = string.Empty;
    public string Benutzer { get; set; } = string.Empty;
    public string EntityTyp { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string Lizenznummer { get; set; } = string.Empty;   // MDR: Arztlizenz für Audit-Pflicht
    public string Änderungen { get; set; } = string.Empty;
    public DateTime Zeitstempel { get; set; }
}
