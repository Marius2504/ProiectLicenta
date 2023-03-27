using System.Text.Json.Serialization;

namespace ProiectLicenta.Entities
{
    public class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<Song> Songs { get; set; }
        [JsonIgnore]
        public List<Client> Clients { get; set; }
    }
}
