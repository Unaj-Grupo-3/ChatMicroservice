namespace Domain.Entities
{
    public class Location
    {
        public int LocationId { get; set; }
        public int CityId { get; set; }
        public int ProvinceId { get; set; }
        public IList<User> Users { get; set; }
        public City City { get; set; }
        public Province Province { get; set; }
    }
}
