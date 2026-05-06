using MedTech.Common.Data;
using MedTech.Common.Models;

namespace MedTech.Tests.Infrastructure;

public class AuditLogService
{
    private readonly IMedTechDbContext _db;

    public AuditLogService(IMedTechDbContext db) => _db = db;

    public void LogPrescription(
        Arzt arzt,
        Patient patient,
        string medikament,
        string? details,
        string szenario)
    {
        var eintrag = new AuditLogEintrag
        {
            Zeitstempel = DateTime.UtcNow,
            Benutzer = arzt.Name,
            Aktion = "Medikament verschrieben",
            EntityTyp = "Rezept",
            EntityId = 0,
            Änderungen = $"Patient: {patient.Name} ({patient.PatientId}), " +
                         $"Medikament: {medikament}, Szenario: {szenario}" +
                         (details != null ? $", Details: {details}" : "")
        };

        _db.AuditLog.Add(eintrag);
        _db.SaveChanges();
    }
}