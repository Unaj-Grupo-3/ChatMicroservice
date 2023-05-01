

using Application.Models;
using Application.Reponsive;

namespace Application.Interface
{
    public interface IMessageServices
    {
        Task<MessageResponse> CreateMessage(MessageRequest request);
        Task<MessageResponse> UpdateIsReadMessage(int messageId);
    }
}
