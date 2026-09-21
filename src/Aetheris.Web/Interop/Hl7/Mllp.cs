using System.Text;

namespace Aetheris.Web.Interop.Hl7;

public static class Mllp
{
    public const byte Start = 0x0B;
    public const byte End = 0x1C;
    public const byte Cr = 0x0D;

    public static string Wrap(string message)
    {
        var body = message.Replace("\r\n", "\r").Replace("\n", "\r").Trim('\r');
        return $"{(char)Start}{body}\r{(char)End}{(char)Cr}";
    }

    public static string Unwrap(string payload)
    {
        if (string.IsNullOrEmpty(payload)) return "";
        var start = payload.IndexOf((char)Start);
        var end = payload.IndexOf((char)End);
        if (start >= 0 && end > start) return payload[(start + 1)..end];
        return payload;
    }

    public static byte[] WrapBytes(string message) => Encoding.ASCII.GetBytes(Wrap(message));
}
