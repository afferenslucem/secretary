namespace Secretary.Telegram.Commands.Caches;

public class RegisterMailCache: IEquatable<RegisterMailCache>
{
    public string Email;

    public RegisterMailCache(string email)
    {
        Email = email;
    }

    public bool Equals(RegisterMailCache? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Email == other.Email;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((RegisterMailCache)obj);
    }

    public override int GetHashCode()
    {
        return Email.GetHashCode();
    }
}