namespace Domain.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int FromUserId { get; set; }
        public int ChatId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SendDateTime { get; set; }
        public bool IsRead { get; set; }
    }
}
