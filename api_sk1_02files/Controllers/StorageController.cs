using api_sk1_02files.Data;
using api_sk1_02files.InputModels;
using api_sk1_02files.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
        private readonly ThumbnailOptions _thumbnailOptions;
        private readonly PictureOptions _pictureOptions;

        public StorageController(ApplicationDbContext context, ILogger<StorageController> logger, IOptions<ThumbnailOptions> thumbnailOptions, IOptions<PictureOptions> pictureOptions)
        {
            _context = context;
            _logger = logger;
            _thumbnailOptions = thumbnailOptions.Value;
            _pictureOptions = pictureOptions.Value;
        }

        [HttpPost]
        public async Task<IActionResult> UploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");
            MemoryStream ms = new MemoryStream();
            MemoryStream tms = new MemoryStream();
            await file.CopyToAsync(ms);
            if (file.ContentType.StartsWith("image"))
            {
                if (file.ContentType == "image/svg+xml")
                {
                    return BadRequest("SVG format is not supported.");
                }
                using (Image image = Image.Load(ms.ToArray()))
                {
                    if (image.Height > image.Width)
                    {
                        image.Mutate(x => x.Resize(0, _thumbnailOptions.Size));
                    }
                    else
                    {
                        image.Mutate(x => x.Resize(_thumbnailOptions.Size, 0));
                    }
                    int cropSize = Math.Min(image.Width, image.Height);
                    int cropX = (image.Width - cropSize) / 2;
                    int cropY = (image.Height - cropSize) / 2;

                    image.Mutate(x => x.Crop(new Rectangle(cropX, cropY, cropSize, cropSize)));
                    switch (file.ContentType)
                    {
                        case "image/png":
                            image.SaveAsPng(tms);
                            break;
                        case "image/jpeg":
                            image.SaveAsJpeg(tms);
                            break;
                        case "image/bmp":
                            image.SaveAsBmp(tms);
                            break;
                        case "image/gif":
                            image.SaveAsGif(tms);
                            break;
                        case "image/tiff":
                            image.SaveAsTiff(tms);
                            break;
                        case "image/webp":
                            image.SaveAsWebp(tms);
                            break;
                        default:
                            return BadRequest("Unsupported image format for thumbnails.");
                    }
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
                Size = (int)ms.Length
            };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Database item {ItemId} created", item.ItemId);
            foreach (var pictureSize in _pictureOptions.Sizes)
            {
                MemoryStream picNS = new MemoryStream();
                using (Image image = Image.Load(ms.ToArray()))
                {
                    if (pictureSize.Width == 0)
                    {
                        pictureSize.Width = (int)(image.Width * (double)pictureSize.Height / image.Height);
                    }
                    else if (pictureSize.Height == 0)
                    {
                        pictureSize.Height = (int)(image.Height * (double)pictureSize.Width / image.Width);
                    }
                    image.Mutate(x => x.Resize(pictureSize.Width, pictureSize.Height));
                    switch (file.ContentType)
                    {
                        case "image/png":
                            image.SaveAsPng(picNS);
                            break;
                        case "image/jpeg":
                            image.SaveAsJpeg(picNS);
                            break;
                        case "image/bmp":
                            image.SaveAsBmp(picNS);
                            break;
                        case "image/gif":
                            image.SaveAsGif(picNS);
                            break;
                        case "image/tiff":
                            image.SaveAsTiff(picNS);
                            break;
                        case "image/webp":
                            image.SaveAsWebp(picNS);
                            break;
                        default:
                            return BadRequest("Unsupported image format for picture.");
                    }
                }
                var picture = new Picture
                {
                    ItemId = item.ItemId,
                    Width = pictureSize.Width,
                    Height = pictureSize.Height,
                    Content = picNS.ToArray()
                };
                _context.Pictures.Add(picture);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Database picture {ItemId} created", picture.ItemId);
            }
            return Ok(new { id = item.ItemId });
            //return CreatedAtAction(nameof(GetItemAsync), new { id = item.ItemId }, item); <--- WTF?

        }

        [HttpGet]
        public async Task<IActionResult> GetListAsync(DateOnly? date, int? minSize, int? maxSize, ItemsOrder orderBy, int? page, int? size)
        {
            IQueryable<FileListVM> items = _context.Items.Select(i => new FileListVM
            {
                ItemId = i.ItemId,
                Name = i.Name,
                Type = i.Type,
                Created = i.Created,
                Size = i.Size,
                Thumbnail = i.Thumbnail
            });
            int total = await items.CountAsync();
            if (date != null)
            {
                items = items.Where(i => i.Created.Date == date.Value.ToDateTime(TimeOnly.MinValue).Date);
            }
            if (minSize != null)
                items = items.Where(i => i.Size >= minSize);
            if (maxSize != null)
                items = items.Where(i => i.Size <= maxSize);
            switch (orderBy)
            {
                case ItemsOrder.Name:
                    items = items.OrderBy(i => i.Name);
                    break;
                case ItemsOrder.Created:
                    items = items.OrderBy(i => i.Created);
                    break;
                case ItemsOrder.Size:
                    items = items.OrderBy(i => i.Size);
                    break;
                case ItemsOrder.NameDesc:
                    items = items.OrderByDescending(i => i.Name);
                    break;
                case ItemsOrder.CreatedDesc:
                    items = items.OrderByDescending(i => i.Created);
                    break;
                case ItemsOrder.SizeDesc:
                    items = items.OrderByDescending(i => i.Size);
                    break;
            }
            if (page != null)
                items = items.Skip((page.Value - 1) * (size ?? 10));
            if (size != null)
                items = items.Take(size.Value);
            return Ok(new Listing<FileListVM>
            {
                Total = total,
                Page = page ?? 0,
                Size = size ?? 0,
                Count = await items.CountAsync(),
                Items = await items.ToListAsync()
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemAsync(Guid id)
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
        public async Task<IActionResult> DownloadAsync(Guid id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return NotFound();
            return File(item.Content, item.Type, item.Name);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return NotFound();
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}/thumbnail")]
        public async Task<IActionResult> GetThumbnailAsync(Guid id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null || item.Thumbnail == null)
                return NotFound();
            return File(item.Thumbnail, item.Type, item.Name);
        }
    }

    public enum ItemsOrder
    {
        None,
        Name,
        Created,
        Size,
        NameDesc,
        CreatedDesc,
        SizeDesc
    }

    public class ThumbnailOptions
    {
        public int Size { get; set; }
    }

    public class PictureSize
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class  PictureOptions
    {
        public PictureSize[] Sizes { get; set; } = Array.Empty<PictureSize>();
    }
}
