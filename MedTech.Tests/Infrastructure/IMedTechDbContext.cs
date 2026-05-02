using Microsoft.EntityFrameworkCore;

namespace MedTech.Tests.Infrastructure;

/// <summary>
/// Dependency Inversion: RezeptService hängt von dieser Abstraktion ab,
/// nicht von der konkreten EF-Core-Klasse.
/// TestDbContext (InMemory) implementiert dieses Interface für Tests.
/// MedTechDbContext (SQL Server) würde es für Produktion implementieren.
/// </summary>
public interface IMedTechDbContext
{
    DbSet<Patient> Patienten { get; }
    DbSet<Arzt> Aerzte { get; }
    DbSet<Rezept> Rezepte { get; }
    DbSet<Laborwert> Laborwerte { get; }
    DbSet<AuditLogEintrag> AuditLog { get; }
    int SaveChanges();
}
