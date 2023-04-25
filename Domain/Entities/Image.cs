namespace Domain.Entities
{
    public class Image
    {
        public int ImageId { get; set; }
        public int UserId { get; set; }
        public string Url { get; set; }
        public int Order { get; set; }
        public User User { get; set; }
    }
}
