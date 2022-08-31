using System.Net.Mail;
using MailKit.Security;
using MimeKit;
using secretary.documents.creators;
using secretary.storage.models;
using secretary.telegram.exceptions;
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

    public override async Task Execute()
    {
        var document = await Context.DocumentStorage.GetOrCreateDocument(ChatId, TimeOffCommand.Key);
        var user = await Context.UserStorage.GetUser(ChatId);

        IEnumerable<Email> emails = await this.Context.EmailStorage.GetForDocument(document.Id);

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
                    "Убедитесь, что токен выдан для вашего рабочего почтового ящика.\r\n" +
                    "Если ящик нужный, то перейдите в <a href=\"https://mail.yandex.ru/#setup/client\">настройки</a> " +
                    "и разрешите отправку по OAuth-токену с сервера imap.\r\n" +
                    "Не спешите пугаться незнакомых слов, вам просто нужно поставить одну галочку по ссылке"
                    );
            }
        }
    }
}