using Secretary.Configuration;
using Secretary.JiraManager;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands.Abstractions;

namespace Secretary.Telegram.Commands.Jira.RegisterJiraToken;

public class RegisterJiraTokenActionCommand : Command
{
    public IJiraReporterFactory JiraReporterFactory = new JiraReporterFactory();
    
    
    
    public override async Task Execute()
    {
        await TelegramClient.SendMessage("Отправьте свой Personal Access Token для JIRA");
    }

    public override async Task<int> OnMessage()
    {
        var reporter = JiraReporterFactory.Create(Config.Instance.JiraConfig.Host, Message);

        var username = await reporter.GetMyUsername();

        var user = await UserStorage.GetUser();

        user ??= new User()
        {
            ChatId = ChatId
        };

        user.JiraPersonalAccessToken = Message;

        await UserStorage.SetUser(user);

        await TelegramClient.SendMessage($"Вы добавили токен для пользователя \"{username}\"");
        await TelegramClient.DeleteMessage(Context.UserMessage.MessageId!.Value);
        
        return ExecuteDirection.RunNext;
    }
}