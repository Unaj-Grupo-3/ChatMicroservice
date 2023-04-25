using Microsoft.EntityFrameworkCore;
using Application.Reponsive;
using Application.Interface;
using Infrastructure.Persistance;
using Domain.Entities;

namespace Application.UseCase
{
    public class MessageFunction : IMessageFunction
    {
        ChatAppContext _chatAppContext;
        IUserFunction _userFunction;
        public MessageFunction(ChatAppContext chatAppContext, IUserFunction userFunction)
        {
            _chatAppContext = chatAppContext;
            _userFunction = userFunction;
        }

        public async Task<int> AddMessage(int fromUserId, int toUserId, string message)
        {
            var entity = new Message
            {
                FromUserId = fromUserId,
                ToUserId = toUserId,
                Content = message,
                SendDateTime = DateTime.Now,
                IsRead = false
            };

            _chatAppContext.Messages.Add(entity);
            var result = await _chatAppContext.SaveChangesAsync();

            return result;
        }

        public async Task<IEnumerable<LastestMessage>> GetLastestMessage(int userId)
        {
            var result = new List<LastestMessage>();

            var userFriends = await _chatAppContext.UserFriends
                .Where(x => x.UserId == userId).ToListAsync();

            foreach (var userFriend in userFriends)
            {
                var lastMessage = await _chatAppContext.Messages
                    .Where(x => x.FromUserId == userId && x.ToUserId == userFriend.FriendId
                             || x.FromUserId == userFriend.FriendId && x.ToUserId == userId)
                    .OrderByDescending(x => x.SendDateTime)
                    .FirstOrDefaultAsync();

                if (lastMessage != null)
                {
                    result.Add(new LastestMessage
                    {
                        UserId = userId,
                        Content = lastMessage.Content,
                        UserFriendInfo = _userFunction.GetUserById(userFriend.FriendId),
                        Id = lastMessage.Id,
                        IsRead = lastMessage.IsRead,
                        SendDateTime = lastMessage.SendDateTime
                    });
                }
            }
            return result;
        }

        public async Task<IEnumerable<MessageResponse>> GetMessages(int fromUserId, int toUserId)
        {
            var entities = await _chatAppContext.Messages
                .Where(x => x.FromUserId == fromUserId && x.ToUserId == toUserId
                    || x.FromUserId == toUserId && x.ToUserId == fromUserId)
                .OrderBy(x => x.SendDateTime)
                .ToListAsync();

            return entities.Select(x => new MessageResponse
            {
                Id = x.Id,
                Content = x.Content,
                FromUserId = x.FromUserId,
                ToUserId = x.ToUserId,
                SendDateTime = x.SendDateTime,
                IsRead = x.IsRead,
            });
        }
    }
}
