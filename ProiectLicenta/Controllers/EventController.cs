using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProiectLicenta.DTOs.Create;
using ProiectLicenta.Entities;
using ProiectLicenta.Repositories;
using ProiectLicenta.Repositories.Interfaces;

namespace ProiectLicenta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : GenericController<EventCreateDTO,Event>
    {
        private readonly EventRepository _eventRepository;

        public EventController(EventRepository eventRepository) : base(eventRepository)
        {
            this._eventRepository = eventRepository;
        }

    }
}
