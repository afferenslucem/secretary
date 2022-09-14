using System.Text.RegularExpressions;

namespace Secretary.Telegram.Utils;

public class StringCleaner
{
    private static readonly Regex Whitespaces = new Regex(@"\s\s+");

    public string Clean(string message)
    {
        var temp = message.Trim();

        temp = Whitespaces.Replace(temp, " ");
        
        return temp;
    }
}