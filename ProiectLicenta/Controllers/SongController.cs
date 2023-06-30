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
        private readonly MessageRepository _messageRepository;

        public SongController(SongRepository repository, MessageRepository messageRepository):base(repository)
        {
            this._repository = repository;
            this._messageRepository = messageRepository;
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
        [HttpGet("includes/{id}")]
        public async Task<IActionResult> GetByIdWithIncludes(int id)
        {
            var obj = await _repository.GetSongWithIncludes(id);
            var objToSend = mapper.Map<SongCreateDTO>(obj);
            return Ok(objToSend);
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
        [HttpGet("trending/{start}/{cantity}")]
        public async Task<IActionResult> GetTrending(int start, int cantity)
        {
            var songs =await _repository.GetTrending(start, cantity);
            return Ok(songs);
        }

        [HttpPost]
       // [Authorize(Roles =UserRoles.Artist)]
        public virtual async Task<IActionResult> Create(SongCreateDTO obj)
        {
            var song = new Song();
            song.Name = obj.Name;
            song.Duration = 0;
            song.ServerLink = "";
            song.ImagePath = "";
            song.ArtistId = obj.ArtistId;
            song.AlbumId = obj.AlbumId;
            song.GenreId= obj.GenreId;
            
            await _repository.Add(song);
            song = await _repository.GetByName(song.Name);
            return Ok(song);
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
            var obj = await _repository.GetSongWithIncludes(id);
            if (obj != null)
            {
                await _repository.Delete(id);
                return Ok(obj);
            }
            return BadRequest("Item doesn't exist");
        }
        [HttpPost("addMessage")]
        //[Authorize()]
        public async Task<IActionResult> AddMessage(MessageCreateDTO message)
        {
            var song = await _repository.GetSongWithIncludes(message.SongId);
            if (song != null)
            {
                Message actual = new Message();
                actual.Text= message.Text;
                actual.WhoSentId= message.WhoSentId;
                actual.SongId= message.SongId;
                actual.Song = song;
                await _messageRepository.Add(actual);

                actual = await _messageRepository.Get(actual.Id);
                song.Messages.Add(actual);
                return Ok(actual);
            }
            return BadRequest("Wrong song id");
        }
        [HttpDelete("removeMessage/{songId}/{id}")]
        //[Authorize()]
        public async Task<IActionResult> DeleteMessage(int songId,int id)
        {
            var song = await _repository.GetSongWithIncludes(songId);
            var message = await _messageRepository.GetByIdWithIncludes(id);
            if (song != null)
            {
                if(message !=null)
                {
                    message.LikesFromUsers.Clear();
                    await _messageRepository.Update(message);
                    message = await _messageRepository.GetByIdWithIncludes(id);

                    song.Messages.Remove(message);
                    await _repository.Update(song);

                    
                    await _messageRepository.Delete(id);
                    return Ok(message);
                }
                return BadRequest("Wrong message id");      
            }
            return BadRequest("Wrong song id");
        }
    }
}

