using Application.Reponsive;
using Domain.Entities;

namespace Application.Interface
{
    public interface IUserFunction
    {
        User GetUserById(int id);
    }
}
