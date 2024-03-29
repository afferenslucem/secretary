﻿using Secretary.Storage.Models;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Commands.Caches.Documents.Interfaces;
using Secretary.Telegram.Exceptions;
using Secretary.Telegram.Utils;
using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram.Commands.Documents.Common;

public class SetReceiversCommand<T> : Command
    where T: class, IEmailsCache, IDocumentKeyCache
{
    public override async Task Execute()
    {
        T cache = await CacheService.GetEntity<T>();

        var document = await DocumentStorage.GetOrCreateDocument(cache!.DocumentKey);
        var emails = await EmailStorage.GetForDocument(document.Id);

        if (emails.Count() > 0)
        {
            await SendRepeat(emails);
        }
        else
        {
            await SendAskEmails();
        }
    }

    public Task SendAskEmails()
    {
        return TelegramClient.SendMessage( 
            "Отправьте список адресов для рассылки в формате:\n" +
            "<code>" +
            "a.pushkin@infinnity.ru (Александр Пушкин)\n" +
            "s.esenin@infinnity.ru (Сергей Есенин)\n" +
            "v.mayakovskii@infinnity.ru\n" +
            "</code>\n\n" +
            "Если вы укажете адрес без имени в скобках, то в имени отправителя будет продублирован почтовый адрес");
    }

    public Task SendRepeat(IEnumerable<Email> emails)
    {
        var emailsPrints = emails
            .Select(item => item.DisplayName != null ? $"{item.Address} ({item.DisplayName})" : item.Address);

        var emailTable = string.Join("\n", emailsPrints);

        var message = "В прошлый раз вы сделали рассылку на эти адреса:\n" +
                      "<code>\n" +
                      $"{emailTable}" +
                      "</code>\n" +
                      "\n" +
                      "Повторить?";
        
        return TelegramClient.SendMessage(message, (ReplyKeyboardMarkup)"Повторить"!);
    }

    public override async Task<int> OnMessage()
    {
        if (Message == "Повторить")
        {
            return 2;
        }
        else
        {
            return await ParseAndSaveEmails();
        }
    }

    private async Task<int> ParseAndSaveEmails()
    {
        try
        {
            var cache = await CacheService.GetEntity<T>();
            if (cache == null) throw new InternalException();
            
            cache.Emails = new EmailParser().ParseMany(Message);
            await CacheService.SaveEntity(cache);

            return ExecuteDirection.RunNext;
        }
        catch (IncorrectEmailException e)
        {
            await TelegramClient.SendMessage($"Почтовый адрес <code>{e.IncorrectEmail}</code> имеет некорректный формат.\n" +
                "Поправьте его и отправте список адресов еще раз.");

            return ExecuteDirection.Retry;
        }
    }
}