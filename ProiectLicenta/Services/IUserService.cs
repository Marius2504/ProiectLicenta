using ProiectLicenta.DTOs;
using ProiectLicenta.Entities;
using ProiectLicenta.Entities.Register;

namespace ProiectLicenta.Services
{
    public interface IUserService
    {
        Task<AppUser?> Create(RegisterUser user);
        Task<string> GetConfirmationEmail(string id);
        Task<bool> UserExists(string email);
        Task<bool> Login(string email,string password);
        Task Logout();
        Task<bool> DeleteUser(string name);
        Task<AppUserDTO> GetById(string id);
        Task<AppUserDTO> GetByEmail(string email);
        Task<List<AppUserDTO>> GetAll();
        Task<AppUserDTO> Update(AppUserDTO dto);
        Task<List<string>> GetRoles(string id);
        Task<bool> GetConfirmationEmail(string id,string code);
        Task<string> GeneratePasswordToken(string id);
        Task<bool> VerifyToken(string id,string code);
        Task<AppUser?> GetUserWithInclude(string id);
    }
}
