using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectLicenta.Entities;
using ProiectLicenta.Repositories;

namespace ProiectLicenta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopController : ControllerBase
    {
        private readonly SongRepository _songRepository;

        public TopController(SongRepository songRepository)
        {
            this._songRepository = songRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetTop50()
        {
            List<Song> allSongs = await _songRepository.GetAllQuerry().
                OrderByDescending(x =>x.UsersWhoLiked.Count).Take(50).ToListAsync();
            return Ok(allSongs);
        }
  
    }
}
