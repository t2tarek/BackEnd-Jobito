using System.ComponentModel.DataAnnotations;

namespace CompanyDashboardAPI.Models;

public class Favorite
{
    public long Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public long JobId { get; set; }
    public Job Job { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
