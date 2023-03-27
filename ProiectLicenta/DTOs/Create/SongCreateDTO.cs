using System.Text.Json.Serialization;

namespace ProiectLicenta.DTOs.Create
{
    public class SongCreateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public string ServerLink { get; set; }

        public int ArtistId { get; set; }
        public int? AlbumId { get; set; }
        public int GenreId { get; set; }
    }
}
