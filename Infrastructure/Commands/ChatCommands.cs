using Application.Interface;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Commands
{
    public class ChatCommands : IChatCommands
    {
        private readonly ChatAppContext _context;

        public ChatCommands(ChatAppContext context)
        {
            _context = context;
        }

        public async Task<Chat> CreateChat(Chat chat)
        {
            _context.Chats.Add(chat);

            await _context.SaveChangesAsync();

            return chat;
        }
    }
}
