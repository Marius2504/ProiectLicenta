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
        protected MapperConfiguration configuration;
        Mapper mapper;
        public EventController(EventRepository eventRepository) : base(eventRepository)
        {
            this._eventRepository = eventRepository;
            configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EventCreateDTO, Event>().ReverseMap();
            });
            mapper = new Mapper(configuration);
        }

        [HttpGet("artist/{id}")]
        public IActionResult GetEventsFromArtistId(int id)
        {
            var songs = _eventRepository.GetAllQuerry().Where(q => q.Artists.Any(artist => artist.Id == id));
            return Ok(songs);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public virtual async Task<IActionResult> Create(EventCreateDTO obj)
        {
            var result = mapper.Map<Event>(obj);
            await _eventRepository.Add(result);
            return Ok(obj);
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
            var obj = GetById(id);
            await _eventRepository.Delete(id);
            return Ok(obj);
        }

    }
}
