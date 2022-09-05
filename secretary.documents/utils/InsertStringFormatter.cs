using System.Linq;

namespace secretary.documents.utils;

public class InsertStringFormatter
{
    private static readonly char[] SkipPointValues = { '.', '?', '!' };

    public string Format(string line)
    {
        return SkipPointValues.Any(symbol => line.EndsWith(symbol)) ? line : $"{line}.";
    }
}