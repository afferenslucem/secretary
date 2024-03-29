﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Secretary.Configuration;
using Secretary.Logging;
using ILogger = Serilog.ILogger;

namespace Secretary.Storage.Models
{
    public partial class DatabaseContext : DbContext
    {
        private ILogger _logger = LogPoint.GetLogger<DatabaseContext>();
        
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
        public virtual DbSet<EventLog> EventLogs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(Config.Instance.DbConnectionString);
            }

            // optionsBuilder.LogTo(_logger.Debug);
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
                .HasForeignKey(document => document.UserChatId);
            
            modelBuilder.Entity<User>()
                .Property(e => e.ChatId)
                .ValueGeneratedNever();
            
            modelBuilder.Entity<EventLog>()
                .HasOne(@event => @event.User)
                .WithMany(user => user.Events)
                .HasForeignKey(@event => @event.UserChatId);
        }
    }
}
