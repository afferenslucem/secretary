namespace secretary.yandex.mail;

public class SecretaryMailMessage
{
    public SecretaryMailAddress Sender { get; set; }
    public IEnumerable<SecretaryMailAddress> Receivers { get; set; }
    
    public string Token { get; set; }
    
    public string Theme { get; set; }
    public string HtmlBody { get; set; }
    
    public IEnumerable<SecretaryAttachment> Attachments { get; set; }
}