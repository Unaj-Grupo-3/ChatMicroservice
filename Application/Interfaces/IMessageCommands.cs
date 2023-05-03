
using Application.Models;
using Application.Reponsive;
using Domain.Entities;

namespace Application.Interface
{
    public interface IMessageCommands
    {
        Task<Message> CreateMessage(Message message);
        Task<Message> UpdateIsReadMessage(int messageId);
    }
}
