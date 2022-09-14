namespace Secretary.Yandex.Mail;

public class SecretaryMailMessage
{
    public SecretaryMailAddress Sender { get; set; } = null!;
    public IEnumerable<SecretaryMailAddress> Receivers { get; set; } = null!;
    
    public string Token { get; set; } = null!;
    
    public string Theme { get; set; } = null!;
    public string HtmlBody { get; set; } = null!;
    
    public IEnumerable<SecretaryAttachment> Attachments { get; set; } = null!;
}