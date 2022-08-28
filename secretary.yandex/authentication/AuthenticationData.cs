namespace secretary.mail.Authentication;

public class AuthenticationData
{
     public string device_code { get; set; }
     public int expires_in { get; set; }
     public int interval { get; set; }
     public string user_code { get; set; } 
     public string verification_url { get; set; }
}