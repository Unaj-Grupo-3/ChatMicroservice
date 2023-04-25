using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class City
    {
        public int CityId { get; set; }
        public string Name { get; set; }
        public IList<Location> Location { get; set; }
    }
}
