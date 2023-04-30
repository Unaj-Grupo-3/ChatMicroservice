﻿

using Application.Reponsive;

namespace Application.Interface
{
    public interface IMessageQueries
    {
        Task<IEnumerable<LastestMessage>> GetLastestMessage(int userId);
        Task<IEnumerable<MessageResponse>> GetListMessages(int pageSize, int pageIndex, int chatId);
        Task<IEnumerable<MessageResponse>> GetListMessagesId(int chatId);

    }
}
