using System.ComponentModel.DataAnnotations;

namespace CompanyDashboardAPI.Models;

public class Category
{
    public long Id { get; set; }

    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? NameEn { get; set; }

    public string? Description { get; set; }
    public string? DescriptionEn { get; set; }

    // Relationships
    public ICollection<Job> Jobs { get; set; } = new List<Job>();
    public ICollection<Job> JobsMany { get; set; } = new List<Job>();
}
