using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProiectLicenta.Data.Auth;
using ProiectLicenta.DTOs.Create;
using ProiectLicenta.Entities;
using ProiectLicenta.Repositories;

namespace ProiectLicenta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : GenericController<TicketCreateDTO,Ticket>
    {
        private readonly TicketRepository _repository;
        protected MapperConfiguration configuration;
        Mapper mapper;
        public TicketController(TicketRepository repository):base(repository)
        {
            this._repository = repository;
            configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TicketCreateDTO, Ticket>().ReverseMap();
            });
            mapper = new Mapper(configuration);
        }
        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public virtual async Task<IActionResult> Create(TicketCreateDTO obj)
        {
            var result = mapper.Map<Ticket>(obj);
            await _repository.Add(result);
            return Ok(obj);
        }
        [HttpPut("update")]
        [Authorize(Roles = UserRoles.Admin)]
        public virtual async Task<IActionResult> Update(TicketCreateDTO obj)
        {
            var result = mapper.Map<Ticket>(obj);
            await _repository.Update(result);
            return Ok(obj);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var obj = await _repository.Get(id);
            if (obj != null)
            {
                await _repository.Delete(id);
                return Ok(obj);
            }
            return BadRequest("Item doesn't exist");
        }
    }
}
