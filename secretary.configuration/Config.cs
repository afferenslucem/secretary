using System.Text.Json;

namespace secretary.configuration;

public class Config
{
    public string TelegramApiKey { get; set; } = null!;

    public MailConfig MailConfig { get; set; } = null!;
    
    public string DbPath { get; set; } = null!;
    
    public string TemplatesPath { get; set; } = null!;

    public static Config Instance { get; set; }

    static Config()
    {
        var data = File.ReadAllText("config.json");

        var config = JsonSerializer.Deserialize<Config>(data);

        Instance = config ?? throw new JsonException("Wrong config format");
    }
}