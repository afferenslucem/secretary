using Microsoft.Extensions.Logging;
using secretary.logging;
using secretary.storage.models;
using secretary.telegram.commands.caches;

namespace secretary.telegram.commands.registeruser;

public class EnterNameCommand : Command
{
    private readonly ILogger<EnterNameCommand> _logger = LogPoint.GetLogger<EnterNameCommand>();
    
    public override Task Execute()
    {
        _logger.LogInformation($"{ChatId}: Started register user");
        
        return TelegramClient.SendMessage("Введите ваши имя и фамилию в именительном падеже.\n" +
                                          "Так они будут указаны в почтовом ящике, с которого будет отправляться письмо.\n" +
                                          @"Например: <i>Александр Пушкин</i>");
    }

    public override async Task<int> OnMessage()
    {
        var cache = new RegisterUserCache();
        cache.Name = Message;

        await CacheService.SaveEntity(cache);
        
        return ExecuteDirection.RunNext;
    }
}