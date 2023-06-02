using System.Text.Json.Serialization;

namespace ProiectLicenta.DTOs.Create
{
    public class EventCreateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }=DateTime.Now;
        public int LocationId { get; set; }
       
    }
}
