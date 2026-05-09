namespace CompanyDashboardAPI.Models;

public class JobUpdate
{
    public long Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public long CompanyId { get; set; }
    public Company Company { get; set; } = null!;
}
