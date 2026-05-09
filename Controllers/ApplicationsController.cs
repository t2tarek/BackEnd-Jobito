using System.Security.Claims;
using CompanyDashboardAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyDashboardAPI.Controllers;

public class ApplyDto
{
    public string ResumeUrl { get; set; } = string.Empty;
    public string CoverLetter { get; set; } = string.Empty;
}

public class UpdateStatusDto
{
    public string Status { get; set; } = string.Empty;
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationsService _applicationsService;

    public ApplicationsController(IApplicationsService applicationsService)
    {
        _applicationsService = applicationsService;
    }

    [HttpGet("my-applications")]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyApplications()
    {
        var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var applications = await _applicationsService.GetMyApplicationsAsync(userId);
        
        // Map to anonymous object or DTO to avoid circular reference issues with JSON serialization
        var result = applications.Select(a => new
        {
            applicationId = a.Id,
            status = a.Status,
            appliedAt = a.AppliedAt,
            resumeUrl = a.ResumeUrl,
            job = new
            {
                title = a.Job?.Title,
                company = a.Job?.Company != null ? new
                {
                    name = a.Job.Company.Name,
                    logoUrl = a.Job.Company.LogoUrl
                } : null,
                user = a.Job?.User != null ? new
                {
                    fullName = a.Job.User.FullName,
                    avatarUrl = a.Job.User.AvatarUrl
                } : null
            }
        });

        return Ok(result);
    }

    [Authorize(Roles = "company")]
    [HttpGet("job/{jobId}")]
    public async Task<IActionResult> GetApplicationsForJob(long jobId)
    {
        var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var applications = await _applicationsService.GetApplicationsForJobAsync(jobId, userId);
        
        var result = applications.Select(a => new
        {
            applicationId = a.Id,
            status = a.Status,
            appliedAt = a.AppliedAt,
            coverLetter = a.CoverLetter,
            resumeUrl = a.ResumeUrl,
            user = new
            {
                userId = a.UserId,
                fullName = a.User?.FullName,
                email = a.User?.Email,
                avatarUrl = a.User?.AvatarUrl,
                phone = a.User?.PhoneNumber
            }
        });

        return Ok(result);
    }

    [HttpPost("job/{jobId}/apply")]
    public async Task<IActionResult> ApplyToJob(long jobId, [FromBody] ApplyDto dto)
    {
        var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var success = await _applicationsService.ApplyToJobAsync(jobId, userId, dto.ResumeUrl, dto.CoverLetter);
        if (!success) return BadRequest(new { message = "You have already applied for this job." });

        return Ok(new { message = "Application submitted successfully." });
    }

    [Authorize(Roles = "company")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetApplicationById(long id)
    {
        var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var application = await _applicationsService.GetApplicationByIdAsync(id, userId);
        if (application == null) return NotFound(new { message = "Application not found or you don't have permission." });

        var result = new
        {
            applicationId = application.Id,
            status = application.Status,
            appliedAt = application.AppliedAt,
            coverLetter = application.CoverLetter,
            resumeUrl = application.ResumeUrl,
            job = new
            {
                title = application.Job?.Title,
                category = new { name = application.Job?.CategoryId.ToString() },
                jobType = application.Job?.JobType
            },
            user = new
            {
                userId = application.UserId,
                fullName = application.User?.FullName,
                email = application.User?.Email,
                avatarUrl = application.User?.AvatarUrl,
                phone = application.User?.PhoneNumber,
                bio = application.User?.Bio,
                location = application.User?.Location,
                gender = application.User?.Gender,
                dob = application.User?.Dob,
                skills = !string.IsNullOrEmpty(application.User?.SkillsJson) ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(application.User.SkillsJson) : new List<string>(),
                socialLinks = !string.IsNullOrEmpty(application.User?.SocialLinksJson) ? System.Text.Json.JsonSerializer.Deserialize<object>(application.User.SocialLinksJson) : null,
                experiences = !string.IsNullOrEmpty(application.User?.ExperiencesJson) ? System.Text.Json.JsonSerializer.Deserialize<List<object>>(application.User.ExperiencesJson) : new List<object>(),
                educations = !string.IsNullOrEmpty(application.User?.EducationsJson) ? System.Text.Json.JsonSerializer.Deserialize<List<object>>(application.User.EducationsJson) : new List<object>(),
                portfolios = !string.IsNullOrEmpty(application.User?.PortfoliosJson) ? System.Text.Json.JsonSerializer.Deserialize<List<object>>(application.User.PortfoliosJson) : new List<object>()
            }
        };

        return Ok(result);
    }

    [Authorize(Roles = "company")]
    [HttpPut("{applicationId}/status")]
    public async Task<IActionResult> UpdateApplicationStatus(long applicationId, [FromBody] UpdateStatusDto dto)
    {
        var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var success = await _applicationsService.UpdateApplicationStatusAsync(applicationId, dto.Status, userId);
        if (!success) return NotFound(new { message = "Application not found or you don't have permission." });

        return Ok(new { message = "Status updated successfully." });
    }
}
