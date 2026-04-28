namespace MedTech.Tests.Infrastructure;

public class AuditLogService
{
    private readonly TestDbContext _db;

    public AuditLogService(TestDbContext db)
    {
        _db = db;
    }

    public void LogPrescription(
        Arzt arzt,
        Patient patient,
        string medikament,
        string? details,
        string szenario)
    {
        var eintrag = new AuditLog
        {
            ZeitpunktUtc = DateTime.UtcNow,
            ArztName = arzt.Name,
            Aktion = "Medikament verschrieben",
            PatientName = patient.Name,
            PatientId = patient.PatientId,
            Medikament = medikament,
            Details = details,
            Szenario = szenario
        };

        _db.AuditLogs.Add(eintrag);
        _db.SaveChanges();
    }
}