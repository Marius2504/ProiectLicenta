using System.Text.Json.Serialization;

namespace ProiectLicenta.Entities
{
    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public List<Event> Events { get; set; }
        [JsonIgnore]
        public List<Album> Albums { get; set; }

        [JsonIgnore]
        public List<Song> Songs { get; set; }
    }
}
