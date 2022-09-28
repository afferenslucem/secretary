using MimeKit;

namespace Secretary.Yandex.Mail.Data;

public class MessageAttachment
{
    public MessageAttachment(string path, string fileName, ContentType contentType)
    {
        Path = path;
        FileName = fileName;
        ContentType = contentType;
    }

    public string Path { get; set; }
    public string FileName { get; set; }
    public ContentType ContentType { get; set; }
}