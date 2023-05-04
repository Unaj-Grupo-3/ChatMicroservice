
using Application.Reponsive;

namespace Application.Models
{
    public class ChatResponse
    {
        public Guid User2Id { get; set; }
        public Guid User1Id { get; set; }
        public int ChatId { get; set; }
        public IList<MessageResponse>? Messages { get; set; }
    }
}
