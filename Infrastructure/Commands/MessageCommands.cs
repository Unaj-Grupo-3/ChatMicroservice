

using Application.Interface;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Commands
{
    public class MessageCommands : IMessageCommands
    {
        private readonly ChatAppContext _chatAppContext;
        public MessageCommands(ChatAppContext chatAppContext)
        {
            _chatAppContext = chatAppContext;
        }
        public async Task<int> AddMessage(int fromUserId, int toUserId, string message)
        {
            var entity = new Message
            {
                FromUserId = fromUserId,
                Content = message,
                SendDateTime = DateTime.Now,
                IsRead = false
            };

            _chatAppContext.Messages.Add(entity);
            var result = await _chatAppContext.SaveChangesAsync();

            return result;
        }
    }
}
