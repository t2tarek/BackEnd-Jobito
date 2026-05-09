using CompanyDashboardAPI.Data;
using CompanyDashboardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyDashboardAPI.Services;

public interface IApplicationsService
{
    Task<IEnumerable<JobApplication>> GetApplicationsForJobAsync(long jobId, string companyUserId);
    Task<IEnumerable<JobApplication>> GetMyApplicationsAsync(string userId);
    Task<bool> ApplyToJobAsync(long jobId, string userId, string resumeUrl, string coverLetter);
    Task<bool> UpdateApplicationStatusAsync(long applicationId, string status, string companyUserId);
    Task<JobApplication?> GetApplicationByIdAsync(long applicationId, string companyUserId);
}

public class ApplicationsService : IApplicationsService
{
    private readonly AppDbContext _context;

    public ApplicationsService(AppDbContext context)
    {
        _context = context;
    }

    private async Task<bool> IsUserJobOwner(Job? job, string userId)
    {
        if (job == null) return false;
        if (job.UserId == userId) return true;

        if (job.CompanyId.HasValue)
        {
            var userEmail = await _context.Users.Where(u => u.Id == userId).Select(u => u.Email).FirstOrDefaultAsync();
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.ContactEmail == userEmail);
            if (company != null && company.Id == job.CompanyId.Value) return true;
        }

        return false;
    }

    public async Task<IEnumerable<JobApplication>> GetApplicationsForJobAsync(long jobId, string companyUserId)
    {
        var job = await _context.Jobs.Include(j => j.Company).FirstOrDefaultAsync(j => j.Id == jobId);
        if (!await IsUserJobOwner(job, companyUserId)) return Enumerable.Empty<JobApplication>();

        return await _context.JobApplications
            .Include(a => a.User)
            .Where(a => a.JobId == jobId)
            .ToListAsync();
    }

    public async Task<IEnumerable<JobApplication>> GetMyApplicationsAsync(string userId)
    {
        return await _context.JobApplications
            .Include(a => a.Job)
                .ThenInclude(j => j.Company)
            .Include(a => a.Job)
                .ThenInclude(j => j.User)
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }

    public async Task<bool> ApplyToJobAsync(long jobId, string userId, string resumeUrl, string coverLetter)
    {
        var existing = await _context.JobApplications.FirstOrDefaultAsync(a => a.JobId == jobId && a.UserId == userId);
        if (existing != null) return false; 

        var application = new JobApplication
        {
            JobId = jobId,
            UserId = userId,
            ResumeUrl = resumeUrl,
            CoverLetter = coverLetter,
            Status = "applied",
            AppliedAt = DateTime.UtcNow
        };

        _context.JobApplications.Add(application);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateApplicationStatusAsync(long applicationId, string status, string companyUserId)
    {
        var application = await _context.JobApplications.Include(a => a.Job).FirstOrDefaultAsync(a => a.Id == applicationId);
        if (!await IsUserJobOwner(application?.Job, companyUserId)) return false;

        application!.Status = status;
        _context.JobApplications.Update(application);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<JobApplication?> GetApplicationByIdAsync(long applicationId, string companyUserId)
    {
        var application = await _context.JobApplications
            .Include(a => a.Job)
                .ThenInclude(j => j.Company)
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == applicationId);
            
        if (!await IsUserJobOwner(application?.Job, companyUserId)) return null;
        
        return application;
    }
}
