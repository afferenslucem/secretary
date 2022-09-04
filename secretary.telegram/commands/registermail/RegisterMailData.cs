namespace secretary.telegram.commands.registermail;

public class RegisterMailData: IEquatable<RegisterMailData>
{
    public string Email;

    public RegisterMailData(string email)
    {
        Email = email;
    }

    public bool Equals(RegisterMailData? other)
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
        return Equals((RegisterMailData)obj);
    }

    public override int GetHashCode()
    {
        return Email.GetHashCode();
    }
}