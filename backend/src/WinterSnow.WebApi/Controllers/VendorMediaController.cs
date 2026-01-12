using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinterSnow.Core.Domain.Catalog;
using WinterSnow.Core.Domain.Media;
using WinterSnow.Data;

namespace WinterSnow.WebApi.Controllers;

[ApiController]
[Authorize(Roles = "Vendor")]
[Route("api/vendor/listings")]
public class VendorMediaController : ControllerBase
{
    private readonly WinterSnowDbContext _db;

    public VendorMediaController(WinterSnowDbContext db)
    {
        _db = db;
    }

    private int VendorId => int.TryParse(User.FindFirst("vendorId")?.Value, out var id) ? id : 0;

    [HttpPost("{productId:int}/media/upload")]
    [RequestSizeLimit(10_000_000)] // 10 MB
    public async Task<ActionResult<object>> Upload([FromRoute] int productId, [FromForm] IFormFile file, CancellationToken ct)
    {
        if (VendorId <= 0)
            return Unauthorized();
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "No file uploaded." });

        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId && p.VendorId == VendorId, ct);
        if (product is null)
            return NotFound(new { message = "Listing not found." });

        var ext = Path.GetExtension(file.FileName);
        var safeExt = string.IsNullOrWhiteSpace(ext) ? ".bin" : ext;
        var fileName = $"{Guid.NewGuid():N}{safeExt}";

        var relativeDir = Path.Combine("uploads", "products", productId.ToString());
        var env = HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
        var wwwroot = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
        var absDir = Path.Combine(wwwroot, relativeDir);
        Directory.CreateDirectory(absDir);

        var absPath = Path.Combine(absDir, fileName);
        await using (var stream = System.IO.File.Create(absPath))
        {
            await file.CopyToAsync(stream, ct);
        }

        var urlPath = "/" + Path.Combine(relativeDir, fileName).Replace("\\", "/");

        var attachment = new MediaAttachment
        {
            OriginalFileName = file.FileName,
            ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
            Length = file.Length,
            Url = urlPath,
            StoragePath = absPath
        };
        _db.MediaAttachments.Add(attachment);
        await _db.SaveChangesAsync(ct);

        var displayOrder = await _db.ProductMedia.Where(m => m.ProductId == productId).MaxAsync(m => (int?)m.DisplayOrder, ct) ?? 0;
        _db.ProductMedia.Add(new ProductMedia
        {
            ProductId = productId,
            Url = urlPath,
            MediaType = "image",
            DisplayOrder = displayOrder + 1,
            MediaAttachmentId = attachment.Id
        });
        await _db.SaveChangesAsync(ct);

        return Ok(new
        {
            url = urlPath,
            attachmentId = attachment.Id
        });
    }
}

