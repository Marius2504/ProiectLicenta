using System.Text.Json.Serialization;

namespace ProiectLicenta.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public List<Song> Songs { get; set; }
    }
}
