namespace Aetheris.Web.Interop.Hl7;

public static class BloodGasProfile
{
    public const string PanelLoinc = "24336-0";
    public const string PanelName = "Blood gases";

    public static readonly IReadOnlyDictionary<string, string> Analytes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["2744-1"] = "pH arterial",
        ["2019-8"] = "pCO2 arterial",
        ["2703-7"] = "pO2 arterial",
        ["1960-4"] = "HCO3 arterial",
        ["11555-0"] = "Base excess",
        ["32693-1"] = "Lactate arterial",
        ["2951-2"] = "Sodium",
        ["2823-3"] = "Potassium",
        ["2069-3"] = "Chloride",
        ["11557-6"] = "Oxygen saturation",
        ["33037-3"] = "Hemoglobin",
    };

    public static readonly HashSet<string> AnalyzerApps = new(StringComparer.OrdinalIgnoreCase)
    {
        "ABL90", "ABL800", "ABL80", "GEM5000", "GEM4000", "RAPIDPOINT", "RAPIDPOINT500", "ISTAT", "EPOC",
    };

    public static bool IsAnalyzer(string sendingApplication) =>
        AnalyzerApps.Contains(sendingApplication.Trim());

    public static bool IsCriticalPh(string value) =>
        double.TryParse(value, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out var ph)
        && (ph < 7.20 || ph > 7.60);
}
