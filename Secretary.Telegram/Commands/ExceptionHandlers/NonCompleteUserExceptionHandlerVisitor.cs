using Secretary.Telegram;
using Secretary.Telegram.Exceptions;

namespace Secretary.Telegram.Commands.ExceptionHandlers;

public class NonCompleteUserExceptionHandlerVisitor
{
    public async Task Handle(NonCompleteUserException e, long chatId, ITelegramClient client)
    {
        if (e.Message == "User does not exist")
        {
            await client.SendMessage(chatId, "Вы – незарегистрированный пользователь.\n\n" +
                                             "Выполните команды:\n" +
                                             "/registeruser\n" +
                                             "/registermail");
        }
        if (e.Message == "User has not got registered email")
        {
            await client.SendMessage(chatId, "У вас не зарегистрирована почта.\n" +
                                             "Выполните команду: /registermail");
        }
        if (e.Message == "User has not got personal info")
        {
            await client.SendMessage(chatId, "У вас не заданы данные о пользователе.\n" +
                                             "Выполните команду /registeruser");
        }
    }
}