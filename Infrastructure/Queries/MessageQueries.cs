using Application.Interface;
using Application.Reponsive;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Queries
{
    public class MessageQueries : IMessageQueries
    {
        private readonly ChatAppContext _chatAppContext;

        public MessageQueries(ChatAppContext chatAppContext)
        {
            _chatAppContext = chatAppContext;
        }
        public async Task<IEnumerable<LastestMessage>> GetLastestMessage(int userId)
        {
            var result = new List<LastestMessage>();

            var userFriends = await _chatAppContext.Chats
                .Where(x => x.UserId1 == userId || x.UserId2 == userId).ToListAsync();

            foreach (var userFriend in userFriends)
            {
                //var lastMessage = await _chatAppContext.Message
                //    .Where(x => x.FromUserId == userId && x.ToUserId == userFriend.FriendId
                //             || x.FromUserId == userFriend.FriendId && x.ToUserId == userId)
                //    .OrderByDescending(x => x.SendDateTime)
                //    .FirstOrDefaultAsync();

                //if (lastMessage != null)
                //{
                //    result.Add(new LastestMessage
                //    {
                //        UserId = userId,
                //        Content = lastMessage.Content,
                //        UserFriendInfo = _userFunction.GetUserById(userFriend.FriendId),
                //        Id = lastMessage.Id,
                //        IsRead = lastMessage.IsRead,
                //        SendDateTime = lastMessage.SendDateTime
                //    });
                //}
            }
            return result;
        }
        public async Task<IEnumerable<MessageResponse>> GetListMessagesId(int chatId)
        {
            var entities = await _chatAppContext.Messages
                .Where(x => x.ChatId == chatId)
                .OrderBy(c => c.SendDateTime)
                .ToListAsync();

            return entities.Select(x => new MessageResponse
            {
                Id = x.Id,
                Content = x.Content,
                FromUserId = x.FromUserId,
                ChatId = x.ChatId,
                SendDateTime = x.SendDateTime,
                IsRead = x.IsRead,
            });
        }
        public async Task<IEnumerable<MessageResponse>> GetListMessages(int pageSize, int pageIndex, int chatId)
        {
           // var entities = await _chatAppContext.Messages.OrderBy(x => x.SendDateTime).ToListAsync();
            //.Where(x => x.FromUserId == fromUserId && x.FromUserId == toUserId|| x.FromUserId == toUserId && x.ToUserId == fromUserId)
            var entities = await _chatAppContext.Messages
                .Where(x => x.ChatId == chatId)
                .OrderBy(c => c.SendDateTime)
                .Skip(pageSize * pageIndex)
                .Take(pageSize).ToListAsync();

            return entities.Select(x => new MessageResponse
            {
                Id = x.Id,
                Content = x.Content,
                FromUserId = x.FromUserId,
                ChatId = x.ChatId,
                SendDateTime = x.SendDateTime,
                IsRead = x.IsRead,
            });
        }
    }
}
