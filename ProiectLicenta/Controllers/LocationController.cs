﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProiectLicenta.Data.Auth;
using ProiectLicenta.DTOs.Create;
using ProiectLicenta.Entities;
using ProiectLicenta.Repositories;
using System.Data;

namespace ProiectLicenta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : GenericController<LocationCreateDTO,Location>
    {
        private readonly LocationRepository _repository;
        protected MapperConfiguration configuration;
        Mapper mapper;
        public LocationController(LocationRepository repository):base(repository)
        {
            this._repository = repository;
            configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<LocationCreateDTO, Location>().ReverseMap();
            });
            mapper = new Mapper(configuration);
        }
        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public virtual async Task<IActionResult> Create(LocationCreateDTO obj)
        {
            var result = mapper.Map<Location>(obj);
            await _repository.Add(result);
            return Ok(obj);
        }
        [HttpPut("update")]
        [Authorize(Roles = UserRoles.Admin)]
        public virtual async Task<IActionResult> Update(LocationCreateDTO obj)
        {
            var result = mapper.Map<Location>(obj);
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
