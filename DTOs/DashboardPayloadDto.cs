using CompanyDashboardAPI.Models;

namespace CompanyDashboardAPI.DTOs;

public class DashboardPayloadDto
{
    public int Days { get; set; } = 7;
    public List<Job> Jobs { get; set; } = new();
    public List<JobApplication> JobApplications { get; set; } = new();
    public List<DailyStat> DailyStats { get; set; } = new();
    public List<JobUpdate> JobUpdates { get; set; } = new();
}
