namespace Application.Reponsive
{
    public class MessageResponse
    {
        public int Id { get; set; }
        public int FromUserId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SendDateTime { get; set; }
        public bool IsRead { get; set; }
    }
}
