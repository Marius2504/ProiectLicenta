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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly DataContext _dataContext;
        protected MapperConfiguration configuration;
        Mapper mapper;

        public UserService(IEmailSender email, ArtistRepository artistRepository, ClientRepository clientRepository, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, DataContext dataContext)
        {
            this._email = email;
            this._artistRepository = artistRepository;
            this._clientRepository = clientRepository;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this.roleManager = roleManager;
            this._dataContext = dataContext;
            configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApplicationUser, ApplicationUserDTO>();
            });
            mapper = new Mapper(configuration);
        }
        public async Task<ApplicationUser?> Create(RegisterUser user)
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

                return currentUser;
            }
            return null;
        }

        public async Task<bool> DeleteUser(string email)
        {
            ApplicationUser user =await _userManager.FindByEmailAsync(email);
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
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<string> GeneratePasswordToken(string id)
        {
            var currentUser = await _userManager.FindByIdAsync(id);
            return await _userManager.GeneratePasswordResetTokenAsync(currentUser);
        }

        public async Task<List<ApplicationUserDTO>> GetAll()
        {
            var users = await _userManager.Users.ToListAsync();
            var list = new List<ApplicationUserDTO>();
            foreach (var user in users)
            {
                list.Add(mapper.Map<ApplicationUserDTO>(user));
            }
            
            return list;
        }

        public async Task<ApplicationUserDTO> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return mapper.Map<ApplicationUserDTO>(user);
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

        public async Task<ApplicationUserDTO> Update(ApplicationUserDTO dto)
        {
            var user = mapper.Map<ApplicationUser>(dto);
            if (user != null)
            {
                _dataContext.Entry(user).State = EntityState.Modified;
                await _dataContext.SaveChangesAsync();
            }
            return await GetById(dto.Id);
        }

        public async Task<bool> UserExists(string email)
        {
            var user = await _userManager.FindByIdAsync(email);
            return user != null;
        }

        public async Task<bool> VerifyToken(string id, string code)
        {
            var user= await _userManager.FindByIdAsync(id); 
            var result = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, UserManager<ApplicationUser>.ResetPasswordTokenPurpose, code);
            return result;
        }
    }
}
