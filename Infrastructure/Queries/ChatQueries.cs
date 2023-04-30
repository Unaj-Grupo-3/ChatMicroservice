

using Application.Interface;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Queries
{
    public class ChatQueries : IChatQueries
    {
        private readonly ChatAppContext _chatAppContext;
        public ChatQueries(ChatAppContext chatAppContext)
        {
            _chatAppContext = chatAppContext;
        }
        public async Task<List<Chat>> GetListChat(int userId)
        {
            var entities = await _chatAppContext.Chats
                .Where(x => x.UserId1 == userId)
                .ToListAsync();

            //    var result = entities.Select(x => _userFunction.GetUserById(x.FriendId));

            //    if (result == null) result = new List<User>();

            return entities;
        }
    }
}
