using secretary.telegram.exceptions;

namespace secretary.telegram.commands.registeruser;

public class EnterNameGenitiveCommand : Command
{
    protected override Task ExecuteRoutine()
    {
        return Context.TelegramClient.SendMessage(ChatId, "Введите ваши имя и фамилию в родительном падеже.\r\n" +
                                                          "Так они будут указаны в отправоляемом документе в графе \"от кого\".\r\n" +
                                                          @"Например: От <i>Пушкина Александра Сергеевича</i>");
    }

    protected override async Task OnMessageRoutine()
    {
        var user = await Context.UserStorage.GetUser(ChatId);

        if (user == null) throw new InternalException();
        
        user.NameGenitive = Message;

        await Context.UserStorage.UpdateUser(user);
    }
}