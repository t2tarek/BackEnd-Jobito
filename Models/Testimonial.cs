using System.ComponentModel.DataAnnotations;

namespace CompanyDashboardAPI.Models;

public class Testimonial
{
    public long Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public string Body { get; set; } = string.Empty;
    public string? BodyEn { get; set; }

    public bool IsFeatured { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
