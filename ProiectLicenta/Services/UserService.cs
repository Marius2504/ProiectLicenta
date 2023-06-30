using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Data;
using ProiectLicenta.DTOs;
using ProiectLicenta.DTOs.Create;
using ProiectLicenta.Email;
using ProiectLicenta.Entities;
using ProiectLicenta.Entities.Register;
using ProiectLicenta.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ProiectLicenta.Services
{
    public class UserService:IUserService
    {
        private readonly IEmailSender _email;
        private readonly ArtistRepository _artistRepository;
        private readonly ClientRepository _clientRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly DataContext _dataContext;
        private readonly AlbumRepository _albumRepository;
        protected MapperConfiguration configuration;
        Mapper mapper;

        public UserService(IEmailSender email, ArtistRepository artistRepository, ClientRepository clientRepository, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, DataContext dataContext, AlbumRepository albumRepository)
        {
            this._email = email;
            this._artistRepository = artistRepository;
            this._clientRepository = clientRepository;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._albumRepository = albumRepository;
            this.roleManager = roleManager;
            this._dataContext = dataContext;
            configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AppUser, AppUserDTO>().ReverseMap(); 
            });
            mapper = new Mapper(configuration);
            
        }
        public async Task<AppUser?> Create(RegisterUser user)
        {
            AppUser currentUser = new AppUser();
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
                    artist.AppUserId = currentUser.Id;
                    artist.Description = "";
                    await _artistRepository.Add(artist);

                }
                else
                {
                    await _userManager.AddToRoleAsync(currentUser, "Client");
                    Client client = new Client();
                    client.Name = user.UserName;
                    client.AppUserId = currentUser.Id;
                    client.Age = 0;
                    client.Email = "";
                    await _clientRepository.Add(client);
                }

                await _signInManager.SignInAsync(currentUser, isPersistent: false);

                return currentUser;
            }
            return null;
        }

        public async Task<bool> DeleteUser(string id)
        {
            var user = await GetUserWithInclude(id);
            if (user != null)
            {
                user.LikedSongs.Clear();
                user.Messages.Clear();
                await Update(user);
                user = await GetUserWithInclude(id);

                if(user.Artist!= null)
                {
                    var artist = await _artistRepository.GetArtistWithIncludes(user.Artist.Id);
                    foreach(var album in artist.Albums.ToList())
                    {
                        await _albumRepository.Delete(album.Id);
                    }
                }
                await _userManager.DeleteAsync(user);
            }
            return true;
        }

        public async Task<string> GeneratePasswordToken(string id)
        {
            var currentUser = await _userManager.FindByIdAsync(id);
            return await _userManager.GeneratePasswordResetTokenAsync(currentUser);
        }

        public async Task<List<AppUserDTO>> GetAll()
        {
            var users = await _userManager.Users.ToListAsync();
            var list = new List<AppUserDTO>();
            foreach (var user in users)
            {
                list.Add(mapper.Map<AppUserDTO>(user));
            }
            
            return list;
        }

        public async Task<AppUserDTO> GetByEmail(string email)
        {
            var user = await _dataContext.Users.Include(x => x.LikedSongs).Include(a => a.Messages).FirstOrDefaultAsync(a => a.Email == email);
            return mapper.Map<AppUserDTO>(user);
        }

        public async Task<AppUserDTO> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return mapper.Map<AppUserDTO>(user);
        }

        public async Task<string> GetConfirmationEmail(string id)
        {
            var currentUser = await _userManager.FindByIdAsync(id);
            return await _userManager.GenerateEmailConfirmationTokenAsync(currentUser);
        }

        public async Task<bool> GetConfirmationEmail(string id,string code)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return result.Succeeded;
        }

        public async Task<List<string>> GetRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return (List<string>)await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> Login(string email, string password)
        {
            var user = await _userManager.FindByIdAsync(email);
            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: true);
           
            return result !=null;
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();      
        }
       
        public async Task<AppUserDTO> Update(AppUser dto)
        {
            var user = await _userManager.FindByIdAsync(dto.Id);
            if (user != null)
            {
                if (user.Name != dto.Name) user.Name = dto.Name;
                if (user.Email != dto.Email) 
                { 
                    user.Email = dto.Email;
                    user.NormalizedEmail = dto.Email.ToUpper();
                }
                if(user.EmailConfirmed != dto.EmailConfirmed) user.EmailConfirmed = dto.EmailConfirmed;
                if (user.PhoneNumber != dto.PhoneNumber) user.PhoneNumber = dto.PhoneNumber;
                if (user.ImagePath != dto.ImagePath) user.ImagePath = dto.ImagePath;

                var entry = _dataContext.Entry(user);
                entry.State= EntityState.Modified;
               // _dataContext.Entry(user).State = EntityState.Modified;
                await _dataContext.SaveChangesAsync();
            }
            return await GetById(dto.Id);
        }

        public async Task<bool> UserExists(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        public async Task<bool> VerifyToken(string id, string code)
        {
            var user= await _userManager.FindByIdAsync(id); 
            var result = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, UserManager<AppUser>.ResetPasswordTokenPurpose, code);
            return result;
        }
        public async Task<AppUser?> GetUserWithInclude(string id)
        {
            var user = await _dataContext.Users.Include(x => x.LikedSongs).Include(a => a.Messages).Include(a=> a.Artist).Include(a=>a.Client).FirstOrDefaultAsync(a => a.Id == id);
            return user;

        }
    }
}

