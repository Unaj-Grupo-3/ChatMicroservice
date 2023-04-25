using Microsoft.EntityFrameworkCore;
using Application.Reponsive;
using Application.Interface;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Application.UseCase
{
    public class MessageFunction
    {
        ChatAppContext _chatAppContext;

        public MessageFunction(ChatAppContext chatAppContext)
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

            _chatAppContext.Message.Add(entity);
            var result = await _chatAppContext.SaveChangesAsync();

            return result;
        }

        public async Task<IEnumerable<LastestMessage>> GetLastestMessage(int userId)
        {
            var result = new List<LastestMessage>();

            var userFriends = await _chatAppContext.Chat
                .Where(x => x.UserId1 == userId || x.UserId2 == userId ).ToListAsync();

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

        //public async Task<IEnumerable<MessageResponse>> GetMessages(int fromUserId, int toUserId)
        //{
        //    var entities = await _chatAppContext.Message
        //        .Where(x => x.FromUserId == fromUserId && x.FromUserId == toUserId
        //            || x.FromUserId == toUserId && x.ToUserId == fromUserId)
        //        .OrderBy(x => x.SendDateTime)
        //        .ToListAsync();

        //    return entities.Select(x => new MessageResponse
        //    {
        //        Id = x.Id,
        //        Content = x.Content,
        //        FromUserId = x.FromUserId,
        //        ToUserId = x.ToUserId,
        //        SendDateTime = x.SendDateTime,
        //        IsRead = x.IsRead,
        //    });
        //}
    }
}
