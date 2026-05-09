using CompanyDashboardAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyDashboardAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly AppDbContext _context;

    public AiController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("smart-search")]
    public async Task<IActionResult> SmartSearch([FromQuery] string? q, [FromQuery] string? classification, [FromQuery] string? excludeClassification, [FromQuery] string? location)
    {
        var query = _context.Jobs
            .Include(j => j.Company)
            .Include(j => j.User)
            .AsQueryable();

        if (!string.IsNullOrEmpty(q))
        {
            var searchLower = q.ToLower();
            query = query.Where(j => 
                (j.Title != null && j.Title.ToLower().Contains(searchLower)) || 
                (j.Description != null && j.Description.ToLower().Contains(searchLower)));
        }

        if (!string.IsNullOrEmpty(classification))
        {
            var clsLower = classification.ToLower();
            query = query.Where(j => j.Classification != null && j.Classification.ToLower() == clsLower);
        }

        if (!string.IsNullOrEmpty(excludeClassification))
        {
            var exclLower = excludeClassification.ToLower();
            query = query.Where(j => j.Classification == null || j.Classification.ToLower() != exclLower);
        }

        if (!string.IsNullOrEmpty(location))
        {
            var locLower = location.ToLower();
            query = query.Where(j => j.Address != null && j.Address.ToLower().Contains(locLower));
        }

        var jobs = await query.ToListAsync();

        return Ok(new
        {
            data = jobs.Select(j => new
            {
                jobId = j.Id,
                title = j.Title,
                address = j.Address,
                jobType = j.JobType,
                slotsAvailable = j.SlotsAvailable,
                salary = j.SalaryMin,
                salaryMin = j.SalaryMin,
                salaryMax = j.SalaryMax,
                description = j.Description,
                classification = j.Classification,
                createdAt = j.CreatedAt,
                company = j.Company != null ? new { name = j.Company.Name, logoUrl = j.Company.LogoUrl } : null,
                user = j.User != null ? new { fullName = j.User.FullName, avatarUrl = j.User.AvatarUrl } : null
            }),
            expandedTags = new[] { q }
        });
    }
}
