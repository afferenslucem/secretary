﻿using Secretary.Logging;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Commands.Caches.Documents;
using Secretary.Telegram.Exceptions;
using Secretary.Yandex.Authentication;
using Secretary.Yandex.Exceptions;
using Serilog;

namespace Secretary.Telegram.Commands.RegisterMail;

public class EnterCodeCommand: Command
{
    private ILogger _logger = LogPoint.GetLogger<EnterCodeCommand>();

    public override Task Execute()
    {
        _ = RegisterMail();
        
        return Task.CompletedTask;
    }

    private async Task RegisterMail()
    {
        try
        {
            var data = await YandexAuthenticator.GetAuthenticationCode(CancellationToken.Token);

            await TelegramClient.SendMessage(
                "Пожалуйста, <strong>УБЕДИТЕСЬ</strong>, что вы авторизуетесь в рабочей почте!\n" +
                $"Введите этот код: <code>{data.user_code}</code> в поле ввода по этой ссылке: {data.verification_url}. Регистрация может занять пару минут.");

            var tokenData = await AskRegistration(YandexAuthenticator, data, DateTime.Now);

            if (tokenData != null)
            {
                await SetTokens(tokenData);
                await TelegramClient.SendMessage("Ура, токен для почты получен!");
                _logger.Information($"{ChatId}: handled tokens");
            }
        }
        catch (YandexAuthenticationException e)
        {
            _logger.Error(e, "Could not get auth info from server");
            
            await TelegramClient.SendMessage(
                "При запросе токена для авторизации произошла ошибка:(\n" +
                "Попробуйте через пару минут, если не сработает, то обратитесь по вот этому адресу @hrodveetnir");
        }
        catch (Exception e)
        {
            _logger.Error(e, "Unhandled exception");
            throw;
        }
        finally
        {
            await CacheService.DeleteEntity<RegisterMailCache>();
        }
    }

    private async Task<TokenData?> AskRegistration(IYandexAuthenticator client, AuthenticationData data,
        DateTime startTime)
    {

        if (DateTime.Now >= startTime.AddSeconds(data.expires_in) || CancellationToken.IsCancellationRequested)
        {
            return null;
        }

        var token = await client.CheckToken(data, CancellationToken.Token);

        if (token?.access_token != null) return token;

        await Task.Delay(data.interval * 1500, CancellationToken.Token);

        return await AskRegistration(client, data, startTime);
    }

    protected virtual async Task SetTokens(TokenData data)
    {
        var cache = await CacheService.GetEntity<RegisterMailCache>();
        if (cache == null) throw new InternalException();
        
        var user = await UserStorage.GetUser();
        user = user ?? new User() { ChatId = ChatId };

        user.Email = cache.Email;
        user.AccessToken = data.access_token;
        user.RefreshToken = data.refresh_token;
        user.TokenExpirationSeconds = data.expires_in;
        
        user.TokenCreationTime = DateTime.UtcNow;

        await UserStorage.SetUser(user);
    }
}