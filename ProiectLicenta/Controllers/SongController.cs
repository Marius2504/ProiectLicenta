using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using ProiectLicenta.Data.Auth;
using ProiectLicenta.DTOs.Create;
using ProiectLicenta.Entities;
using ProiectLicenta.Repositories;
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

        public SongController(SongRepository repository):base(repository)
        {
            this._repository = repository;
            configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SongCreateDTO, Song>();
            });
            mapper = new Mapper(configuration);
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
        [HttpGet("all")]
        public async Task<IActionResult> GetAllSongs()
        {
            var filePath = @"e:\VS_Projects";
            var source = "data:audio/wav;base64," + Convert
                .ToBase64String(System.IO.File.ReadAllBytes(Path.Combine(filePath, "file")));
            return Ok(source);
        }
        [HttpPost]
        [Authorize(Roles =UserRoles.Artist + "," + UserRoles.Admin)]
        public virtual async Task<IActionResult> Create(SongCreateDTO obj)
        {
            var result = mapper.Map<Song>(obj);
            await _repository.Add(result);
            return Ok(obj);
        }
        [HttpPut("update")]
        [Authorize(Roles = UserRoles.Artist + "," + UserRoles.Admin)]
        public virtual async Task<IActionResult> Update(SongCreateDTO obj)
        {
            var result = mapper.Map<Song>(obj);
            await _repository.Update(result);
            return Ok(obj);
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
