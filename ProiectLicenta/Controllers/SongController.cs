using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using System.IO;

namespace ProiectLicenta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> UploadAudioFile(IFormFile file)
        {
            /* 
             * the content types of Wav are many
             * audio/wave
             * audio/wav
             * audio/x-wav
             * audio/x-pn-wav
             * see "https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types"
            */
            if (!file.ContentType.Contains("audio"))
            {
                return BadRequest("Wrong file type");
            }
           // var uploads = Path.Combine(HostingEnvironment.WebRootPath, "uploads");//uploads where you want to save data inside wwwroot
          //  var filePath = Path.Combine(uploads, file.FileName);
            var filePath = @"e:\VS_Projects\"+file.Name;
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return Ok("File uploaded successfully");
        }
        [HttpGet]
        public async Task<IActionResult> GetAllSongs()
        {
            var filePath = @"e:\VS_Projects";
            var source = "data:audio/wav;base64," + Convert
                .ToBase64String(System.IO.File.ReadAllBytes(Path.Combine(filePath, "file")));
            return Ok(source);
        }
    }
}
