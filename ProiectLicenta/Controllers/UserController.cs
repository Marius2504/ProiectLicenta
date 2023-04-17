using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Data;
using ProiectLicenta.DTOs;
using ProiectLicenta.DTOs.Create;
using ProiectLicenta.Email;
using ProiectLicenta.Entities;
using ProiectLicenta.Entities.Login;
using ProiectLicenta.Entities.Register;
using ProiectLicenta.Repositories;
using ProiectLicenta.Repositories.Interfaces;
using ProiectLicenta.Services;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text.Unicode;

namespace ProiectLicenta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailSender _email;
        private readonly ArtistRepository _artistRepository;
        private readonly ClientRepository _clientRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly DataContext _dataContext;
       
        public UserController(IUserService userService,IEmailSender email, ArtistRepository artistRepository, ClientRepository clientRepository, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, DataContext dataContext)
        {
            this._userService = userService;
            this._email = email;
            this._artistRepository = artistRepository;
            this._clientRepository = clientRepository;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this.roleManager = roleManager;
            this._dataContext = dataContext;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUser user)
        {
            var currentUser = await _userService.Create(user);
            if (currentUser !=null)
            {
                //Confirmation e-mail
                var code = await _userService.GetConfirmationEmail(currentUser.Id);
                var link = Url.Action("VerifyEmail", "User", new { id = currentUser.Id, code }, "https", "localhost:7255");
                await _email.Send(currentUser.Email, "Email verification", $"<a href=\"{link}\">Verify Email</a>");

                return Ok("You need to confirm e-mail");
            }
            return BadRequest();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUser user)
        {
            var result = await _userService.UserExists(user.Email);
            if (result)
            {
                var resultLogin =await _userService.Login(user.Email,user.Password);
                if (resultLogin)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("You have been locked out");
                }

            }
            return BadRequest("Invalid username or password");
        }
        
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _userService.Logout();
            return Ok();
        }

        [HttpDelete("delete/{email}")]
        [Authorize("Admin")]
        public async Task<IActionResult> Delete(string email)
        {
            var result = await _userService.UserExists(email);
            if (result)
            {
                var resultDelete = await _userService.DeleteUser(email);
                if (resultDelete == false)
                {
                    return BadRequest("The user haven't been deleted");
                }
                return Ok();
            }
            return BadRequest("The user no longer exists");
        }
        [HttpGet("all")]
        [Authorize("Admin")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _userService.GetAll();
            return Ok(list);
        }
        [HttpGet("{id}")]
        [Authorize("Admin")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetById(id);
            if (user != null)
            {
                return Ok(user);
            }
            return BadRequest("User no longer exists");
        }
        [HttpPut]
        [Authorize("Client,Admin")]
        public async Task<IActionResult> Update(ApplicationUserDTO user)
        {
            if (user != null)
            {
                var currentUser = await _userService.Update(user);
                if(currentUser.Equals(user))
                {
                    return Ok(currentUser);
                }
                return BadRequest("Error while updating");
            }
            return BadRequest("Null user");
        }
        [HttpGet("roles")]
        [Authorize("Admin")]
        public async Task<IActionResult> GetUserWithRoles(string id)
        {
            var user = await _userService.GetById(id);
            if (user != null)
            {
                var roles = await _userService.GetRoles(id); 
                return Ok(new { user, roles });
            }
            return BadRequest("User doesn't exist");
        }
        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string id, string code)
        {
            var user = await _userService.GetById(id);
            if (user == null)
            {
                return BadRequest();
            }
            var result = await _userService.GetConfirmationEmail(id, code);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpPost("resetPassword/{id}")]
        public async Task<IActionResult> LostPassword(string id)
        {
            var currentUser = await _userService.GetById(id);
            var code = await _userService.GeneratePasswordToken(id);
            var link = Url.Action("ResetPassword", "User", new { id = currentUser.Id, code }, "https", "localhost:7255");
            await _email.Send(currentUser.Email, "Reset password", $"<a href=\"{link}\">Reset password</a>");
            return Ok("Email sent");
        }
        [HttpGet("reset")]
        public async Task<IActionResult> ResetPassword(string id, string code)
        {
            var user = await _userService.GetById(id);
            if (user == null) return BadRequest();
            var result = await _userService.VerifyToken(id, code);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
