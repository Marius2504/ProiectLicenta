using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Data;
using ProiectLicenta.DTOs.Create;
using ProiectLicenta.Entities;
using ProiectLicenta.Entities.Login;
using ProiectLicenta.Entities.Register;
using ProiectLicenta.Repositories;
using ProiectLicenta.Repositories.Interfaces;
using System.Security.Cryptography;
using System.Text.Unicode;

namespace ProiectLicenta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ArtistRepository _artistRepository;
        private readonly ClientRepository _clientRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly DataContext _dataContext;
       
        public UserController(ArtistRepository artistRepository, ClientRepository clientRepository, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, DataContext dataContext)
        {
            this._artistRepository = artistRepository;
            this._clientRepository = clientRepository;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._dataContext = dataContext;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUser user)
        {
            ApplicationUser currentUser = new ApplicationUser();
            currentUser.Email = user.Email;
            currentUser.UserName = user.UserName;
            currentUser.Name = user.UserName;
            var result = await _userManager.CreateAsync(currentUser, user.Password);
            await _dataContext.SaveChangesAsync();


            if (result.Succeeded)
            {
                currentUser = await _userManager.FindByIdAsync(currentUser.Id);
                if (user.isArtist == true)
                {
                    await _userManager.AddToRoleAsync(currentUser, "Artist");

                    Artist artist = new Artist();
                    artist.Name = user.UserName;
                    artist.ApplicationUserId = currentUser.Id;
                    artist.Description = "";
                    await _artistRepository.Add(artist);

                }
                else
                {
                    await _userManager.AddToRoleAsync(currentUser, "Client");
                    Client client = new Client();
                    client.Name = user.UserName;
                    client.ApplicationUserId = currentUser.Id;
                    client.Age = 0;
                    client.Email = "";
                    await _clientRepository.Add(client);
                }

                await _signInManager.SignInAsync(currentUser, isPersistent: false);

                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUser user)
        {
            ApplicationUser currentUser = await _userManager.FindByEmailAsync(user.Email);
            if (currentUser != null)
            {
                var result = await _signInManager.PasswordSignInAsync(currentUser, user.Password, isPersistent: false, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    return Ok();
                }
                if (result.IsLockedOut)
                {
                    return BadRequest("You have been locked out");
                }

            }
            return BadRequest("Invalid username or password");
        }
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpDelete("delete/{email}")]
        public async Task<IActionResult> Delete(string email)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                if (await _userManager.IsInRoleAsync(user, "Artist"))
                {
                    var artist = user.Artist;
                    await _artistRepository.Delete(artist.Id);
                }
                else
                {
                    var client = user.Client;
                    await _clientRepository.Delete(client.Id);
                }
                await _userManager.DeleteAsync(user);
                return Ok();
            }
            return BadRequest("The user no longer exists");
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _userManager.Users.ToListAsync();
            return Ok(list);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                return Ok(user);
            }
            return BadRequest("User no longer exists");
        }
        [HttpPut]
        public async Task<IActionResult> Update(ApplicationUser user)
        {
            if (user != null)
            {
                var currentUser = await _userManager.FindByIdAsync(user.Id);
                if (currentUser != null)
                {
                    // await _userManager.UpdateAsync(user);
                    _dataContext.Entry(user).State = EntityState.Modified;
                    _dataContext.SaveChanges();
                    return Ok();
                }
                return BadRequest("User doesn't exist");
            }
            return BadRequest("Null user");
        }
        [HttpGet("roles")]
        public async Task<IActionResult> GetUserWithRoles(string id)
        {
            var user =await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                return Ok(new { user, roles });
            }
            return BadRequest("User doesn't exist");
        }
        [HttpPut("password")]
        public async Task<IActionResult> LostPassword(string email, string password)
        {
            var user =await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = passwordHasher.HashPassword(user, password);
                _dataContext.Entry(user).State= EntityState.Modified;
                _dataContext.SaveChanges();
                return Ok();
            }
            return BadRequest("User doesn't exist");
        }
    }
}
