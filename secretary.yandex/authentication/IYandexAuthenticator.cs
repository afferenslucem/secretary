namespace secretary.yandex.authentication;

public interface IYandexAuthenticator
{
    Task<AuthenticationData?> GetAuthenticationCode(CancellationToken cancellationToken);

    Task<TokenData?> CheckToken(AuthenticationData data, CancellationToken cancellationToken);

    bool IsUserDomainAllowed(string email);
}