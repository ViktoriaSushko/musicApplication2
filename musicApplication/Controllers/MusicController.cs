using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using musicApplication.Data;
using musicApplication.Model;
using musicApplication.Services;

namespace musicApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly S3Service s3;
        public MusicController(AppDbContext context, S3Service s3)
        {
            this.context = context;
            this.s3 = s3;
        }
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file,string title,string artist,string genre)
        {
            var key = await s3.UploadFileAsync(file);
            var song = new Song()
            {
                Title = title,
                Artist = artist,
                Genre = genre,
                FileName = file.FileName,
                FileSize = file.Length,
                S3Key = key,
                S3Url = s3.GetFileUrl(key),
                UploadData=DateTime.UtcNow
            };
            context.Songs.Add(song);
            await context.SaveChangesAsync();
            return Ok(song);

        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await context.Songs.ToListAsync());
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var song = await context.Songs.FindAsync(id);
            if (song == null) return NotFound();
            await s3.DeleteFileAsync(song.S3Key);
            context.Songs.Remove(song);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
