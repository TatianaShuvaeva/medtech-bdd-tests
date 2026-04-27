using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MedTech.Tests.Infrastructure;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

    public DbSet<Arzt> Aerzte => Set<Arzt>();
    public DbSet<Patient> Patienten => Set<Patient>();
    public DbSet<Rezept> Rezepte => Set<Rezept>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var listToStringConverter = new ValueConverter<List<string>, string>(
            v => string.Join(";", v),
            v => string.IsNullOrWhiteSpace(v)
                ? new List<string>()
                : v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList());

        var listComparer = new ValueComparer<List<string>>(
            (a, b) => (a ?? new()).SequenceEqual(b ?? new()),
            v => v.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
            v => v.ToList());

        modelBuilder.Entity<Patient>()
            .Property(p => p.Allergien)
            .HasConversion(listToStringConverter)
            .Metadata.SetValueComparer(listComparer);

        modelBuilder.Entity<Patient>()
            .Property(p => p.AktiveMedikamente)
            .HasConversion(listToStringConverter)
            .Metadata.SetValueComparer(listComparer);

        base.OnModelCreating(modelBuilder);
    }
}