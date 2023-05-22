

using Application.Models;
using Application.Reponsive;

namespace Application.Interface
{
    public interface IMessageQueries
    {
        Task<IEnumerable<MessageResponse>> GetListMessages(int pageSize, int pageIndex, int chatId);
        Task<IEnumerable<MessageResponse>> GetListMessagesId(int chatId);
        Task<IList<MessageSimple>> GetMessageByListId(IEnumerable<int> ids);
    }
}
