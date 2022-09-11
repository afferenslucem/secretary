using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using secretary.documents.creators;
using secretary.logging;
using secretary.storage.models;
using secretary.telegram.commands.caches;
using secretary.telegram.exceptions;
using secretary.yandex.mail;
using Serilog;

namespace secretary.telegram.commands.timeoff;

public class SendDocumentCommand : Command
{
    private readonly ILogger _logger = LogPoint.GetLogger<SendDocumentCommand>();
    
    public ITimeOffCreator MessageCreator;


    public SendDocumentCommand()
    {
        MessageCreator = new TimeOffMessageCreator();
    }

    public override async Task Execute()
    {
        var cache = await CacheService.GetEntity<TimeOffCache>();

        if (cache == null) throw new InternalException();
        
        var document = await DocumentStorage.GetOrCreateDocument(TimeOffCommand.Key);
        var user = await UserStorage.GetUser();

        IEnumerable<Email> emails = await EmailStorage.GetForDocument(document.Id);

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
                new SecretaryAttachment(cache.FilePath!, "Заявление.docx", new ContentType("application", "msword"))
            },
            Sender = new SecretaryMailAddress(user.Email!, user.Name!),
            Receivers = receivers,
            Theme = $"{user.Name} [Отгул {cache.Period!.DayPeriod}]",
            HtmlBody = MessageCreator.Create(data)
        };

        return result;
    }

    public async Task SendMail(SecretaryMailMessage message)
    {
        try
        {
            await MailClient.SendMail(message);

            await TelegramClient.SendMessage("Заяление отправлено");

            _logger.Information($"{ChatId}: sent mail");
        }
        catch (AuthenticationException e)
        {
            if (e.Message.Contains("This user does not have access rights to this service"))
            {
                await TelegramClient.SendMessage(
                    "Не достаточно прав для отправки письма!\n\n" +
                    "Убедитесь, что токен выдан для вашего рабочего почтового ящика.\n" +
                    "Если ящик нужный, то перейдите в <a href=\"https://mail.yandex.ru/#setup/client\">настройки</a> " +
                    "и разрешите отправку по OAuth-токену с сервера imap.\n" +
                    "Не спешите пугаться незнакомых слов, вам просто нужно поставить одну галочку по ссылке"
                );
            }
        }
        catch (SmtpCommandException e)
        {
            if (e.Message.Contains("Sender address rejected: not owned by auth user"))
            {
                await TelegramClient.SendSticker(Stickers.Guliki);
                
                await TelegramClient.SendMessage($"Вы отправляете письмо с токеном не принадлежащим ящику <code>{e.Mailbox.Address}</code>");
                
                _logger.Warning(e, "Guliki detected. {@ChatId} tried use {@Email}", ChatId, message.Sender.Address);
            }
        }
        finally
        {
            await CacheService.DeleteEntity<TimeOffCache>();
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
            _logger.Warning(e, $"Could not delete file {filename}");
        }
    }
}