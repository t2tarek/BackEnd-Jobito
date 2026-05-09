namespace CompanyDashboardAPI.DTOs;

public class UserDashboardDto
{
    public int TotalJobsApplied { get; set; }
    public int Interviewed { get; set; }
    public JobsAppliedStatusDto JobsAppliedStatus { get; set; } = new();
    public List<RecentApplicationDto> RecentApplicationsHistory { get; set; } = new();
}

public class JobsAppliedStatusDto
{
    public double UnsuitablePercentage { get; set; }
    public double InterviewedPercentage { get; set; }
    public double OtherPercentage { get; set; }
}

public class RecentApplicationDto
{
    public int ApplicationId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; }
}
