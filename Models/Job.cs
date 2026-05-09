using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyDashboardAPI.Models;

public class Job
{
    public long Id { get; set; }

    public long? CompanyId { get; set; }
    public Company? Company { get; set; }

    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public long? CategoryId { get; set; }
    public Category? Category { get; set; }

    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Salary { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? SalaryMin { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? SalaryMax { get; set; }

    public string? Address { get; set; }

    [Column(TypeName = "decimal(10, 7)")]
    public decimal? Latitude { get; set; }

    [Column(TypeName = "decimal(10, 7)")]
    public decimal? Longitude { get; set; }

    public string? JobType { get; set; } // JSON array of strings
    public string? WorkLocationType { get; set; } // JSON array of strings
    
    [MaxLength(50)]
    public string? Classification { get; set; }

    public string? FieldOfWork { get; set; } // JSON array of strings
    public int SlotsAvailable { get; set; } = 1;

    [MaxLength(50)]
    public string PriceType { get; set; } = "fixed";

    public bool IsNegotiable { get; set; } = false;
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }

    public string? WorkTime { get; set; } // JSON array
    public string? Images { get; set; } // JSON array
    public string? Skills { get; set; } // JSON array

    // Relationships
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
}
