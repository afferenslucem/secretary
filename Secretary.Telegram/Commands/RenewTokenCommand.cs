using Secretary.Logging;
using Secretary.Telegram.Commands.RegisterMail;
using Secretary.Telegram.Commands.Validation;
using Secretary.Yandex.Authentication;
using Serilog;

namespace Secretary.Telegram.Commands;

public class RenewTokenCommand: EnterCodeCommand
{
    private readonly ILogger _logger = LogPoint.GetLogger<RenewTokenCommand>();
    
    public const string Key = "/renewtoken";
    
    public override async Task Execute()
    {
        await ValidateUser();
        
        _logger.Information($"{ChatId}: renewing token");

        await base.Execute();
    }
    
    protected override async Task SetTokens(TokenData data)
    {
        var user = await UserStorage.GetUser();

        user.AccessToken = data.access_token;
        user.RefreshToken = data.refresh_token;
        user.TokenExpirationSeconds = data.expires_in;
        
        user.TokenCreationTime = DateTime.UtcNow;

        await UserStorage.SetUser(user);
    }
    
    private async Task ValidateUser()
    {
        var user = await UserStorage.GetUser();
        
        new UserValidationVisitor().Validate(user);
    }
}