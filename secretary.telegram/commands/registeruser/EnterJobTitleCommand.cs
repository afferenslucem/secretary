using secretary.telegram.exceptions;

namespace secretary.telegram.commands.registeruser;

public class EnterJobTitleCommand : Command
{
    protected override Task ExecuteRoutine()
    {
        return Context.TelegramClient.SendMessage(ChatId, "Введите вашу должность в именительном падеже.\r\n" +
                                                          "Так она будут указана в подписи письма.\r\n" +
                                                          @"Например: С уважением, <i>поэт</i> Александр Пушкин");
    }

    protected override async Task OnMessageRoutine()
    {
        var user = await Context.UserStorage.GetUser(ChatId);

        if (user == null) throw new InternalException();
        
        user.JobTitle = Message;

        await Context.UserStorage.UpdateUser(user);
    }
}