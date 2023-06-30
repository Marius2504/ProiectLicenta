using System.Text.Json.Serialization;

namespace ProiectLicenta.Entities
{
    public class Song
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public string ServerLink { get; set; }

        public string ImagePath { get; set; }
        public int ArtistId { get; set; }
        public int? AlbumId { get; set; }
        public int GenreId { get; set; }
        public int Likes => UsersWhoLiked?.Count ?? 0;
        [JsonIgnore]
        public Artist Artist { get; set; }
        [JsonIgnore]
        public Album Album { get; set; }
        [JsonIgnore]
        public Genre Genre { get; set; }
        [JsonIgnore]
        public List<Message> Messages { get; set; }
        [JsonIgnore]
        public List<AppUser> UsersWhoLiked { get; set; }


    }
}
