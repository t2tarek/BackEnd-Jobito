using System.ComponentModel.DataAnnotations;

namespace CompanyDashboardAPI.Models;

public class JobApplication
{
    public long Id { get; set; }

    public long JobId { get; set; }
    public Job Job { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public string? PortfolioUrl { get; set; }
    public string? Address { get; set; }
    public string? CoverLetter { get; set; }
    public string? ResumeUrl { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "applied";

    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
}
