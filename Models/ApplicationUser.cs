using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CompanyDashboardAPI.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(255)]
    public string FullName { get; set; } = string.Empty;

    public string Role { get; set; } = "user"; // e.g. user, company, admin

    [MaxLength(100)]
    public string? Classification { get; set; }

    [Column(TypeName = "decimal(10, 7)")]
    public decimal? Latitude { get; set; }

    [Column(TypeName = "decimal(10, 7)")]
    public decimal? Longitude { get; set; }

    public int ServiceRadiusKm { get; set; } = 10;
    
    public bool IsPhoneVerified { get; set; } = false;

    // We can store NotificationPreferences as JSON in a string column
    public string NotificationPreferences { get; set; } = "{\"applications\": true, \"jobs\": false, \"recs\": false}";

    [MaxLength(255)]
    public string? GoogleId { get; set; }

    public string? AvatarUrl { get; set; }
    public string? BannerUrl { get; set; }
    public string? Location { get; set; }
    public string? RegistrationData { get; set; }

    // Rich profile fields (stored as JSON strings)
    public string? Bio { get; set; }
    public string? Dob { get; set; }
    public string? Gender { get; set; }
    public string? SocialLinksJson { get; set; }   // JSON: { instagram, twitter, linkedin }
    public string? SkillsJson { get; set; }         // JSON: ["skill1", "skill2"]
    public string? ExperiencesJson { get; set; }    // JSON: [{role,period,location,desc}]
    public string? EducationsJson { get; set; }     // JSON: [{school,degree,period,desc}]
    public string? PortfoliosJson { get; set; }     // JSON: ["url1", "url2"]
    public string? ServicesJson { get; set; }       // JSON: ["service1"]

    [MaxLength(10)]
    public string ThemePreference { get; set; } = "light";

    [MaxLength(10)]
    public string LanguagePreference { get; set; } = "en";

    public DateTime? DeletionRequestedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsBanned { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    public ICollection<Job> Jobs { get; set; } = new List<Job>();
    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
}
