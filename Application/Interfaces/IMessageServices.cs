

using Application.Reponsive;

namespace Application.Interface
{
    public interface IMessageServices
    {
        Task<IEnumerable<MessageResponse>> GetMessages(int pageSize, int pageIndex, int chatId);
        Task<int> GetMessagesLong(int chatId);
    }
}
