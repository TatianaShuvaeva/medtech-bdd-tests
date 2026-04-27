namespace MedTech.Tests.Infrastructure;

public class PatientenakteService
{
    public BlutdruckAnalyseErgebnis AnalysiereBlutdruckverlauf(IReadOnlyCollection<BlutdruckMessung> messungen)
    {
        if (messungen == null || messungen.Count == 0)
        {
            throw new ArgumentException("Mindestens eine Blutdruckmessung ist erforderlich.", nameof(messungen));
        }

        var sortiert = messungen
            .OrderBy(m => m.Datum)
            .ToList();

        var durchschnittSystolisch = (int)sortiert.Average(m => m.Systolisch);
        var durchschnittDiastolisch = sortiert.Average(m => m.Diastolisch);

        var trend = sortiert[^1].Systolisch > sortiert[0].Systolisch
            ? "RISING"
            : sortiert[^1].Systolisch < sortiert[0].Systolisch
                ? "FALLING"
                : "STABLE";

        var klinischerHinweis = durchschnittSystolisch >= 140 || durchschnittDiastolisch >= 90
            ? "Hypertension Stage 1"
            : "No clinical alert";

        return new BlutdruckAnalyseErgebnis
        {
            DurchschnittSystolisch = durchschnittSystolisch,
            Trend = trend,
            KlinischerHinweis = klinischerHinweis
        };
    }
}
