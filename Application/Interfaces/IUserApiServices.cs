
using Application.Reponsive;

namespace Application.Interfaces
{
    public interface IUserApiServices
    {
        Task<UserResponse> GetUserById(int id);
    }
}
