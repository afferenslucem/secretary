namespace secretary.mail.Authentication;

public interface IYandexAuthenticator
{
    Task<AuthenticationData?> GetAuthenticationCode(CancellationToken cancellationToken);

    Task<TokenData?> CheckToken(AuthenticationData data, CancellationToken cancellationToken);
}