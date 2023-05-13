using Application.Reponsive;

namespace Application.Models
{
    public class ChatSimpleResponse
    {
        public int ChatId { get; set; }
        public UserResponse? UserFriend { get; set; }
        public LastestMessage? LatestMessage { get; set; }
        public Paginacion? Paginacion { get; set; }
        public DateTime? Order { get; set; }
    }
}
