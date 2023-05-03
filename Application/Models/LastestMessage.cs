

namespace Application.Reponsive
{
    public class LastestMessage
    {
        public string Content { get; set; } = null!;
        public DateTime SendDateTime { get; set; }
        public bool IsRead { get; set; }
    }
}
