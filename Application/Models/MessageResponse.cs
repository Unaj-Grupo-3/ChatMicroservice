namespace Application.Reponsive
{
    public class MessageResponse
    {
        public int Id { get; set; }
        public Guid FromUserId { get; set; }
        public int ChatId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SendDateTime { get; set; }
        public bool IsRead { get; set; }
    }
}
