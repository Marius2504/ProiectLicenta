using AutoMapper;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProiectLicenta.Data.Auth;
using ProiectLicenta.DTOs;
using ProiectLicenta.DTOs.Create;
using ProiectLicenta.Email;
using ProiectLicenta.Entities;
using ProiectLicenta.Entities.Login;
using ProiectLicenta.Entities.Register;
using ProiectLicenta.Repositories;
using ProiectLicenta.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProiectLicenta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailSender _email;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ArtistRepository _artistRepository;
        private readonly ClientRepository _clientRepository;
        private readonly SongRepository _songRepository;
        private readonly MessageRepository _messageRepository;

        protected MapperConfiguration configurationMap;
        Mapper mapper;

        public UserController(IUserService userService, IEmailSender email,
            UserManager<AppUser> userManager,RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ArtistRepository artistRepository,
            ClientRepository clientRepository,
            SongRepository songRepository,
            MessageRepository messageRepository
            )
        {
            this._userService = userService;
            this._email = email;
            this._userManager = userManager;
            this._roleManager = roleManager;    
            this._configuration = configuration;
            this._artistRepository = artistRepository;
            this._clientRepository = clientRepository;
            this._songRepository = songRepository;
            this._messageRepository = messageRepository;
            configurationMap = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AppUserDTO, AppUser>().ReverseMap(); 
            });
            mapper = new Mapper(configurationMap);
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
            return BadRequest("E-mail or password incorrect");
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUser user)
        {
            
            var userExists = await _userManager.FindByNameAsync(user.UserName);
            var userExistsByEmail = await _userManager.FindByEmailAsync(user.Email);
            if (userExists != null || userExistsByEmail!=null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            AppUser currentUser = new()
            {
                Email = user.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                Name = user.UserName,
                UserName = user.UserName,
                ImagePath = ""
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

                    Artist artist = new Artist();
                    artist.Name = user.UserName;
                    artist.AppUser = currentUser;
                    artist.AppUserId = currentUser.Id;
                    artist.Description = "no description";
                    artist.ImagePath = "";
                    await _artistRepository.Add(artist);
                }
            }
            else
            {
                if (await _roleManager.RoleExistsAsync(UserRoles.Client))
                {
                    await _userManager.AddToRoleAsync(currentUser, UserRoles.Admin);
                    Client client = new Client();
                    client.Name = user.UserName;
                    client.Email = user.Email;
                    client.Age = 18;
                    client.AppUser = currentUser;
                    client.AppUserId = currentUser.Id;
     
                    await _clientRepository.Add(client);
                }
            }
            var code = await _userService.GetConfirmationEmail(currentUser.Id);
            var link = Url.Action("VerifyEmail", "User", new { id = currentUser.Id, code }, "https", "localhost:7255");
            await _email.Send(currentUser.Email, "Email verification", $"Welcome to REMIX apllication <br><br> This e-mail is auto-generated and represents the verification of identity <a href=\"{link}\">Verify your e-mail</a>");
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

    

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userService.GetUserWithInclude(id);
            if (result != null)
            {
                await _userService.DeleteUser(id);
                return Ok();
            }
            return BadRequest("The user no longer exists");
        }
        [HttpGet("all")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> GetAll()
        {
            var list = await _userService.GetAll();
            return Ok(list);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetById(id);
            if (user != null)
            {
                return Ok(user);
            }
            return BadRequest("User no longer exists");
        }
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _userService.GetByEmail(email);
            if (user != null)
            {
                return Ok(user);
            }
            return BadRequest("User no longer exists");
        }
        [HttpPut("update")]
        [Authorize()]
        public async Task<IActionResult> Update(AppUserDTO dto)
        {
            var user = await _userManager.FindByIdAsync(dto.Id);
            var userName = await _userManager.FindByNameAsync(dto.Name);
            var userEmail = await _userManager.FindByEmailAsync(dto.Email);

            if((userName !=null && userName.Id != user.Id) ||
                (userEmail !=null && userEmail.Id != user.Id))
            {
                return BadRequest("Name or e-mail taken");
            }

            if(user !=null)
            {
                if (dto.Name != user.Name) user.Name = dto.Name;
                if (dto.ImagePath != user.ImagePath) user.ImagePath = dto.ImagePath;
                if (dto.Email !=user.Email) user.Email = dto.Email;
                if (dto.PhoneNumber != user.PhoneNumber) user.PhoneNumber = dto.PhoneNumber;
                if (dto.EmailConfirmed != null && dto.EmailConfirmed != user.EmailConfirmed) 
                { 
                     user.EmailConfirmed = (bool)dto.EmailConfirmed; 
                }

                var currentUser = await _userService.Update(user);
                return Ok(currentUser);
            }
            return BadRequest("Null user");
        }
        /*
        [HttpGet("roles/")]
        [Authorize(Roles = UserRoles.Admin)]
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
        */
        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string id, string code)
        {
            var user = await _userService.GetUserWithInclude(id);
            if (user == null)
            {
                return BadRequest();
            }
            var result = await _userService.GetConfirmationEmail(id, code);
            if (result)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
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

        [HttpPost("addLike/{userId}/{songId}")]
        public virtual async Task<IActionResult> Addlike(string userId, int songId)
        {
            var user = await _userService.GetUserWithInclude(userId);
            var song = await _songRepository.Get(songId);
            if (song != null)
            {
                if (!user.LikedSongs.Contains(song))
                {
                    user.LikedSongs.Add(song);
                    await _userService.Update(user);
                }
                return Ok(song);
            }
            return BadRequest("Invalid song id");
        }
        [HttpPost("removeLike/{userId}/{songId}")]
        public virtual async Task<IActionResult> Removelike(string userId, int songId)
        {
            var user = await _userService.GetUserWithInclude(userId);
            var song = await _songRepository.Get(songId);
            if (song != null)
            {
                if (user.LikedSongs.Contains(song))
                {
                    user.LikedSongs.Remove(song);
                    var userDTO = mapper.Map<AppUserDTO>(user);
                    await _userService.Update(user);
                }
                return Ok(song);
            }
            return BadRequest("Invalid song id");
        }

        [HttpPost("message/addLike/{userId}/{messageId}")]
        public virtual async Task<IActionResult> AddLikeToMessage(string userId, int messageId)
        {
            var user = await _userService.GetUserWithInclude(userId);
            var message = await _messageRepository.Get(messageId);
            if (message != null)
            {
                if (!user.Messages.Contains(message))
                {
                    user.Messages.Add(message);
                    var userDTO = mapper.Map<AppUserDTO>(user);
                    await _userService.Update(user);
                }
                return Ok(message);
            }
            return BadRequest("Invalid song id");
        }
        [HttpPost("message/removeLike/{userId}/{messageId}")]
        public virtual async Task<IActionResult> RemoveLikeToMessage(string userId, int messageId)
        {
            var user = await _userService.GetUserWithInclude(userId);
            var message = await _messageRepository.Get(messageId);
            if (message != null)
            {
                if (user.Messages.Contains(message))
                {
                    user.Messages.Remove(message);
                    var userDTO = mapper.Map<AppUserDTO>(user);
                    await _userService.Update(user);
                }
                return Ok(message);
            }
            return BadRequest("Invalid song id");
        }

    }
}
