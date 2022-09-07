﻿using Microsoft.Extensions.Logging;
using secretary.logging;
using secretary.storage.models;
using secretary.telegram.commands.caches;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands.registeruser;

public class EnterJobTitleGenitiveCommand : Command
{
    private readonly ILogger<EnterJobTitleGenitiveCommand> _logger = LogPoint.GetLogger<EnterJobTitleGenitiveCommand>();
    public override Task Execute()
    {
        return TelegramClient.SendMessage("Введите вашу должность в родительном падеже.\r\n" +
                                          "Так она будут указана в графе \"от кого\".\r\n" +
                                          @"Например: От <i>поэта</i> Пушкина Александра Сергеевича");
    }

    public override async Task<int> OnMessage()
    {
        try
        {
            var cache = await Context.CacheService.GetEntity<RegisterUserCache>(ChatId);
            if (cache == null) throw new InternalException();

            var user = await UserStorage.GetUser();
            user = user ?? new User() { ChatId = ChatId };

            user.Name = cache.Name;
            user.NameGenitive = cache.NameGenitive;
            user.JobTitle = cache.JobTitle;
            user.JobTitleGenitive = Message;

            await UserStorage.SetUser(user);

            await TelegramClient.SendMessage("Ваш пользователь успешно сохранен");

            _logger.LogInformation($"{ChatId}: registered user {user.Name}");

            return ExecuteDirection.RunNext;
        }
        finally
        {
            await Context.CacheService.DeleteEntity<RegisterUserCache>(ChatId);
        }
    }
}