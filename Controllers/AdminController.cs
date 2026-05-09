using CompanyDashboardAPI.Data;
using CompanyDashboardAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyDashboardAPI.Controllers;

[Authorize(Roles = "admin")] // تأمين اللوحة للمشرفين فقط
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AdminController> _logger;

    public AdminController(AppDbContext context, UserManager<ApplicationUser> userManager, ILogger<AdminController> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    #region إحصائيات النظام (System Statistics)
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = new
        {
            TotalUsers = await _userManager.Users.CountAsync(),
            TotalCompanies = await _context.Companies.CountAsync(),
            TotalJobs = await _context.Jobs.CountAsync(),
            TotalApplications = await _context.JobApplications.CountAsync(),
            PendingCompanies = await _context.Companies.CountAsync(c => c.VerificationStatus == "PENDING"),
            ActiveJobs = await _context.Jobs.CountAsync(j => j.IsActive)
        };

        return Ok(stats);
    }
    #endregion

    #region إدارة الشركات (Company Management)
    [HttpGet("companies/pending")]
    public async Task<IActionResult> GetPendingCompanies()
    {
        var pendingCompanies = await _context.Companies
            .Where(c => c.VerificationStatus == "PENDING")
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return Ok(pendingCompanies);
    }

    [HttpPost("companies/{id}/review")]
    public async Task<IActionResult> ReviewCompany(long id, [FromBody] ReviewRequest request)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company == null) return NotFound("الشركة غير موجودة.");

        if (request.Status == "APPROVED")
        {
            company.VerificationStatus = "APPROVED";
            company.RejectionReason = null;
        }
        else if (request.Status == "REJECTED")
        {
            company.VerificationStatus = "REJECTED";
            company.RejectionReason = request.Reason;
        }
        else
        {
            return BadRequest("حالة غير صالحة. استخدم APPROVED أو REJECTED.");
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = $"تم تحديث حالة الشركة إلى {company.VerificationStatus}" });
    }
    #endregion

    #region إدارة المستخدمين (User Management)
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers([FromQuery] string? search)
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u => u.FullName.Contains(search) || u.Email.Contains(search) || u.UserName.Contains(search));
        }

        var users = await query
            .Select(u => new 
            {
                u.Id,
                u.FullName,
                u.Email,
                u.Role,
                u.IsBanned,
                u.CreatedAt
            })
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        return Ok(users);
    }

    [HttpPost("users/{id}/toggle-status")]
    public async Task<IActionResult> ToggleUserStatus(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound("المستخدم غير موجود.");

        // منع المشرف من حظر نفسه
        var currentUserId = _userManager.GetUserId(User);
        if (id == currentUserId) return BadRequest("لا يمكنك حظر حسابك الخاص.");

        user.IsBanned = !user.IsBanned;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded) return BadRequest(result.Errors);

        return Ok(new { message = user.IsBanned ? "تم حظر الحساب بنجاح." : "تم فك الحظر عن الحساب بنجاح." });
    }
    #endregion
}

public class ReviewRequest
{
    public string Status { get; set; } = string.Empty; // APPROVED or REJECTED
    public string? Reason { get; set; }
}
