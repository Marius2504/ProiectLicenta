using System.Text.Json.Serialization;

namespace ProiectLicenta.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string WhoSentId { get; set; }
        public int SongId { get; set; }
        [JsonIgnore]
        public Song Song { get; set; }
        [JsonIgnore]
        public List<AppUser> LikesFromUsers { get; set; }


    }
}
