using Application.Reponsive;
using Domain.Entities;

namespace Application.Interface
{
    public interface IUserFriendFunction
    {
        Task<IEnumerable<User>> GetListUserFriend(int userId);
    }
}
