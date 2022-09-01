using secretary.documents.creators;
using secretary.storage.models;
using secretary.telegram.exceptions;
using secretary.telegram.utils;

namespace secretary.telegram.commands.timeoff;

public class SetEmailsCommand : Command
{
    public override async Task Execute()
    {
        if (Message.ToLower() != "да" && !Context.BackwardRedirect)
        {
            await this.CancelCommand();
            this.ForceComplete();
        }

        var document = await this.Context.DocumentStorage.GetOrCreateDocument(ChatId, TimeOffCommand.Key);
        var emails = await this.Context.EmailStorage.GetForDocument(document.Id);

        if (emails.Count() > 0)
        {
            await this.SendRepeat(emails);
        }
        else
        {
            await this.SendAskEmails();
        }
    }

    public Task CancelCommand()
    {
        return Context.TelegramClient.SendMessage(ChatId, "Дальнейшее выполнение команды прервано");
    }

    public Task SendAskEmails()
    {
        return Context.TelegramClient.SendMessage(ChatId, 
            "Отправьте список адресов для рассылки в формате:\r\n" +
            "<code>" +
            "a.pushkin@infinnity.ru (Александр Пушкин)\r\n" +
            "s.esenin@infinnity.ru (Сергей Есенин)\r\n" +
            "v.mayakovskii@infinnity.ru\r\n" +
            "</code>\r\n\r\n" +
            "Если вы укажете адрес без имени в скобках, то в имени отправителя будет продублированпочтовый адрес");
    }

    public Task SendRepeat(IEnumerable<Email> emails)
    {
        var emailsPrints = emails
            .Select(item => item.DisplayName != null ? $"{item.Address} ({item.DisplayName})" : item.Address);

        var emailTable = string.Join("\r\n", emailsPrints);

        var message = "В прошлый раз вы сделали рассылку на эти адреса:\r\n" +
                      "<code>\r\n" +
                      $"{emailTable}" +
                      "</code>\r\n" +
                      "\r\n" +
                      "Повторить?";
        
        return Context.TelegramClient.SendMessageWithKeyBoard(ChatId, message, new [] { "Повторить" });
    }

    public override async Task<int> OnMessage()
    {
        var document = await Context.DocumentStorage.GetOrCreateDocument(ChatId, TimeOffCommand.Key);

        if (Message == "Повторить")
        {
            return 2;
        }
        else
        {
            IEnumerable<Email> emails = new EmailParser().ParseMany(Message);
            await Context.EmailStorage.SaveForDocument(document.Id, emails);

            return RunNext;
        }
    }
}