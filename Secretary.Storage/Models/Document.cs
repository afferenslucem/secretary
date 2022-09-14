namespace Secretary.Storage.Models;

public class Document
{
    public long Id { get; set; }
    public long ChatId { get; set; }
    public string DocumentName { get; set; } = null!;

    public Document(long chatId, string documentName)
    {
        ChatId = chatId;
        DocumentName = documentName;
    }
    
    public Document() {}
}