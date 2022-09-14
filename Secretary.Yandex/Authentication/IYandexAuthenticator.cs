namespace Secretary.Yandex.Authentication;

public interface IYandexAuthenticator
{
    Task<AuthenticationData?> GetAuthenticationCode(CancellationToken cancellationToken);

    Task<TokenData?> CheckToken(AuthenticationData data, CancellationToken cancellationToken);

    bool IsUserDomainAllowed(string email);
}