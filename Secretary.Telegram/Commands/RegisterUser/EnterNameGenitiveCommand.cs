using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Exceptions;

namespace Secretary.Telegram.Commands.RegisterUser;

public class EnterNameGenitiveCommand : Command
{
    public override Task Execute()
    {
        return TelegramClient.SendMessage("Введите ваши ФИО в родительном падеже.\n" +
                                          "Так они будут указаны в отправляемом документе в графе \"от кого\".\n" +
                                          @"Например: От <i>Пушкина Александра Сергеевича</i>");
    }

    public override async Task<int> OnMessage()
    {
        var cache = await CacheService.GetEntity<RegisterUserCache>();
        if (cache == null) throw new InternalException();
        
        cache.NameGenitive = Message;

        await CacheService.SaveEntity(cache);
        
        return ExecuteDirection.RunNext;
    }
}