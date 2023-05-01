
namespace Application.Models
{
    public class MessageRequest
    {
        public int FromUserId { get; set; }
        public int ChatId { get; set; }
        public string Content { get; set; }
    }
}
