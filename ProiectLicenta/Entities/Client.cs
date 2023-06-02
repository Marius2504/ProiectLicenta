using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProiectLicenta.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Email { get; set; }
        public int Age { get; set; }
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        [JsonIgnore]
        public AppUser AppUser { get; set; }
        [JsonIgnore]
        public List<Playlist> Playlists { get; set; }
        [JsonIgnore]
        public List<Ticket> Ticket { get; set; }
    }
}
