using System.ComponentModel.DataAnnotations;

namespace CompanyDashboardAPI.Models;

public class HelpArticle
{
    public long Id { get; set; }

    public long CategoryId { get; set; }
    public HelpCategory Category { get; set; } = null!;

    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? TitleEn { get; set; }

    public string Content { get; set; } = string.Empty;
    public string? ContentEn { get; set; }

    public int IsHelpfulYes { get; set; } = 0;
    public int IsHelpfulNo { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
