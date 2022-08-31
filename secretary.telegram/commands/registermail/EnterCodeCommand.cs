using Microsoft.Extensions.Logging;
using secretary.logging;
using secretary.mail.Authentication;
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

            await Context.TelegramClient.SendMessage(ChatId,
                $"Пожалуйста, <strong>УБЕДИТЕСЬ</strong>, что вы авторизуетесь в рабочей почте!\r\n" +
                $"Введите этот код: <code>{data.user_code}</code> в поле ввода по этой ссылке: {data.verification_url}");

            var tokenData = await this.AskRegistration(Context.YandexAuthenticator, data, DateTime.Now);

            if (tokenData != null)
            {
                await this.SetTokens(tokenData);
                await Context.TelegramClient.SendMessage(ChatId, "Ура, вы успешно зарегистрировали почту");
            }
        }
        catch (YandexAuthenticationException e)
        {
            await this.Context.TelegramClient.SendMessage(ChatId,
                "При запросе токена для авторизации произошла ошибка:(\r\n" +
                "Попробуйте через пару минут, если не сработает, то обратитесь по вот этому адресу @hrodveetnir");
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
        var user = await Context.UserStorage.GetUser(ChatId);

        if (user == null) throw new InternalException();
        
        user.AccessToken = data.access_token;
        user.RefreshToken = data.refresh_token;

        await Context.UserStorage.UpdateUser(user);
    }
}