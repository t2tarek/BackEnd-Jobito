using System.Collections.Generic;

namespace CompanyDashboardAPI.DTOs;

public class JobDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ExperienceLevel { get; set; } = string.Empty;
    public string EducationLevel { get; set; } = string.Empty;
    public List<string> JobType { get; set; } = new();
    public List<string> WorkLocationType { get; set; } = new();
    public List<string> FieldOfWork { get; set; } = new();
    public List<string> Images { get; set; } = new();
    public List<string> Skills { get; set; } = new();
    public decimal? Salary { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public long? CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public long? CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? CompanyLogoUrl { get; set; }
    public string? OfficePhoto1Url { get; set; }
    public string? OfficePhoto2Url { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Classification { get; set; } = string.Empty;
    public List<string> WorkTime { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public string? UserId { get; set; }
    public string? UserFullName { get; set; }
    public string? UserAvatarUrl { get; set; }
    public int SlotsAvailable { get; set; }
    public int AppliedCount { get; set; }
}

public class CreateJobDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ExperienceLevel { get; set; }
    public string? EducationLevel { get; set; }
    public string? JobType { get; set; } // Can be string or JSON array string
    public string? WorkLocationType { get; set; }
    public string? FieldOfWork { get; set; }
    public string? Qualifications { get; set; }
    public List<string>? Skills { get; set; }
    public string? JobImages { get; set; }
    public List<string>? Images { get; set; }
    public decimal? Salary { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public long? CategoryId { get; set; }
    public long? CompanyId { get; set; }
    public string? Address { get; set; }
    public string? Classification { get; set; }
    public List<string>? WorkTime { get; set; }
    public bool? IsActive { get; set; }
    public int? SlotsAvailable { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
