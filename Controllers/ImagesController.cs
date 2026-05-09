using System.Security.Claims;
using CompanyDashboardAPI.Data;
using CompanyDashboardAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyDashboardAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    public class UploadImageRequest
    {
        public IFormFile File { get; set; } = null!;
        [FromForm(Name = "entity_type")]
        public string? EntityType { get; set; }
        [FromForm(Name = "entity_id")]
        public string? EntityId { get; set; }
        [FromForm(Name = "image_type")]
        public string? ImageType { get; set; }
    }
    public class UploadSingleImageRequest
    {
        public IFormFile File { get; set; } = null!;
    }

    private readonly IFileStorageService _fileStorageService;
    private readonly AppDbContext _context;

    public ImagesController(IFileStorageService fileStorageService, AppDbContext context)
    {
        _fileStorageService = fileStorageService;
        _context = context;
    }

    /// <summary>General image upload</summary>
    [HttpPost("upload")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest request)
    {
        try
        {
            var file = request.File;
            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");
            var folder = request.EntityType ?? "general";
            var url = await _fileStorageService.UploadFileAsync(file, folder);
            return Ok(new { imageUrl = url });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Upload/replace user profile avatar  – PUT /api/images/profile</summary>
    [HttpPut("profile")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadProfileImage([FromForm] UploadSingleImageRequest request)
    {
        try
        {
            var file = request.File;
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "لم يتم اختيار ملف" });

            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var url = await _fileStorageService.UploadFileAsync(file, "avatars");

            // Save to DB
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                user.AvatarUrl = url;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return Ok(new { imageUrl = url, image_url = url });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Upload/replace user banner image – PUT /api/images/banner</summary>
    [HttpPut("banner")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadBannerImage([FromForm] UploadSingleImageRequest request)
    {
        try
        {
            var file = request.File;
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "لم يتم اختيار ملف" });

            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var url = await _fileStorageService.UploadFileAsync(file, "banners");

            // Save to DB
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                user.BannerUrl = url;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return Ok(new { imageUrl = url, image_url = url });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
