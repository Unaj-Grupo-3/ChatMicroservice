

namespace Application.Reponsive
{
    public class LastestMessage
    {
        public int fromUserId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SendDateTime { get; set; }
        public bool IsRead { get; set; }
    }
}
