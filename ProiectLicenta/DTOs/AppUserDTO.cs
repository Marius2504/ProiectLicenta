using Microsoft.AspNetCore.Identity;
using ProiectLicenta.Entities;
using System.Text.Json.Serialization;

namespace ProiectLicenta.DTOs
{
    public class AppUserDTO
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? ImagePath { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? EmailConfirmed { get; set; }
        public List<Song>? LikedSongs { get; set; }
        public List<Message>? Messages { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            AppUserDTO appUser = obj as AppUserDTO;
            return this.Id == appUser.Id &&
                this.Name == appUser.Name &&
                this.ImagePath == appUser.ImagePath &&
                this.Email == appUser.Email && 
                this.PhoneNumber == appUser.PhoneNumber;
        }
    }
}
