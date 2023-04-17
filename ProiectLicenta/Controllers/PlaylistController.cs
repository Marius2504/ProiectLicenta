using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProiectLicenta.DTOs.Create;
using ProiectLicenta.Entities;
using ProiectLicenta.Repositories;
using ProiectLicenta.Repositories.Interfaces;
using System.Data;

namespace ProiectLicenta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistController : GenericController<PlaylistCreateDTO, Playlist>
    {
        private readonly PlaylistRepository _repository;
        protected MapperConfiguration configuration;
        Mapper mapper;
        public PlaylistController(PlaylistRepository repository) : base(repository)
        {
            this._repository = repository;
            configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PlaylistCreateDTO, Playlist>();
            });
            mapper = new Mapper(configuration);
        }
        [HttpPost]
        [Authorize()]
        public virtual async Task<IActionResult> Create(PlaylistCreateDTO obj)
        {
            var result = mapper.Map<Playlist>(obj);
            await _repository.Add(result);
            return Ok(obj);
        }
        [HttpPut("update")]
        [Authorize()]
        public virtual async Task<IActionResult> Update(PlaylistCreateDTO obj)
        {
            var result = mapper.Map<Playlist>(obj);
            await _repository.Update(result);
            return Ok(obj);
        }

        [HttpDelete("delete/{id}")]
        [Authorize()]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var obj = GetById(id);
            await _repository.Delete(id);
            return Ok(obj);
        }
    }
}
