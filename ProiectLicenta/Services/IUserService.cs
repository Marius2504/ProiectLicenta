using ProiectLicenta.DTOs;
using ProiectLicenta.Entities;
using ProiectLicenta.Entities.Register;

namespace ProiectLicenta.Services
{
    public interface IUserService
    {
        Task<ApplicationUser?> Create(RegisterUser user);
        Task<string> GetConfirmationEmail(string id);
        Task<bool> UserExists(string email);
        Task<bool> Login(string email,string password);
        Task Logout();
        Task<bool> DeleteUser(string name);
        Task<ApplicationUserDTO> GetById(string id);
        Task<List<ApplicationUserDTO>> GetAll();
        Task<ApplicationUserDTO> Update(ApplicationUserDTO dto);
        Task<List<string>> GetRoles(string id);
        Task<bool> GetConfirmationEmail(string id,string code);
        Task<string> GeneratePasswordToken(string id);
        Task<bool> VerifyToken(string id,string code);
    }
}
