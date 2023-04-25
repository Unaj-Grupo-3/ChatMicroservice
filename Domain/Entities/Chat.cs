namespace Domain.Entities
{
    public class Chat
    {
        public int Id { get; set; }
        public int UserId1 { get; set; }
        public int UserId2 { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
