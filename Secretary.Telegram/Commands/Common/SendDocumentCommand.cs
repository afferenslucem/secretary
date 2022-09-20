using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Secretary.Documents.utils;
using Secretary.Logging;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Commands.Caches.Interfaces;
using Secretary.Telegram.Documents;
using Secretary.Telegram.Exceptions;
using Secretary.Yandex.Mail;
using Serilog;

namespace Secretary.Telegram.Commands.Common;

public class SendDocumentCommand<T> : Command
    where T: class, IMailCreator, IDocumentKeyCache, IFilePathCache, IPeriodCache
{
    private readonly ILogger _logger = LogPoint.GetLogger<SendDocumentCommand<T>>();

    public IFileManager FileManager;

    public SendDocumentCommand()
    {
        FileManager = new FileManager();
    }

    public override async Task Execute()
    {
        var cache = await CacheService.GetEntity<T>();

        if (cache == null) throw new InternalException();
        
        var document = await DocumentStorage.GetOrCreateDocument(cache.DocumentKey);
        var user = await UserStorage.GetUser();

        IEnumerable<Email> emails = await EmailStorage.GetForDocument(document.Id);

        var message = this.GetMailMessage(user!, emails, cache);

        await SendMail(message);
        
        DeleteDocument(cache.FilePath!);
    }

    public SecretaryMailMessage GetMailMessage(User user, IEnumerable<Email> emails, T cache)
    {
        var sender = new SecretaryMailAddress(user.Email!, user.Name!);
        var receivers = emails.Select(item => item.ToMailAddress()).Append(sender);

        DocumentContext documentContext = DocumentContextProvider.GetContext(cache.DocumentKey);
        
        var result = new SecretaryMailMessage()
        {
            Token = user.AccessToken!,
            Attachments = new[]
            {
                new SecretaryAttachment(cache.FilePath!, documentContext.DisplayName, new ContentType("application", "msword"))
            },
            Sender = new SecretaryMailAddress(user.Email!, user.Name!),
            Receivers = receivers,
            Theme = $"{user.Name} [{documentContext.MailTheme} {cache.Period!.DayPeriod}]",
            HtmlBody = cache.CreateMail(user)
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
            _logger.Warning(e, "Yandex authentications exception");
            
            if (e.Message.Contains("This user does not have access rights to this service"))
            {
                await TelegramClient.SendMessage(
                    "Не достаточно прав для отправки письма!\n\n" +
                    "Убедитесь, что токен выдан для вашего рабочего почтового ящика.\n" +
                    "Если ящик нужный, то перейдите в <a href=\"https://mail.yandex.ru/#setup/client\">настройки</a> " +
                    "и разрешите отправку по OAuth-токену с сервера imap.\n" +
                    "Не спешите пугаться незнакомых слов, вам просто нужно поставить одну галочку по ссылке"
                );
                
                return;
            }
            
            if (e.Message.Contains("Invalid user or password"))
            {
                await TelegramClient.SendMessage(
                    "Проблема с токеном!\n\n" +
                    "Выполните команду /registermail.\n" +
                    "Если проблема не исчезнет, то напишите @hrodveetnir"
                );
                
                return;
            }

            throw;
        }
        catch (SmtpCommandException e)
        {
            _logger.Warning(e, "SMTP exception");
            
            if (e.Message.Contains("Sender address rejected: not owned by auth user"))
            {
                await TelegramClient.SendSticker(Stickers.Guliki);
                
                await TelegramClient.SendMessage($"Вы отправляете письмо с токеном не принадлежащим ящику <code>{e.Mailbox.Address}</code>");
                
                return;
            }
            
            throw;
        }
        finally
        {
            await CacheService.DeleteEntity<TimeOffCache>();
        }
    }

    private void DeleteDocument(string filename)
    {
        FileManager.DeleteFile(filename);
    }
}