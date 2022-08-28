using MimeKit;

namespace secretary.yandex.mail;

public class SecretaryAttachment
{
    public string Path { get; set; }
    public string FileName { get; set; }
    public ContentType ContentType { get; set; }
}