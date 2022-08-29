namespace secretary.yandex.exceptions;

public class YandexAuthenticationException: Exception
{
    public YandexAuthenticationException(string message, Exception innerException): base(message, innerException) {}
    
    public YandexAuthenticationException(): base() {}
}