using System.Text.RegularExpressions;
using secretary.storage.models;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands.registermail;

public class EnterEmailCommand : Command
{
    public override Task Execute()
    {
        return Context.TelegramClient.SendMessage(ChatId, "Введите вашу почту, с которой вы отправляете заявления.\r\n" +
                                                          @"Например: <i>a.pushkin@infinnity.ru</i>");
    }

    public override async Task<int> OnMessage()
    {
        var user = await Context.UserStorage.GetUser(ChatId);

        user = user ?? new User()
        {
            ChatId = ChatId,
        };

        user.Email = Message;

        await Context.UserStorage.SetUser(user);
        
        return RunNext;
    }

    public override async Task ValidateMessage()
    {
        var emailRegex = new Regex(@"^[\w_\-\.]+@([\w\-_]+\.)+[\w-]{2,4}");

        if (!emailRegex.IsMatch(Message))
        {
            await this.Context.TelegramClient.SendMessage(ChatId, "Некорректный формат почты. Введите почту еще раз");
            throw new IncorrectFormatException();
        }
    }
}