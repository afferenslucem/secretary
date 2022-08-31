using secretary.telegram.exceptions;

namespace secretary.telegram.commands.registeruser;

public class EnterNameGenitiveCommand : Command
{
    public override Task Execute()
    {
        return Context.TelegramClient.SendMessage(ChatId, "Введите ваши имя и фамилию в родительном падеже.\r\n" +
                                                          "Так они будут указаны в отправоляемом документе в графе \"от кого\".\r\n" +
                                                          @"Например: От <i>Пушкина Александра Сергеевича</i>");
    }

    public override async Task<int> OnMessage()
    {
        var user = await Context.UserStorage.GetUser(ChatId);

        if (user == null) throw new InternalException();
        
        user.NameGenitive = Message;

        await Context.UserStorage.UpdateUser(user);
        
        return RunNext;
    }
}