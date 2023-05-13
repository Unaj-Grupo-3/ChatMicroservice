using Application.Models;
using Application.Reponsive;

namespace Application.Interface
{
    public interface IMessageServices
    {
        Task<MessageResponse> CreateMessage(MessageRequest request);
        Task UpdateIsRead(int Id);
        Task<IEnumerable<MessageResponse>> GetMessages(int pageSize, int pageIndex, int chatId);
        Task<int> GetMessagesLong(int chatId);
    }
}
