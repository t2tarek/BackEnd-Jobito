using System.Security.Claims;
using CompanyDashboardAPI.DTOs;
using CompanyDashboardAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyDashboardAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobsService _jobsService;

    public JobsController(IJobsService jobsService)
    {
        _jobsService = jobsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllJobs(
        [FromQuery] string? location, 
        [FromQuery] string? classification, 
        [FromQuery] string? excludeClassification, 
        [FromQuery] string? userId, 
        [FromQuery] long? companyId,
        [FromQuery] int? limit)
    {
        var jobs = await _jobsService.GetAllJobsAsync(userId, companyId);
        
        var query = jobs.AsQueryable();

        if (!string.IsNullOrEmpty(location))
        {
            query = query.Where(j => j.Address != null && j.Address.Contains(location, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(classification))
        {
            query = query.Where(j => j.Classification == classification);
        }

        if (!string.IsNullOrEmpty(excludeClassification))
        {
            query = query.Where(j => j.Classification != excludeClassification);
        }

        if (limit.HasValue)
        {
            query = query.Take(limit.Value);
        }

        var result = query.ToList();
        return Ok(new { data = result });
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            var jobs = await _jobsService.GetAllJobsAsync(null, null);
            var categoriesList = jobs.GroupBy(j => !string.IsNullOrEmpty(j.Classification) ? j.Classification : "غير تقني")
                                 .Select(g => new 
                                 {
                                     CategoryId = (long)Math.Abs(g.Key.GetHashCode()),
                                     Name = g.Key,
                                     JobCount = g.Count()
                                 })
                                 .ToList();
                                 
            // Ensure default categories exist if no jobs match
            var defaultCats = new[] { "تقني", "غير تقني", "خدمات" };
            foreach(var c in defaultCats)
            {
                if(!categoriesList.Any(cat => cat.Name == c))
                {
                    categoriesList.Add(new { CategoryId = (long)Math.Abs(c.GetHashCode()), Name = c, JobCount = 0 });
                }
            }

            return Ok(categoriesList);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error loading categories", detail = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetJobById(long id)
    {
        var job = await _jobsService.GetJobByIdAsync(id);
        if (job == null) return NotFound(new { message = "Job not found" });

        return Ok(job);
    }

    [Authorize(Roles = "company,user")]
    [HttpPost]
    public async Task<IActionResult> CreateJob([FromBody] CreateJobDto dto)
    {
        try
        {
            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var job = await _jobsService.CreateJobAsync(dto, userId);
            return CreatedAtAction(nameof(GetJobById), new { id = job.Id }, job);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "company,user")]
    [HttpPut("{id}")]
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateJob(long id, [FromBody] CreateJobDto dto)
    {
        try
        {
            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var success = await _jobsService.UpdateJobAsync(id, dto, userId);
            if (!success) return NotFound(new { message = $"Job {id} not found or you don't have permission to update it. User: {userId}" });

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "company,user")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJob(long id)
    {
        try
        {
            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var success = await _jobsService.DeleteJobAsync(id, userId);
            if (!success) return NotFound(new { message = $"Job {id} not found or you don't have permission to delete it. User: {userId}" });

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
