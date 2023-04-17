using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProiectLicenta.DTOs.Create;
using ProiectLicenta.Entities;
using ProiectLicenta.Repositories;
using System.Data;

namespace ProiectLicenta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : GenericController<ArtistCreateDTO,Artist>
    {
        private readonly ArtistRepository _repository;
        protected MapperConfiguration configuration;
        Mapper mapper;

        public ArtistController(ArtistRepository repository):base(repository)
        {
            this._repository = repository;
            configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ArtistCreateDTO, Artist>();
            });
            mapper = new Mapper(configuration);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public virtual async Task<IActionResult> Create(ArtistCreateDTO obj)
        {
            var result = mapper.Map<Artist>(obj);
            await _repository.Add(result);
            return Ok(obj);
        }
        [HttpPut("update")]
        [Authorize(Roles = "Admin")]
        public virtual async Task<IActionResult> Update(ArtistCreateDTO obj)
        {
            var result = mapper.Map<Artist>(obj);
            await _repository.Update(result);
            return Ok(obj);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var obj = GetById(id);
            await _repository.Delete(id);
            return Ok(obj);
        }
    }
}
