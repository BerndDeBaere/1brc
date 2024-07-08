using System.Text;

namespace _1brc;

public static class SpanHelper
{
    public static string Readable(this Span<byte> input) => Encoding.UTF8.GetString(input);
}