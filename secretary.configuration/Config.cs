using System.IO;
using System.Text.Json;

namespace secretary.configuration;

public class Config
{
    public string TelegramApiKey { get; set; }

    public MailConfig MailConfig { get; set; }
    
    public string DbPath { get; set; }
    
    public string TemplatesPath { get; set; }

    public static Config Instance { get; set; }

    static Config()
    {
        var data = File.ReadAllText("config.json");

        Instance = JsonSerializer.Deserialize<Config>(data);
    }
}