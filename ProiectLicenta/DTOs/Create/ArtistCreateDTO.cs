using System.Text.Json.Serialization;

namespace ProiectLicenta.DTOs.Create
{
    public class ArtistCreateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string AppUserId { get; set; }

    }
}
