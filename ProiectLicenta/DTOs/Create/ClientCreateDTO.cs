using System.Text.Json.Serialization;

namespace ProiectLicenta.DTOs.Create
{
    public class ClientCreateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        
    }
}
