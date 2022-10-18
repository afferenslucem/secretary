using Newtonsoft.Json;

namespace Secretary.Configuration;

public class Config
{
    static Config()
    {
        var data = File.ReadAllText("config.json");

        var config = JsonConvert.DeserializeObject<Config>(data);

        Instance = config ?? throw new JsonException("Wrong config format");

        ValidateConfig();
    }

    public string? Environment { get; init; } = null;
    public string TelegramApiKey { get; init; } = null!;

    public MailConfig MailConfig { get; init; } = null!;

    public JiraConfig JiraConfig { get; init; } = null;

    public string DbConnectionString { get; init; } = null!;

    public string TemplatesPath { get; init; } = null!;
    public string CalendarsPath { get; init; } = null!;

    public string RedisHost { get; init; } = null!;

    public static Config Instance { get; set; }

    private static void ValidateConfig()
    {
        var domains = Instance.MailConfig.AllowedSenderDomains;
        if (domains == null || domains.Count() == 0)
            throw new ApplicationException("Config hasn\'t got any allowed domains");
    }
}