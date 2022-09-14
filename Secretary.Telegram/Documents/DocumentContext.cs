namespace Secretary.Telegram.Documents;

public class DocumentContext
{
    public string DisplayName { get; init; }
    public string MailTheme { get; init; }
    public string Key { get; init; }
    
    public DocumentContext(string key, string displayName, string mailTheme)
    {
        Key = key;
        DisplayName = displayName;
        MailTheme = mailTheme;
    }
}