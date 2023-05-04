

using Application.Models;
using Application.Reponsive;

using Application.Reponsive;

namespace Application.Interface
{
    public interface IMessageQuery
    {
        Task<MessageResponse> CreateMessage(MessageRequest request);
        Task<MessageResponse> UpdateIsReadMessage(int messageId);
        Task<IEnumerable<MessageResponse>> GetMessages(int pageSize, int pageIndex, int chatId);
        Task<int> GetMessagesLong(int chatId);
    }
}
