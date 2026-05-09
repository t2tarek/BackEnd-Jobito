using System.ComponentModel.DataAnnotations;

namespace CompanyDashboardAPI.Models;

public class Rating
{
    public long Id { get; set; }

    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public long? CompanyId { get; set; }
    public Company? Company { get; set; }

    public short RatingValue { get; set; }

    [MaxLength(20)]
    public string RaterType { get; set; } = "USER";

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
