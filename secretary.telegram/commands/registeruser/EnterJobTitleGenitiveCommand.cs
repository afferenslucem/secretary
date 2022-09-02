using Microsoft.Extensions.Logging;
using secretary.logging;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands.registeruser;

public class EnterJobTitleGenitiveCommand : Command
{
    private readonly ILogger<EnterJobTitleGenitiveCommand> _logger = LogPoint.GetLogger<EnterJobTitleGenitiveCommand>();
    public override Task Execute()
    {
        return Context.TelegramClient.SendMessage(ChatId, "Введите вашу должность в родительном падеже.\r\n" +
                                                          "Так она будут указана в графе \"от кого\".\r\n" +
                                                          @"Например: От <i>поэта</i> Пушкина Александра Сергеевича");
    }

    public override async Task<int> OnMessage()
    {
        var user = await Context.UserStorage.GetUser(ChatId);

        if (user == null) throw new InternalException();
        
        user.JobTitleGenitive = Message;

        await Context.UserStorage.UpdateUser(user);

        await Context.TelegramClient.SendMessage(ChatId, "Ваш пользователь успешно сохранен");
        
        _logger.LogInformation($"{ChatId}: registered user {user.Name}");

        return RunNext;
    }
}