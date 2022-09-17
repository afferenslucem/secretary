namespace Secretary.Yandex.Mail;

public class SecretaryMailAddress: IEquatable<SecretaryMailAddress>
{
    public readonly string Address;
    public readonly string? DisplayName;
    
    public SecretaryMailAddress(string address, string? displayName)
    {
        Address = address;
        DisplayName = displayName;
    }

    public bool Equals(SecretaryMailAddress? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Address == other.Address && DisplayName == other.DisplayName;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SecretaryMailAddress)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Address, DisplayName);
    }
}