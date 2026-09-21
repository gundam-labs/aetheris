namespace Aetheris.Web.Interop.Hl7;

public static class Hl7Parser
{
    public static Hl7Message Parse(string raw)
    {
        raw = Mllp.Unwrap(raw).Replace("\r\n", "\r").Replace("\n", "\r").Trim('\r', ' ', '\t');
        if (string.IsNullOrWhiteSpace(raw))
        {
            throw new FormatException("Empty HL7 payload.");
        }

        var lines = raw.Split('\r', StringSplitOptions.RemoveEmptyEntries);
        if (!lines[0].StartsWith("MSH", StringComparison.Ordinal))
        {
            throw new FormatException("HL7 message must start with MSH.");
        }

        var separator = lines[0].Length > 3 ? lines[0][3] : '|';
        var segments = new List<Hl7Segment>(lines.Length);
        foreach (var line in lines)
        {
            var fields = line.Split(separator);
            segments.Add(new Hl7Segment { Name = fields[0], Fields = fields });
        }

        return new Hl7Message { Segments = segments };
    }
}
