
using Application.Reponsive;

namespace Application.Models
{
    public class UserChat
    {
        public UserResponse? UserMe { get; set; }
        public IList<ChatSimpleResponse> ListChat { get; set; }
    }
}
