using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProiectLicenta.Data;
using ProiectLicenta.Data.Auth;
using ProiectLicenta.DTOs;
using ProiectLicenta.DTOs.Create;
using ProiectLicenta.Email;
using ProiectLicenta.Entities;
using ProiectLicenta.Entities.Login;
using ProiectLicenta.Entities.Register;
using ProiectLicenta.Repositories;
using ProiectLicenta.Repositories.Interfaces;
using ProiectLicenta.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;

        public UserController(IUserService userService, IEmailSender email, ArtistRepository artistRepository, ClientRepository clientRepository,
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager,
            DataContext dataContext, IConfiguration configuration)
        {
            this._userService = userService;
            this._email = email;
            this._artistRepository = artistRepository;
            this._clientRepository = clientRepository;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
            this._dataContext = dataContext;
            this._configuration = configuration;
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUser user)
        {
            var currentUser = await _userManager.FindByEmailAsync(user.Email);
            if (currentUser != null && await _userManager.CheckPasswordAsync(currentUser, user.Password))
            {
                var roles = await _userManager.GetRolesAsync(currentUser);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                foreach (var role in roles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });

            }
            return Unauthorized();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUser user)
        {
            var userExists = await _userManager.FindByNameAsync(user.UserName);
            var userExistsByEmail = await _userManager.FindByEmailAsync(user.Email);
            if (userExists != null || userExistsByEmail!=null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser currentUser = new()
            {
                Email = user.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                Name = user.UserName,
                UserName = user.UserName,
            };
            var result = await _userManager.CreateAsync(currentUser, user.Password);

            if (!result.Succeeded) 
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.Client))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Client));
            if (!await _roleManager.RoleExistsAsync(UserRoles.Artist))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Artist));

            if (user.isArtist)
            {
                if (await _roleManager.RoleExistsAsync(UserRoles.Artist))
                {
                    await _userManager.AddToRoleAsync(currentUser, UserRoles.Artist);
                }
            }
            //var code = await _userService.GetConfirmationEmail(currentUser.Id);
            //var link = Url.Action("VerifyEmail", "User", new { id = currentUser.Id, code }, "https", "localhost:7255");
            //await _email.Send(currentUser.Email, "Email verification", $"<a href=\"{link}\">Verify Email</a>");
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
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
