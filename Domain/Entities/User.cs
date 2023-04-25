
namespace Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string Description { get; set; }
        public int? LocationId { get; set; }
        public string Gender { get; set; }
        public Guid AuthId { get; set; }
        public Location? Location { get; set; }
        public IList<Image>? Images { get; set; }

    }
}
