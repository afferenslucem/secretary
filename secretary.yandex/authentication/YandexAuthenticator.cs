using System.Text.Json;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using secretary.configuration;

namespace secretary.mail.Authentication;

public class YandexAuthenticator: IYandexAuthenticator
{
    private readonly HttpClient _httpClient = new HttpClient();
    private MailConfig _mailConfig;

    public YandexAuthenticator(MailConfig mailConfig)
    {
        _mailConfig = mailConfig;
    }

    
    public async Task<AuthenticationData?> GetAuthenticationCode()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.yandex.ru/device/code");

        request.Content = new StringContent($"client_id={_mailConfig.ClientId}");

        var response = await _httpClient.SendAsync(request);

        var responseStream = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<AuthenticationData>(responseStream);

        return result;
    }

    public async Task<TokenData?> CheckToken(AuthenticationData data)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.yandex.ru/token");

        request.Content = new StringContent($"grant_type=device_code&code={data.device_code}&client_id={_mailConfig.ClientId}&client_secret={_mailConfig.ClientSecret}");

        var response = await _httpClient.SendAsync(request);

        var responseData = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<TokenData>(responseData);

        return result;
    }
}