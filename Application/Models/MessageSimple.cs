
namespace Application.Models
{
    public class MessageSimple
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public int FromUserId { get; set; }
        public bool IsRead { get; set; }
    }
}
