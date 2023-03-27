using System.Text.Json.Serialization;

namespace ProiectLicenta.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public double Price { get; set; }
        public int EventId { get; set; }
        public int ClientId { get; set; }
        [JsonIgnore]
        public Event Event { get; set; }
        [JsonIgnore]
        public Client Client { get; set; }
        
    }
}
