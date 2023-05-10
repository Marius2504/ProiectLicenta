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
    [Authorize]
    public class ClientController : GenericController<ClientCreateDTO,Client>
    {
        private readonly ClientRepository _repository;
        protected MapperConfiguration configuration;
        Mapper mapper;
        public ClientController(ClientRepository repository):base(repository) 
        {
            this._repository = repository;
            configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ClientCreateDTO, Client>();
            });
            mapper = new Mapper(configuration);
        }
        [HttpPost]

        public virtual async Task<IActionResult> Create(ClientCreateDTO obj)
        {
            var result = mapper.Map<Client>(obj);
            await _repository.Add(result);
            return Ok(obj);
        }
        [HttpPut("update")]
        [Authorize(Roles = UserRoles.Client + "," + UserRoles.Admin)]
        public virtual async Task<IActionResult> Update(ClientCreateDTO obj)
        {
            var result = mapper.Map<Client>(obj);
            await _repository.Update(result);
            return Ok(obj);
        }

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
