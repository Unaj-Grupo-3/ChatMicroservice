
using Domain.Entities;

namespace Application.Reponsive
{
    public class ListChatInitializeResponse
    {
        //public User User { get; set; } = null!;
        //public IEnumerable<User> UserFriends { get; set; } = null!;
        public IEnumerable<LastestMessage> LastestMessages { get; set; } = null!;
    }
}
