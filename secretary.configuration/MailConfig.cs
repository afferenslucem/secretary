namespace secretary.configuration;

public class MailConfig
{
    public string ClientSecret { get; init; } = null!;
    public string ClientId { get; init; } = null!;

    public IEnumerable<string> AllowedSenderDomains { get; set; } = null!;
}