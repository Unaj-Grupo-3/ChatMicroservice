using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance
{
    public class ChatAppContext : DbContext
    {
        public ChatAppContext(DbContextOptions<ChatAppContext> options) :base (options)
        { }

        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserFriend> UserFriends { get; set; } = null!;
        public virtual DbSet<Message> Messages { get; set; } = null!;
    }
}
