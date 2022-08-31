using secretary.yandex.mail;

namespace secretary.storage.models;

public class Email: IEquatable<Email>
{
    public long DocumentId { get; set; }
    public string Address { get; set; } = null!;
    public string? DisplayName { get; set; } = null!;
    
    public Email(string address, string? displayName = null)
    {
        Address = address;
        DisplayName = displayName;
    }
    
    public Email() {}

    public bool Equals(Email? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return DocumentId == other.DocumentId && Address == other.Address && DisplayName == other.DisplayName;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Email)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(DocumentId, Address, DisplayName);
    }

    public SecretaryMailAddress ToMailAddress()
    {
        return new SecretaryMailAddress(this.Address, this.DisplayName);
    }
}