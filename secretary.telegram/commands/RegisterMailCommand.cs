using secretary.mail;
using secretary.mail.Authentication;
using secretary.telegram.exceptions;
using secretary.storage.models;

namespace secretary.telegram.commands;

public class RegisterMailCommand: StatedCommand
{
    public const string Key = "/registermail";
    
    public override List<Command> ConfigureStates()
    {
        return new List<Command>
        {
            new EmptyCommand(),
            new EnterEmailCommand(),
            new EnterCodeCommand(),
        };
    }
}

class EnterEmailCommand : Command
{
    protected override Task ExecuteRoutine()
    {
        return Context.TelegramClient.SendMessage(ChatId, "Введите вашу почту, с которой вы отправляете заявления.\r\n" +
                                                          @"Например: <i>a.pushkin@infinnity.ru</i>");
    }

    protected override async Task OnMessageRoutine()
    {
        var user = await Context.UserStorage.GetUser(ChatId);

        user = user ?? new User()
        {
            ChatId = ChatId,
        };

        user.Email = Message;

        await Context.UserStorage.SetUser(user);
    }
}

public class EnterCodeCommand: Command
{
    public const string Key = "/registerMail";
    
    protected override async Task ExecuteRoutine()
    {
        var data = await Context.YandexAuthenticator.GetAuthenticationCode();

        await Context.TelegramClient.SendMessage(ChatId,
            $"Пожалуйста, <strong>УБЕДИТЕСЬ</strong>, что вы авторизуетесь в рабочей почте!\r\n" +
            $"Введите этот код: <code>{data.user_code}</code> в поле ввода по этой ссылке: {data.verification_url}");

        var tokenData = await this.AskRegistration(Context.YandexAuthenticator, data);

        if (tokenData != null)
        {
            await this.SetTokens(tokenData);
            await Context.TelegramClient.SendMessage(ChatId, "Ура, вы успешно зарегистрировали почту");
        }
    }

    private async Task<TokenData?> AskRegistration(IYandexAuthenticator client, AuthenticationData data)
    {
        var startTime = DateTime.Now;
        
        while ((DateTime.Now - startTime).TotalSeconds < data.expires_in)
        {
            var token = await client.CheckToken(data);

            if (token?.access_token != null) return token;

            await Task.Delay((int) (data.interval * 1000 * 1.5));
        }

        return null;
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