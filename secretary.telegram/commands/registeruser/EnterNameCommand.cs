using secretary.storage.models;

namespace secretary.telegram.commands.registeruser;

public class EnterNameCommand : Command
{
    protected override Task ExecuteRoutine()
    {
        return Context.TelegramClient.SendMessage(ChatId, "Введите ваши имя и фамилию в именительном падеже.\r\n" +
                                                          "Так они будут указаны в почтовом ящике, с которого будет отправляться письмо.\r\n" +
                                                          @"Например: <i>Александр Пушкин</i>");
    }

    protected override async Task OnMessageRoutine()
    {
        var user = await Context.UserStorage.GetUser(ChatId);

        user = user ?? new User()
        {
            ChatId = ChatId,
        };

        user.Name = Message;

        await Context.UserStorage.SetUser(user);
    }
}