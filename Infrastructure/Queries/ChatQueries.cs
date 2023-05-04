using Application.Interface;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Queries
{
    public class ChatQueries : IChatQueries
    {
        private readonly ChatAppContext _context;

        public ChatQueries(ChatAppContext context)
        {
            _context = context;
        }

        public async Task<Chat> GetChatById(int id)
        {
            Chat chat = await _context.Chats.Include(x => x.Messages.OrderBy(m => m.SendDateTime))
                                            .SingleOrDefaultAsync(x => x.Id == id);

            return chat;
        }

        public async Task<IList<Chat>> GetChatsByUserId(Guid userId)
        {
            IList<Chat> chats = await _context.Chats.Include(x => x.Messages.OrderBy(m => m.SendDateTime))
                                                    .Where(x => x.UserId1 == userId || x.UserId2 == userId)
                                                    .ToListAsync();

            return chats;
        }
    }
}
