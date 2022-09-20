using System.Net;
using System.Text.Json;
using Secretary.Configuration;
using Secretary.Logging;
using Secretary.Yandex.Exceptions;
using Serilog;

namespace Secretary.Yandex.Authentication;

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

    public Task<TokenData?> RefreshToken(string refreshToken, CancellationToken cancellationToken)
    {
        return Retry(
            () => this.SendRefreshTokenRequest(refreshToken, cancellationToken),
            cancellationToken
        );
    }

    private async Task<T> Retry<T>(Func<Task<T>> action, CancellationToken cancellationToken)
    {
        try
        {
            return await action();
        }
        catch (YandexAuthenticationException e)
        {
            if (e.Message == "Refresh token expired")
            {
                throw;
            }
            
            await Task.Delay(10000, cancellationToken);
            return await action();
        }
        catch (Exception)
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
    
    private async Task<TokenData?> SendRefreshTokenRequest(string refreshToken, CancellationToken cancellationToken)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.yandex.ru/token");

            request.Content =
                new StringContent(
                    $"grant_type=refresh_token&refresh_token={refreshToken}&client_id={_mailConfig.ClientId}&client_secret={_mailConfig.ClientSecret}");

            var response = await _httpClient.SendAsync(request, cancellationToken);
            
            var responseData = await response.Content.ReadAsStringAsync();
            
            if (response.StatusCode == HttpStatusCode.BadRequest && responseData.Contains("expired_token"))
            {
                throw new YandexAuthenticationException("Refresh token expired");
            }
            
            var result = JsonSerializer.Deserialize<TokenData>(responseData);

            return result;

        }
        catch (YandexAuthenticationException e)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Ошибка при обновлении токена");
            throw new YandexAuthenticationException("Could not refresh token", e);
        }
    }

    public bool IsUserDomainAllowed(string email)
    {
        return _mailConfig.AllowedSenderDomains.Any(domain => email.EndsWith($"@{domain}"));
    }
}