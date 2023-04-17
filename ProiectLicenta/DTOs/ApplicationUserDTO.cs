using Microsoft.AspNetCore.Identity;

namespace ProiectLicenta.DTOs
{
    public class ApplicationUserDTO:IdentityUser
    {
        public string Name { get; set; }
    }
}
