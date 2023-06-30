using System.Text.Json.Serialization;

namespace ProiectLicenta.DTOs.Create
{
    public class AlbumCreateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string ImagePath { get; set; }
        public int Year { get; set; }
        public int ArtistId { get; set; }
    }
}
