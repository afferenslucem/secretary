using System.Linq;
using System.Text.RegularExpressions;

namespace secretary.documents.utils;

public enum FirstLetter
{
    NotMatter,
    Upper,
    Lower
}

public class InsertStringFormatter
{
    private static readonly Regex WhitespaceRegex = new (@"\s\s+");
    private static readonly char[] SkipPointValues = { '.', '?', '!' };

    public string Format(
        string line, 
        bool extraPoint = true, 
        bool cleanWhitespaces = true, 
        FirstLetter firstLetter = FirstLetter.NotMatter)
    {
        if (line == "") return line;
        
        if (extraPoint)
        {
            line = SkipPointValues.Any(symbol => line.EndsWith(symbol)) ? line : $"{line}.";
        }

        if (cleanWhitespaces)
        {
            line = WhitespaceRegex.Replace(line, " ");
        }

        if (firstLetter == FirstLetter.Upper)
        {
            line = line[0].ToString().ToUpper() + new string(line.Skip(1).ToArray());
        }

        if (firstLetter == FirstLetter.Lower)
        {
            line = line[0].ToString().ToLower() + new string(line.Skip(1).ToArray());
        }
        
        return line;
    }
}