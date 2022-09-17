using Microsoft.EntityFrameworkCore;
using Secretary.Configuration;

namespace Secretary.Storage.Models
{
    public partial class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Document> Documents { get; set; } = null!;
        public virtual DbSet<Email> Emails { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(Config.Instance.DbConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Email>()
                .HasOne(email => email.Document)
                .WithMany(document => document.Emails)
                .HasForeignKey(email => email.DocumentId);
            
            
            modelBuilder.Entity<Document>()
                .HasOne(document => document.User)
                .WithMany(user => user.Documents)
                .HasForeignKey(document => document.ChatId);
        }
    }
}
