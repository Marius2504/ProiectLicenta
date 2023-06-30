using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProiectLicenta.Data.Auth;
using ProiectLicenta.DTOs.Create;
using ProiectLicenta.Entities;
using ProiectLicenta.Repositories;
using ProiectLicenta.Repositories.Interfaces;
using System.Data;

namespace ProiectLicenta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : GenericController<EventCreateDTO,Event>
    {
        private readonly EventRepository _eventRepository;
        private readonly ArtistRepository _artistRepository;
        protected MapperConfiguration configuration;
        Mapper mapper;
        public EventController(EventRepository eventRepository, ArtistRepository artistRepository) : base(eventRepository)
        {
            this._eventRepository = eventRepository;
            configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EventCreateDTO, Event>().ReverseMap();
            });
            mapper = new Mapper(configuration);
            _artistRepository = artistRepository;
        }

        [HttpGet("artist/{id}")]
        public IActionResult GetEventsFromArtistId(int id)
        {
            var events = _eventRepository.GetAllQuerry().Where(q => q.Artists.Any(artist => artist.Id == id));
            return Ok(events);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public virtual async Task<IActionResult> Create(EventCreateDTO obj)
        {
            Event newEvent = new Event();
            newEvent.Name= obj.Name;
            newEvent.DateTime= obj.DateTime;
            newEvent.ImagePath = "";
            newEvent.LocationId = obj.LocationId;
            newEvent = await _eventRepository.Add(newEvent);
            
            return Ok(newEvent);
        }
        [HttpPost("artist/add/{idEvent}/{idArtist}")]
        [Authorize(Roles = UserRoles.Admin)]
        public virtual async Task<IActionResult> AddArtist(int idEvent,int idArtist)
        {
            var artist = await _artistRepository.Get(idArtist);
            if (artist != null)
            {
                var result = await _eventRepository.GetByIdWithIncludes(idEvent);
                if (result != null)
                {
                    result.Artists.Add(artist);
                    result = await _eventRepository.Update(result);
                    return Ok(result);
                }
                return BadRequest("Event doesn't exist");
            }
            return BadRequest("Artist doesn't exist");
        }

        [HttpPost("artist/remove/{idEvent}/{idArtist}")]
        [Authorize(Roles = UserRoles.Admin)]
        public virtual async Task<IActionResult> RemoveArtist(int idEvent, int idArtist)
        {
            var artist = await _artistRepository.Get(idArtist);
            if (artist != null)
            {
                var result = await _eventRepository.GetByIdWithIncludes(idEvent);
                if (result != null)
                {
                    if (result.Artists.Contains(artist))
                    {
                        result.Artists.Remove(artist);
                        result = await _eventRepository.Update(result);
                        return Ok(result);
                    }
                    return BadRequest("Event doesn't have this artist");
                }
                return BadRequest("Event doesn't exist");
            }
            return BadRequest("Artist doesn't exist");
        }

        [HttpPut("update")]
        [Authorize(Roles = UserRoles.Admin)]
        public virtual async Task<IActionResult> Update(EventCreateDTO obj)
        {
            var result = mapper.Map<Event>(obj);
            await _eventRepository.Update(result);
            return Ok(obj);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var obj = await _eventRepository.Get(id);
            if (obj != null)
            {
                await _eventRepository.Delete(id);
                return Ok(obj);
            }
            return BadRequest("Item doesn't exist");
        }

    }
}
