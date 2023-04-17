﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProiectLicenta.Data;
using ProiectLicenta.DTOs.Create;
using ProiectLicenta.Entities;
using ProiectLicenta.Repositories;
using ProiectLicenta.Repositories.Interfaces;

namespace ProiectLicenta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController<EntityDTO, Entity> : Controller where EntityDTO : class where Entity : class
    {
        private readonly IGenericRepository<Entity> _genericRepository;
        public readonly string _requestedRole = "";
        protected MapperConfiguration configuration;
        Mapper mapper;

        public GenericController(IGenericRepository<Entity> genericRepository)
        {
            this._genericRepository = genericRepository;
            configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EntityDTO, Entity>();
            });
            mapper = new Mapper(configuration);
        }
        [HttpGet]

        public virtual async Task<IActionResult> GetAll()
        {
            var listObj = await _genericRepository.GetAll();
            return Ok(listObj);
        }
        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(int id)
        {
            var obj = await _genericRepository.Get(id);
            return Ok(obj);
        }
        /*
        [HttpPost]
        
         public virtual async Task<IActionResult> Create(EntityDTO obj)
         { 
             var result = mapper.Map<Entity>(obj);
             await _genericRepository.Add(result);
             return Ok(obj);
         }
         [HttpPut("update")]
         public virtual async Task<IActionResult> Update(EntityDTO obj)
         {
             var result = mapper.Map<Entity>(obj);
             await _genericRepository.Update(result);
             return Ok(obj);
         }

         [HttpDelete("delete/{id}")]
         public virtual async Task<IActionResult> Delete(int id)
         {
             var obj = GetById(id);
             await _genericRepository.Delete(id);
             return Ok(obj);
         }

         */

    }
}
