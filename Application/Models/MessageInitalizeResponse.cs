

namespace Application.Reponsive
{
    public class MessageInitalizeResponse
    {  
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int TotalItems { get; set; }
        public int ChatId { get; set; }
        public UserResponse? UserMe { get; set; }
        public UserResponse? UserFriend{ get; set; }
        public IEnumerable<MessageResponse> Messages { get; set; } = null!;

    }
}
                    