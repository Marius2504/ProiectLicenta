using ProiectLicenta.Entities;
using System.Text.Json.Serialization;

namespace ProiectLicenta.DTOs.Create
{
    public class MessageCreateDTO
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string WhoSentId { get; set; }
        public int SongId { get; set; }
        public SongCreateDTO? Song { get; set; }
        public List<AppUserDTO>? LikesFromUsers { get; set; }
    }
}
