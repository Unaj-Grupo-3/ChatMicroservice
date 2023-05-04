
using Application.Reponsive;

namespace Application.Interfaces
{
    public interface IUserApiServices
    {
        Task<List<UserResponse>> GetUserById(List<int> userIds);
    }
}
