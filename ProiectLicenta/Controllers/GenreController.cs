using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProiectLicenta.Data.Auth;
using ProiectLicenta.DTOs.Create;
using ProiectLicenta.Entities;
using ProiectLicenta.Repositories.Interfaces;
using System.Data;

namespace ProiectLicenta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : GenericController<GenreCreateDTO, Genre>
    {
        IGenericRepository<Genre> _repository;
        protected MapperConfiguration configuration;
        Mapper mapper;
        public GenreController(IGenericRepository<Genre> repository) : base(repository)
        {
            this._repository = repository;
            configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GenreCreateDTO, Genre>();
            });
            mapper = new Mapper(configuration);
        }
        [HttpPost]
       // [Authorize(Roles = "Artist,Admin")]
        public virtual async Task<IActionResult> Create(GenreCreateDTO obj)
        {
            var result = mapper.Map<Genre>(obj);
            await _repository.Add(result);
            return Ok(obj);
        }
        [HttpPut("update")]
        [Authorize(Roles = UserRoles.Artist + "," + UserRoles.Admin)]
        public virtual async Task<IActionResult> Update(GenreCreateDTO obj)
        {
            var result = mapper.Map<Genre>(obj);
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
