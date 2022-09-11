using System.Text.Json;
using secretary.configuration;
using secretary.logging;
using secretary.yandex.exceptions;
using Serilog;

namespace secretary.yandex.authentication;

public class YandexAuthenticator: IYandexAuthenticator
{
    private ILogger _logger = LogPoint.GetLogger<YandexAuthenticator>();

    private readonly HttpClient _httpClient = new();
    private MailConfig _mailConfig;

    public YandexAuthenticator(MailConfig mailConfig)
    {
        _mailConfig = mailConfig;
    }

    
    public Task<AuthenticationData?> GetAuthenticationCode(CancellationToken cancellationToken)
    {
        return Retry(
            () => this.SendAuthenticationCodeRequest(cancellationToken),
            cancellationToken
        );
    }

    public Task<TokenData?> CheckToken(AuthenticationData data, CancellationToken cancellationToken)
    {
        return Retry(
            () => this.SendCheckTokenRequest(data, cancellationToken),
            cancellationToken
        );
    }

    private async Task<T> Retry<T>(Func<Task<T>> action, CancellationToken cancellationToken)
    {
        try
        {
            return await action();
        }
        catch (Exception e)
        {
            await Task.Delay(10000, cancellationToken);
            return await action();
        }
    }

    private async Task<AuthenticationData?> SendAuthenticationCodeRequest(CancellationToken cancellationToken)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.yandex.ru/device/code");

            request.Content = new StringContent($"client_id={_mailConfig.ClientId}");

            var response = await _httpClient.SendAsync(request, cancellationToken);

            var responseStream = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<AuthenticationData>(responseStream);

            return result;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Ошибка при получении кода авторизации");
            throw new YandexAuthenticationException("Could not get auth data", e);
        }
    }
    
    private async Task<TokenData?> SendCheckTokenRequest(AuthenticationData data, CancellationToken cancellationToken)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.yandex.ru/token");

            request.Content =
                new StringContent(
                    $"grant_type=device_code&code={data.device_code}&client_id={_mailConfig.ClientId}&client_secret={_mailConfig.ClientSecret}");

            var response = await _httpClient.SendAsync(request, cancellationToken);

            var responseData = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<TokenData>(responseData);

            return result;

        }
        catch (Exception e)
        {
            _logger.Error(e, "Ошибка при получении токена");
            throw new YandexAuthenticationException("Could not get token", e);
        }
    }

    public bool IsUserDomainAllowed(string email)
    {
        return _mailConfig.AllowedSenderDomains.Any(domain => email.EndsWith($"@{domain}"));
    }
}