using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Secretary.Storage.Models
{
    public class Document
    {
        [Key]
        public long Id { get; set; }
        
        public long ChatId { get; set; }

        public User User { get; set; } = null!;
        
        
        [MaxLength(128)]
        public string DocumentName { get; set; } = null!;

        public IEnumerable<Email> Emails { get; set; } = null!;

        public Document() {}

        public Document(long chatId, string documentName)
        {
            ChatId = chatId;
            DocumentName = documentName;
        }
    }
}
