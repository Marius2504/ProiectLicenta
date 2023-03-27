using System.Text.Json.Serialization;

namespace ProiectLicenta.Entities
{
    public class Location
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        [JsonIgnore]
        public Event Event { get; set; }
    }

}
