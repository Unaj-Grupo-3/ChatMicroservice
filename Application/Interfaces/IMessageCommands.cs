
namespace Application.Interface
{
    public interface IMessageCommands
    {
        Task<int> AddMessage(int fromUserId, int toUserId, string message);
    }
}
