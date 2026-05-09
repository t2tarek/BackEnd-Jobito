using System.Security.Claims;
using System.Text.Json;
using CompanyDashboardAPI.Data;
using CompanyDashboardAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyDashboardAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<CompanyDashboardAPI.Models.ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        AppDbContext context,
        UserManager<CompanyDashboardAPI.Models.ApplicationUser> userManager,
        ITokenService tokenService,
        ILogger<UsersController> logger)
    {
        _context = context;
        _userManager = userManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    // ─────────────────────────────────────────────
    // GET /api/users/me  – Return full profile
    // ─────────────────────────────────────────────
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return NotFound(new { message = "User not found" });

        return Ok(BuildProfileResponse(user));
    }

    // ─────────────────────────────────────────────
    // PUT /api/users/me  – Update profile data
    // ─────────────────────────────────────────────
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDto dto)
    {
        var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return NotFound(new { message = "User not found" });

        // Basic fields
        if (dto.FullName != null) user.FullName = dto.FullName;
        if (dto.Phone != null) user.PhoneNumber = dto.Phone;
        if (dto.Classification != null) user.Classification = dto.Classification;
        if (dto.Location != null) user.Location = dto.Location;
        if (dto.AvatarUrl != null) user.AvatarUrl = dto.AvatarUrl;
        if (dto.BannerUrl != null) user.BannerUrl = dto.BannerUrl;

        // Rich profile fields
        if (dto.Bio != null) user.Bio = dto.Bio;
        if (dto.Dob != null) user.Dob = dto.Dob;
        if (dto.Gender != null) user.Gender = dto.Gender;

        // JSON fields – serialize arrays/objects to string
        if (dto.SocialLinks != null)
            user.SocialLinksJson = JsonSerializer.Serialize(dto.SocialLinks);
        if (dto.Skills != null)
            user.SkillsJson = JsonSerializer.Serialize(dto.Skills);
        if (dto.Experiences != null)
            user.ExperiencesJson = JsonSerializer.Serialize(dto.Experiences);
        if (dto.Educations != null)
            user.EducationsJson = JsonSerializer.Serialize(dto.Educations);
        if (dto.Portfolios != null)
            user.PortfoliosJson = JsonSerializer.Serialize(dto.Portfolios);
        if (dto.Services != null)
            user.ServicesJson = JsonSerializer.Serialize(dto.Services);

        // Email change – keep Identity in sync
        if (dto.Email != null && dto.Email != user.Email)
        {
            var oldEmail = user.Email;
            user.Email = dto.Email;
            user.UserName = dto.Email;
            user.NormalizedEmail = dto.Email.ToUpper();
            user.NormalizedUserName = dto.Email.ToUpper();

            if (user.Role == "company")
            {
                var company = await _context.Companies
                    .FirstOrDefaultAsync(c => c.ContactEmail == oldEmail);
                if (company != null)
                {
                    company.ContactEmail = dto.Email;
                    _context.Companies.Update(company);
                }
            }
        }

        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"[Users] Profile updated for {user.Email}");

        // Generate a fresh JWT so avatar/banner are immediately reflected in the Header
        var newToken = _tokenService.CreateToken(user);

        return Ok(new
        {
            access_token = newToken,          // Frontend saves this → Header updates instantly
            profile = BuildProfileResponse(user)
        });
    }

    // ─────────────────────────────────────────────
    // PUT /api/users/me/password  – Change password
    // ─────────────────────────────────────────────
    [HttpPut("me/password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound(new { message = "User not found" });

        var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new { message = $"فشل تغيير كلمة المرور: {errors}" });
        }

        return Ok(new { message = "تم تغيير كلمة المرور بنجاح" });
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────
    private static object BuildProfileResponse(CompanyDashboardAPI.Models.ApplicationUser user)
    {
        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        return new
        {
            // Identity fields
            user.Id,
            user.FullName,
            user.Email,
            user.Role,
            PhoneNumber = user.PhoneNumber,
            user.AvatarUrl,
            user.BannerUrl,
            user.Classification,
            user.Location,
            user.CreatedAt,

            // Rich fields
            user.Bio,
            user.Dob,
            user.Gender,
            SocialLinks = TryDeserialize<object>(user.SocialLinksJson, opts),
            Skills       = TryDeserialize<List<string>>(user.SkillsJson, opts) ?? new List<string>(),
            Experiences  = TryDeserialize<List<object>>(user.ExperiencesJson, opts) ?? new List<object>(),
            Educations   = TryDeserialize<List<object>>(user.EducationsJson, opts) ?? new List<object>(),
            Portfolios   = TryDeserialize<List<string>>(user.PortfoliosJson, opts) ?? new List<string>(),
            Services     = TryDeserialize<List<string>>(user.ServicesJson, opts) ?? new List<string>(),
        };
    }

    private static T? TryDeserialize<T>(string? json, JsonSerializerOptions opts)
    {
        if (string.IsNullOrWhiteSpace(json)) return default;
        try { return JsonSerializer.Deserialize<T>(json, opts); }
        catch { return default; }
    }

    [HttpGet("debug/list")]
    public async Task<IActionResult> ListUsers()
    {
        var users = await _context.Users
            .Select(u => new { u.Id, u.Email, u.Role, u.IsActive })
            .ToListAsync();
        return Ok(users);
    }
}

// ─────────────────────────────────────────────
// DTOs
// ─────────────────────────────────────────────
public class UpdateProfileDto
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public string? BannerUrl { get; set; }
    public string? Classification { get; set; }
    public string? Location { get; set; }
    public string? Bio { get; set; }
    public string? Dob { get; set; }
    public string? Gender { get; set; }
    public object? SocialLinks { get; set; }
    public List<string>? Skills { get; set; }
    public List<object>? Experiences { get; set; }
    public List<object>? Educations { get; set; }
    public List<string>? Portfolios { get; set; }
    public List<string>? Services { get; set; }
}

public class ChangePasswordDto
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
