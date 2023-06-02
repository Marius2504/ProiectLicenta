using ProiectLicenta.Entities;
using System.Text.Json.Serialization;

namespace ProiectLicenta.DTOs.Create
{
    public class PlaylistCreateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public List<Song>? Songs { get; set; }
       
    }
}
