using ProiectLicenta.Entities;

namespace ProiectLicenta.DTOs.Create
{
    public class GenreCreateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImagePath { get; set; }
        public List<Song>? Songs { get; set; }
    }
}
