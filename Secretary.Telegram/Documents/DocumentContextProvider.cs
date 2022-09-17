using Secretary.Logging;
using Secretary.Telegram.Commands.Distant;
using Secretary.Telegram.Commands.TimeOff;
using Secretary.Telegram.Commands.Vacation;
using Serilog;

namespace Secretary.Telegram.Documents;

public class DocumentContextProvider
{
    private static ILogger _logger = LogPoint.GetLogger<DocumentContextProvider>();
    
    private static Dictionary<string, DocumentContext> _dictionary = new();

    public static IEnumerable<DocumentContext> AllDocuments => _dictionary.Values;

    static DocumentContextProvider()
    {
        _dictionary.Add(TimeOffCommand.Key, new DocumentContext(TimeOffCommand.Key, "Заявление на отгул.docx", "Отгул"));
        _dictionary.Add(VacationCommand.Key, new DocumentContext(VacationCommand.Key, "Заявление на отпуск.docx", "Отпуск"));
        _dictionary.Add(DistantCommand.Key, new DocumentContext(DistantCommand.Key, "Заявление на удаленную работу.docx", "Удаленная работа"));
    }
    
    private DocumentContextProvider() {}

    public static DocumentContext GetContext(string documentType)
    {
        try
        {
            return _dictionary[documentType];
        }
        catch (KeyNotFoundException e)
        {
            _logger.Error(e, $"Could not find document for \"{documentType}\" key");
            throw;
        }
    }
}