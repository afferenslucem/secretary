namespace Secretary.Yandex.Exceptions;

public class YandexApiException: Exception
{
    public YandexApiException(string message, Exception innerException): base(message, innerException) {}
    
    public YandexApiException(string message): base(message) {}
    
    public YandexApiException(): base() {}
}