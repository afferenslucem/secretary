using System.Net.Mail;
using MailKit.Security;
using MimeKit;
using secretary.documents.creators;
using secretary.storage.models;
using secretary.telegram.utils;
using secretary.yandex.mail;

namespace secretary.telegram.commands.timeoff;

public class SendDocumentCommand : Command
{
    public ITimeOffCreator MessageCreator;


    public SendDocumentCommand()
    {
        MessageCreator = new TimeOffMessageCreator();
    }

    protected override async Task ExecuteRoutine()
    {
        if (Message != "Да") return;

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

    protected override async Task OnMessageRoutine()
    {
        var document = await Context.DocumentStorage.GetOrCreateDocument(ChatId, TimeOffCommand.Key);
        var user = await Context.UserStorage.GetUser(ChatId);

        IEnumerable<Email> emails;

        if (Message == "Повторить")
        {
            emails = await this.Context.EmailStorage.GetForDocument(document.Id);
        }
        else
        {
            emails = new EmailParser().ParseMany(Message);
            await Context.EmailStorage.SaveForDocument(document.Id, emails);
        }

        var message = this.GetMailMessage(user, emails);

        await SendMail(message);
    }

    public SecretaryMailMessage GetMailMessage(User user, IEnumerable<Email> emails)
    {
        var parent = (ParentCommand as TimeOffCommand)!;
        
        var sender = new SecretaryMailAddress(user.Email, user.Name);
        var receivers = emails.Select(item => item.ToMailAddress()).Append(sender);

        var data = parent.Data.ToDocumentData();
        data.Name = user.Name;
        data.JobTitle = user.JobTitle;
        
        var result = new SecretaryMailMessage()
        {
            Token = user.AccessToken,
            Attachments = new[]
            {
                new SecretaryAttachment()
                {
                    Path = parent.Data.FilePath,
                    FileName = "Заявление.docx",
                    ContentType = new ContentType("application",
                        "vnd.openxmlformats-officedocument.wordprocessingml.document")
                }
            },
            Sender = new SecretaryMailAddress(user.Email, user.Name),
            Receivers = receivers,
            Theme = $"[Отгул {data.PeriodYear}]",
            HtmlBody = MessageCreator.Create(data)
        };

        return result;
    }

    public async Task SendMail(SecretaryMailMessage message)
    {
        try
        {
            await Context.MailClient.SendMail(message);

            await Context.TelegramClient.SendMessage(ChatId, "Заяление отправлено");
        }
        catch (AuthenticationException e)
        {
            if (e.Message.Contains("This user does not have access rights to this service"))
            {
                await Context.TelegramClient.SendMessage(ChatId, 
                    "Не достаточно прав для отправки письма!\r\n\r\n" +
                    "Перейдите в <a href=\"https://mail.yandex.ru/#setup/client\">настройки почтового ящика</a>, c которого отправляете письмо, " +
                    "и разрешите отправку по OAuth-токену с сервера imap"
                    );
            }
        }
    }
}