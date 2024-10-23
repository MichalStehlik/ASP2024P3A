using api_sk1_02files.Data;
using api_sk1_02files.InputModels;
using api_sk1_02files.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace api_sk1_02files.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StorageController> _logger;
        const string folder = "Storage";

        public StorageController(ApplicationDbContext context, ILogger<StorageController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");
            MemoryStream ms = new MemoryStream();
            MemoryStream tms = new MemoryStream();
            await file.CopyToAsync(ms);
            if (file.ContentType.StartsWith("image"))
            {
                using (Image image = Image.Load(ms.ToArray()))
                {
                    image.Mutate(x => x.Resize(100, 100));
                    image.SaveAsJpeg(tms);
                }
            }
            var item = new Item
            {
                ItemId = Guid.NewGuid(),
                Name = file.FileName,
                Created = DateTime.Now,
                Type = file.ContentType,
                Content = ms.ToArray(),
                Thumbnail = tms.Length != 0 ? tms.ToArray() : null,
            };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Database item {ItemId} created", item.ItemId);
            /*
            try
            {
                var path = Path.Combine(folder, item.ItemId.ToString());
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                _logger.LogInformation("File {Name} saved to {Path}", item.Name, path);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to save file " + item.ItemId);
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
                return StatusCode(500, "Failed to save file");
            }
            */
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            List<FileListVM> items = await _context.Items.Select(i => new FileListVM
            {
                ItemId = i.ItemId,
                Name = i.Name,
                Type = i.Type,
                Created = i.Created
            }).ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItem(Guid id)
        {
            FileInfoVM? item = await _context.Items.Where(i => i.ItemId == id).Select(i => new FileInfoVM
            {
                ItemId = i.ItemId,
                Name = i.Name,
                Type = i.Type,
                Created = i.Created
            }).FirstOrDefaultAsync();
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(Guid id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return NotFound();
            return File(item.Content, item.Type, item.Name);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return NotFound();
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{id}/thumbnail")]
        public async Task<IActionResult> GetThumbnail(Guid id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null || item.Thumbnail == null)
                return NotFound();
            return File(item.Thumbnail, item.Type, item.Name);
        }
    }
}
