
using Application.Models;

namespace Application.Interface
{
    public interface IChatServices
    {
        Task<ChatResponse> GetChatById(int id);
        Task<IList<ChatSimpleResponse>> GetChatsByUserId(Guid userId);
        Task<ChatResponse> CreateChat(ChatRequest request); 
    }
}
