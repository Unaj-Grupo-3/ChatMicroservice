
using Application.Models;

namespace Application.Interface
{
    public interface IChatServices
    {
        Task<ChatResponse> GetChatById(int id);
        Task<UserChat> GetChatsByUserId(int userId);
        Task<ChatResponse> CreateChat(ChatRequest request); 
    }
}
