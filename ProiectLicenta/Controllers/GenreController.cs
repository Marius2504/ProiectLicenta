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
                cfg.CreateMap<GenreCreateDTO, Genre>().ReverseMap();
            });
            mapper = new Mapper(configuration);
        }
        [HttpPost]
       // [Authorize(Roles = "Artist,Admin")]
        public virtual async Task<IActionResult> Create(GenreCreateDTO obj)
        {
            var result = mapper.Map<Genre>(obj);
            result.ImagePath = "";
            await _repository.Add(result);
            var returned = mapper.Map<GenreCreateDTO>(result);
            return Ok(returned);
        }
        [HttpPut("update")]
       // [Authorize(Roles = UserRoles.Artist + "," + UserRoles.Admin)]
        public virtual async Task<IActionResult> Update(GenreCreateDTO dto)
        {
            var genre =await _repository.Get(dto.Id);
            if (genre != null) {
                if (genre.Name != dto.Name) genre.Name = dto.Name;
                if (genre.ImagePath != dto.ImagePath) genre.ImagePath = dto.ImagePath;
                await _repository.Update(genre);
                return Ok(genre);
            }
            return BadRequest("Genre doesn't exist");
        }
        

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = UserRoles.Artist + "," + UserRoles.Admin)]
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
