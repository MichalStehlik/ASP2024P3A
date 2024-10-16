using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api_sk1_02files.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        const string folder = "Uploads";

        [HttpGet]
        public IActionResult GetFiles()
        {
            var files = Directory.GetFiles(folder).Select(Path.GetFileName).ToList();
            return Ok(files);
        }

        [HttpGet("name")]
        public IActionResult GetFile(string name)
        {
            var path = Path.Combine(Environment.CurrentDirectory,folder, name);
            if (!System.IO.File.Exists(path))
                return NotFound();
            Console.WriteLine(path);
            Console.WriteLine(Environment.CurrentDirectory);
            return PhysicalFile(path, "application/octet-stream", enableRangeProcessing: true);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");
            Console.WriteLine(file.FileName);
            Console.WriteLine(file.Length);
            Console.WriteLine(file.ContentType);
            foreach (var header in file.Headers)
            {
                Console.WriteLine(header.Key + ": " + header.Value);
            }
            var path = Path.Combine(folder, file.FileName);
            Console.WriteLine(path);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Ok();
        }

        [HttpDelete("name")]
        public IActionResult DeleteFile(string name)
        {
            var path = Path.Combine(folder, name);
            if (!System.IO.File.Exists(path))
                return NotFound();
            System.IO.File.Delete(path);
            return NoContent();
        }
    }
}
