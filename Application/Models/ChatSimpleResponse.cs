using Application.Reponsive;

namespace Application.Models
{
    public class ChatSimpleResponse
    {
        public int User2Id { get; set; }
        public int ChatId { get; set; }
        public MessageResponse? LatestMesage { get; set; }
    }
}
