using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Secretary.Storage.Models
{
    public class Document
    {
        [Key]
        public long Id { get; set; }
        
        public long UserChatId { get; set; }

        public User User { get; set; } = null!;
        
        
        [MaxLength(128)]
        public string DocumentName { get; set; } = null!;

        public IEnumerable<Email> Emails { get; set; } = null!;

        public Document() {}

        public Document(long userChatId, string documentName)
        {
            UserChatId = userChatId;
            DocumentName = documentName;
        }
    }
}
