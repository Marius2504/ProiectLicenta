using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace ProiectLicenta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        [HttpPost("profile"), DisableRequestSizeLimit]
        public async Task<IActionResult> Upload()
        {
            return await StoreImage("profile");
        }

        [HttpPost("song"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadSong()
        {
            return await StoreImage("song");
        }

        [HttpPost("songFile"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadSongFile()
        {
            return await StoreImage("songFile");
        }

        [HttpPost("album"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadAlbum()
        {
            return await StoreImage("album");
        }
        [HttpPost("playlist"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadPlaylist()
        {
            return await StoreImage("playlist");
        }
        [HttpPost("event"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadEvent()
        {
            return await StoreImage("event");
        }
        [HttpPost("genre"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadGenre()
        {
            return await StoreImage("genre");
        }

        private async Task<IActionResult> StoreImage(string section)
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                var userId = formCollection.Keys.First();
                var folderName = "";

                if (section == "songFile")
                {
                    folderName = Path.Combine("Resources", "Songs");

                    var validExtensions = new[] { ".mp3", ".wav" }; // Extensiile valide pentru fișierele audio

                    var fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (!validExtensions.Contains(fileExtension))
                    {
                        return BadRequest("Wrong format song");
                    }
                }
                else
                {
                    folderName = Path.Combine("Resources", "Images");
                    folderName = Path.Combine(folderName, section);

                    var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" }; // Extensiile valide pentru fișierele de imagine

                    var fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (!validExtensions.Contains(fileExtension))
                    {
                        return BadRequest("Wrong format image");
                    }
                }

                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    //var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                    var fileExtension = Path.GetExtension(file.FileName); // Obțineți extensia fișierului original
                    var fileName = $"{userId}{fileExtension}";
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return Ok(new { dbPath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}
