using System.ComponentModel.DataAnnotations;

namespace CompanyDashboardAPI.Models;

public class HelpCategory
{
    public long Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? NameEn { get; set; }

    [MaxLength(50)]
    public string? Icon { get; set; }

    public ICollection<HelpArticle> Articles { get; set; } = new List<HelpArticle>();
}
