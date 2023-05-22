using System.Text.Json.Serialization;

namespace ProiectLicenta.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        
        public int ApplicationUserId { get; set; }
        [JsonIgnore]
        public ApplicationUser ApplicationUser { get; set; }
        public int SongId { get; set; }
        [JsonIgnore]
        public Song Song { get; set; }
        [JsonIgnore]
        public List<ApplicationUser> LikesFromUsers { get; set; }
    }
}
