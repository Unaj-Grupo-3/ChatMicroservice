using Application.Interface;
using Application.Reponsive;
using Domain.Entities;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase
{
    public class UserFriendFunction : IUserFriendFunction
    {
        private readonly ChatAppContext _chatAppContext;
        private readonly IUserFunction _userFunction;
        public UserFriendFunction(ChatAppContext chatAppContext, IUserFunction userFunction)
        {
            _chatAppContext = chatAppContext;
            _userFunction = userFunction;
        }
        public async Task<IEnumerable<User>> GetListUserFriend(int userId)
        {
            var entities = await _chatAppContext.UserFriends
                .Where(x => x.UserId == userId)
                .ToListAsync();

            var result = entities.Select(x => _userFunction.GetUserById(x.FriendId));

            if (result == null) result = new List<User>();

            return result;
        }
    }
}
