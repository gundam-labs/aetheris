using Aetheris.Web.Interop.Hl7;

namespace Aetheris.Tests;

internal static class Hl7Samples
{
    public static string BloodGasOru(string mrn = "HV-100004", string family = "CHEN", string given = "ROBERT", string ph = "7.11", string control = "BG-1001") =>
        string.Join("\r",
            $"MSH|^~\\&|ABL90|ICU|AETHERIS|HARBORVIEW|20260922005500||ORU^R01|{control}|P|2.5",
            $"PID|||{mrn}||{family}^{given}||19550129|M",
            "PV1||I|WARD3B^12",
            "OBR|1||BG-88421|24336-0^Blood gases^LN|||20260922005500",
            $"OBX|1|NM|2744-1^pH^LN||{ph}|[pH]|7.35-7.45|L|||F",
            "OBX|2|NM|2019-8^pCO2^LN||62|mmHg|35-45|H|||F",
            "OBX|3|NM|2703-7^pO2^LN||58|mmHg|80-100|L|||F",
            "OBX|4|NM|1960-4^HCO3^LN||24|mmol/L|22-26|N|||F",
            "OBX|5|NM|32693-1^Lactate^LN||3.1|mmol/L|0.5-2.2|H|||F") + "\r";

    public static string LisCbcOru(string mrn = "HV-100002", string control = "LIS-2001") =>
        string.Join("\r",
            $"MSH|^~\\&|HARBORVIEW-LIS|LAB|AETHERIS|harborview|20260922100000||ORU^R01|{control}|P|2.5",
            $"PID|||{mrn}||OKONKWO^SAMUEL||19591103|M",
            "OBR|1|AE-9001|LAB-4411|57021-8^CBC panel^LN|||20260922093000",
            "OBX|1|NM|718-7^Hemoglobin^LN||9.4|g/dL|13.5-17.5|L|||F",
            "OBX|2|NM|4544-3^Hematocrit^LN||29|%|41-53|L|||F") + "\r";

    public static string WrappedBloodGas() => Mllp.Wrap(BloodGasOru());
}
