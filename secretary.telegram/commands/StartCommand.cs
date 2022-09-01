namespace secretary.telegram.commands;

public class StartCommand: Command
{
    public const string Key = "/start";
    
    public override async Task Execute()
    {
        await this.Context.TelegramClient.SendMessage(
            ChatId, 
            "Добро пожаловать!\r\n" +
            "\r\n" +
            "Перед началом работы вам необходимо:\r\n" +
            "/registeruser – зарегистрироваться\r\n" +
            "/registermail – зарегистрировать рабочую почту");
    }
}