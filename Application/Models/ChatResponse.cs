
using Application.Reponsive;

namespace Application.Models
{
    public class ChatResponse
    {
        public int User2Id { get; set; }
        public int User1Id { get; set; }
        public int ChatId { get; set; }
        public IList<MessageResponse>? Messages { get; set; }
    }
}
