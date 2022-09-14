namespace Secretary.Storage.Models;

public class User
{
    public long ChatId { get; set; }
    public string? Name { get; set; }
    
    public string? NameGenitive { get; set; }
    
    public string? JobTitle { get; set; }
    
    public string? JobTitleGenitive { get; set; }
    
    public string? Email { get; set; }
    
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}