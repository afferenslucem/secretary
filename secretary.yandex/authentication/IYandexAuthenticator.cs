namespace secretary.mail.Authentication;

public interface IYandexAuthenticator
{
    Task<AuthenticationData?> GetAuthenticationCode();

    Task<TokenData?> CheckToken(AuthenticationData data);
}