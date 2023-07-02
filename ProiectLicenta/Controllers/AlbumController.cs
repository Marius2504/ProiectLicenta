using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Data.Auth;
using ProiectLicenta.DTOs.Create;
using ProiectLicenta.Entities;
using ProiectLicenta.Repositories;
using ProiectLicenta.Repositories.Interfaces;

namespace ProiectLicenta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumController : GenericController<AlbumCreateDTO, Album>
    {
        private readonly AlbumRepository _repository;
        private readonly SongRepository _songRepository;
        protected MapperConfiguration configuration;
        Mapper mapper;

        public AlbumController(AlbumRepository repository, SongRepository songRepository) : base(repository)
        {
            this._repository = repository;
            this._songRepository = songRepository;
            configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AlbumCreateDTO, Album>().ReverseMap();
            });
            mapper = new Mapper(configuration);
        }
        [HttpGet("artist/{id}")]
        public async Task<IActionResult> GetAlbumsFromArtistId(int id)
        {
            var albums =await _repository.GetAllQuerry().Where(q => q.ArtistId == id).ToListAsync();
            return Ok(albums);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Artist + "," + UserRoles.Admin)]
        public virtual async Task<IActionResult> Create(AlbumCreateDTO obj)
        {
            Album createAlbum = new Album();
            createAlbum.ArtistId = obj.ArtistId;
            createAlbum.Description= obj.Description;
            createAlbum.Name= obj.Name;
            createAlbum.Year= obj.Year;
            createAlbum.ImagePath = obj.ImagePath;
            await _repository.Add(createAlbum);
            createAlbum = await _repository.GetByName(obj.Name);           
           // Album createAlbum = await _repository.GetByName("The Slim Shady LP");
            return Ok(mapper.Map<AlbumCreateDTO>(createAlbum));
        }
        [HttpPut("update")]
        [Authorize(Roles = UserRoles.Artist + "," + UserRoles.Admin)]
        public virtual async Task<IActionResult> Update(AlbumCreateDTO obj)
        {           
            var album = await _repository.Get(obj.Id);
            if (album != null)
            {
                album.Name = obj.Name;
                album.Description = obj.Description;
                album.Year = obj.Year;
                album.ImagePath = obj.ImagePath;
                await _repository.Update(album);
                return Ok(obj);
            }
            return BadRequest("Album doesn't exist");
        }
       
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = UserRoles.Artist + "," + UserRoles.Admin)]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var obj =await _repository.GetAlbumWithIncludes(id);
            if (obj != null)
            {
                await _repository.Delete(id);
                return Ok(obj);
            }
            return BadRequest("Item doesn't exist");
        }

        
    }
}
