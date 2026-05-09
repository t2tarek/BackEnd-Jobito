using System.ComponentModel.DataAnnotations;

namespace CompanyDashboardAPI.Models;

public class Company
{
    public long Id { get; set; }

    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? Address { get; set; }

    [MaxLength(255)]
    public string? ContactEmail { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(50)]
    public string? TaxId { get; set; }

    [MaxLength(100)]
    public string? LicenseNumber { get; set; }

    public string? CrDocumentUrl { get; set; }

    [MaxLength(50)]
    public string VerificationStatus { get; set; } = "PENDING";

    public string? RejectionReason { get; set; }

    [MaxLength(255)]
    public string? Website { get; set; }

    [MaxLength(50)]
    public string? Employees { get; set; }

    [MaxLength(100)]
    public string? Industry { get; set; }

    [MaxLength(100)]
    public string? Classification { get; set; }

    [MaxLength(50)]
    public string? FoundedDay { get; set; }

    [MaxLength(50)]
    public string? FoundedMonth { get; set; }

    [MaxLength(50)]
    public string? FoundedYear { get; set; }

    // Stored as JSON strings
    public string? SocialLinks { get; set; }
    public string? Benefits { get; set; }
    public string? TechStack { get; set; }
    public string? LocationTags { get; set; }

    public string? LogoUrl { get; set; }
    public string? OfficePhoto1Url { get; set; }
    public string? OfficePhoto2Url { get; set; }

    [MaxLength(50)]
    public string? OfficialNationalId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}
