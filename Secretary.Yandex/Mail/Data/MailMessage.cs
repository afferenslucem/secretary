namespace Secretary.Yandex.Mail.Data;

public class MailMessage
{
    public MailAddress Sender { get; set; } = null!;
    public IEnumerable<MailAddress> Receivers { get; set; } = null!;
    
    public string Token { get; set; } = null!;
    
    public string Theme { get; set; } = null!;
    public string HtmlBody { get; set; } = null!;
    
    public IEnumerable<MessageAttachment> Attachments { get; set; } = null!;
}