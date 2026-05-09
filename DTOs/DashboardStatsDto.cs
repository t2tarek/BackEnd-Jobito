namespace CompanyDashboardAPI.DTOs;

public class DashboardStatsDto
{
    public int TotalJobViews { get; set; }
    public int TotalJobApplied { get; set; }
    public int TotalJobOpened { get; set; }

    public IEnumerable<DailyStatDto> ChartData { get; set; } = new List<DailyStatDto>();
}

public class DailyStatDto
{
    public string Date { get; set; } = string.Empty;
    public int JobsApplied { get; set; }
    public int JobsViewed { get; set; }
}
