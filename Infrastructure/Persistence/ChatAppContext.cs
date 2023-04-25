using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ChatAppContext : DbContext
    {
        public ChatAppContext(DbContextOptions<ChatAppContext> options) : base(options)
        { }

        public DbSet<Chat> Chat { get; set; }
        public DbSet<Message> Message { get; set; }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<Chat>(entity =>
            {
                // Reglas
            });

            modelbuilder.Entity<Message>(entity =>
            {
                // Reglas
            });
        }
    }
}
