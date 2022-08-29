using secretary.mail.Authentication;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands.registermail;

public class EnterCodeCommand: Command
{
    protected override async Task ExecuteRoutine()
    {
        var data = await Context.YandexAuthenticator.GetAuthenticationCode();

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

    private async Task<TokenData?> AskRegistration(IYandexAuthenticator client, AuthenticationData data, DateTime startTime)
    {
        if (DateTime.Now >= startTime.AddSeconds(data.expires_in) || this.CancellationToken.IsCancellationRequested)
        {
            return null;
        }
        
        var token = await client.CheckToken(data, CancellationToken.Token);

        if (token?.access_token != null) return token;

        await Task.Delay(data.interval * 1000, CancellationToken.Token);

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