using System.Text.Json.Serialization;

namespace ProiectLicenta.DTOs.Create
{
    public class LocationCreateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        
    }
}
