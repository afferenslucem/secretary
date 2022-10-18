using System.Text.RegularExpressions;
using Secretary.Configuration;
using Secretary.JiraManager;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Commands.Caches.Jira;
using Secretary.Telegram.Commands.Formatters;

namespace Secretary.Telegram.Commands.Jira.LogTime;

public class LogTimeActionCommand: Command
{
    public IJiraReporterFactory JiraReporterFactory;

    public LogTimeActionCommand()
    {
        JiraReporterFactory = new JiraReporterFactory();
    }
    
    public override async Task Execute()
    {
        var cache = new LogTimeCache();

        cache.IssueKey = Context.UserMessage.CommandArgument;

        await CacheService.SaveEntity(cache);

        await TelegramClient.SendMessage($"Логгирование времени в задачу <a href=\"{IssueFormatter.GetIssueLink(cache.IssueKey)}\">{cache.IssueKey}</a>\n\n" +
                                         $"Введите время в формате <i>d</i>h <i>dd</i>m для логгирования с минутами\n" +
                                         $"Пример: <i>1h 30m</i>\n\n" +
                                         $"Введите просто число для логгирования часов\n" +
                                         $"Пример: <i>1</i>");
    }
    
    public override async Task<int> OnMessage()
    {
        var cache = await CacheService.GetEntity<LogTimeCache>();

        var time = ReadTime();
        
        var user = await UserStorage.GetUser();
        var reporter = JiraReporterFactory.Create(Config.Instance.JiraConfig.Host, user.JiraPersonalAccessToken);
        
        await reporter.LogTime(cache.IssueKey, time);

        await TelegramClient.SendMessage($"Вы залоггировали {time} в <a href=\"{IssueFormatter.GetIssueLink(cache.IssueKey)}\">{cache.IssueKey}</a>");

        return ExecuteDirection.RunNext;
    }

    public string ReadTime()
    {
        int hours;

        if (int.TryParse(Context.Message, out hours))
        {
            return $"{hours}h";
        }
        else
        {
            return Context.Message;
        }
    }
}