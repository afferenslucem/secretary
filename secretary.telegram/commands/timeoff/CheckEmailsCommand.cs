using secretary.documents.creators;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands.timeoff;

public class CheckEmailsCommand : Command
{
    public override async Task Execute()
    {
        var document = await this.Context.DocumentStorage.GetOrCreateDocument(ChatId, TimeOffCommand.Key);
        var emails = await this.Context.EmailStorage.GetForDocument(document.Id);
        
        var emailsPrints = emails
            .Select(item => item.DisplayName != null ? $"{item.Address} ({item.DisplayName})" : item.Address);

        var emailTable = string.Join("\r\n", emailsPrints);

        var message = "Заявление будет отправлено на следующие адреса:\r\n" +
                      "<code>\r\n" +
                      $"{emailTable}" +
                      "</code>\r\n" +
                      "\r\n" +
                      "Все верно?";
        
        await Context.TelegramClient.SendMessageWithKeyBoard(ChatId, message, new [] { "Верно", "Нет, нужно поправить" });
    }

    public override Task<int> OnMessage()
    {
        if (Message.ToLower() == "верно")
        {
            return Task.FromResult<int>(RunNext);
        }
        else
        {
            return Task.FromResult<int>(-1);
        }
    }
}