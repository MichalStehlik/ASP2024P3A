using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api_sk1_02files.Data;
using api_sk1_02files.Models;

namespace api_sk1_02files.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicturesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PicturesController> _logger;

        public PicturesController(ApplicationDbContext context, ILogger<PicturesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Pictures
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Picture>>> GetPictures()
        {
            return await _context.Pictures.ToListAsync();
        }

        // GET: api/Pictures/5
        [HttpGet("{id}/{width}x{height}")]
        public async Task<ActionResult<Picture>> GetPicture(Guid id, int width, int height)
        {
            var picture = await _context.Pictures.Where(x => x.ItemId == id && x.Width == width && x.Height == height).FirstOrDefaultAsync();

            if (picture == null)
            {
                return NotFound();
            }

            return picture;
        }

        [HttpGet("{id}/{width}x{height}/download")]
        public async Task<ActionResult<Picture>> DownloadPicture(Guid id, int width, int height)
        {
            var picture = await _context.Pictures.Where(x => x.ItemId == id && x.Width == width && x.Height == height).FirstOrDefaultAsync();

            if (picture == null)
            {
                return NotFound();
            }

            return File(picture.Content, "image/jpeg");
        }

        // POST: api/Pictures
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public IActionResult PostPicture()
        {
            return BadRequest("Use POST /api/storage instead");
        }

        // DELETE: api/Pictures/5
        [HttpDelete("{id}/{width}x{height}")]
        public async Task<IActionResult> DeletePicture(Guid id, int width, int height)
        {
            var picture = await _context.Pictures.Where(x => x.ItemId == id && x.Width == width && x.Height == height).FirstOrDefaultAsync();
            if (picture == null)
            {
                return NotFound();
            }

            _context.Pictures.Remove(picture);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PictureExists(Guid id)
        {
            return _context.Pictures.Any(e => e.ItemId == id);
        }
    }
}
