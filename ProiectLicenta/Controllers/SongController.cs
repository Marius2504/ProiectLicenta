using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using ProiectLicenta.Data.Auth;
using ProiectLicenta.DTOs.Create;
using ProiectLicenta.Entities;
using ProiectLicenta.Repositories;
using ProiectLicenta.Repositories.Interfaces;
using System.IO;

namespace ProiectLicenta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : GenericController<SongCreateDTO,Song>
    {
        protected MapperConfiguration configuration;
        Mapper mapper;
        private readonly SongRepository _repository;
        private readonly UserManager<AppUser> _userManager;

        public SongController(SongRepository repository, UserManager<AppUser> repo):base(repository)
        {
            this._repository = repository;
            this._userManager = repo;
            configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SongCreateDTO, Song>().ReverseMap();
            });
            mapper = new Mapper(configuration);
        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var obj = await _repository.GetByName(name);
            return Ok(obj);
        }
        [HttpGet("album/{id}")]
        public IActionResult GetSongsFromAlbumId(int id)
        {
            var songs =  _repository.GetAllQuerry().Where(q => q.AlbumId == id);
            return Ok(songs);
        }

        [HttpGet("artist/{id}")]
        public IActionResult GetSongsFromArtistId(int id)
        {
            var songs = _repository.GetAllQuerry().Where(q => q.ArtistId == id);
            return Ok(songs);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAudioFile(IFormFile file)
        {
            /* 
             * the content types of Wav are many
             * audio/wave
             * audio/wav
             * audio/x-wav
             * audio/x-pn-wav
             * see "https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types"
            */
            if (!file.ContentType.Contains("audio"))
            {
                return BadRequest("Wrong file type");
            }
           // var uploads = Path.Combine(HostingEnvironment.WebRootPath, "uploads");//uploads where you want to save data inside wwwroot
          //  var filePath = Path.Combine(uploads, file.FileName);
            var filePath = @"e:\VS_Projects\"+file.Name;
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return Ok("File uploaded successfully");
        }
        
        [HttpPost]
        [Authorize(Roles =UserRoles.Artist + "," + UserRoles.Admin)]
        public virtual async Task<IActionResult> Create(SongCreateDTO obj)
        {
            var result = mapper.Map<Song>(obj);
            result.ImagePath = "";
            await _repository.Add(result);
            return Ok(obj);
        }
        

        [HttpPut("update")]
        [Authorize(Roles = UserRoles.Artist + "," + UserRoles.Admin)]
        public virtual async Task<IActionResult> Update(SongCreateDTO obj)
        {
            var result =await _repository.Get(obj.Id);

            if (result != null)
            {
                if (result.Name != obj.Name) result.Name = obj.Name;
                if (result.ImagePath != obj.ImagePath) result.ImagePath = obj.ImagePath;
                if (result.ServerLink != obj.ServerLink) result.ServerLink = obj.ServerLink;

                await _repository.Update(result);
                return Ok(obj);
            }
            return BadRequest("Song doesn't exist");
        }
        


        [HttpDelete("delete/{id}")]
        [Authorize(Roles = UserRoles.Artist + "," + UserRoles.Admin)]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var obj = GetById(id);
            await _repository.Delete(id);
            return Ok(obj);
        }
    }
}
