using MimeKit;

namespace Secretary.Yandex.Mail.Data;

public class MailAddress: IEquatable<MailAddress>
{
    public readonly string Address;

    private readonly string? _displayName;
    public string DisplayName => _displayName ?? Address;

    public MailAddress(string address, string? displayName)
    {
        Address = address;
        _displayName = displayName;
    }

    public MailboxAddress ToMailkitAddress()
    {
        return new MailboxAddress(DisplayName, Address);
    }

    public bool Equals(MailAddress? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Address == other.Address && DisplayName == other.DisplayName;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((MailAddress)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Address, DisplayName);
    }
}