using Microsoft.Extensions.Logging;
using secretary.logging;
using secretary.yandex.authentication;
using secretary.storage.models;
using secretary.telegram.commands.caches;
using secretary.telegram.exceptions;
using secretary.yandex.exceptions;

namespace secretary.telegram.commands.registermail;

public class EnterCodeCommand: Command
{
    private ILogger<EnterCodeCommand> _logger = LogPoint.GetLogger<EnterCodeCommand>();

    public override Task Execute()
    {
        _ = this.RegisterMail();
        
        return Task.CompletedTask;
    }

    private async Task RegisterMail()
    {
        try
        {
            var data = await Context.YandexAuthenticator.GetAuthenticationCode(CancellationToken.Token);

            await TelegramClient.SendMessage(
                "Пожалуйста, <strong>УБЕДИТЕСЬ</strong>, что вы авторизуетесь в рабочей почте!\r\n" +
                $"Введите этот код: <code>{data.user_code}</code> в поле ввода по этой ссылке: {data.verification_url}. Регистрация может занять пару минут.");

            var tokenData = await this.AskRegistration(Context.YandexAuthenticator, data, DateTime.Now);

            if (tokenData != null)
            {
                await this.SetTokens(tokenData);
                await TelegramClient.SendMessage("Ура, вы успешно зарегистрировали почту");
                _logger.LogInformation($"{ChatId}: handled tokens");
            }
        }
        catch (YandexAuthenticationException e)
        {
            _logger.LogError(e, "Could not get auth info from server");
            
            await this.TelegramClient.SendMessage(
                "При запросе токена для авторизации произошла ошибка:(\r\n" +
                "Попробуйте через пару минут, если не сработает, то обратитесь по вот этому адресу @hrodveetnir");
        }
        finally
        {
            await Context.CacheService.DeleteEntity<RegisterMailCache>(ChatId);
        }
    }

    private async Task<TokenData?> AskRegistration(IYandexAuthenticator client, AuthenticationData data,
        DateTime startTime)
    {

        if (DateTime.Now >= startTime.AddSeconds(data.expires_in) || this.CancellationToken.IsCancellationRequested)
        {
            return null;
        }

        var token = await client.CheckToken(data, CancellationToken.Token);

        if (token?.access_token != null) return token;

        await Task.Delay(data.interval * 1500, CancellationToken.Token);

        return await this.AskRegistration(client, data, startTime);
    }

    private async Task SetTokens(TokenData data)
    {
        var cache = await Context.CacheService.GetEntity<RegisterMailCache>(ChatId);
        if (cache == null) throw new InternalException();
        
        var user = await UserStorage.GetUser();
        user = user ?? new User() { ChatId = ChatId };

        user.Email = cache.Email;
        user.AccessToken = data.access_token;
        user.RefreshToken = data.refresh_token;

        await UserStorage.SetUser(user);
    }
}