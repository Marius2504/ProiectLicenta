using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProiectLicenta.Data.Auth;
using ProiectLicenta.DTOs.Create;
using ProiectLicenta.Entities;
using ProiectLicenta.Repositories;
using ProiectLicenta.Services;
using System.Data;

namespace ProiectLicenta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : GenericController<ArtistCreateDTO,Artist>
    {
        private readonly ArtistRepository _repository;
        private readonly UserService _userService;
        protected MapperConfiguration configuration;
        Mapper mapper;

        public ArtistController(ArtistRepository repository):base(repository)
        {
            this._repository = repository;
            configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ArtistCreateDTO, Artist>().ReverseMap();
            });
            mapper = new Mapper(configuration);
        }
        [HttpGet("user/{userId}")]
        [Authorize(Roles = UserRoles.Artist + "," + UserRoles.Admin)]
        public async Task<IActionResult> GetArtistOfUserId(string userId)
        {
            var artist = await _repository.GetArtistOfUser(userId);
            if(artist !=null)
            {
                return Ok(artist);
            }
            return BadRequest("This user is not related to an artist");
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public virtual async Task<IActionResult> Create(ArtistCreateDTO obj)
        {
            var result = mapper.Map<Artist>(obj);
            await _repository.Add(result);
            return Ok(obj);
        }
        [HttpPut("update")]
        [Authorize(Roles = UserRoles.Artist + "," + UserRoles.Admin)]
        public virtual async Task<IActionResult> Update(ArtistCreateDTO obj)
        {
            Artist artist =await _repository.Get(obj.Id);
            artist.Id= obj.Id;
            artist.Name = obj.Name;
            artist.Description = obj.Description;
            artist.ImagePath = obj.ImagePath;
            await _repository.Update(artist);
            
            return Ok(obj);
        }
        /*
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string AppUserId { get; set; }

        */
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var obj = GetById(id);
            await _repository.Delete(id);
            return Ok(obj);
        }
    }
}
