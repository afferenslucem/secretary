namespace Secretary.Yandex.Exceptions;

public class YandexAuthenticationException: Exception
{
    public YandexAuthenticationException(string message, Exception innerException): base(message, innerException) {}
    
    public YandexAuthenticationException(string message): base(message) {}
    
    public YandexAuthenticationException(): base() {}
}