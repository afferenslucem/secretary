namespace secretary.yandex.authentication;

public class TokenData
{
     public string token_type { get; set; } = null!;
     public string access_token { get; set; } = null!;
     public int expires_in { get; set; }
     public string refresh_token { get; set; }  = null!;
     public string scope { get; set; } = null!;
}