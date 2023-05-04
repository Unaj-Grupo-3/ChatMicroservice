namespace Domain.Entities
{
    public class Chat
    {
        public int Id { get; set; }
        public Guid UserId1 { get; set; }
        public Guid UserId2 { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public IList<Message> Messages { get; set;}
    }
}
