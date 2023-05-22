
using Microsoft.AspNetCore.Authentication;
using System.Text.Json.Serialization;

namespace ProiectLicenta.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
        public string ImagePath { get; set; }
        public int LocationId { get; set; }
        [JsonIgnore]
        public Location Location { get; set; }
        [JsonIgnore]
        public List<Ticket> Tickets { get; set; }
        [JsonIgnore]
        public List<Artist> Artists { get; set; }
    }
}
