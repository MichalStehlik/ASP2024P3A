using api_sk1_02files.Data;
using api_sk1_02files.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

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
            var item = new Item
            {
                ItemId = Guid.NewGuid(),
                Name = file.FileName,
                Created = DateTime.Now,
                Type = file.ContentType
            };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Database item {ItemId} created", item.ItemId);
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
            return Ok();
        }
    }
}
