using MedTech.Common.Models;

namespace MedTech.Common.Data;

public static class MedTechDbSeeder
{
    public static void Seed(MedTechDbContext db)
    {
        if (db.Patienten.Any()) return; // Idempotent

        db.Patienten.AddRange(
            new Patient { PatientId = "P001", Name = "Hans Schmidt", Allergien = [] },
            new Patient { PatientId = "P002", Name = "Anna Klein", Allergien = ["Penicillin"] },
            new Patient { PatientId = "P003", Name = "Max Müller", Allergien = [] },
            new Patient { PatientId = "P004", Name = "Maria Schmidt", Allergien = [] }
        );

        db.Aerzte.Add(new Arzt
        {
            Name = "Dr. Müller",
            Fachrichtung = "Allgemeinmedizin",
            Lizenznummer = "DE-2024-001"
        });

        db.SaveChanges();
    }
}