using System.Text.Json;
using CompanyDashboardAPI.Data;
using CompanyDashboardAPI.DTOs;
using CompanyDashboardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyDashboardAPI.Services;

public interface ICompaniesService
{
    Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync();
    Task<CompanyDto?> GetCompanyByIdAsync(long id);
    Task<CompanyDto?> GetCompanyByUserIdAsync(string userId);
    Task<bool> UpdateCompanyAsync(long id, UpdateCompanyDto dto);
    Task<object> GetDashboardSummaryAsync(string userId);
}

public class CompaniesService : ICompaniesService
{
    private readonly AppDbContext _context;

    public CompaniesService(AppDbContext context)
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
            
            return new List<string> { json }; 
        }
        catch
        {
            return new List<string> { json ?? "" };
        }
    }

    public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync()
    {
        var companies = await _context.Companies
            .Include(c => c.Jobs)
            .ToListAsync();

        return companies.Select(c => new CompanyDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            Website = c.Website,
            LogoUrl = c.LogoUrl,
            ContactEmail = c.ContactEmail,
            Phone = c.Phone,
            Address = c.Address,
            Industry = c.Industry,
            TechStack = c.TechStack,
            SocialLinks = c.SocialLinks,
            EstablishedYear = c.FoundedYear,
            FoundedDay = c.FoundedDay,
            FoundedMonth = c.FoundedMonth,
            Employees = c.Employees,
            Benefits = c.Benefits,
            Classification = c.Classification,
            VerificationStatus = c.VerificationStatus,
            Jobs = c.Jobs.Select(j => new JobDto
            {
                Id = j.Id,
                Title = j.Title ?? string.Empty,
                Classification = j.Classification ?? string.Empty
            }).ToList()
        });
    }

    public async Task<CompanyDto?> GetCompanyByIdAsync(long id)
    {
        var c = await _context.Companies
            .Include(c => c.Jobs)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (c == null) return null;

        return new CompanyDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            Website = c.Website,
            LogoUrl = c.LogoUrl,
            ContactEmail = c.ContactEmail,
            Phone = c.Phone,
            Address = c.Address,
            Industry = c.Industry,
            TechStack = c.TechStack,
            SocialLinks = c.SocialLinks,
            Benefits = c.Benefits,
            EstablishedYear = c.FoundedYear,
            FoundedDay = c.FoundedDay,
            FoundedMonth = c.FoundedMonth,
            Employees = c.Employees,
            Classification = c.Classification,
            VerificationStatus = c.VerificationStatus,
            Jobs = c.Jobs.Select(j => new JobDto
            {
                Id = j.Id,
                Title = j.Title ?? string.Empty,
                Classification = j.Classification ?? string.Empty,
                Address = j.Address ?? string.Empty,
                JobType = SafeDeserialize(j.JobType),
                Salary = j.Salary,
                CreatedAt = j.CreatedAt
            }).ToList()
        };
    }

    public async Task<CompanyDto?> GetCompanyByUserIdAsync(string userId)
    {
        var email = await _context.Users.Where(u => u.Id == userId).Select(u => u.Email).FirstOrDefaultAsync();
        var c = await _context.Companies
            .Include(c => c.Jobs)
            .FirstOrDefaultAsync(c => c.ContactEmail == email);
        if (c == null) return null;

        return new CompanyDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            Website = c.Website,
            LogoUrl = c.LogoUrl,
            ContactEmail = c.ContactEmail,
            Phone = c.Phone,
            Address = c.Address,
            Industry = c.Industry,
            TechStack = c.TechStack,
            SocialLinks = c.SocialLinks,
            Benefits = c.Benefits,
            EstablishedYear = c.FoundedYear,
            FoundedDay = c.FoundedDay,
            FoundedMonth = c.FoundedMonth,
            Employees = c.Employees,
            Classification = c.Classification,
            VerificationStatus = c.VerificationStatus,
            Jobs = c.Jobs.Select(j => new JobDto
            {
                Id = j.Id,
                Title = j.Title ?? string.Empty,
                Classification = j.Classification ?? string.Empty,
                Address = j.Address ?? string.Empty,
                JobType = SafeDeserialize(j.JobType),
                Salary = j.Salary,
                CreatedAt = j.CreatedAt
            }).ToList()
        };
    }

    public async Task<bool> UpdateCompanyAsync(long id, UpdateCompanyDto dto)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company == null) return false;

        if (dto.Name != null) company.Name = dto.Name;
        if (dto.Description != null) company.Description = dto.Description;
        if (dto.Website != null) company.Website = dto.Website;
        if (dto.LogoUrl != null) company.LogoUrl = dto.LogoUrl;
        if (dto.ContactEmail != null) company.ContactEmail = dto.ContactEmail;
        if (dto.Phone != null) company.Phone = dto.Phone;
        if (dto.Address != null) company.Address = dto.Address;
        if (dto.Industry != null) company.Industry = dto.Industry;
        if (dto.TechStack != null) company.TechStack = dto.TechStack;
        if (dto.SocialLinks != null) company.SocialLinks = dto.SocialLinks;
        if (dto.EstablishedYear != null) company.FoundedYear = dto.EstablishedYear;
        if (dto.FoundedDay != null) company.FoundedDay = dto.FoundedDay;
        if (dto.FoundedMonth != null) company.FoundedMonth = dto.FoundedMonth;
        if (dto.Employees != null) company.Employees = dto.Employees;
        if (dto.Benefits != null) company.Benefits = dto.Benefits;
        if (dto.Classification != null) company.Classification = dto.Classification;

        _context.Companies.Update(company);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<object> GetDashboardSummaryAsync(string userId)
    {
        var email = await _context.Users.Where(u => u.Id == userId).Select(u => u.Email).FirstOrDefaultAsync();
        var company = await _context.Companies.FirstOrDefaultAsync(c => c.ContactEmail == email);
        if (company == null) return null;

        var newCandidates = await _context.JobApplications
            .Include(a => a.Job)
            .Where(a => a.Job != null && a.Job.CompanyId == company.Id && (a.Status == "applied" || a.Status == "reviewing"))
            .CountAsync();

        return new
        {
            new_candidates = newCandidates,
            schedule_today = 0,
            messages_received = 0
        };
    }
}
