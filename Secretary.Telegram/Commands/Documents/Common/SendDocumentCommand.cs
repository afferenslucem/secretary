using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Secretary.Documents.utils;
using Secretary.Logging;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Commands.Caches.Documents.Interfaces;
using Secretary.Telegram.Documents;
using Secretary.Telegram.Exceptions;
using Secretary.Yandex.Mail.Data;
using Serilog;

namespace Secretary.Telegram.Commands.Documents.Common;

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
        try
        {
            var cache = await CacheService.GetEntity<T>();

            if (cache == null) throw new InternalException();

            var document = await DocumentStorage.GetOrCreateDocument(cache.DocumentKey);
            var user = await UserStorage.GetUser();

            IEnumerable<Email> emails = await EmailStorage.GetForDocument(document.Id);

            var message = GetMailMessage(user!, emails, cache);

            await SendMail(message);

            DeleteDocument(cache.FilePath!);
        }
        finally
        {
            await DeleteCache();
        }
    }

    public MailMessage GetMailMessage(User user, IEnumerable<Email> emails, T cache)
    {
        var receivers = emails.Select(item => item.ToMailAddress());

        DocumentContext documentContext = DocumentContextProvider.GetContext(cache.DocumentKey);
        
        var result = new MailMessage()
        {
            Token = user.AccessToken!,
            Attachments = new[]
            {
                new MessageAttachment(cache.FilePath!, documentContext.DisplayName, new ContentType("application", "msword"))
            },
            Sender = new MailAddress(user.Email!, user.Name!),
            Receivers = receivers,
            Theme = $"{user.Name} [{documentContext.MailTheme} {cache.Period!.DayPeriod}]",
            HtmlBody = cache.CreateMail(user)
        };

        return result;
    }

    public async Task SendMail(MailMessage message)
    {
        try
        {
            await MailSender.SendMail(message);

            await TelegramClient.SendMessage("Заяление отправлено");

            _logger.Information($"{ChatId}: sent mail");
        }
        catch (AuthenticationException e)
        {
            _logger.Warning(e, "Yandex authentications exception");
            
            if (e.Message.Contains("This user does not have access rights to this service") || e.Message.Contains("Authentication failed"))
            {
                await TelegramClient.SendMessage(
                    "Не достаточно прав для отправки письма!\n\n" +
                    "Перейдите в <a href=\"https://mail.yandex.ru/#setup/client\">настройки</a> " +
                    "и разрешите отправку по OAuth-токену с сервера imap.\n" +
                    "Не спешите пугаться незнакомых слов, вам просто нужно поставить одну галочку по ссылке.\n\n" +
                    "Если доступ разрешен, то выполните команду /renewtoken и проверьте, что токен выпускается для рабочей почты"
                );
                
                return;
            }
            
            if (e.Message.Contains("Invalid user or password"))
            {
                await TelegramClient.SendMessage(
                    "Невалидный токен!\n\n" +
                    "Выполните команду /renewtoken"
                );

                await UserStorage.RemoveTokens();
                
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
                
                await TelegramClient.SendMessage($"Вы отправляете письмо с токеном не принадлежащим ящику <code>{e.Mailbox.Address}</code>\n\n" +
                                                 $"Выполните команду /renewtoken");

                await UserStorage.RemoveTokens();
                
                return;
            }
            
            throw;
        }
    }

    private void DeleteDocument(string filename)
    {
        FileManager.DeleteFile(filename);
    }

    public async Task DeleteCache()
    {
        await CacheService.DeleteEntity<T>();
    }
}