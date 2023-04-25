using Application.Interface;
using Application.Reponsive;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase
{
    public class UserFriendFunction
    {
        private readonly ChatAppContext _chatAppContext;

        public UserFriendFunction(ChatAppContext chatAppContext)
        {
            _chatAppContext = chatAppContext;
        }
        //public async Task<IEnumerable<User>> GetListUserFriend(int userId)
        //{
        //    var entities = await _chatAppContext.UserFriends
        //        .Where(x => x.UserId == userId)
        //        .ToListAsync();

        //    var result = entities.Select(x => _userFunction.GetUserById(x.FriendId));

        //    if (result == null) result = new List<User>();

        //    return result;
        //}
    }
}
