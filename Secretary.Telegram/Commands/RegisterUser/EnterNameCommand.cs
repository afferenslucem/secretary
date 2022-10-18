using Secretary.Logging;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Commands.Caches.Documents;
using Serilog;

namespace Secretary.Telegram.Commands.RegisterUser;

public class EnterNameCommand : Command
{
    private readonly ILogger _logger = LogPoint.GetLogger<EnterNameCommand>();
    
    public override Task Execute()
    {
        _logger.Information($"{ChatId}: Started register user");
        
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