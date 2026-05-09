namespace CompanyDashboardAPI.Models;

public class DailyStat
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public int JobsApplied { get; set; } 
    public int JobsViewed { get; set; } 

    // Traffic Channels
    public int ViewsDirect { get; set; }
    public int ViewsSocial { get; set; }
    public int ViewsOrganic { get; set; }
    public int ViewsOther { get; set; }

    public long CompanyId { get; set; }
    public Company Company { get; set; } = null!;
}
