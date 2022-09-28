using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Secretary.Yandex.Mail;
using Secretary.Yandex.Mail.Data;

namespace Secretary.Storage.Models
{
    public partial class Email: IEquatable<Email>
    {
        [Key]
        public long Id { get; set; }
        
        public long DocumentId { get; set; }
        public Document Document { get; set; } = null!;
        
        [MaxLength(128)]
        public string Address { get; set; } = null!;
        
        [MaxLength(256)]
        public string? DisplayName { get; set; }
        
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

        public MailAddress ToMailAddress()
        {
            return new MailAddress(this.Address, this.DisplayName);
        }
    }
}
