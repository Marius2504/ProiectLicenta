using System.Text.Json.Serialization;

namespace ProiectLicenta.DTOs.Create
{
    public class TicketCreateDTO
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public double Price { get; set; }
        public int EventId { get; set; }
        public int ClientId { get; set; }
    }
}
