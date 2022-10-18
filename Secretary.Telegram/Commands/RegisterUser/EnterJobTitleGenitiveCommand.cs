using Secretary.Logging;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Commands.Caches.Documents;
using Secretary.Telegram.Exceptions;
using Serilog;

namespace Secretary.Telegram.Commands.RegisterUser;

public class EnterJobTitleGenitiveCommand : Command
{
    private readonly ILogger _logger = LogPoint.GetLogger<EnterJobTitleGenitiveCommand>();
    public override Task Execute()
    {
        return TelegramClient.SendMessage("Введите вашу должность в родительном падеже.\n" +
                                          "Так она будут указана в графе \"от кого\".\n" +
                                          @"Например: От <i>поэта</i> Пушкина Александра Сергеевича");
    }

    public override async Task<int> OnMessage()
    {
        try
        {
            var cache = await CacheService.GetEntity<RegisterUserCache>();
            if (cache == null) throw new InternalException();

            var user = await UserStorage.GetUser();
            user = user ?? new User() { ChatId = ChatId };

            user.Name = cache.Name;
            user.NameGenitive = cache.NameGenitive;
            user.JobTitle = cache.JobTitle;
            user.JobTitleGenitive = Message;

            await UserStorage.SetUser(user);

            await TelegramClient.SendMessage("Ваш пользователь успешно сохранен");

            _logger.Information($"{ChatId}: registered user {user.Name}");

            return ExecuteDirection.RunNext;
        }
        finally
        {
            await CacheService.DeleteEntity<RegisterUserCache>();
        }
    }
}