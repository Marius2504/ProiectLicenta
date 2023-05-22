using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace ProiectLicenta.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }

        [JsonIgnore]
        public List<Song> LikedSongs { get; set; }
        [JsonIgnore]
        public List<Message> Messages { get; set; }
        [JsonIgnore]
        public Client Client { get; set; }
        [JsonIgnore]
        public Artist Artist { get; set; }
    }
}
