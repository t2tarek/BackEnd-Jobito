namespace CompanyDashboardAPI.DTOs;

public class JobListingStatsDto
{
    public int TotalViews { get; set; }
    public int TotalApplied { get; set; }
    public TrafficChannelDto TrafficChannels { get; set; } = new();
    public IEnumerable<DailyViewsDto> ViewStats { get; set; } = new List<DailyViewsDto>();
}

public class TrafficChannelDto
{
    public double Direct { get; set; }
    public double Social { get; set; }
    public double Organic { get; set; }
    public double Other { get; set; }
}

public class DailyViewsDto
{
    public string Date { get; set; } = string.Empty;
    public int Views { get; set; }
}
