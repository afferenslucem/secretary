using secretary.telegram.exceptions;

namespace secretary.telegram.commands.exceptionHandlers;

public class NonCompleteUserExceptionHandlerVisitor
{
    public async Task Handle(NonCompleteUserException e, long chatId, ITelegramClient client)
    {
        if (e.Message == "User does not exist")
        {
            await client.SendMessage(chatId, "Вы – незарегистрированный пользователь.\r\n\r\n" +
                                             "Выполните команды:\r\n" +
                                             "/registeruser\r\n" +
                                             "/registermail");
        }
        if (e.Message == "User has not got registered email")
        {
            await client.SendMessage(chatId, "У вас не зарегистрирована почта.\r\n" +
                                             "Выполните команду: /registermail");
        }
        if (e.Message == "User has not got personal info")
        {
            await client.SendMessage(chatId, "У вас не заданы данные о пользователе.\r\n" +
                                             "Выполните команду /registeruser");
        }
    }
}