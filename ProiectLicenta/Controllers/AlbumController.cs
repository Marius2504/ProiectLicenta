using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    public class AlbumController : GenericController<AlbumCreateDTO, Album>
    {
        private readonly AlbumRepository _repository;
        protected MapperConfiguration configuration;
        Mapper mapper;

        public AlbumController(AlbumRepository repository) : base(repository)
        {
            this._repository = repository;
            configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AlbumCreateDTO, Album>();
            });
            mapper = new Mapper(configuration);
        }
        
        [HttpPost]
        [Authorize(Roles ="Artist,Admin")]
        public virtual async Task<IActionResult> Create(AlbumCreateDTO obj)
        { 
            var result = mapper.Map<Album>(obj);
            await _repository.Add(result);
            return Ok(obj);
        }
        [HttpPut("update")]
        [Authorize(Roles = "Artist,Admin")]
        public virtual async Task<IActionResult> Update(AlbumCreateDTO obj)
        {
            var result = mapper.Map<Album>(obj);
            await _repository.Update(result);
            return Ok(obj);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Artist,Admin")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var obj = GetById(id);
            await _repository.Delete(id);
            return Ok(obj);
        }

        
    }
}
