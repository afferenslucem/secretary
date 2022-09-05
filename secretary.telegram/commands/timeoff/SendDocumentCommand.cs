using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using secretary.documents.creators;
using secretary.logging;
using secretary.storage.models;
using secretary.telegram.commands.caches;
using secretary.telegram.exceptions;
using secretary.yandex.mail;

namespace secretary.telegram.commands.timeoff;

public class SendDocumentCommand : Command
{
    private readonly ILogger<SendDocumentCommand> _logger = LogPoint.GetLogger<SendDocumentCommand>();
    
    public ITimeOffCreator MessageCreator;


    public SendDocumentCommand()
    {
        MessageCreator = new TimeOffMessageCreator();
    }

    public override async Task Execute()
    {
        var cache = await Context.CacheService.GetEntity<TimeOffCache>(ChatId);

        if (cache == null) throw new InternalException();
        
        var document = await Context.DocumentStorage.GetOrCreateDocument(ChatId, TimeOffCommand.Key);
        var user = await Context.UserStorage.GetUser(ChatId);

        IEnumerable<Email> emails = await this.Context.EmailStorage.GetForDocument(document.Id);

        var message = this.GetMailMessage(user!, emails, cache);

        await SendMail(message);
        
        DeleteDocument(cache.FilePath!);
    }

    public SecretaryMailMessage GetMailMessage(User user, IEnumerable<Email> emails, TimeOffCache cache)
    {
        var sender = new SecretaryMailAddress(user.Email!, user.Name!);
        var receivers = emails.Select(item => item.ToMailAddress()).Append(sender);

        var data = cache.ToDocumentData();
        data.Name = user.Name;
        data.JobTitle = user.JobTitle;
        
        var result = new SecretaryMailMessage()
        {
            Token = user.AccessToken!,
            Attachments = new[]
            {
                new SecretaryAttachment()
                {
                    Path = cache.FilePath!,
                    FileName = "Заявление.docx",
                    ContentType = new ContentType("application",
                        "vnd.openxmlformats-officedocument.wordprocessingml.document")
                }
            },
            Sender = new SecretaryMailAddress(user.Email!, user.Name!),
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

            _logger.LogInformation($"{ChatId}: sent mail");
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
        catch (SmtpCommandException e)
        {
            if (e.Message.Contains("Sender address rejected: not owned by auth user"))
            {
                await Context.TelegramClient.SendMessage(ChatId,
                    "Guliki detected!\r\n" +
                    $"Вы отправляете письмо с токеном не принадлежащим ящику <code>{e.Mailbox.Address}</code>"
                );
            }
        }
        finally
        {
            await Context.CacheService.DeleteEntity<TimeOffCache>(ChatId);
        }
    }

    private void DeleteDocument(string filename)
    {
        try
        {
            File.Delete(filename);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, $"Could not delete file {filename}");
        }
    }
}