using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Secretary.Storage.Models
{
    public partial class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ChatId { get; set; }

        [MaxLength(256)] public string? Name { get; set; }

        [MaxLength(256)] public string? NameGenitive { get; set; }

        [MaxLength(256)] public string? JobTitle { get; set; }

        [MaxLength(256)] public string? JobTitleGenitive { get; set; }

        [MaxLength(128)] public string? Email { get; set; }

        [MaxLength(256)] public string? AccessToken { get; set; }

        [MaxLength(256)] public string? RefreshToken { get; set; }

        public DateTime? TokenCreationTime { get; set; }

        public int? TokenExpirationSeconds { get; set; }

        public IEnumerable<Document> Documents = null;

        public IEnumerable<EventLog> Events = null;
    }
}
