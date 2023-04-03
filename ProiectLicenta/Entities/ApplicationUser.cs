using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace ProiectLicenta.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
        [JsonIgnore]
        public Client Client { get; set; }
        [JsonIgnore]
        public Artist Artist { get; set; }
    }
}
