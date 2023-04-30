
using Domain.Entities;

namespace Application.Interface
{
    public interface IChatCommands
    {
        Task<Chat> CreateChat(Chat chat);
    }
}
