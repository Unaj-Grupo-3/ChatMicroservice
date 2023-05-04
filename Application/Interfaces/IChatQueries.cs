
using Domain.Entities;

namespace Application.Interface
{
    public interface IChatQueries
    {
        Task<Chat> GetChatById(int id);
        Task<IList<Chat>> GetChatsByUserId(Guid userId);
    }
}
