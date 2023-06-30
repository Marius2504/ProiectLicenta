using ProiectLicenta.Entities;
using System.Text.Json.Serialization;

namespace ProiectLicenta.DTOs.Create
{
    public class SongCreateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ServerLink { get; set; }
        public string ImagePath { get; set; }
        public int ArtistId { get; set; }
        public int? AlbumId { get; set; }
        public int GenreId { get; set; }
        public int likes { get; set; }
        public List<Message>? Messages { get; set; }
    }
}
