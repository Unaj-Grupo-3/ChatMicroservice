using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ChatAppContext : DbContext
    {
        public ChatAppContext(DbContextOptions<ChatAppContext> options) : base(options)
        { }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<Chat>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).ValueGeneratedOnAdd();
                entity.Property(c => c.UserId1).IsRequired();
                entity.Property(c => c.UserId2).IsRequired();
                entity.Property(c => c.CreatedAt).HasDefaultValue(DateTime.Now);
                entity.Property(c => c.UpdatedAt).ValueGeneratedOnUpdate().HasDefaultValue(DateTime.Now);
            });

            modelbuilder.Entity<Message>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Id).ValueGeneratedOnAdd();
                entity.Property(m => m.FromUserId).IsRequired();
                entity.Property(m => m.Content).IsRequired();
                entity.Property(m => m.SendDateTime).IsRequired().ValueGeneratedOnAdd();
                entity.Property(m => m.IsRead).IsRequired().HasDefaultValue(false);

                // Relacion de 0 a muchos con chat
                entity.HasOne<Chat>(m => m.Chat)
                      .WithMany(c => c.Messages)
                      .HasForeignKey(c => c.ChatId);
            });
        }
    }
}
