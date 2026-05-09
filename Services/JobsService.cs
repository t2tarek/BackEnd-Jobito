using System.Text.Json;
using CompanyDashboardAPI.Data;
using CompanyDashboardAPI.DTOs;
using CompanyDashboardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyDashboardAPI.Services;

public interface IJobsService
{
    Task<IEnumerable<JobDto>> GetAllJobsAsync(string? userId = null, long? companyId = null);
    Task<JobDto?> GetJobByIdAsync(long id);
    Task<JobDto> CreateJobAsync(CreateJobDto dto, string userId);
    Task<bool> UpdateJobAsync(long id, CreateJobDto dto, string userId);
    Task<bool> DeleteJobAsync(long id, string userId);
}

public class JobsService : IJobsService
{
    private readonly AppDbContext _context;

    public JobsService(AppDbContext context)
    {
        _context = context;
    }

    private List<string> SafeDeserialize(string? json)
    {
        if (string.IsNullOrEmpty(json)) return new List<string>();
        try
        {
            if (json.TrimStart().StartsWith("["))
                return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            
            return new List<string> { json }; // Fallback for plain strings
        }
        catch
        {
            return new List<string> { json ?? "" };
        }
    }

    public async Task<IEnumerable<JobDto>> GetAllJobsAsync(string? userId = null, long? companyId = null)
    {
        var query = _context.Jobs
            .Include(j => j.Company)
            .Include(j => j.User)
            .Include(j => j.Category)
            .Include(j => j.Applications)
            .AsQueryable();

        // If filtering by owner (userId or companyId), show all jobs. 
        // Otherwise (public board), show only active ones.
        if (string.IsNullOrEmpty(userId) && !companyId.HasValue)
        {
            query = query.Where(j => j.IsActive);
        }

        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(j => j.UserId == userId);
        }

        if (companyId.HasValue)
        {
            query = query.Where(j => j.CompanyId == companyId.Value);
        }

        var jobs = await query.ToListAsync();

        return jobs.Select(j => new JobDto
        {
            Id = j.Id,
            Title = j.Title ?? string.Empty,
            Description = j.Description ?? string.Empty,
            JobType = SafeDeserialize(j.JobType),
            WorkLocationType = SafeDeserialize(j.WorkLocationType),
            FieldOfWork = SafeDeserialize(j.FieldOfWork),
            Images = SafeDeserialize(j.Images),
            Skills = SafeDeserialize(j.Skills),
            Salary = j.Salary,
            SalaryMin = j.SalaryMin,
            SalaryMax = j.SalaryMax,
            CategoryId = j.CategoryId,
            CategoryName = j.Category != null ? j.Category.Name : string.Empty,
            CompanyId = j.CompanyId,
            CompanyName = j.Company != null ? j.Company.Name : string.Empty,
            CompanyLogoUrl = j.Company != null ? j.Company.LogoUrl : string.Empty,
            OfficePhoto1Url = j.Company != null ? j.Company.OfficePhoto1Url : string.Empty,
            OfficePhoto2Url = j.Company != null ? j.Company.OfficePhoto2Url : string.Empty,
            Address = j.Address ?? string.Empty,
            Classification = j.Classification ?? string.Empty,
            WorkTime = SafeDeserialize(j.WorkTime),
            CreatedAt = j.CreatedAt,
            IsActive = j.IsActive,
            UserId = j.UserId,
            UserFullName = j.User != null ? j.User.FullName : string.Empty,
            UserAvatarUrl = j.User != null ? j.User.AvatarUrl : string.Empty,
            SlotsAvailable = j.SlotsAvailable,
            AppliedCount = j.Applications != null ? j.Applications.Count : 0
        });
    }

    public async Task<JobDto?> GetJobByIdAsync(long id)
    {
        var j = await _context.Jobs
            .Include(j => j.Company)
            .Include(j => j.User)
            .Include(j => j.Category)
            .Include(j => j.Applications)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (j == null) return null;

        return new JobDto
        {
            Id = j.Id,
            Title = j.Title ?? string.Empty,
            Description = j.Description ?? string.Empty,
            JobType = SafeDeserialize(j.JobType),
            WorkLocationType = SafeDeserialize(j.WorkLocationType),
            FieldOfWork = SafeDeserialize(j.FieldOfWork),
            Images = SafeDeserialize(j.Images),
            Skills = SafeDeserialize(j.Skills),
            Salary = j.Salary,
            SalaryMin = j.SalaryMin,
            SalaryMax = j.SalaryMax,
            CategoryId = j.CategoryId,
            CategoryName = j.Category != null ? j.Category.Name : string.Empty,
            CompanyId = j.CompanyId,
            CompanyName = j.Company != null ? j.Company.Name : string.Empty,
            CompanyLogoUrl = j.Company != null ? j.Company.LogoUrl : string.Empty,
            OfficePhoto1Url = j.Company != null ? j.Company.OfficePhoto1Url : string.Empty,
            OfficePhoto2Url = j.Company != null ? j.Company.OfficePhoto2Url : string.Empty,
            Address = j.Address ?? string.Empty,
            Classification = j.Classification ?? string.Empty,
            WorkTime = SafeDeserialize(j.WorkTime),
            CreatedAt = j.CreatedAt,
            IsActive = j.IsActive,
            UserId = j.UserId,
            UserFullName = j.User != null ? j.User.FullName : string.Empty,
            UserAvatarUrl = j.User != null ? j.User.AvatarUrl : string.Empty,
            SlotsAvailable = j.SlotsAvailable,
            AppliedCount = j.Applications != null ? j.Applications.Count : 0
        };
    }

    public async Task<JobDto> CreateJobAsync(CreateJobDto dto, string userId)
    {
        long? companyId = dto.CompanyId;
        if (!companyId.HasValue || companyId.Value == 0)
        {
            var email = await _context.Users.Where(u => u.Id == userId).Select(u => u.Email).FirstOrDefaultAsync();
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.ContactEmail == email);
            if (company != null) companyId = company.Id;
        }

        var job = new Job
        {
            Title = dto.Title,
            Description = dto.Description,
            JobType = dto.JobType ?? "[\"FullTime\"]",
            WorkLocationType = dto.WorkLocationType ?? "[\"OnSite\"]",
            FieldOfWork = dto.FieldOfWork ?? "[]",
            Images = dto.Images != null ? JsonSerializer.Serialize(dto.Images) : (dto.JobImages ?? "[]"),
            Skills = dto.Skills != null ? JsonSerializer.Serialize(dto.Skills) : (dto.Qualifications ?? "[]"),
            WorkTime = dto.WorkTime != null ? JsonSerializer.Serialize(dto.WorkTime) : "[]",
            Address = dto.Address,
            Classification = dto.Classification,
            Salary = dto.Salary,
            SalaryMin = dto.SalaryMin,
            SalaryMax = dto.SalaryMax,
            CategoryId = dto.CategoryId,
            CompanyId = companyId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            SlotsAvailable = dto.SlotsAvailable ?? 1,
            ExpiresAt = dto.ExpiresAt
        };

        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();

        return await GetJobByIdAsync(job.Id) ?? throw new Exception("Failed to retrieve created job.");
    }

    public async Task<bool> UpdateJobAsync(long id, CreateJobDto dto, string userId)
    {
        // Find user email to check company ownership
        var userEmail = await _context.Users.Where(u => u.Id == userId).Select(u => u.Email).FirstOrDefaultAsync();
        var userCompany = await _context.Companies.FirstOrDefaultAsync(c => c.ContactEmail == userEmail);

        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id);
        if (job == null) return false;

        // Check ownership
        if (job.UserId != userId && (userCompany == null || job.CompanyId != userCompany.Id))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(dto.Title)) job.Title = dto.Title;
        if (!string.IsNullOrEmpty(dto.Description)) job.Description = dto.Description;
        if (dto.JobType != null) job.JobType = dto.JobType;
        if (dto.WorkLocationType != null) job.WorkLocationType = dto.WorkLocationType;
        
        if (dto.Images != null) job.Images = JsonSerializer.Serialize(dto.Images);
        else if (dto.JobImages != null) job.Images = dto.JobImages;

        if (dto.Skills != null) job.Skills = JsonSerializer.Serialize(dto.Skills);
        else if (dto.Qualifications != null) job.Skills = dto.Qualifications;

        if (dto.WorkTime != null) job.WorkTime = JsonSerializer.Serialize(dto.WorkTime);
        
        if (dto.Address != null) job.Address = dto.Address;
        if (dto.Classification != null) job.Classification = dto.Classification;
        if (dto.Salary.HasValue) job.Salary = dto.Salary;
        if (dto.SlotsAvailable.HasValue) job.SlotsAvailable = dto.SlotsAvailable.Value;
        if (dto.ExpiresAt.HasValue) job.ExpiresAt = dto.ExpiresAt.Value;
        if (dto.SalaryMin.HasValue) job.SalaryMin = dto.SalaryMin;
        if (dto.SalaryMax.HasValue) job.SalaryMax = dto.SalaryMax;
        if (dto.CategoryId.HasValue) job.CategoryId = dto.CategoryId;
        if (dto.IsActive.HasValue) job.IsActive = dto.IsActive.Value;

        _context.Jobs.Update(job);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteJobAsync(long id, string userId)
    {
        var userEmail = await _context.Users.Where(u => u.Id == userId).Select(u => u.Email).FirstOrDefaultAsync();
        var userCompany = await _context.Companies.FirstOrDefaultAsync(c => c.ContactEmail == userEmail);

        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id && (j.UserId == userId || (userCompany != null && j.CompanyId == userCompany.Id)));
        if (job == null) return false;

        _context.Jobs.Remove(job);
        await _context.SaveChangesAsync();
        return true;
    }
}
