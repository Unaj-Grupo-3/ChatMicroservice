

using Application.Interface;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Commands
{
    public class MessageCommands : IMessageCommands
    {
        private readonly ChatAppContext _context;

        public MessageCommands(ChatAppContext context) 
        {
            _context = context;
        }

        public async Task<Message> CreateMessage(Message message)
        {
            _context.Messages.Add(message);

            await _context.SaveChangesAsync();

            return message;
        }

        public async Task<Message> UpdateIsReadMessage(int messageId)
        {
            Message updated = await _context.Messages.FirstOrDefaultAsync(x => x.Id == messageId);  

            updated.IsRead = true;

            await _context.SaveChangesAsync();

            return updated;
        }
    }
}
