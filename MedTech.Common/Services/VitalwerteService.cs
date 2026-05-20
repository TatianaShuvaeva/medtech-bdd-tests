namespace MedTech.Common.Services;

public class VitalwerteService
{
    public string ValidiereVitalwert(string vitalwert, string messwert, string einheit)
    {
        return vitalwert switch
        {
            "Blutdruck" => ValidiereBlutdruck(messwert),
            "Puls" => ValidierePuls(int.Parse(messwert)),
            "Sauerstoffsättigung" => ValidiereSauerstoff(int.Parse(messwert)),
            _ => throw new ArgumentException($"Unbekannter Vitalwert: {vitalwert}")
        };
    }

    private static string ValidiereBlutdruck(string messwert)
    {
        var teile = messwert.Split('/');
        var systolisch = int.Parse(teile[0]);
        var diastolisch = int.Parse(teile[1]);

        if (systolisch >= 180 || diastolisch >= 110)
            return "KRITISCH — Hypertoniekrise";

        return "NORMAL";
    }

    private static string ValidierePuls(int puls)
    {
        if (puls >= 100)
            return "WARNUNG — Tachykardie";

        return "NORMAL";
    }

    private static string ValidiereSauerstoff(int saettigung)
    {
        if (saettigung < 90)
            return "KRITISCH — Hypoxie";

        return "NORMAL";
    }
}
