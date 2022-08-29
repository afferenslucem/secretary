using secretary.storage.models;

namespace secretary.telegram.commands.registermail;

public class EnterEmailCommand : Command
{
    protected override Task ExecuteRoutine()
    {
        return Context.TelegramClient.SendMessage(ChatId, "Введите вашу почту, с которой вы отправляете заявления.\r\n" +
                                                          @"Например: <i>a.pushkin@infinnity.ru</i>");
    }

    protected override async Task OnMessageRoutine()
    {
        var user = await Context.UserStorage.GetUser(ChatId);

        user = user ?? new User()
        {
            ChatId = ChatId,
        };

        user.Email = Message;

        await Context.UserStorage.SetUser(user);
    }
}