using Microsoft.Extensions.Logging;
using secretary.logging;
using secretary.storage.models;

namespace secretary.telegram.commands.registeruser;

public class EnterNameCommand : Command
{
    private readonly ILogger<EnterNameCommand> _logger = LogPoint.GetLogger<EnterNameCommand>();
    
    public override Task Execute()
    {
        _logger.LogInformation($"{ChatId}: Started register user");
        
        return Context.TelegramClient.SendMessage(ChatId, "Введите ваши имя и фамилию в именительном падеже.\r\n" +
                                                          "Так они будут указаны в почтовом ящике, с которого будет отправляться письмо.\r\n" +
                                                          @"Например: <i>Александр Пушкин</i>");
    }

    public override async Task<int> OnMessage()
    {
        var user = await Context.UserStorage.GetUser(ChatId);

        user = user ?? new User()
        {
            ChatId = ChatId,
        };

        user.Name = Message;

        await Context.UserStorage.SetUser(user);
        
        return RunNext;
    }
}