
using Domain.Entities;

namespace Application.Reponsive
{
    public class MessageInitalizeResponse
    {
        //public User FriendInfo { get; set; } = null!;
        public IEnumerable<MessageResponse> Messages { get; set; } = null!;

    }
}
