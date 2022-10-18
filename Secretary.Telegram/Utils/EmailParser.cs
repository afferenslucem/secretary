using System.Text.RegularExpressions;
using Secretary.Storage.Models;
using Secretary.Telegram.Exceptions;

namespace Secretary.Telegram.Utils;

public class EmailParser
{
    private Regex _emailRegex = new(@"^[\w_\-\.]+@([\w\-_]+\.)+[\w-]{2,4}");
    private Regex _nameRegex = new(@"\([\w-]+\s+[\w-]+(\s[\w-]+)?\)");
    private Regex _spaceSequences = new(@"\s\s+|\t");
    
    public IEnumerable<Email> ParseMany(string message)
    {
        var lines = message.Split("\n")
            .Select(item => item.Trim());

        var result = lines.Select(item => Parse(item)).ToArray();

        return result;
    }

    public Email Parse(string message)
    {
        var cleanString = _spaceSequences.Replace(message, " ");
        
        var addressMatch = _emailRegex.Match(cleanString);
        
        if (!addressMatch.Success)
        {
            throw new IncorrectEmailException(message, $"Address \"{message}\" has invalid format");
        }

        var nameMatch = _nameRegex.Match(message);

        var name = nameMatch.Success ? nameMatch.Value.Trim(new[] { '(', ')' }) : null;

        return new Email(addressMatch.Value, name);
    }
}